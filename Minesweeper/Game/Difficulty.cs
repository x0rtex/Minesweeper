using System;

namespace Minesweeper;

public enum DifficultyLevel
{
    Easy,
    Normal,
    Hard,
    Extreme
}

public static class Difficulty
{
    // Define the grid sizes depending on difficulty
    private static readonly Point GridSizeEasy = new(9, 9);
    private static readonly Point GridSizeNormal = new(16, 16);
    private static readonly Point GridSizeHard = new(16, 30);
    private static readonly Point GridSizeExtreme = new(20, 30);

    // Define the mine counts depending on difficulty
    private const int MineCountEasy = 10;
    private const int MineCountNormal = 40;
    private const int MineCountHard = 99;
    private const int MineCountExtreme = 130;

    // Return a tuple holding grid size and mine count depending on difficulty
    public static (Point, int) GetGridSizeAndMineCount(DifficultyLevel difficulty) => difficulty switch
    {
        DifficultyLevel.Easy => (GridSizeEasy, MineCountEasy),
        DifficultyLevel.Normal => (GridSizeNormal, MineCountNormal),
        DifficultyLevel.Hard => (GridSizeHard, MineCountHard),
        DifficultyLevel.Extreme => (GridSizeExtreme, MineCountExtreme),
        _ => throw new Exception("Invalid Difficulty")
    };

    // Return the absolute size of the board in pixels
    public static Point GetAbsoluteBoardSize(Point gridSize) => new(gridSize.X * 32, gridSize.Y * 32);
}