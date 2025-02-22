using System.Drawing;
using System.Windows.Controls.Primitives;

namespace Minesweeper;

public class MineGrid : UniformGrid
{
    private readonly Point _gridDimensions;
    private readonly int _mineCount;
    private readonly Cell[,] _cells;
    private List<Point> _cellPositions = [];
    private List<Point> _minePositions = [];
    
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
        Random random = new Random();
        _cellPositions = CreateCellPositions(_gridDimensions);
        _minePositions = _cellPositions.OrderBy(_ => random.Next()).Take(_mineCount).ToList();
        
        for (int x = 0; x < _gridDimensions.X; x++)
        for (int y = 0; y < _gridDimensions.Y; y++)
        {
            Cell cell = new Cell(new Point(x, y));
            _cells[x, y] = cell;
            Children.Add(cell);
            
            if (_minePositions.Contains(cell.Pos))
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

    private void UpdateAdjacentMines(Cell cell)
    {
        cell.AdjacentMines = 0;
        
        for (int adjX = cell.Pos.X - 1; adjX <= cell.Pos.X + 1; adjX++)
        for (int adjY = cell.Pos.Y - 1; adjY <= cell.Pos.Y + 1; adjY++)
        {
            if (CellIsOutsideBoardOrItself(adjX, adjY, cell.Pos))
                continue;
                
            Cell adjacentCell = _cells[adjX, adjY];
            if (adjacentCell.IsMine)
                    cell.AdjacentMines++;
        }
    }

    private bool CellIsOutsideBoardOrItself(int adjX, int adjY, Point cellPos)
    {
        return adjX < 0 
               || adjX >= _gridDimensions.X 
               || adjY < 0 
               || adjY >= _gridDimensions.Y 
               || (adjX == cellPos.X && adjY == cellPos.Y);
    }

    private void UpdateAllAdjacentMines()
    {
        foreach (Cell cell in _cells)
            UpdateAdjacentMines(cell);
    }
}