using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using RutGeo.Core.Models.Results;
using RutGeo.Core.Models.Types;

namespace RutGeo.UI.ViewModels;

public partial class LimitDefenseViewModel : ObservableObject
{
    [ObservableProperty] private string _userLeftLimit = string.Empty;
    [ObservableProperty] private string _userRightLimit = string.Empty;
    [ObservableProperty] private string _userLimitExists = string.Empty;
    [ObservableProperty] private string _userValueFA = string.Empty;
    [ObservableProperty] private string _userContinuous = string.Empty;
    [ObservableProperty] private string _userDiscontinuity = string.Empty;

    [ObservableProperty] private string _leftLimitStatus = string.Empty;
    [ObservableProperty] private string _rightLimitStatus = string.Empty;
    [ObservableProperty] private string _limitExistsStatus = string.Empty;
    [ObservableProperty] private string _valueFAStatus = string.Empty;
    [ObservableProperty] private string _continuousStatus = string.Empty;
    [ObservableProperty] private string _discontinuityStatus = string.Empty;
    [ObservableProperty] private string _limitJustificationStatus = string.Empty;

    [ObservableProperty] private string _expectedLeftLimit = "";
    [ObservableProperty] private string _expectedRightLimit = "";
    [ObservableProperty] private string _expectedLimitExists = "";
    [ObservableProperty] private string _expectedValueFA = "";
    [ObservableProperty] private string _expectedContinuous = "";
    [ObservableProperty] private string _expectedDiscontinuity = "";
    [ObservableProperty] private string _expectedJustification = "";

    public List<string> YesNoOptions { get; } = new() { "Sí", "No" };
    public List<string> DiscontinuityOptions { get; } = new() { "Removible", "De Salto", "Infinita", "Ninguna" };

    private LimitAnalysisResult? _expected;

    public void SetExpectedValues(LimitAnalysisResult? result)
    {
        _expected = result;
        if (_expected != null)
        {
            ExpectedLeftLimit = _expected.LeftLimit;
            ExpectedRightLimit = _expected.RightLimit;
            ExpectedLimitExists = _expected.LimitExists ? "Sí" : "No";
            ExpectedValueFA = _expected.FunctionValueAtCriticalPoint;
            ExpectedContinuous = _expected.IsContinuous ? "Sí" : "No";
            ExpectedDiscontinuity = DiscontinuityLabel();
            ExpectedJustification = _expected.Justification;
        }
        else
        {
            ExpectedLeftLimit = ExpectedRightLimit = ExpectedLimitExists = "";
            ExpectedValueFA = ExpectedContinuous = ExpectedDiscontinuity = ExpectedJustification = "";
        }
    }

    public void Corroborate()
    {
        if (_expected == null)
        {
            LeftLimitStatus = RightLimitStatus = LimitExistsStatus = "";
            ValueFAStatus = ContinuousStatus = DiscontinuityStatus = "";
            LimitJustificationStatus = "";
            return;
        }

        LeftLimitStatus = Compare(UserLeftLimit, _expected.LeftLimit) ? "✓" : "✗";
        RightLimitStatus = Compare(UserRightLimit, _expected.RightLimit) ? "✓" : "✗";
        LimitExistsStatus = Compare(UserLimitExists, _expected.LimitExists ? "Sí" : "No") ? "✓" : "✗";
        ValueFAStatus = Compare(UserValueFA, _expected.FunctionValueAtCriticalPoint) ? "✓" : "✗";
        ContinuousStatus = Compare(UserContinuous, _expected.IsContinuous ? "Sí" : "No") ? "✓" : "✗";
        DiscontinuityStatus = Compare(UserDiscontinuity, DiscontinuityLabel()) ? "✓" : "✗";
        LimitJustificationStatus = "✓";
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

    private static bool Compare(string user, string expected)
    {
        if (string.IsNullOrWhiteSpace(user)) return false;
        return user.Trim().ToLowerInvariant().Replace(" ", "")
            == expected.Trim().ToLowerInvariant().Replace(" ", "");
    }
}
