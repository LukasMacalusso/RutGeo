using Avalonia.Controls;

namespace RutGeo.UI.Views;

public partial class Description : UserControl
{
    public Description()
    {
        InitializeComponent();
    }

    public void SwitchToConics()
    {
        ConicContent.IsVisible = true;
        LimitContent.IsVisible = false;
    }

    public void SwitchToLimits()
    {
        ConicContent.IsVisible = false;
        LimitContent.IsVisible = true;
    }
}
