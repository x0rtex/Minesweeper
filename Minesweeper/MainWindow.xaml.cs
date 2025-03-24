using System;
using System.Windows;

namespace Minesweeper;

public partial class MainWindow
{
    // Constructor
    public MainWindow()
    {
        InitializeComponent();
        Array difficulties = Enum.GetValues(typeof(DifficultyLevel));
        CbxDifficulty.ItemsSource = difficulties;
        CbxDifficulty.SelectedIndex = 0;
    }

    // Starts the game - opens a new game window and closes the main window 
    private void Button_Click(object sender, RoutedEventArgs e)
    {
        if (CbxDifficulty.SelectedIndex == -1)
            return;

        DifficultyLevel difficulty = (DifficultyLevel)CbxDifficulty.SelectedItem;

        new GameWindow(difficulty).Show();
        Close();
    }
}