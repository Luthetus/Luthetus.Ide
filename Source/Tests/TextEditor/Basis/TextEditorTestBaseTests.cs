using Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;

namespace Luthetus.TextEditor.Tests.Basis;

/// <summary>
/// <see cref="TextEditorTestBase"/>
/// </summary>
public class TextEditorTestBaseTests : TextEditorTestBase
{
    /// <summary>
    /// <see cref="TextEditorTestBase.InitializeBackgroundTasks()"/>
    /// </summary>
    [Fact]
    public async Task InitializeBackgroundTasks_Test_A()
    {
        var currentThread = Thread.CurrentThread;

        var (backgroundTaskService, backgroundTaskWorker, dispatcher, textEditorService) = InitializeBackgroundTasks();

        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;

        var backgroundTaskThread = new Thread(async () =>
        {
            await backgroundTaskWorker.StartAsync(cancellationToken);
        });

        var number = 0;

        for (var i = 0; i < 3; i++)
        {
            await textEditorService.PostTakeMostRecent(
                "Name_Test",
                "Identifier_Test",
                editContext =>
                {
                    number++;
                    return Task.CompletedTask;
                });
        }

        backgroundTaskThread.Start();

        // await Task.Yield();

        await backgroundTaskWorker.StopAsync(CancellationToken.None);
        backgroundTaskThread.Join();

        Assert.Equal(1, number);
    }

    /// <summary>
    /// <see cref="TextEditorTestBase.InitializeBackgroundTasks()"/>
    /// </summary>
    [Fact]
    public async Task InitializeBackgroundTasks_Test_B()
    {
        var currentThread = Thread.CurrentThread;

        var (backgroundTaskService, backgroundTaskWorker, dispatcher, textEditorService) = InitializeBackgroundTasks();

        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;

        var backgroundTaskThread = new Thread(async () =>
        {
            await backgroundTaskWorker.StartAsync(cancellationToken);
        });

        var number = 0;

        for (var i = 0; i < 3; i++)
        {
            await textEditorService.PostSimpleBatch(
                "Name_Test",
                "Identifier_Test",
                editContext =>
                {
                    number++;
                    return Task.CompletedTask;
                });
        }

        backgroundTaskThread.Start();

        // await Task.Yield();

        await backgroundTaskWorker.StopAsync(CancellationToken.None);
        backgroundTaskThread.Join();

        Assert.Equal(3, number);
    }

    /// <summary>
    /// <see cref="TextEditorTestBase.InitializeBackgroundTasks()"/>
    /// </summary>
    [Fact]
    public async Task InitializeBackgroundTasks_Test_C()
    {
        var currentThread = Thread.CurrentThread;

        var (backgroundTaskService, backgroundTaskWorker, dispatcher, textEditorService) = InitializeBackgroundTasks();

        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;

        var backgroundTaskThread = new Thread(async () =>
        {
            await backgroundTaskWorker.StartAsync(cancellationToken);
        });

        var number = 0;

        for (var i = 0; i < 3; i++)
        {
            await textEditorService.Post(new AsIsTextEditorTask(
                "Name_Test",
                editContext =>
                {
                    number++;
                    return Task.CompletedTask;
                }));
        }

        backgroundTaskThread.Start();

        // await Task.Yield();

        await backgroundTaskWorker.StopAsync(CancellationToken.None);
        backgroundTaskThread.Join();

        Assert.Equal(3, number);
    }
}
