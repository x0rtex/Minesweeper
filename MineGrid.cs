using System.Drawing;
using System.Windows.Controls.Primitives;

namespace Minesweeper;

public class MineGrid : UniformGrid
{
    private Point GridDimensions { get; set; }
    private int MineCount { get; set; }
    
    public MineGrid(Point gridDimensions, Point absoluteBoardDimensions, int mineCount)
    {
        GridDimensions = gridDimensions;
        MineCount = mineCount;
        
        Rows = gridDimensions.X;
        Columns = gridDimensions.Y;
        Width = absoluteBoardDimensions.Y;
        Height = absoluteBoardDimensions.X;
    }
    
    public void GenerateCells()
    {
        Random random = new Random();
        List<Point> cellPositions = CreateCellPositions(GridDimensions);
        List<Point> minePositions = cellPositions.OrderBy(_ => random.Next()).Take(MineCount).ToList();
        
        for (int x = 0; x < GridDimensions.X; x++)
        for (int y = 0; y < GridDimensions.Y; y++)
        {
            Cell cell = new Cell(new Point(x, y));
            Children.Add(cell);
            
            if (minePositions.Contains(cell.Position))
                cell.IsMine = true;
        }
    }
    
    private static List<Point> CreateCellPositions(Point gridDimensions)
    {
        List<Point> cellPositions = [];
        for (int x = 0; x < gridDimensions.X; x++)
        for (int y = 0; y < gridDimensions.Y; y++)
            cellPositions.Add(new Point(x, y));
            
        return cellPositions;
    }
}