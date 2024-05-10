using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.Common.RazorLib.Reactives.Models.Internals;

/// <summary>
/// I need to avoid collision with the existing throttle code,
/// so I'm adding the word 'Counter' to these type names
/// for the time being.
/// ==========================================================
/// 
/// I think the solution will be 'Async' in the end, so I'd
/// like to start with this interface.
/// </summary>
public interface ICounterThrottleAsync
{
    /// <summary>
    /// Do not worry about 'ShouldWaitForPreviousWorkItemToComplete' logic until
    /// the simpler cases work properly.
    /// </summary>
    // public bool ShouldWaitForPreviousWorkItemToComplete { get; }
    // public bool IsStoppingFurtherPushes { get; }

    public TimeSpan ThrottleTimeSpan { get; }

    /// <summary>
    /// Get rid of the CancellationToken until the simpler cases work properly
    /// </summary>
    public Task PushEvent(Func<Task> workItem);

    // public Task StopFurtherPushes();

    /// <summary>
    /// This method awaits the last task prior to returning.<br/><br/>
    /// 
    /// This method does NOT prevent pushes while flushing.
    /// To do so, invoke <see cref="StopFurtherPushes()"/>
    /// prior to invoking this method.<br/><br/>
    /// 
    /// The implementation of this method is a polling solution
    /// (as of this comment (2024-05-09)).
    /// </summary>
    //public Task UntilIsEmpty(
    //    TimeSpan? pollingTimeSpan = null,
    //    CancellationToken cancellationToken = default);
}
