using System;
using System.Windows;
using Minesweeper.Data;

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
    
    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        UpdateLoginDisplay();
    }

    // Starts the game - opens a new game window and closes the main window 
    private void BtnStart_Click(object sender, RoutedEventArgs e)
    {
        if (CbxDifficulty.SelectedIndex == -1)
            return;

        DifficultyLevel difficulty = (DifficultyLevel)CbxDifficulty.SelectedItem;

        int? seed = int.TryParse(TbxSeed.Text, out int parsed)
            ? parsed
            : null;
        
        new GameWindow(difficulty, seed).Show();
        Close();
    }

    private void BtnLogin_Click(object sender, RoutedEventArgs e)
    {
        if (Session.CurrentUser == null)
            new LoginWindow().ShowDialog();
        else
            Session.CurrentUser = null;  // Log out
        UpdateLoginDisplay();
    }
    
    private void UpdateLoginDisplay()
    {
        if (Session.CurrentUser != null)
        {
            LblUser.Content = "Logged in as: " + Session.CurrentUser.Name;
            BtnLogin.Content = "Log Out";
        }
        else
        {
            LblUser.Content = "Not logged in";
            BtnLogin.Content = "Login";
        }
    }

    private void BtnStatistics_Click(object sender, RoutedEventArgs e) => new StatisticsWindow().Show();
}