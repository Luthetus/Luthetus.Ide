using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

public class ContinuousBackgroundTaskWorkerTests
{
    [Fact]
    public void Constructor()
    {
        /*
        public ContinuousBackgroundTaskWorker(
                Key<BackgroundTaskQueue> queueKey, IBackgroundTaskService backgroundTaskService, ILoggerFactory loggerFactory)
            : base(queueKey, backgroundTaskService, loggerFactory)
         */

        var services = new ServiceCollection()
            .AddSingleton<ILoggerFactory, NullLoggerFactory>();

        var sp = services.BuildServiceProvider();

        var queueKey = BlockingBackgroundTaskWorker.Queue.Key;

        var continuousBackgroundTaskWorker = new ContinuousBackgroundTaskWorker(
            queueKey,
            sp.GetRequiredService<IBackgroundTaskService>(),
            sp.GetRequiredService<ILoggerFactory>());

        throw new NotImplementedException("TODO: Testing");
    }
}