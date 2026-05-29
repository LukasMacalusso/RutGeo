using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace RutGeo.UI.Views;

public partial class Mode : UserControl
{
    public event EventHandler? OnConicModeRequested;
    public event EventHandler? OnLimitsRequested;

    public Mode()
    {
        InitializeComponent();
    }

    private void ConicsButton_Click(object? sender, RoutedEventArgs e)
    {
        UpdateButtons(true);
        OnConicModeRequested?.Invoke(this, EventArgs.Empty);
    }

    private void LimitsButton_Click(object? sender, RoutedEventArgs e)
    {
        UpdateButtons(false);
        OnLimitsRequested?.Invoke(this, EventArgs.Empty);
    }

    private void UpdateButtons(bool isConic)
    {
        var activeBrush = Brushes.MediumSlateBlue; 
        var inactiveBrush = Brushes.Transparent; 
        BtnConics.Background = isConic ? activeBrush : inactiveBrush;
        BtnConics.Foreground = isConic ? Brushes.White : Brushes.LightGray;
        BtnLimits.Background = !isConic ? activeBrush : inactiveBrush;
        BtnLimits.Foreground = !isConic ? Brushes.White : Brushes.LightGray;
    }
}
