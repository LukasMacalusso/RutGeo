using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace RutGeo.UI.Views;

public partial class RutInputView : UserControl
{
    public event EventHandler? OnToggleLogVisibilityRequested;

    public RutInputView()
    {
        InitializeComponent();
    }
    
    private void ToggleLogButton_Click(object? sender, RoutedEventArgs e)
    {
        OnToggleLogVisibilityRequested?.Invoke(this, EventArgs.Empty);
    }
}