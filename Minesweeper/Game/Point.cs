namespace Minesweeper;

// Point struct for storing x and y coordinates of a cell
// 'Record' is used for equality based on value instead of reference
public record struct Point(int X, int Y)
{
    public int X { get; } = X;
    public int Y { get; } = Y;
}