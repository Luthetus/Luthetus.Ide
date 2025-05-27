using System.Collections.Concurrent;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.Htmls.Models;

namespace Luthetus.Ide.RazorLib.InputFiles.Models;

public class InputFileIdeApi : IBackgroundTaskGroup
{
    private readonly IdeBackgroundTaskApi _ideBackgroundTaskApi;
    private readonly IIdeComponentRenderers _ideComponentRenderers;
    private readonly BackgroundTaskService _backgroundTaskService;
    private readonly IDialogService _dialogService;
    private readonly IInputFileService _inputFileService;

    public InputFileIdeApi(
        IdeBackgroundTaskApi ideBackgroundTaskApi,
        IIdeComponentRenderers ideComponentRenderers,
        BackgroundTaskService backgroundTaskService,
        IDialogService dialogService,
        IInputFileService inputFileService)
    {
        _ideBackgroundTaskApi = ideBackgroundTaskApi;
        _ideComponentRenderers = ideComponentRenderers;
        _backgroundTaskService = backgroundTaskService;
        _dialogService = dialogService;
        _inputFileService = inputFileService;
    }

    public Key<IBackgroundTaskGroup> BackgroundTaskKey { get; } = Key<IBackgroundTaskGroup>.NewKey();

    public bool __TaskCompletionSourceWasCreated { get; set; }

    private readonly ConcurrentQueue<InputFileIdeApiWorkArgs> _workQueue = new();

    public void Enqueue(InputFileIdeApiWorkArgs workArgs)
    {
        _workQueue.Enqueue(workArgs);
        _backgroundTaskService.Continuous_EnqueueGroup(this);
    }

    private ValueTask Do_RequestInputFileStateForm(
        string message,
        Func<AbsolutePath, Task> onAfterSubmitFunc,
        Func<AbsolutePath, Task<bool>> selectionIsValidFunc,
        List<InputFilePattern> inputFilePatternsList)
    {
        _inputFileService.StartInputFileStateForm(
            message,
            onAfterSubmitFunc,
            selectionIsValidFunc,
            inputFilePatternsList);

        var inputFileDialog = new DialogViewModel(
            DialogFacts.InputFileDialogKey,
            "Input File",
            _ideComponentRenderers.InputFileRendererType,
            null,
            HtmlFacts.Classes.DIALOG_PADDING_0,
            true,
            null);

        _dialogService.ReduceRegisterAction(inputFileDialog);

        return ValueTask.CompletedTask;
    }

    public ValueTask HandleEvent()
    {
        if (!_workQueue.TryDequeue(out InputFileIdeApiWorkArgs workArgs))
            return ValueTask.CompletedTask;

        switch (workArgs.WorkKind)
        {
            case InputFileIdeApiWorkKind.RequestInputFileStateForm:
                return Do_RequestInputFileStateForm(
                    workArgs.Message, workArgs.OnAfterSubmitFunc, workArgs.SelectionIsValidFunc, workArgs.InputFilePatterns);
            default:
                Console.WriteLine($"{nameof(InputFileIdeApi)} {nameof(HandleEvent)} default case");
				return ValueTask.CompletedTask;
        }
    }
}
