using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;

namespace Luthetus.TextEditor.Tests.Basis.BackgroundTasks.Models;

/// <summary>
/// <see cref="TakeMostRecentTextEditorTask"/>
/// </summary>
public class TakeMostRecentTextEditorTaskTests : TextEditorTestBase
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

        var (backgroundTaskService, backgroundTaskWorker, dispatcher, textEditorService) = InitializeBackgroundTasks();

        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;

        var backgroundTaskThread = new Thread(async () =>
        {
            await backgroundTaskWorker.StartAsync(cancellationToken);
        });

        await textEditorService.PostTakeMostRecent(
            "Name_Test",
            "Identifier_Test",
            editContext =>
            {
                return Task.CompletedTask;
            });

        backgroundTaskThread.Start();

        // await Task.Yield();

        await backgroundTaskWorker.StopAsync(CancellationToken.None);
        backgroundTaskThread.Join();
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
}
