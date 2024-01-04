namespace Handlers;

public class AsyncLogger(string logFile) : ThreadedLogger
{
    private readonly string _logFile = logFile;

    protected override void AsyncLogMessage(string message)
    {
        lock (_logFile)
        {
            using (StreamWriter writer = new(_logFile, append: true))
            {
                writer.WriteLine(message);
            }
        }
    }
}
