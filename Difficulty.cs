using Point = System.Drawing.Point;

namespace Minesweeper
{
    public enum DifficultyLevel
    {
        Easy,
        Normal,
        Hard,
        Extreme
    }

    public static class Difficulty
    {
        public static Point GetGridSize(DifficultyLevel difficulty) => difficulty switch
        {
            DifficultyLevel.Easy => new Point(9, 9),
            DifficultyLevel.Normal => new Point(16, 16),
            DifficultyLevel.Hard => new Point(16, 30),
            DifficultyLevel.Extreme => new Point(20, 30),
            _ => throw new Exception("Invalid Difficulty")
        };

        public static Point GetGameBoardSize(Point gridSize) => new(gridSize.X * 32, gridSize.Y * 32);

        public static int GetMineCount(DifficultyLevel difficulty) => difficulty switch
        {
            DifficultyLevel.Easy => 10,
            DifficultyLevel.Normal => 40,
            DifficultyLevel.Hard => 99,
            DifficultyLevel.Extreme => 130,
            _ => throw new Exception("Invalid Difficulty")
        };
    }
}
