using System.Windows;

namespace Minesweeper
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Array difficulties = Enum.GetValues(typeof(DifficultyLevel));
            CbxDifficulty.ItemsSource = difficulties;
            CbxDifficulty.SelectedIndex = 0;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (CbxDifficulty.SelectedIndex == -1)
                return;

            DifficultyLevel difficulty = (DifficultyLevel)CbxDifficulty.SelectedItem;

            new GameWindow(difficulty).Show();
            Close();
        }
    }
}