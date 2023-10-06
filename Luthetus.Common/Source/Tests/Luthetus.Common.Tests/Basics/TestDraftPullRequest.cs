using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.Common.Tests.Basics;

public class TestDraftPullRequest
{
    /*
     * I want to list out the 'topmost' directories within Luthetus.Common.RazorLib
     * as this might help me plan out testing all the functionality.
     *
     * BackgroundTasks
     * Clipboards
     * Commands
     * ComponentRenderers
     * ComponentRunners
     * Contexts
     * CustomEvents
     * Dialogs
     * Dimensions
     * Drags
     * Dropdowns
     * FileSystems
     * Htmls
     * Icons
     * Installations
     * JavaScriptObjects
     * Keyboards
     * Keymaps
     * Keys
     * Menus
     * Namespaces
     * Notifications
     * Options
     * OutOfBoundClicks
     * Panels
     * Reactives
     * RenderStates
     * Resizes
     * ShouldRenderBoundaries
     * StateHasChangedBoundaries
     * Storages
     * Tabs
     * Themes
     * TreeViews
     * WatchWindows
     * 
     * I suppose I'll start by going down the list and make 1 test case foreach
     * toplevel directory.
     * 
     * Then I'll have seen the entire scope of Luthetus.Common.RazorLib, and add
     * more tests with that knowledge.
     */

    [Fact]
    public async Task BackgroundTasks_Test()
    {
        /*
         * The IDE makes use of a true BackgroundTaskService.
         * 
         * But, the Unit Tests use BackgroundTaskServiceSynchronous
         * make task execution deterministic.
         * 
         * Would a UnitTest for BackgroundTaskServiceSynchronous provide value here?
         * I presume the true BackgroundTaskService would be the intended target for testing.
         * 
         * I'll see about writing an async [Fact].
         * Then I can
         *     -Create a local BackgroundTaskService instance
         *     -Start the background task service as a fire and forget task
         *     -Enqueue two background tasks
         *     -Await the method for stopping the BackgroundTaskService
         *     -Assert that both background tasks completed.
         */

        var backgroundTaskService = new BackgroundTaskService();

        backgroundTaskService.RegisterQueue(ContinuousBackgroundTaskWorker.Queue);

        var serviceProvider = new ServiceCollection()
            .AddLogging()
            .BuildServiceProvider();

        var factory = serviceProvider.GetRequiredService<ILoggerFactory>();

        var backgroundTaskWorker = new ContinuousBackgroundTaskWorker(
            ContinuousBackgroundTaskWorker.Queue.Key,
            backgroundTaskService,
            factory);

        var firstBackgroundTask = new BackgroundTask(
            Key<BackgroundTask>.NewKey(),
            ContinuousBackgroundTaskWorker.Queue.Key,
            "firstTask",
            async () =>
            {
                for (var i = 0; i < 3; i++)
                {
                    await Task.Delay(33);
                }
            });

        backgroundTaskService.Enqueue(firstBackgroundTask);
        
        var secondBackgroundTask = new BackgroundTask(Key<BackgroundTask>.NewKey(),
            ContinuousBackgroundTaskWorker.Queue.Key,
            "secondTask",
            async () =>
            {
                for (var i = 0; i < 3; i++)
                {
                    await Task.Delay(33);
                }
            });

        backgroundTaskService.Enqueue(secondBackgroundTask);

        var startCts = new CancellationTokenSource();

        _ = Task.Run(async () =>
        {
            await backgroundTaskWorker.StartAsync(startCts.Token);
        });

        var stopCts = new CancellationTokenSource();
        await backgroundTaskWorker.StopAsync(stopCts.Token);

        Assert.NotNull(firstBackgroundTask.WorkProgress);
        Assert.NotNull(secondBackgroundTask.WorkProgress);
        
        Assert.True(firstBackgroundTask.WorkProgress!.IsCompleted);
        Assert.True(secondBackgroundTask.WorkProgress!.IsCompleted);
    }
}
