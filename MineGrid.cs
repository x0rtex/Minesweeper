using System.Drawing;
using System.Windows.Controls.Primitives;

namespace Minesweeper;

public class MineGrid : UniformGrid
{
    private readonly Point _gridDimensions;
    private readonly int _mineCount;
    public Cell[,] Cells { get; }

    // Constructor
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
    
    // Generates cells and updates their adjacent mines
    public void PrepareBoard()
    {
        GenerateCells();
        UpdateAllAdjacentMines();
    }

    // Generates all cells
    private void GenerateCells()
    {
        List<Point> cellPositions = CreateAllCellPositions(_gridDimensions);
        List<Point> minePositions = CreateMinePositions(cellPositions);

        for (int x = 0; x < _gridDimensions.X; x++)
        for (int y = 0; y < _gridDimensions.Y; y++)
        {
            Cell cell = Cell.CreateCell(new Point(x, y));
            Cells[x, y] = cell;
            Children.Add(cell);

            if (minePositions.Contains(cell.Pos))
                cell.IsMine = true;
        }
    }

    // Creates a list of all cell positions
    private static List<Point> CreateAllCellPositions(Point gridDimensions)
    {
        List<Point> cellPositions = [];
        for (int x = 0; x < gridDimensions.X; x++)
        for (int y = 0; y < gridDimensions.Y; y++)
            cellPositions.Add(new Point(x, y));
            
        return cellPositions;
    }
    
    // Creates a list of random mine positions
    private List<Point> CreateMinePositions(List<Point> cellPositions)
    {
        Random random = new Random();
        
        return cellPositions
            .OrderBy(_ => random.Next())
            .Take(_mineCount)
            .ToList();
    }

    // Updates the number of adjacent mines for all cells
    private void UpdateAllAdjacentMines()
    {
        foreach (Cell cell in Cells)
            UpdateAdjacentMines(cell);
    }
    
    // Updates the number of adjacent mines for a given cell
    private void UpdateAdjacentMines(Cell cell)
    {
        if (cell.IsMine)
            return;
        
        cell.AdjacentMines = 0;
        HashSet<Cell> adjacentCells = GetAdjacentCells(cell);
        foreach (Cell _ in adjacentCells.Where(adjacentCell => adjacentCell.IsMine))
            cell.AdjacentMines++;
    }

    // Recursively reveals cells with 0 adjacent mines, and their adjacent cells
    public void RevealEmptyAdjacentCells(Cell cell)
    {
        foreach (Cell adjCell in GetAdjacentCells(cell)
                     .Where(a => a is { IsMine: false, IsEnabled: true })
                )
        {
            if (adjCell.AdjacentMines == 0)
            {
                adjCell.RevealEmptyCell();
                RevealEmptyAdjacentCells(adjCell);
            }

            if (cell.AdjacentMines != 0) 
                continue;
        
            adjCell.RevealEmptyCell();
        }
    }

    // Returns the adjacent cells of a given cell
    private HashSet<Cell> GetAdjacentCells(Cell cell)
    {
        HashSet<Cell> adjacentCells = [];
        
        for (int offsetX = -1; offsetX <= 1; offsetX++)
        for (int offsetY = -1; offsetY <= 1; offsetY++)
        {
            Point adjacent = new Point(cell.Pos.X + offsetX, cell.Pos.Y + offsetY);
            if (IsItselfOrOutsideBoard(cell.Pos, adjacent))
                continue;
            
            adjacentCells.Add(Cells[adjacent.X, adjacent.Y]);
        }

        return adjacentCells;
    }

    // Checks if a cell is itself or lies outside the board
    private bool IsItselfOrOutsideBoard(Point cellPos, Point adj)
    {
        bool isItself = adj.X == cellPos.X && adj.Y == cellPos.Y;
        bool isOutsideBoard = adj.X < 0 || adj.Y < 0 || adj.X >= _gridDimensions.X || adj.Y >= _gridDimensions.Y;
    
        return isItself || isOutsideBoard;
    }
}