using System;
using System.Collections.Generic;
using System.Windows;
using System.Data.Entity;
using Minesweeper.Data;

namespace Minesweeper;

public partial class StatisticsWindow
{
    private List<UserStats> _globalStats = [];
    private UserStats? _userStats;

    public StatisticsWindow()
    {
        InitializeComponent();
        Loaded += StatisticsWindow_Loaded;
    }

    private async void StatisticsWindow_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            using (var db = new MinesweeperData())
            {
                // Load global stats
                _globalStats = await db.UserStats
                    .Include(us => us.User)
                    .ToListAsync();

                // Load user stats if logged in
                if (Session.CurrentUser is not null)
                {
                    _userStats = await db.UserStats
                        .Include(us => us.User)
                        .FirstOrDefaultAsync(us => us.UserId == Session.CurrentUser.Id);
                }
            }
            
            UpdateDataGrids();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading statistics: {ex.Message}", "Error", 
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void UpdateDataGrids()
    {
        GlobalStatsDataGrid.ItemsSource = _globalStats;

        if (_userStats is not null)
        {
            UserStatsDataGrid.Visibility = Visibility.Visible;
            UserStatsDataGrid.ItemsSource = new List<UserStats> { _userStats };
        }
        else
            UserStatsDataGrid.Visibility = Visibility.Collapsed;
    }
}