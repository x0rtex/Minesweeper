﻿using System.Collections.Generic;
using System;
using System.Linq;
using System.Windows.Controls.Primitives;

namespace Minesweeper;

public class MineGrid : UniformGrid
{
    private readonly Point _gridDimensions;
    private readonly int _mineCount;

    // 2D array of cells
    public Cell[,] Cells { get; }

    // Constructor
    public MineGrid(Point gridDimensions, Point absoluteBoardDimensions, int mineCount)
    {
        _gridDimensions = gridDimensions;
        _mineCount = mineCount;
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
        UpdateAllAdjacentMines();
    }

    // Generates all cells based on grid dimensions
    // I changed List to HashSet - each cell is unique,
    // thus retrieval with hash table is faster. 
    private void GenerateCells()
    {
        HashSet<Point> cellPositions = CreateAllCellPositions(_gridDimensions);
        HashSet<Point> minePositions = CreateMinePositions(cellPositions);

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

    // Creates a HashSet of all cell positions
    private static HashSet<Point> CreateAllCellPositions(Point gridDimensions)
    {
        HashSet<Point> cellPositions = [];
        for (int x = 0; x < gridDimensions.X; x++)
            for (int y = 0; y < gridDimensions.Y; y++)
                cellPositions.Add(new Point(x, y));

        return cellPositions;
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

    // Reveals empty cells around a given cell that was clicked
    // Recursively reveals cells with 0 adjacent mines, as well as their adjacent cells
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

    // Returns the adjacent cells of a given cell.
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
    private bool IsItselfOrOutsideBoard(Point cellPos, Point adj)
    {
        bool isItself = adj.X == cellPos.X && adj.Y == cellPos.Y;
        bool isOutsideBoard = adj.X < 0 
                              || adj.Y < 0 
                              || adj.X >= _gridDimensions.X 
                              || adj.Y >= _gridDimensions.Y;

        return isItself || isOutsideBoard;
    }
}