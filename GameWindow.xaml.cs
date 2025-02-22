using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
            Point boardDimensions = Difficulty.GetGameBoardSize(gridDimensions);
            int mineCount = Difficulty.GetMineCount(_difficulty);

            // Create board
            Grid board = new() { Width = boardDimensions.Y, Height = boardDimensions.X };

            // Create mine grid
            UniformGrid mineGrid = new() {
                Rows = gridDimensions.X,
                Columns = gridDimensions.Y,
                Width = boardDimensions.Y,
                Height = boardDimensions.X
            };
            
            // Create a list of all possible cell positions
            List<Point> cellPositions = CreateCellPositions(gridDimensions);

            // Randomly select mine positions
            Random random = new Random();
            List<Point> minePositions = cellPositions.OrderBy(_ => random.Next()).Take(mineCount).ToList();
            
            // Create cells
            GenerateCells(gridDimensions, mineGrid, minePositions);

            // Add mine grid to the game board
            board.Children.Add(mineGrid);
            BrdrMineGrid.Child = board;

            // Start Game Timer
            StartGameTimer();
        }

        private static List<Point> CreateCellPositions(Point gridDimensions)
        {
            List<Point> cellPositions = [];
            for (int x = 0; x < gridDimensions.X; x++)
                for (int y = 0; y < gridDimensions.Y; y++)
                    cellPositions.Add(new Point(x, y));
            
            return cellPositions;
        }

        private static void GenerateCells(Point gridDimensions, UniformGrid mineGrid, List<Point> minePositions)
        {
            for (int x = 0; x < gridDimensions.X; x++)
            for (int y = 0; y < gridDimensions.Y; y++)
            {
                Cell cell = new Cell(new Point(x, y));
                mineGrid.Children.Add(cell);
                    
                // Check if the cell is a mine
                if (minePositions.Contains(cell.Position))
                    cell.IsMine = true;
            }
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
