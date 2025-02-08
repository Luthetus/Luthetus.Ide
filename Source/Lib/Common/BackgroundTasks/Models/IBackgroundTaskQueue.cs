using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

public interface IBackgroundTaskQueue
{
    public Key<IBackgroundTaskQueue> Key { get; }
    public string DisplayName { get; }
    public int Count { get; }
	
	/// <summary>
	/// TODO: Decide how to not have this public. (cast the interface to its concrete type?).
	///
	/// More explanation:
	/// I absolutely can cast the interface to its concrete type and then have
	/// this public on the concrete type (which still feels hacky).
	///
	/// But this is the "hottest" path in the entire application I imagine.
	/// Probably should measure the difference it might not even be measurable it's so small.
	/// but yeah it's a matter of worry, and other things need done than measure this.
	/// </summary>
	public SemaphoreSlim __DequeueSemaphoreSlim { get; }
	
	/// <summary>
	/// TODO: Decide how to not have this public. (cast the interface to its concrete type?).
	///
	/// More explanation:
	/// I absolutely can cast the interface to its concrete type and then have
	/// this public on the concrete type (which still feels hacky).
	///
	/// But this is the "hottest" path in the entire application I imagine.
	/// Probably should measure the difference it might not even be measurable it's so small.
	/// but yeah it's a matter of worry, and other things need done than measure this.
	/// </summary>
	public IBackgroundTask? __DequeueOrDefault();
	
	public List<IBackgroundTask> GetBackgroundTaskList();
}
