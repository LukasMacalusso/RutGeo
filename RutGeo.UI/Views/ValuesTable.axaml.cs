using Avalonia.Controls;
using Avalonia.Interactivity;
namespace RutGeo.UI.Views;

public partial class ValuesTable : UserControl
{
    public ValuesTable()
    {
        InitializeComponent();
    }
    
    private void ButtonClick(object? sender, RoutedEventArgs e)
    {
        TableContentContainer.IsVisible = !TableContentContainer.IsVisible;
        ToggleValuesTable.Content = TableContentContainer.IsVisible ? "Ocultar Tabla" : "Mostrar Tabla";
    }
}