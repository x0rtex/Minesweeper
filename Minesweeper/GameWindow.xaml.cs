using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Minesweeper.Data;

namespace Minesweeper;

public partial class GameWindow
{
    private readonly DifficultyLevel _difficulty;
    private readonly DispatcherTimer _gameTimer;
    private readonly int? _seed;
    private TimeSpan _time = TimeSpan.Zero;
    private int _currentMineCount;

    // Constructor
    public GameWindow(DifficultyLevel selectedDifficulty, int? seed = null)
    {
        InitializeComponent();

        _seed = seed;
        _difficulty = selectedDifficulty;
        _gameTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _gameTimer.Tick += GameTimerTick;
        
        AddHandler(Cell.FlagToggledEvent, new RoutedEventHandler(OnCellFlagToggled));

        InitializeGame();
    }

    // Initialize game by creating mine grid and timer
    private void InitializeGame()
    {
        int seed = _seed ?? Environment.TickCount;
        TblkSeed.Text = $"Seed: {seed}";
        
        (Point gridDimensions, int mineCount) = Difficulty.GetGridSizeAndMineCount(_difficulty);
        Point absoluteBoardDimensions = Difficulty.GetAbsoluteBoardSize(gridDimensions);
        _currentMineCount = mineCount;
        TblkMines.Text = $"Mines: {mineCount}";

        Grid board = new() { Width = absoluteBoardDimensions.Y, Height = absoluteBoardDimensions.X };
        MineGrid mineGrid = new(gridDimensions, absoluteBoardDimensions, mineCount, this, seed);
        mineGrid.PrepareBoard();

        board.Children.Add(mineGrid);
        BrdrMineGrid.Child = board;

        _gameTimer.Start();
    }

    // Update game information to database
    private async Task UpdateSavedGame(GameState newState)
    {
        using var db = new MinesweeperData();

        int? userId = await GetCurrentUserId(db);

        var savedGame = new SavedGame
        {
            ElapsedTime = (int)_time.TotalSeconds,
            Difficulty = _difficulty,
            GameState = newState,
            UserId = userId
        };

        db.SavedGames.Add(savedGame);
        await db.SaveChangesAsync();
        await UserStatsUpdater.UpdateUserStats();
    }
    
    
    // Retrieve the current user ID from the session
    private static async Task<int?> GetCurrentUserId(MinesweeperData db)
    {
        if (Session.CurrentUser == null)
            return null;
        
        var user = await db.Users
            .SingleOrDefaultAsync(u => u.Id == Session.CurrentUser.Id);
        
        if (user != null) 
            return user.Id;
        
        MessageBox.Show("The logged-in user does not exist in the database. Saving as a guest.");
        return null;

    }
    
    // Handle game win event
    private async Task OnGameWon()
    {
        try
        {
            _gameTimer.Stop();
            
            // Use Dispatcher to access UI elements
            await Application.Current.Dispatcher.Invoke(async () =>
            {
                MineGrid? mineGrid = GetMineGrid();
                mineGrid?.RevealRemainingCells(); // UI operation
                await UpdateSavedGame(GameState.Won);
                MessageBox.Show("You won!");
            });
        }
        catch (Exception e)
        {
            Console.WriteLine($@"Error occurred: {e.Message}");
            throw; // TODO handle exception
        }
    }

    // Handle game loss event
    public async Task OnGameLost()
    {
        _gameTimer.Stop();
        await UpdateSavedGame(GameState.Lost);
        MessageBox.Show("Game Over!");
    }

    // Increment timer by 1 every second (caps at 999 - faithful to original game)
    private void GameTimerTick(object? sender, EventArgs e)
    {
        if (_time.TotalSeconds >= 999)
            _gameTimer.Stop();
        else
        {
            _time = _time.Add(TimeSpan.FromSeconds(1));
            TblkTimer.Text = $"Timer: {(int)_time.TotalSeconds:000}";
        }
    }
    
    // Adjust mine count when a cell is flagged
    private void OnCellFlagToggled(object sender, RoutedEventArgs e)
    {
        if (e is not FlagToggledEventArgs flagEvent) 
            return;
        
        if (flagEvent.IsFlagged)
            AdjustMineCount(-1);
        else
            AdjustMineCount(1);
    }
    
    // Increment mine count up or down by amount
    private void AdjustMineCount(int amount)
    {
        _currentMineCount += amount;
        TblkMines.Text = $"Mines: {_currentMineCount}";

        if (_currentMineCount != 0) 
            return;
        
        bool isGameWon = CheckAllFlagsCorrect();
        if (isGameWon)
            _ = Task.Run(async () => await OnGameWon());
    }
    
    // Check if all mines are flagged correctly
    private bool CheckAllFlagsCorrect()
    {
        MineGrid? mineGrid = GetMineGrid();
        
        return mineGrid != null && mineGrid.Cells
            .Cast<Cell>()
            .All(cell => 
                    cell is { IsMine: true, IsFlagged: true } ||  // Mines must be flagged
                    cell is { IsMine: false, IsFlagged: false }   // Non-mines must NOT be flagged
            );
    }
    
    // Return the mine grid instance
    private MineGrid? GetMineGrid()
    {
        if (BrdrMineGrid.Child is not Grid board || board.Children.Count == 0)
            return null;

        return board.Children[0] as MineGrid;
    }

    // Open new game window and close current game window
    private void BtnRestart_Click(object sender, RoutedEventArgs e)
    {
        new GameWindow(_difficulty).Show();
        Close();
    }

    // Open main window and close game window
    private void BtnBack_Click(object sender, RoutedEventArgs e)
    {
        new MainWindow().Show();
        Close();
    }
}