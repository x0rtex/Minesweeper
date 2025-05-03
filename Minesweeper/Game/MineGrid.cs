using System.Collections.Generic;
using System;
using System.Linq;
using System.Windows.Controls.Primitives;

namespace Minesweeper;

public class MineGrid : UniformGrid
{
    private readonly Point _gridDimensions;
    private readonly int _mineCount;
    private readonly GameWindow _gameWindow;
    
    public Cell[,] Cells { get; }  // 2D array of cells
    public bool MinesGenerated { get; private set; }

    // Constructor
    public MineGrid(Point gridDimensions, Point absoluteBoardDimensions, int mineCount, GameWindow gameWindow)
    {
        _gridDimensions = gridDimensions;
        _mineCount = mineCount;
        _gameWindow = gameWindow;
        Cells = new Cell[gridDimensions.X, gridDimensions.Y];

        Rows = gridDimensions.X;
        Columns = gridDimensions.Y;

        // Absolute width and height in pixels
        Width = absoluteBoardDimensions.Y;
        Height = absoluteBoardDimensions.X;
    }

    // Generates cells and updates their adjacent mines
    public void PrepareBoard()
    {
        GenerateCells();
        // UpdateAllAdjacentMines();
    }
    
    public void GenerateMinesExceptFirstClick(Point firstClick)
    {
        HashSet<Point> allCellPositions = CreateAllCellPositions(_gridDimensions);
        
        allCellPositions.ExceptWith(GetAdjacentCells(Cells[firstClick.X, firstClick.Y])
            .Select(c => c.Pos)
            .Append(firstClick));
        
        HashSet<Point> minePositions = CreateMinePositions(allCellPositions);
        
        foreach (Point pos in minePositions)
            Cells[pos.X, pos.Y].IsMine = true;
        
        UpdateAllAdjacentMines();

        MinesGenerated = true;
    }

    // Generates all cells based on grid dimensions
    private void GenerateCells()
    {
        for (int x = 0; x < _gridDimensions.X; x++)
            for (int y = 0; y < _gridDimensions.Y; y++)
            {
                Cell cell = Cell.CreateCell(new Point(x, y), _gameWindow, this);
                Cells[x, y] = cell;
                Children.Add(cell);
            }
    }

    // Creates a HashSet of all cell positions
    private HashSet<Point> CreateAllCellPositions(Point gridDimensions)
    {
        return
        [
            ..from x in Enumerable.Range(0, _gridDimensions.X)
            from y in Enumerable.Range(0, _gridDimensions.Y)
            select new Point(x, y)
        ];
    }

    // Creates a HashSet of random mine positions based on all cell positions
    private HashSet<Point> CreateMinePositions(HashSet<Point> cellPositions)
    {
        Random random = new();

        return cellPositions
            .OrderBy(_ => random.Next())
            .Take(_mineCount)
            .ToHashSet();
    }

    // Updates the visual number of adjacent mines for all cells
    private void UpdateAllAdjacentMines()
    {
        foreach (Cell cell in Cells)
            UpdateAdjacentMines(cell);
    }

    // Updates the visual number of adjacent mines for a given cell
    private void UpdateAdjacentMines(Cell cell)
    {
        if (cell.IsMine)
            return;

        cell.AdjacentMines = 0;
        HashSet<Cell> adjacentCells = GetAdjacentCells(cell);
        foreach (Cell _ in adjacentCells.Where(adjacentCell => adjacentCell.IsMine))
            cell.AdjacentMines++;
    }

    // Reveal empty cells around a given cell that was clicked
    // Recursively reveal cells with 0 adjacent mines, as well as their adjacent cells
    public void RevealEmptyAdjacentCells(Cell cell)
    {
        // Checking against struct, suggested by Rider IDE
        var unrevealedSafeCells = GetAdjacentCells(cell)
            .Where(a => a is { IsMine: false, IsEnabled: true });

        foreach (Cell adjCell in unrevealedSafeCells)
        {
            if (adjCell.AdjacentMines == 0)
            {
                ForceUnFlagAndRevealCell(adjCell);
                RevealEmptyAdjacentCells(adjCell);
            }

            if (cell.AdjacentMines != 0)
                continue;

            ForceUnFlagAndRevealCell(adjCell);
        }
    }

    private static void ForceUnFlagAndRevealCell(Cell cell)
    {
        if (cell.IsFlagged)
            cell.UnFlagCell();

        cell.RevealEmptyCell();
    }

    // Return the adjacent cells of a given cell.
    private HashSet<Cell> GetAdjacentCells(Cell cell)
    {
        HashSet<Cell> adjacentCells = [];

        for (int offsetX = -1; offsetX <= 1; offsetX++)
            for (int offsetY = -1; offsetY <= 1; offsetY++)
            {
                Point adjacent = new(cell.Pos.X + offsetX, cell.Pos.Y + offsetY);
                if (IsItselfOrOutsideBoard(cell.Pos, adjacent))
                    continue;

                adjacentCells.Add(Cells[adjacent.X, adjacent.Y]);
            }

        return adjacentCells;
    }

    // Checks if a cell is itself or lies outside the board
    private bool IsItselfOrOutsideBoard(Point cellPos, Point adjacent)
    {
        return cellPos == adjacent
               || adjacent.X < 0 
               || adjacent.Y < 0 
               || adjacent.X >= _gridDimensions.X 
               || adjacent.Y >= _gridDimensions.Y;
    }
    
    // Reveals remaining cells that are not flagged and not mines
    public void RevealRemainingCells()
    {
        for (int x = 0; x < Cells.GetLength(0); x++)
            for (int y = 0; y < Cells.GetLength(1); y++)
            {
                Cell cell = Cells[x, y];
                if (cell is { IsMine: false, IsFlagged: false, IsEnabled: true })
                    cell.RevealEmptyCell();
            }
    }
}