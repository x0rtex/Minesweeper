using System.Collections.Generic;

namespace Minesweeper.Data;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DifficultyLevel SelectedDifficulty { get; set; }

    public virtual List<SavedGame> SavedGames { get; set; }
}