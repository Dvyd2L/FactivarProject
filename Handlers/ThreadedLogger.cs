namespace Handlers;

public abstract class ThreadedLogger : IDisposable
{
    private readonly Queue<Action> _queue = new();
    private readonly AutoResetEvent _hasNewItems = new(false);
    private readonly Thread _loggingThread;
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public ThreadedLogger()
    {
        _loggingThread = new Thread(ProcessQueue)
        {
            IsBackground = true
        };

        _loggingThread.Start();
    }

    private void ProcessQueue()
    {
        while (true)
        {
            _ = _hasNewItems.WaitOne(10000, true);

            Queue<Action> queueCopy;
            lock (_queue)
            {
                queueCopy = new Queue<Action>(_queue);
                _queue.Clear();
            }

            foreach (Action log in queueCopy)
            {
                log();
            }
        }
    }

    public void LogMessage(string message)
    {
        lock (_queue)
        {
            _queue.Enqueue(() => AsyncLogMessage(message));
        }

        _ = _hasNewItems.Set();
    }

    protected abstract void AsyncLogMessage(string message);

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        _loggingThread.Join();
        _hasNewItems.Dispose();
        GC.SuppressFinalize(this);
    }
}