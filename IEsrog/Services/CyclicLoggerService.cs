using IEsrog.Extensions;

namespace IEsrog.Services;

enum LogLevel
{
    Debug,
    Info,
    Warning,
    Error
}

public class CyclicLoggerService
{
    static readonly Queue<string> Queue = new();
    const int Capacity = 5_000;

    public void LogInformation(string msg) => Log(LogLevel.Info, msg);
    public void LogDebug(string msg) => Log(LogLevel.Debug, msg);
    public void LogWarning(string msg) => Log(LogLevel.Warning, msg);
    public void LogError(string msg) => Log(LogLevel.Error, msg);
    
    void Log(LogLevel level, string message)
    {
        var now = DateTime.Now.ToCaDate();
        Queue.Enqueue($"{now:G} [{level}] {message}");
        if (Queue.Count > Capacity)
        {
            Queue.Dequeue();
        }
    }

    public static string[] GetLogs()
    {
        return Queue.ToArray();
    }
}