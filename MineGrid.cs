using System.Drawing;
using System.Windows.Controls.Primitives;

namespace Minesweeper;

public class MineGrid : UniformGrid
{
    private readonly Point _gridDimensions;
    private readonly int _mineCount;
    public Cell[,] Cells { get; }

    public MineGrid(Point gridDimensions, Point absoluteBoardDimensions, int mineCount)
    {
        _gridDimensions = gridDimensions;
        _mineCount = mineCount;
        Cells = new Cell[gridDimensions.X, gridDimensions.Y];
        
        Rows = gridDimensions.X;
        Columns = gridDimensions.Y;
        Width = absoluteBoardDimensions.Y;
        Height = absoluteBoardDimensions.X;
    }

    public void PrepareBoard()
    {
        GenerateCells();
        UpdateAllAdjacentMines();
    }

    private void GenerateCells()
    {
        List<Point> cellPositions = CreateAllCellPositions(_gridDimensions);
        List<Point> minePositions = CreateMinePositions(cellPositions);

        for (int x = 0; x < _gridDimensions.X; x++)
        for (int y = 0; y < _gridDimensions.Y; y++)
        {
            Cell cell = new Cell(new Point(x, y));
            Cells[x, y] = cell;
            Children.Add(cell);

            if (minePositions.Contains(cell.Pos))
                cell.IsMine = true;
        }
    }

    private static List<Point> CreateAllCellPositions(Point gridDimensions)
    {
        List<Point> cellPositions = [];
        for (int x = 0; x < gridDimensions.X; x++)
        for (int y = 0; y < gridDimensions.Y; y++)
            cellPositions.Add(new Point(x, y));
            
        return cellPositions;
    }
    
    private List<Point> CreateMinePositions(List<Point> cellPositions)
    {
        Random random = new Random();
        
        return cellPositions
            .OrderBy(_ => random.Next())
            .Take(_mineCount)
            .ToList();
    }

    private void UpdateAllAdjacentMines()
    {
        foreach (Cell cell in Cells)
            UpdateAdjacentMines(cell);
    }
    
    private void UpdateAdjacentMines(Cell cell)
    {
        if (cell.IsMine)
            return;
        
        cell.AdjacentMines = 0;
    
        for (int offsetX = -1; offsetX <= 1; offsetX++)
        for (int offsetY = -1; offsetY <= 1; offsetY++)
        {
            Point adjacent = new Point(cell.Pos.X + offsetX, cell.Pos.Y + offsetY);
            if (IsItselfOrOutsideBoard(cell.Pos, adjacent))
                continue;
            
            Cell adjacentCell = Cells[adjacent.X, adjacent.Y];
            if (adjacentCell.IsMine) 
                cell.AdjacentMines++;
        }
    }

    private bool IsItselfOrOutsideBoard(Point cellPos, Point adj)
    {
        bool isItself = adj.X == cellPos.X && adj.Y == cellPos.Y;
        bool isOutsideBoard = adj.X < 0 || adj.Y < 0 || adj.X >= _gridDimensions.X || adj.Y >= _gridDimensions.Y;
    
        return isItself || isOutsideBoard;
    }
}