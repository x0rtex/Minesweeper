namespace Minesweeper.Data;

public class SavedGame
{
    public int Id { get; set; }
    public int ElapsedTime { get; set; }
    public DifficultyLevel Difficulty { get; set; }
    public GameState GameState { get; set; }

    public int UserId { get; set; }
    public virtual User User { get; set; }
}