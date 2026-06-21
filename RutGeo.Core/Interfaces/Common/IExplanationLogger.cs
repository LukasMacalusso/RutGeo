namespace RutGeo.Core.Interfaces.Common;

public interface IExplanationLogger
{

    void StartProcess(string title);
    void AppendStep(string stepDetails);
    void AppendEquation(string equation);
    string GetFullLog();
    void Clear();

}
