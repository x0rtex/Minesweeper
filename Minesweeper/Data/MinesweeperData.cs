using System.Data.Entity;

namespace Minesweeper.Data;

public class MinesweeperData : DbContext
{
    public MinesweeperData() : base("MinesweeperData") {}

    public DbSet<User> Users { get; set; }
    public DbSet<SavedGame> SavedGames { get; set; }
    public DbSet<UserStats> UserStats { get; set; }
}