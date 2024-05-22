using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;

namespace Luthetus.TextEditor.Tests.Basis.BackgroundTasks.Models;

/// <summary>
/// <see cref="AsIsTextEditorTask"/>
/// </summary>
public class AsIsTextEditorTaskTests
{
    /// <summary>
    /// <see cref="AsIsTextEditorTask(string, TextEditorEdit, TimeSpan?)"/>
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
            var simpleBatchTextEditorTask = new SimpleBatchTextEditorTask(
                name,
                identifier,
                edit);

            Assert.Equal(name, simpleBatchTextEditorTask.Name);
            Assert.Equal(identifier, simpleBatchTextEditorTask.Identifier);
        }

        // Test two timespan values to ensure a default value isn't overwriting the constructor argument.
        {
            {
                var timeSpan = TimeSpan.FromMilliseconds(1_000);

                var simpleBatchTextEditorTask = new SimpleBatchTextEditorTask(
                    name,
                    identifier,
                    edit,
                    timeSpan);

                Assert.Equal(timeSpan, simpleBatchTextEditorTask.ThrottleTimeSpan);

                Assert.Equal(name, simpleBatchTextEditorTask.Name);
                Assert.Equal(identifier, simpleBatchTextEditorTask.Identifier);
            }
            {
                var timeSpan = TimeSpan.FromMilliseconds(512);

                var simpleBatchTextEditorTask = new SimpleBatchTextEditorTask(
                    name,
                    identifier,
                    edit,
                    timeSpan);

                Assert.Equal(timeSpan, simpleBatchTextEditorTask.ThrottleTimeSpan);

                Assert.Equal(name, simpleBatchTextEditorTask.Name);
                Assert.Equal(identifier, simpleBatchTextEditorTask.Identifier);
            }
        }
    }

    /// <summary>
    /// <see cref="AsIsTextEditorTask.InvokeWithEditContext(IEditContext)"/>
    /// </summary>
    [Fact]
    public void InvokeWithEditContext()
    {
    }

    /// <summary>
    /// <see cref="AsIsTextEditorTask.BatchOrDefault(IBackgroundTask)"/>
    /// </summary>
    [Fact]
    public void BatchOrDefault()
    {
    }

    /// <summary>
    /// <see cref="AsIsTextEditorTask.HandleEvent(CancellationToken)"/>
    /// </summary>
    [Fact]
    public void HandleEvent()
    {
    }
}
