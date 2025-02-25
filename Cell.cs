using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Point = System.Drawing.Point;

namespace Minesweeper;

public class Cell : Button
{
    public Point Pos { get; }
    public bool IsMine { get; set; }
    public bool IsFlagged { get; set; }
    private int _adjacentMines;

    public Cell(Point pos)
    {
        Pos = pos;
        Margin = new Thickness(1);
        Background = Brushes.White;
        FontWeight = FontWeights.Bold;
    }
    
    public int AdjacentMines
    {
        get => _adjacentMines;
        set
        {
            _adjacentMines = value;
            Foreground = GetColor();
        }
    }

    protected override void OnClick() => CheckCell();

    private void CheckCell()
    {
        if (IsMine)
            ExplodeMineCell();
        else
            ClearEmptyCell();
    }

    private void ExplodeMineCell()
    {
        IsEnabled = false;
        Foreground = Brushes.Red;
        Content = "\ud83d\udca5";
        MessageBox.Show("Game Over!");
    }

    private void ClearEmptyCell()
    {
        IsEnabled = false;
        Content = _adjacentMines.ToString();
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