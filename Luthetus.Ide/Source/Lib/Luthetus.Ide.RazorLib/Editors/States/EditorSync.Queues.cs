using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.InputFiles.Models;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.Editors.States;

public partial class EditorSync
{
    public void OpenInEditor(
        IAbsolutePath? absolutePath,
        bool shouldSetFocusToEditor,
        Key<TextEditorGroup>? editorTextEditorGroupKey = null)
    {
        BackgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.GetQueueKey(),
            "OpenInEditor",
            async () => await OpenInEditorAsync(
                absolutePath,
                shouldSetFocusToEditor,
                editorTextEditorGroupKey));
    }

    public Task ShowInputFile()
    {
        _inputFileSync.RequestInputFileStateForm("TextEditor",
            absolutePath =>
            {
                OpenInEditor(absolutePath, true);
                return Task.CompletedTask;
            },
            absolutePath =>
            {
                if (absolutePath is null || absolutePath.IsDirectory)
                    return Task.FromResult(false);

                return Task.FromResult(true);
            },
            new[]
            {
                    new InputFilePattern("File", absolutePath => !absolutePath.IsDirectory)
            }.ToImmutableArray());

        return Task.CompletedTask;
    }
}
