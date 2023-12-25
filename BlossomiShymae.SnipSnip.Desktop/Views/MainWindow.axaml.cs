using Avalonia.Controls;

namespace BlossomiShymae.SnipSnip.Desktop.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        var console = this.FindControl<ScrollViewer>("ConsoleViewer");
        if (console != null)
        {
            // Tail output of console
            console.PropertyChanged += (s, e) =>
            {
                console.ScrollToEnd();
            };
        }
    }
}