using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace RutGeo.UI.Views;

public partial class Mode : UserControl
{
    public event EventHandler? OnConicModeRequested;
    public event EventHandler? OnLimitsRequested;

    private readonly IBrush _activeBg;
    private readonly IBrush _activeFg;
    private readonly IBrush _inactiveBg;
    private readonly IBrush _inactiveFg;

    public Mode()
    {
        InitializeComponent();
        var resources = Application.Current!.Resources;
        _activeBg = (IBrush)resources["PrimaryLightBrush"]!;
        _activeFg = (IBrush)resources["HeaderForegroundBrush"]!;
        _inactiveBg = (IBrush)resources["InactiveBrush"]!;
        _inactiveFg = (IBrush)resources["InactiveForegroundBrush"]!;
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
        BtnConics.Background = isConic ? _activeBg : _inactiveBg;
        BtnConics.Foreground = isConic ? _activeFg : _inactiveFg;
        BtnLimits.Background = !isConic ? _activeBg : _inactiveBg;
        BtnLimits.Foreground = !isConic ? _activeFg : _inactiveFg;
    }
}
