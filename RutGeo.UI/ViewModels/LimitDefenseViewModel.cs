using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RutGeo.Core.Models.Results;
using RutGeo.Core.Models.Types;

namespace RutGeo.UI.ViewModels;

public partial class LimitDefenseViewModel : ObservableObject
{
    public DefenseField LeftLimit { get; } = new();
    public DefenseField RightLimit { get; } = new();
    public DefenseField LimitExists { get; } = new();
    public DefenseField ValueFA { get; } = new();
    public DefenseField Continuous { get; } = new();
    public DefenseField Discontinuity { get; } = new();

    [ObservableProperty] private bool _isDiscontinuityVisible = true;

    [ObservableProperty] private string _expectedJustification = "";

    public LimitDefenseViewModel()
    {
        Continuous.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(DefenseField.UserValue))
                IsDiscontinuityVisible = string.IsNullOrEmpty(Continuous.UserValue) || Continuous.UserValue != "Sí";
        };
    }

    public List<string> YesNoOptions { get; } = new() { "Sí", "No" };
    public List<string> DiscontinuityOptions { get; } = new() { "Removible", "De Salto", "Infinita", "Ninguna" };

    private LimitAnalysisResult? _expected;

    public void SetExpectedValues(LimitAnalysisResult? result)
    {
        _expected = result;

        if (result == null)
        {
            LeftLimit.Clear();
            RightLimit.Clear();
            LimitExists.Clear();
            ValueFA.Clear();
            Continuous.Clear();
            Discontinuity.Clear();
            ExpectedJustification = "";
            return;
        }

        LeftLimit.ExpectedValue = result.LeftLimit;
        RightLimit.ExpectedValue = result.RightLimit;
        LimitExists.ExpectedValue = result.LimitExists ? "Sí" : "No";
        ValueFA.ExpectedValue = result.FunctionValueAtCriticalPoint;
        Continuous.ExpectedValue = result.IsContinuous ? "Sí" : "No";
        Discontinuity.ExpectedValue = DiscontinuityLabel();
        ExpectedJustification = result.Justification;
    }

    [RelayCommand]
    private void SetLeftMinusInfinity() => LeftLimit.UserValue = "-∞";

    [RelayCommand]
    private void SetLeftPlusInfinity() => LeftLimit.UserValue = "+∞";

    [RelayCommand]
    private void SetRightMinusInfinity() => RightLimit.UserValue = "-∞";

    [RelayCommand]
    private void SetRightPlusInfinity() => RightLimit.UserValue = "+∞";

    public void Corroborate()
    {
        LeftLimit.Corroborate();
        RightLimit.Corroborate();
        LimitExists.Corroborate();
        ValueFA.Corroborate();
        Continuous.Corroborate();
        Discontinuity.Corroborate();
    }

    private string DiscontinuityLabel()
    {
        if (_expected == null) return "";
        return _expected.DiscontinuityType switch
        {
            DiscontinuityType.Removable => "Removible",
            DiscontinuityType.Jump => "De Salto",
            DiscontinuityType.Infinite => "Infinita",
            _ => "Ninguna"
        };
    }
}
