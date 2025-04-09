using System.Linq;

namespace Minesweeper.Data;

public static class UserStatsUpdater
{
    public static void UpdateUserStats()
    {
        using var db = new MinesweeperData();

        // Group saved games by UserId and calculate stats
        var userStatsData = db.SavedGames
            .Where(sg => sg.UserId != null) // Exclude guest games
            .GroupBy(sg => sg.UserId)
            .Select(group => new
            {
                UserId = group.Key.Value,
                
                TotalGamesPlayed = group
                    .Count(),
                
                Wins = group
                    .Count(sg => sg.GameState == GameState.Won),
                
                Losses = group
                    .Count(sg => sg.GameState == GameState.Lost),
                
                AverageTime = group
                    .Any(sg => sg.ElapsedTime > 0) 
                    ? (int)group
                        .Average(sg => sg.ElapsedTime) 
                    : 0
            })
            .ToList();

        // Update or create UserStats for each user
        foreach (var stats in userStatsData)
        {
            var userStats = db.UserStats.SingleOrDefault(us => us.UserId == stats.UserId);

            if (userStats == null)
            {
                // Create new UserStats record if it doesn't exist
                userStats = new UserStats
                {
                    UserId = stats.UserId,
                    TotalGamesPlayed = stats.TotalGamesPlayed,
                    Wins = stats.Wins,
                    Losses = stats.Losses,
                    AverageTime = stats.AverageTime
                };
                db.UserStats.Add(userStats);
            }
            else
            {
                // Update existing UserStats record
                userStats.TotalGamesPlayed = stats.TotalGamesPlayed;
                userStats.Wins = stats.Wins;
                userStats.Losses = stats.Losses;
                userStats.AverageTime = stats.AverageTime;
            }
        }

        db.SaveChanges();
    }
}