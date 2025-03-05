using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Point = System.Drawing.Point;

namespace Minesweeper;

public class Cell : Button
{
    public Point Pos { get; }
    public bool IsMine { get; set; }
    public bool IsFlagged { get; set; }  // TODO: Unused IsFlagged property 
    private int _adjacentMines;

    private Cell(Point pos)
    {
        Pos = pos;
        Margin = new Thickness(1);
        Background = Brushes.White;
        FontWeight = FontWeights.Bold;
    }
    
    public static Cell CreateCell(Point position) => new Cell(position);
    
    public int AdjacentMines
    {
        get => _adjacentMines;
        set
        {
            _adjacentMines = value;
            Foreground = GetColor();
        }
    }

    protected override void OnClick()
    {
        CheckCell();
        ((MineGrid)Parent).ClearEmptyAdjacentCells(this);
    }
    
    private void CheckCell()
    {
        if (IsMine)
            ExplodeMineCell();
        else
            ClearEmptyCell();
    }
    
    // TODO: Right click to flag cell

    private void ExplodeMineCell()
    {
        IsEnabled = false;
        Foreground = Brushes.Red;
        Content = "\ud83d\udca5";
        EndGame();
    }

    private void EndGame()
    {
        MessageBox.Show("Game Over!");
        ClearAllCells();
    }

    private void ClearAllCells()
    {
        MineGrid mineGrid = (MineGrid)Parent;
        foreach (Cell cell in mineGrid.Cells)
        {
            if (cell == this)
                continue;
            if (!cell.IsMine)
                cell.ClearEmptyCell();
            else
            {
                cell.IsEnabled = false;
                cell.Foreground = Brushes.Black;
                cell.Content = "\ud83d\udca3";
            }
        }
    }

    public void ClearEmptyCell()
    {
        IsEnabled = false;
        Content = _adjacentMines != 0 ? _adjacentMines.ToString() : "";
    }
    
    private SolidColorBrush GetColor()
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