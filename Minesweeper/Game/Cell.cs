using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Minesweeper;

public class Cell : Button
{
    public Point Pos { get; }
    public bool IsMine { get; set; }
    public bool IsFlagged { get; set; }  // TODO: Unused IsFlagged property
    private int _adjacentMines;

    // Get or set the number of adjacent mines
    public int AdjacentMines
    {
        get => _adjacentMines;
        set
        {
            _adjacentMines = value;
            Foreground = GetColour();
        }
    }

    // Constructor
    private Cell(Point pos)
    {
        Pos = pos;
        Margin = new Thickness(1);
        FontSize = 18;
        Background = Brushes.White;
        FontWeight = FontWeights.Bold;
    }

    // Returns a new cell
    public static Cell CreateCell(Point position) => new(position);

    // Handle cell click
    protected override void OnClick()
    {
        // TODO: First click must be safe (i.e. no mine)
        CheckCell();
        ((MineGrid)Parent).RevealEmptyAdjacentCells(this);
    }

    // Checks if a cell is a mine or not
    private void CheckCell()
    {
        if (IsMine)
            ExplodeMineCell();
        else
            RevealEmptyCell();
    }

    // TODO: Right click to flag cell

    // Explode a cell with mine
    private void ExplodeMineCell()
    {
        IsEnabled = false;
        Foreground = Brushes.Red;
        Content = "\ud83d\udca5";
        EndGame();
    }

    // Show game over and reveal all cells
    private void EndGame()
    {
        MessageBox.Show("Game Over!");
        RevealAllCells();
    }

    // Reveal all cells on the board
    // I'm unsure about using the 'Parent' keyword to access the MineGrid
    // instance, there may be a better way to manage this relationship
    private void RevealAllCells()
    {
        MineGrid mineGrid = (MineGrid)Parent;
        foreach (Cell cell in mineGrid.Cells)
        {
            if (cell == this)
                continue;
            if (!cell.IsMine)
                cell.RevealEmptyCell();
            else
            {
                cell.IsEnabled = false;
                cell.Foreground = Brushes.Black;
                cell.Content = "\ud83d\udca3";
            }
        }
    }

    // Reveals an empty cell (i.e. no mine)
    public void RevealEmptyCell()
    {
        IsEnabled = false;
        Content = _adjacentMines != 0 ? _adjacentMines.ToString() : "";
    }

    // Return a text colour based on the number of adjacent mines
    private SolidColorBrush GetColour()
    {
        if (IsMine)
            return Brushes.Black;

        return _adjacentMines switch
        {
            1 => Brushes.Blue,
            2 => Brushes.Green,
            3 => Brushes.Red,
            4 => Brushes.Purple,
            5 => Brushes.Maroon,
            6 => Brushes.Turquoise,
            7 => Brushes.Black,
            8 => Brushes.Gray,
            _ => Brushes.Black
        };
    }
}