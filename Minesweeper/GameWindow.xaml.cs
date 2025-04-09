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
    private SavedGame? _currentSavedGame;

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

    // Initializes game by creating mine grid and timer
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
        
        CreateNewSavedGame();
    }
    
    private void CreateNewSavedGame()
    {
        using var db = new MinesweeperData();

        int? userId = GetCurrentUserId(db);

        _currentSavedGame = new SavedGame
        {
            ElapsedTime = 0,
            Difficulty = _difficulty,
            GameState = GameState.Ongoing,
            UserId = userId
        };

        db.SavedGames.Add(_currentSavedGame);
        db.SaveChanges();
        
        Console.WriteLine($@"New saved game created with ID: {_currentSavedGame.Id}");
    }

    private void UpdateSavedGame(GameState newState)
    {
        Console.WriteLine($@"UpdateSavedGame called with newState: {newState}");
        
        if (_currentSavedGame == null)
        {
            Console.WriteLine(@"No current saved game to update.");
            return;
        }

        using var db = new MinesweeperData();
        var savedGame = db.SavedGames.Find(_currentSavedGame.Id);

        if (savedGame == null)
        {
            Console.WriteLine($@"Saved game with ID {_currentSavedGame.Id} not found in the database.");
            return;
        }

        savedGame.GameState = newState;
        savedGame.ElapsedTime = (int)_time.TotalSeconds;
        db.SaveChanges();
        
        Console.WriteLine($@"GameState updated to {newState} for SavedGame ID {_currentSavedGame.Id}.");
    }
    
    private static int? GetCurrentUserId(MinesweeperData db)
    {
        if (Session.CurrentUser == null)
        {
            Console.WriteLine(@"Session.CurrentUser is null.");
            return null;
        }

        Console.WriteLine($@"Session.CurrentUser.Id: {Session.CurrentUser.Id}");

        var user = db.Users.SingleOrDefault(u => u.Id == Session.CurrentUser.Id);
        
        if (user != null) 
            return user.Id;
        
        Console.WriteLine($@"User with ID {Session.CurrentUser.Id} not found in the database.");
        MessageBox.Show("The logged-in user does not exist in the database. Saving as a guest.");
        return null;

    }
    
    private void OnGameWon()
    {
        _gameTimer.Stop();
        UpdateSavedGame(GameState.Won);
        MessageBox.Show("You won!");
    }

    public void OnGameLost()
    {
        Console.WriteLine(@"OnGameLost called.");
        _gameTimer.Stop();
        UpdateSavedGame(GameState.Lost);
        MessageBox.Show("Game Over!");
    }

    // Timer increments by 1 every second (caps at 999 - faithful to original game)
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
    
    // Adjusts mine count when a cell is flagged
    private void OnCellFlagToggled(object sender, RoutedEventArgs e)
    {
        if (e is not FlagToggledEventArgs flagEvent) 
            return;
        
        if (flagEvent.IsFlagged)
            AdjustMineCount(-1);
        else
            AdjustMineCount(1);
    }
    
    // Increments mine count up or down by amount
    private void AdjustMineCount(int amount)
    {
        _currentMineCount += amount;
        TblkMines.Text = $"Mines: {_currentMineCount}";
        
        if (_currentMineCount == 0)
            OnGameWon();
    }

    // Opens new game window and closes current game window
    private void BtnRestart_Click(object sender, RoutedEventArgs e)
    {
        new GameWindow(_difficulty).Show();
        Close();
    }

    // Opens main window and closes game window
    private void BtnBack_Click(object sender, RoutedEventArgs e)
    {
        new MainWindow().Show();
        Close();
    }
}