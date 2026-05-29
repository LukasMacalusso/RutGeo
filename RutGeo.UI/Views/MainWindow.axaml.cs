using Avalonia.Controls;
using Avalonia.Interactivity;

namespace RutGeo.UI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        ModeSelector.OnConicModeRequested += (s, e) => SwitchToConics();
        ModeSelector.OnLimitsRequested += (s, e) => SwitchToLimits();
    }

    private void SwitchToConics()
    {
        DescView.SwitchToConics();
        DefenseView.SwitchToConics();
        GraphicView.SwitchToConics();
        LimitsTable.IsVisible = false;
    }

    private void SwitchToLimits()
    {
        DescView.SwitchToLimits();
        DefenseView.SwitchToLimits();
        GraphicView.SwitchToLimits();
        LimitsTable.IsVisible = true;
    }


    private void TogglePanelButton_Click(object? sender, RoutedEventArgs e)
    {
        LeftPanel.IsVisible = !LeftPanel.IsVisible;
    }
}