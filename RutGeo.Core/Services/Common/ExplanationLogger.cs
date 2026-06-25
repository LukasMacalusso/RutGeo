using System.Text;
using RutGeo.Core.Helpers;
using RutGeo.Core.Interfaces.Common;

namespace RutGeo.Core.Services.Common;

public class ExplanationLogger : IExplanationLogger
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
        _logBuilder.AppendLine($"  {SignCleaner.Clean(equation)}");
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
