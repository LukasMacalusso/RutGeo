using System;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
using RutGeo.UI.ViewModels;

namespace RutGeo.UI.Views;

public partial class MainWindow : Window
{
    private bool _isLimitMode;

    public MainWindow()
    {
        InitializeComponent();

        ModeSelector.OnConicModeRequested += (s, e) => SwitchToConics();
        ModeSelector.OnLimitsRequested += (s, e) => SwitchToLimits();
        InputView.OnToggleLogVisibilityRequested  += (s, e) => ToggledRutLog();
        DataContextChanged += (s, e) =>
        {
            if (DataContext is MainWindowViewModel vm) vm.PropertyChanged += OnViewModelPropertyChanged;
        };
        Closed += (sender, e) => Environment.Exit(0);
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not MainWindowViewModel vm) return;

        if (e.PropertyName == nameof(vm.GeneralEquation) ||
            e.PropertyName == nameof(vm.Conic) ||
            e.PropertyName == nameof(vm.CanonicalEquation))
        {
            if (!_isLimitMode && vm.GeneralEquation != null && vm.Conic != null && vm.CanonicalEquation != null)
            {
                GraphicView.UpdatePlot(vm.GeneralEquation, vm.Conic, vm.CanonicalEquation);
            }
            else if (vm.GeneralEquation == null)
            {
                GraphicView.ClearGraph();
            }
        }

        if (e.PropertyName == nameof(vm.LimitResult))
        {
            if (_isLimitMode && vm.LimitResult != null)
            {
                GraphicView.UpdateLimitPlot(vm.LimitResult.Condition, vm.LimitResult.CriticalPoint, vm.LimitResult.Digits);
            }
            else if (vm.LimitResult == null)
            {
                GraphicView.ClearGraph();
            }
        }
    }

    public void ToggledRutLog()
    {
        LogView.IsVisible = !LogView.IsVisible;
    }

    private void SwitchToConics()
    {
        _isLimitMode = false;
        DescView.SwitchToConics();
        DefenseView.SwitchToConics();
        SolutionsView.SwitchToConics();
        ModeHeader.Text = "CÓNICAS";
        LimitsTable.IsVisible = false;

        if (DataContext is MainWindowViewModel vm && vm.GeneralEquation != null && vm.Conic != null && vm.CanonicalEquation != null)
        {
            GraphicView.UpdatePlot(vm.GeneralEquation, vm.Conic, vm.CanonicalEquation);
        }
        else
        {
            GraphicView.ClearGraph();
        }
    }

    private void SwitchToLimits()
    {
        _isLimitMode = true;
        DescView.SwitchToLimits();
        DefenseView.SwitchToLimits();
        SolutionsView.SwitchToLimits();
        ModeHeader.Text = "LÍMITES";
        LimitsTable.IsVisible = true;

        if (DataContext is MainWindowViewModel vm && vm.LimitResult != null)
        {
            GraphicView.UpdateLimitPlot(vm.LimitResult.Condition, vm.LimitResult.CriticalPoint, vm.LimitResult.Digits);
        }
        else
        {
            GraphicView.ClearGraph();
        }
    }

    private void TogglePanelButton_Click(object? sender, RoutedEventArgs e)
    {
        LeftPanel.IsVisible = !LeftPanel.IsVisible;
    }
}
