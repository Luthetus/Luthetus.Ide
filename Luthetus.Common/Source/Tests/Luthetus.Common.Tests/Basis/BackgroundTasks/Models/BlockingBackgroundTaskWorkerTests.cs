using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.Tests.Basis.BackgroundTasks.Models;

/// <summary>
/// <see cref="BlockingBackgroundTaskWorker"/>
/// </summary>
public class BlockingBackgroundTaskWorkerTests
{
    /// <summary>
    /// <see cref="BlockingBackgroundTaskWorker(Key{BackgroundTaskQueue}, IBackgroundTaskService, ILoggerFactory)"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var services = new ServiceCollection()
            .AddSingleton<ILoggerFactory, NullLoggerFactory>();

        var sp = services.BuildServiceProvider();

        var queueKey = BlockingBackgroundTaskWorker.Queue.Key;

        var blockingBackgroundTaskWorker =  new BlockingBackgroundTaskWorker(
            queueKey,
            sp.GetRequiredService<IBackgroundTaskService>(),
            sp.GetRequiredService<ILoggerFactory>());

        throw new NotImplementedException("TODO: Testing");
    }
}
