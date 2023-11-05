using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

public class BlockingBackgroundTaskWorkerTests
{
    [Fact]
    public void Constructor()
    {
        /*
        public BlockingBackgroundTaskWorker(
                Key<BackgroundTaskQueue> queueKey, IBackgroundTaskService backgroundTaskService, ILoggerFactory loggerFactory)
            : base(queueKey, backgroundTaskService, loggerFactory)
         */

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
