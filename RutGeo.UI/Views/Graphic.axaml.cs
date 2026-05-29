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
        // Cuando se conecte el Core, aquí se llamará a dibujar la cónica
    }

    public void SwitchToLimits()
    {
        _isConicMode = false;
        // Cuando se conecte el Core, aquí se llamará a dibujar la función por tramos
    }
}
