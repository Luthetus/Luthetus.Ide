using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Misc;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Microsoft.AspNetCore.Components;

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
    public async Task InitializeBackgroundTasks_Test()
    {
        var currentThread = Thread.CurrentThread;

        var (backgroundTaskService, backgroundTaskWorker, dispatcher, textEditorService) = InitializeBackgroundTasks();

        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;

        var backgroundTaskThread = new Thread(async () =>
        {
            await backgroundTaskWorker.StartAsync(cancellationToken);
        });

        //await backgroundTaskService.EnqueueAsync();
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
}
