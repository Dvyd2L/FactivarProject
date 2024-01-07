namespace Handlers;

public class AsyncLogger(string logFile) : ThreadedLogger
{
    private readonly string _logFile = logFile;

    protected override void AsyncLogMessage(string message)
    {
        lock (_logFile)
        {
            try
            {
                using (StreamWriter writer = new(_logFile, append: true))
                {
                    _ = writer.WriteLineAsync(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Algo salió mal en {nameof(AsyncLogger)}: {ex.Message}");
                //throw;
            }
        }
    }
}
