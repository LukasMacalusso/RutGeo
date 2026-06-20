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

        DataContextChanged += (s, e) =>
        {
            if (DataContext is MainWindowViewModel vm) vm.PropertyChanged += OnViewModelPropertyChanged;
        };
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is MainWindowViewModel vm &&
            (e.PropertyName == nameof(vm.GeneralEquation) || 
             e.PropertyName == nameof(vm.Conic) || 
             e.PropertyName == nameof(vm.CanonicalEquation)))
        {
            if (vm.GeneralEquation != null && vm.Conic != null && vm.CanonicalEquation != null)
            {
                GraphicView.UpdatePlot(vm.GeneralEquation, vm.Conic, vm.CanonicalEquation);
            }
        }
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
