using System.Windows;

namespace Minesweeper;

public class FlagToggledEventArgs : RoutedEventArgs
{
    public bool IsFlagged { get; set; }
    
    public FlagToggledEventArgs(RoutedEvent routedEvent, bool isFlagged) : base(routedEvent)
    {
        IsFlagged = isFlagged;
    }
}