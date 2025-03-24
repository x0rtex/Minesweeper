using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Minesweeper;

public class Cell : Button
{
    public Point Pos { get; }
    public bool IsMine { get; set; }
    private int _adjacentMines;

    // Get or set the number of adjacent mines
    public int AdjacentMines
    {
        get => _adjacentMines;
        set
        {
            _adjacentMines = value;
            UpdateColour();
        }
    }
    
    // Get or set (and invoke event) if the cell is flagged
    private bool _isFlagged;
    public bool IsFlagged
    {
        get => _isFlagged;
        private set
        {
            if (_isFlagged == value) 
                return;
            
            _isFlagged = value;
            FlagToggledEventArgs flagEvent = new(FlagToggledEvent, _isFlagged);
            RaiseEvent(flagEvent);
        }
    }
    
    // Register event that is raised when a cell is flagged
    public static readonly RoutedEvent FlagToggledEvent = 
        EventManager.RegisterRoutedEvent(
            "FlagToggled", 
            RoutingStrategy.Bubble, 
            typeof(RoutedEventHandler), 
            typeof(Cell)
            );

    // Constructor
    private Cell(Point pos)
    {
        Pos = pos;
        Margin = new Thickness(1);
        FontSize = 22;
        Background = Brushes.White;
        FontWeight = FontWeights.Bold;
    }

    // Returns a new cell
    public static Cell CreateCell(Point position) => new(position);

    // Checks if a cell is a mine or not
    private void CheckCell()
    {
        if (IsMine)
            ExplodeMineCell();
        else
            RevealEmptyCell();
    }
    
    // Handles left-clicking a cell
    protected override void OnClick()
    {
        // TODO: First click must be safe (i.e. no mine)
        CheckCell();
        ((MineGrid)Parent).RevealEmptyAdjacentCells(this);
    }
    
    // Handles right-clicking a cell
    protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
    {
        base.OnMouseRightButtonUp(e);
        TryFlagCell();
        e.Handled = true;
    }

    // Try to flag or un-flag a cell
    private void TryFlagCell()
    {
        if (IsFlagged)
            UnFlagCell();
        else
            FlagCell();
        
        IsFlagged = !IsFlagged;
    }
    
    // Flag an un-flagged cell
    private void FlagCell()
    {
        Content = "\ud83d\udea9";
        Foreground = Brushes.Red;
    }

    // Un-flag a flagged cell
    public void UnFlagCell()
    {
        Content = "";
        UpdateColour();
    }

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

    private void UpdateColour() => Foreground = GetColour();
    
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