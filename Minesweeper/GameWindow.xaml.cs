using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Minesweeper;

public partial class GameWindow
{
    private readonly DifficultyLevel _difficulty;
    private readonly DispatcherTimer _gameTimer;
    private TimeSpan _time;

    // Constructor
    public GameWindow(DifficultyLevel selectedDifficulty)
    {
        InitializeComponent();

        _difficulty = selectedDifficulty;
        _gameTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _gameTimer.Tick += GameTimerTick;

        InitializeGame();
    }

    // Initializes game by creating mine grid and timer
    private void InitializeGame()
    {
        (Point gridDimensions, int mineCount) = Difficulty.GetGridSizeAndMineCount(_difficulty);
        Point absoluteBoardDimensions = Difficulty.GetAbsoluteBoardSize(gridDimensions);
        TblkMines.Text = $"Mines: {mineCount}";

        Grid board = new() { Width = absoluteBoardDimensions.Y, Height = absoluteBoardDimensions.X };
        MineGrid mineGrid = new(gridDimensions, absoluteBoardDimensions, mineCount);
        mineGrid.PrepareBoard();

        board.Children.Add(mineGrid);
        BrdrMineGrid.Child = board;

        _gameTimer.Start();
    }

    // Timer increments by 1 every second (caps at 999 - faithful to original game)
    private void GameTimerTick(object? sender, EventArgs e)
    {
        if (_time == TimeSpan.FromSeconds(999))
            _gameTimer.Stop();
        else
        {
            _time = _time.Add(TimeSpan.FromSeconds(1));
            TblkTimer.Text = $"Timer: {_time.Seconds:000}";
        }
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