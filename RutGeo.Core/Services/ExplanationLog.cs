using System.Text;
using RutGeo.Core.Interfaces;

namespace RutGeo.Core.Services;

public class ExplanationLog : IExplanationLog
{
    private readonly StringBuilder _logBuilder = new StringBuilder();

    public void StartProcess(string title)
    {
        _logBuilder.AppendLine();
        _logBuilder.AppendLine($"--- {title.ToUpper()} ---");
    }

    public void AppendStep(string stepDetails)
    {
        _logBuilder.AppendLine($"• {stepDetails}");
    }

    public void AppendEquation(string equation)
    {
        _logBuilder.AppendLine($"  {equation}");
        _logBuilder.AppendLine();
    }

    public string GetFullLog()
    {
        return _logBuilder.ToString().Trim();
    }

    public void Clear()
    {
        _logBuilder.Clear();
    }
}