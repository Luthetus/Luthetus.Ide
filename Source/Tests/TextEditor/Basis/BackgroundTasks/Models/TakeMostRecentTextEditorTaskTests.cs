using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;

namespace Luthetus.TextEditor.Tests.Basis.BackgroundTasks.Models;

/// <summary>
/// <see cref="TakeMostRecentTextEditorTask"/>
/// </summary>
public class TakeMostRecentTextEditorTaskTests
{
    /// <summary>
    /// <see cref="TakeMostRecentTextEditorTask(string, string, TextEditorEdit, TimeSpan?)"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var name = "Name_Test";
        var identifier = "Identifier_Test";
        TextEditorEdit edit = editContext => Task.CompletedTask;

        // Test without passing the optional constructor argument:
        // 'TimeSpan? throttleTimeSpan = null'
        {
            var takeMostRecentTextEditorTask = new TakeMostRecentTextEditorTask(
                name,
                identifier,
                edit);

            Assert.Equal(name, takeMostRecentTextEditorTask.Name);
            Assert.Equal(identifier, takeMostRecentTextEditorTask.Identifier);
        }

        // Test two timespan values to ensure a default value isn't overwriting the constructor argument.
        {
            {
                var timeSpan = TimeSpan.FromMilliseconds(1_000);

                var takeMostRecentTextEditorTask = new TakeMostRecentTextEditorTask(
                    name,
                    identifier,
                    edit,
                    timeSpan);

                Assert.Equal(timeSpan, takeMostRecentTextEditorTask.ThrottleTimeSpan);

                Assert.Equal(name, takeMostRecentTextEditorTask.Name);
                Assert.Equal(identifier, takeMostRecentTextEditorTask.Identifier);
            }
            {
                var timeSpan = TimeSpan.FromMilliseconds(512);

                var takeMostRecentTextEditorTask = new TakeMostRecentTextEditorTask(
                    name,
                    identifier,
                    edit,
                    timeSpan);

                Assert.Equal(timeSpan, takeMostRecentTextEditorTask.ThrottleTimeSpan);

                Assert.Equal(name, takeMostRecentTextEditorTask.Name);
                Assert.Equal(identifier, takeMostRecentTextEditorTask.Identifier);
            }
        }
    }

    /// <summary>
    /// <see cref="TakeMostRecentTextEditorTask.InvokeWithEditContext(IEditContext)"/>
    /// </summary>
    [Fact]
    public async void InvokeWithEditContext()
    {
        var currentThread = Thread.CurrentThread;

        var (backgroundTaskService, backgroundTaskWorker) = InitializeTakeMostRecentTextEditorTaskTests();

        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;

        var thread = new Thread(async () =>
        {
            await backgroundTaskWorker.StartAsync(cancellationToken);
        });

        thread.Start();

        await Task.Yield();

        await backgroundTaskWorker.StopAsync(CancellationToken.None);
        thread.Join();
    }

    /// <summary>
    /// <see cref="TakeMostRecentTextEditorTask.BatchOrDefault(IBackgroundTask)"/>
    /// </summary>
    [Fact]
    public void BatchOrDefault()
    {

    }


    /// <summary>
    /// <see cref="TakeMostRecentTextEditorTask.HandleEvent(CancellationToken)"/>
    /// </summary>
    [Fact]
    public void HandleEvent()
    {

    }

    private (BackgroundTaskService backgroundTaskService, ContinuousBackgroundTaskWorker backgroundTaskWorker) InitializeTakeMostRecentTextEditorTaskTests()
    {
        var services = new ServiceCollection()
            .AddScoped<ILoggerFactory, NullLoggerFactory>();

        var serviceProvider = services.BuildServiceProvider();

        var backgroundTaskService = new BackgroundTaskService();
        
        var backgroundTaskWorker = new ContinuousBackgroundTaskWorker(
            backgroundTaskService,
            serviceProvider.GetRequiredService<ILoggerFactory>());

        backgroundTaskService.RegisterQueue(new BackgroundTaskQueue(
            ContinuousBackgroundTaskWorker.GetQueueKey(),
            ContinuousBackgroundTaskWorker.QUEUE_DISPLAY_NAME));

        return (backgroundTaskService, backgroundTaskWorker);
    }
}
