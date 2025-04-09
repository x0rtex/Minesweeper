using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Minesweeper.Data;

namespace Minesweeper;

public partial class GameWindow
{
    private readonly DifficultyLevel _difficulty;
    private readonly DispatcherTimer _gameTimer;
    private TimeSpan _time = TimeSpan.Zero;
    private int _currentMineCount;

    // Constructor
    public GameWindow(DifficultyLevel selectedDifficulty)
    {
        InitializeComponent();

        _difficulty = selectedDifficulty;
        _gameTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _gameTimer.Tick += GameTimerTick;
        
        AddHandler(Cell.FlagToggledEvent, new RoutedEventHandler(OnCellFlagToggled));

        InitializeGame();
    }

    // Initialize game by creating mine grid and timer
    private void InitializeGame()
    {
        (Point gridDimensions, int mineCount) = Difficulty.GetGridSizeAndMineCount(_difficulty);
        Point absoluteBoardDimensions = Difficulty.GetAbsoluteBoardSize(gridDimensions);
        _currentMineCount = mineCount;
        TblkMines.Text = $"Mines: {mineCount}";

        Grid board = new() { Width = absoluteBoardDimensions.Y, Height = absoluteBoardDimensions.X };
        MineGrid mineGrid = new(gridDimensions, absoluteBoardDimensions, mineCount, this);
        mineGrid.PrepareBoard();

        board.Children.Add(mineGrid);
        BrdrMineGrid.Child = board;

        _gameTimer.Start();
    }

    // Update game information to database
    private void UpdateSavedGame(GameState newState)
    {
        using var db = new MinesweeperData();

        int? userId = GetCurrentUserId(db);

        var savedGame = new SavedGame
        {
            ElapsedTime = (int)_time.TotalSeconds,
            Difficulty = _difficulty,
            GameState = newState,
            UserId = userId
        };

        db.SavedGames.Add(savedGame);
        db.SaveChanges();
        
        UserStatsUpdater.UpdateUserStats();
    }
    
    
    // Retrieve the current user ID from the session
    private static int? GetCurrentUserId(MinesweeperData db)
    {
        if (Session.CurrentUser == null)
            return null;
        
        var user = db.Users.SingleOrDefault(u => u.Id == Session.CurrentUser.Id);
        
        if (user != null) 
            return user.Id;
        
        MessageBox.Show("The logged-in user does not exist in the database. Saving as a guest.");
        return null;

    }
    
    // Handle game win event
    private void OnGameWon()
    {
        MessageBox.Show("You won!");
        _gameTimer.Stop();
        UpdateSavedGame(GameState.Won);
    }

    // Handle game loss event
    public void OnGameLost()
    {
        MessageBox.Show("Game Over!");
        _gameTimer.Stop();
        UpdateSavedGame(GameState.Lost);
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
        
        if (_currentMineCount == 0)
            OnGameWon();
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