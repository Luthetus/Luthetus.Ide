using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.InputFiles.Models;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.Editors.States;

public partial class EditorSync
{
    public Task OpenInEditor(
        IAbsolutePath? absolutePath,
        bool shouldSetFocusToEditor,
        Key<TextEditorGroup>? editorTextEditorGroupKey = null)
    {
        return BackgroundTaskService.EnqueueAsync(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.GetQueueKey(),
            "OpenInEditor",
            async () => await OpenInEditorAsync(
                absolutePath,
                shouldSetFocusToEditor,
                editorTextEditorGroupKey));
    }

    public Task ShowInputFile()
    {
        return _inputFileSync.RequestInputFileStateForm("TextEditor",
            absolutePath =>
            {
                return OpenInEditor(absolutePath, true);
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
    }
}
