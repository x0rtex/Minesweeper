using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Point = System.Drawing.Point;

namespace Minesweeper;

public class Cell : Button
{
    public bool IsMine { get; set; }
    public int AdjacentMines { get; set; }
    public Point Position { get; set; }

    public Cell(Point position)
    {
        Position = position;
        Margin = new Thickness(1);
        Background = Brushes.White;
        Foreground = Brushes.Gray;
    }
    
    protected override void OnClick()
    {
        IsEnabled = false;
        
        if (IsMine)
        {
            Background = Brushes.Red;
            Content = "*";
            MessageBox.Show("Game Over!");
        }
        else
        {
            Background = Brushes.LightGray;
            Content = AdjacentMines.ToString();
        }
    }
}