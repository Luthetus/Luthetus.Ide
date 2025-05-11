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
    public Key<BackgroundTaskQueue> QueueKey { get; } = BackgroundTaskFacts.ContinuousQueueKey;
    public string Name { get; } = nameof(InputFileIdeApi);
    public bool EarlyBatchEnabled { get; } = false;

    public bool __TaskCompletionSourceWasCreated { get; set; }

    private readonly Queue<InputFileIdeApiWorkKind> _workKindQueue = new();
    private readonly object _workLock = new();

    private readonly
        Queue<(string message, Func<AbsolutePath, Task> onAfterSubmitFunc, Func<AbsolutePath, Task<bool>> selectionIsValidFunc, List<InputFilePattern> inputFilePatterns)>
        _queue_RequestInputFileStateForm = new();

    public void Enqueue_RequestInputFileStateForm(
        string message,
        Func<AbsolutePath, Task> onAfterSubmitFunc,
        Func<AbsolutePath, Task<bool>> selectionIsValidFunc,
        List<InputFilePattern> inputFilePatterns)
    {
        lock (_workLock)
        {
            _workKindQueue.Enqueue(InputFileIdeApiWorkKind.RequestInputFileStateForm);
            _queue_RequestInputFileStateForm.Enqueue((message, onAfterSubmitFunc, selectionIsValidFunc, inputFilePatterns));
            _backgroundTaskService.EnqueueGroup(this);
        }
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

    public IBackgroundTaskGroup? EarlyBatchOrDefault(IBackgroundTaskGroup oldEvent)
    {
        return null;
    }

    public ValueTask HandleEvent(CancellationToken cancellationToken)
    {
        InputFileIdeApiWorkKind workKind;

        lock (_workLock)
        {
            if (!_workKindQueue.TryDequeue(out workKind))
                return ValueTask.CompletedTask;
        }

        switch (workKind)
        {
            case InputFileIdeApiWorkKind.RequestInputFileStateForm:
            {
                var args = _queue_RequestInputFileStateForm.Dequeue();
                return Do_RequestInputFileStateForm(
                    args.message, args.onAfterSubmitFunc, args.selectionIsValidFunc, args.inputFilePatterns);
            }
            default:
            {
                Console.WriteLine($"{nameof(InputFileIdeApi)} {nameof(HandleEvent)} default case");
				return ValueTask.CompletedTask;
            }
        }
    }
}
