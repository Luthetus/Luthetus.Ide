using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Common.Tests.Basis.Reactives.Models;

/// <summary>
/// <see cref="ThrottleEventQueueAsync"/>
/// </summary>
public class ThrottleEventQueueAsyncTests
{
    /// <summary>
    /// <see cref="ThrottleEventQueueAsync.EnqueueAsync(IBackgroundTask)"/>
    /// <br/>----<br/>
    /// <see cref="ThrottleEventQueueAsync.Count"/>
    /// <see cref="ThrottleEventQueueAsync.ThrottleEventList"/>
    /// </summary>
    public void EnqueueAsync()
    {
        var queue = new ThrottleEventQueueAsync();

        var thread = new Thread(async () =>
        {
            while (true)
            {
                var task = await queue.DequeueOrDefaultAsync();
                await task.HandleEvent(CancellationToken.None);
            }
        });

        var aaa = new TextEditorTask

        queue.EnqueueAsync();

        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="ThrottleEventQueueAsync.DequeueOrDefaultAsync()"/>
    /// <br/>----<br/>
    /// <see cref="ThrottleEventQueueAsync.Count"/>
    /// <see cref="ThrottleEventQueueAsync.ThrottleEventList"/>
    /// </summary>
    public void DequeueOrDefaultAsync()
    {
        throw new NotImplementedException();
    }
}
