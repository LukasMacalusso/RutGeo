using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
namespace RutGeo.UI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void ConicsButton_Click(object? sender, RoutedEventArgs e)
    {
        ConicsTabs.IsVisible = true;
        LimitsTabs.IsVisible = false;
        LimitsTable.IsVisible = false; 
        
        BtnConics.Background = new SolidColorBrush(Color.Parse("#007ACC"));
        BtnConics.Foreground = Brushes.White;

        BtnLimits.Background = Brushes.Transparent;
        BtnLimits.Foreground = Brushes.LightGray;
    }
    
    private void LimitsButton_Click(object? sender, RoutedEventArgs e)
    {
        ConicsTabs.IsVisible = false;
        LimitsTabs.IsVisible = true;
        LimitsTable.IsVisible = true; 
        
        BtnLimits.Background = new SolidColorBrush(Color.Parse("#007ACC"));
        BtnLimits.Foreground = Brushes.White;
        
        BtnConics.Background = Brushes.Transparent;
        BtnConics.Foreground = Brushes.LightGray;
    }
}