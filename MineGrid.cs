using System.Drawing;
using System.Windows.Controls.Primitives;

namespace Minesweeper;

public class MineGrid : UniformGrid
{
    private readonly Point _gridDimensions;
    private readonly int _mineCount;
    private readonly Cell[,] _cells;

    public MineGrid(Point gridDimensions, Point absoluteBoardDimensions, int mineCount)
    {
        _gridDimensions = gridDimensions;
        _mineCount = mineCount;
        _cells = new Cell[gridDimensions.X, gridDimensions.Y];
        
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
        List<Point> cellPositions = CreateCellPositions(_gridDimensions);
        List<Point> minePositions = cellPositions
            .OrderBy(_ => new Random().Next())
            .Take(_mineCount)
            .ToList();
        
        for (int x = 0; x < _gridDimensions.X; x++)
        for (int y = 0; y < _gridDimensions.Y; y++)
        {
            Cell cell = new Cell(new Point(x, y));
            _cells[x, y] = cell;
            Children.Add(cell);
            
            if (minePositions.Contains(cell.Pos))
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

    private void UpdateAllAdjacentMines()
    {
        foreach (Cell cell in _cells)
            UpdateAdjacentMines(cell);
    }
    
    private void UpdateAdjacentMines(Cell cell)
    {
        cell.AdjacentMines = 0;
        
        for (int adjX = cell.Pos.X - 1; adjX <= cell.Pos.X + 1; adjX++)
        for (int adjY = cell.Pos.Y - 1; adjY <= cell.Pos.Y + 1; adjY++)
        {
            if (CellIsItselfOrOutsideBoard(cell.Pos, adjX, adjY))
                continue;
                
            Cell adjacentCell = _cells[adjX, adjY];
            if (adjacentCell.IsMine) 
                cell.AdjacentMines++;
        }
    }

    private bool CellIsItselfOrOutsideBoard(Point cellPos, int adjX, int adjY)
    {
        bool cellIsItself = adjX == cellPos.X && adjY == cellPos.Y;
        bool cellIsOutsideBoard = adjX < 0 || adjY < 0 || adjX >= _gridDimensions.X || adjY >= _gridDimensions.Y;
        
        return cellIsItself || cellIsOutsideBoard;
    }
}