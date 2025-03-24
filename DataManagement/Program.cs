using System;
using Minesweeper;
using Minesweeper.Data;

namespace DataManagement;

internal class Program
{
    private static void Main(string[] args)
    {
        MinesweeperData db = new();

        using (db)
        {
            User user1 = new User()
            {
                Id = 1,
                Name = "Alekss",
                SelectedDifficulty = DifficultyLevel.Extreme,
            };

            SavedGame game1 = new SavedGame()
            {
                Id = 1,
                GameState = GameState.Won,
                Difficulty = DifficultyLevel.Extreme,
                ElapsedTime = 642,
                UserId = user1.Id,
                User = user1
            };

            user1.SavedGames = [game1];

            UserStats user1Stats = new UserStats()
            {
                Id = 1,
                AverageTime = 642,
                Wins = 1,
                Losses = 0,
                TotalGamesPlayed = 1,
                UserId = user1.Id,
                User = user1
            };

            db.Users.Add(user1);
            Console.WriteLine(@"Added user 1 to database.");
            db.SavedGames.Add(game1);
            Console.WriteLine(@"Added game 1 to database.");
            db.UserStats.Add(user1Stats);
            Console.WriteLine(@"Added user 1 stats to database.");
            db.SaveChanges();
            Console.WriteLine(@"Database changes saved.");
        }
    }
}