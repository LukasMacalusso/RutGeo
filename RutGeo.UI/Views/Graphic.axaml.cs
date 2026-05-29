using Avalonia.Controls;

namespace RutGeo.UI.Views;

public partial class Graphic : UserControl
{
    private bool _isConicMode = true;

    public Graphic()
    {
        InitializeComponent();
    }

    public void SwitchToConics()
    {
        _isConicMode = true;
    }

    public void SwitchToLimits()
    {
        _isConicMode = false;
    }
}
