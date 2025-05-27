using Luthetus.Common.RazorLib.Keys.Models;
using System.Collections.Concurrent;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

public sealed class BackgroundTaskQueue
{
    private readonly ConcurrentQueue<IBackgroundTaskGroup> _queue = new();

	public BackgroundTaskQueue(Key<BackgroundTaskQueue> key, string displayName)
    {
        Key = key;
        DisplayName = displayName;
    }
    
	public Key<BackgroundTaskQueue> Key { get; }
    public string DisplayName { get; }

    /// <summary>
    /// Returns the amount of <see cref="IBackgroundTask"/>(s) in the queue.
    /// </summary>
    public int Count => _queue.Count;

    public SemaphoreSlim __DequeueSemaphoreSlim { get; } = new(0);

	public List<IBackgroundTaskGroup> GetBackgroundTaskList() => _queue.ToList();

    public void Enqueue(IBackgroundTaskGroup downstreamEvent)
    {
		_queue.Enqueue(downstreamEvent);
		__DequeueSemaphoreSlim.Release();
    }
    
    public IBackgroundTaskGroup __DequeueOrDefault()
    {
		_queue.TryDequeue(out var backgroundTask);
		return backgroundTask;
    }
}
