using Luthetus.Common.RazorLib.BackgroundTaskCase.Models;
using Luthetus.Common.RazorLib.FileSystem.Models;
using Luthetus.Common.RazorLib.KeyCase.Models;
using Luthetus.Ide.RazorLib.InputFileCase.Models;
using Luthetus.TextEditor.RazorLib.Group.Models;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.EditorCase.States;

public partial class EditorSync
{
    public void OpenInEditor(
        IAbsolutePath? absolutePath,
        bool shouldSetFocusToEditor,
        Key<TextEditorGroup>? editorTextEditorGroupKey = null)
    {
        BackgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.Queue.Key,
            "OpenInEditor",
            async () => await OpenInEditorAsync(
                absolutePath,
                shouldSetFocusToEditor,
                editorTextEditorGroupKey));
    }

    public void ShowInputFile()
    {
        _inputFileSync.RequestInputFileStateForm("TextEditor",
            afp =>
            {
                OpenInEditor(afp, true);
                return Task.CompletedTask;
            },
            afp =>
            {
                if (afp is null || afp.IsDirectory)
                    return Task.FromResult(false);

                return Task.FromResult(true);
            },
            new[]
            {
                    new InputFilePattern("File", afp => !afp.IsDirectory)
            }.ToImmutableArray());
    }
}
