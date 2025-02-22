using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Point = System.Drawing.Point;

namespace Minesweeper
{
    public partial class GameWindow
    {
        private readonly DifficultyLevel _difficulty;
        private DispatcherTimer _dispatcherTimer;
        private TimeSpan _time;
        
        public GameWindow(DifficultyLevel selectedDifficulty)
        {
            InitializeComponent();

            // Game settings
            _difficulty = selectedDifficulty;
            _dispatcherTimer = new DispatcherTimer();
            
            Point gridDimensions = Difficulty.GetGridSize(_difficulty);
            Point absoluteBoardDimensions = Difficulty.GetAbsoluteBoardSize(gridDimensions);
            int mineCount = Difficulty.GetMineCount(_difficulty);

            // Create board and mine grid
            Grid board = new() { Width = absoluteBoardDimensions.Y, Height = absoluteBoardDimensions.X };
            MineGrid mineGrid = new(gridDimensions, absoluteBoardDimensions, mineCount);
            mineGrid.GenerateCells();

            // Add mine grid to the game board
            board.Children.Add(mineGrid);
            BrdrMineGrid.Child = board;

            // Start Game Timer
            StartGameTimer();
        }
        

        private void StartGameTimer()
        {
            _dispatcherTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _dispatcherTimer.Tick += DispatcherTimer_Tick;
            _dispatcherTimer.Start();
        }

        private void DispatcherTimer_Tick(object? sender, EventArgs e)
        {
            if (_time == TimeSpan.FromSeconds(999)) 
                _dispatcherTimer.Stop();

            else
            {
                _time = _time.Add(TimeSpan.FromSeconds(1));
                TblkTimer.Text = $"Timer: {_time.Seconds:000}";
            }
        }

        private void BtnRestart_Click(object sender, RoutedEventArgs e)
        {
            GameWindow gameWindow = new(_difficulty);
            gameWindow.Show();
            Close();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new();
            mainWindow.Show();
            Close();
        }
    }
}
