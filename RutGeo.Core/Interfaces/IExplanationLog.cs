namespace RutGeo.Core.Interfaces;

public interface IExplanationLog
{
    
    void StartProcess(string title);
    void AppendStep(string stepDetails);
    void AppendEquation(string equation);
    string GetFullLog();
    void Clear();
    
}