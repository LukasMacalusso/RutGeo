using System;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
using RutGeo.UI.ViewModels;

namespace RutGeo.UI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        ModeSelector.OnConicModeRequested += (s, e) => SwitchToConics();
        ModeSelector.OnLimitsRequested += (s, e) => SwitchToLimits();
        InputView.OnToggleLogVisibilityRequested  += (s, e) => ToggledRutLog();
        Closed += (sender, e) => Environment.Exit(0);
    }
    
    public void ToggledRutLog()
    {
        LogView.IsVisible = !LogView.IsVisible;
    }
    private void SwitchToConics()
    {
        DescView.SwitchToConics();
        DefenseView.SwitchToConics();
        ModeHeader.Text = "CÓNICAS";
        LimitsTable.IsVisible = false;
    }

    private void SwitchToLimits()
    {
        DescView.SwitchToLimits();
        DefenseView.SwitchToLimits();
        ModeHeader.Text = "LÍMITES";
        LimitsTable.IsVisible = true;
    }

    private void TogglePanelButton_Click(object? sender, RoutedEventArgs e)
    {
        LeftPanel.IsVisible = !LeftPanel.IsVisible;
    }
}
