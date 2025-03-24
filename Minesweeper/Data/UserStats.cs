namespace Minesweeper.Data;

public class UserStats
{
    public int Id { get; set; }
    public int TotalGamesPlayed { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }
    public int AverageTime { get; set; }

    public int UserId { get; set; }
    public virtual User User { get; set; }
}