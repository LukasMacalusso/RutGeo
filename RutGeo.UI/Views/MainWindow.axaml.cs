using System;
using System.ComponentModel;
using Avalonia.Controls;
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

        if (e.PropertyName == nameof(ConicAnalysisViewModel.GeneralEquation) ||
            e.PropertyName == nameof(ConicAnalysisViewModel.Conic) ||
            e.PropertyName == nameof(ConicAnalysisViewModel.CanonicalEquation))
        {
            if (!_isLimitMode && vm.ConicAnalysisVM.GeneralEquation != null && vm.ConicAnalysisVM.Conic != null && vm.ConicAnalysisVM.CanonicalEquation != null)
            {
                GraphicView.UpdatePlot(vm.ConicAnalysisVM.GeneralEquation, vm.ConicAnalysisVM.Conic, vm.ConicAnalysisVM.CanonicalEquation);
            }
            else if (vm.ConicAnalysisVM.GeneralEquation == null)
            {
                GraphicView.ClearGraph();
            }
        }

        if (e.PropertyName == nameof(LimitAnalysisViewModel.LimitResult))
        {
            if (_isLimitMode && vm.LimitAnalysisVM.LimitResult != null)
            {
                GraphicView.UpdateLimitPlot(vm.LimitAnalysisVM.LimitResult.Condition, vm.LimitAnalysisVM.LimitResult.CriticalPoint, vm.LimitAnalysisVM.LimitResult.Digits);
            }
            else if (vm.LimitAnalysisVM.LimitResult == null)
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
        AnalysisView.SwitchToConics();
        DefenseView.SwitchToConics();
        SolutionsView.SwitchToConics();
        ModeHeader.Text = "CÓNICAS";
        ValuesTableView.IsVisible = false;

        if (DataContext is MainWindowViewModel vm && vm.ConicAnalysisVM.GeneralEquation != null && vm.ConicAnalysisVM.Conic != null && vm.ConicAnalysisVM.CanonicalEquation != null)
        {
            GraphicView.UpdatePlot(vm.ConicAnalysisVM.GeneralEquation, vm.ConicAnalysisVM.Conic, vm.ConicAnalysisVM.CanonicalEquation);
        }
        else
        {
            GraphicView.ClearGraph();
        }
    }

    private void SwitchToLimits()
    {
        _isLimitMode = true;
        AnalysisView.SwitchToLimits();
        DefenseView.SwitchToLimits();
        SolutionsView.SwitchToLimits();
        ModeHeader.Text = "LÍMITES";
        ValuesTableView.IsVisible = true;

        if (DataContext is MainWindowViewModel vm && vm.LimitAnalysisVM.LimitResult != null)
        {
            GraphicView.UpdateLimitPlot(vm.LimitAnalysisVM.LimitResult.Condition, vm.LimitAnalysisVM.LimitResult.CriticalPoint, vm.LimitAnalysisVM.LimitResult.Digits);
        }
        else
        {
            GraphicView.ClearGraph();
        }
    }

}
