using System.Collections.Concurrent;

namespace Luthetus.Ide.ClassLib.CompilerServices.ParserTaskCase;

public class ParserTaskQueue : IParserTaskQueue
{
    private readonly ConcurrentQueue<IParserTask> _parserTasks = new();
    private readonly SemaphoreSlim _workItemsQueueSemaphoreSlim = new(0);

    public void QueueParserWorkItem(
        IParserTask parserTask)
    {
        _parserTasks.Enqueue(parserTask);

        _workItemsQueueSemaphoreSlim.Release();
    }

    public async Task<IParserTask?> DequeueAsync(
        CancellationToken cancellationToken)
    {
        IParserTask? backgroundTask;

        try
        {
            await _workItemsQueueSemaphoreSlim.WaitAsync(cancellationToken);

            _parserTasks.TryDequeue(out backgroundTask);
        }
        finally
        {
            _workItemsQueueSemaphoreSlim.Release();
        }

        return backgroundTask;
    }
}