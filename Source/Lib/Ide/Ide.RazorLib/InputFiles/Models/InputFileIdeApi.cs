using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.Htmls.Models;

namespace Luthetus.Ide.RazorLib.InputFiles.Models;

public class InputFileIdeApi
{
    private readonly IdeBackgroundTaskApi _ideBackgroundTaskApi;
    private readonly IIdeComponentRenderers _ideComponentRenderers;
    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly IDialogService _dialogService;
    private readonly IInputFileService _inputFileService;

    public InputFileIdeApi(
        IdeBackgroundTaskApi ideBackgroundTaskApi,
        IIdeComponentRenderers ideComponentRenderers,
        IBackgroundTaskService backgroundTaskService,
        IDialogService dialogService,
        IInputFileService inputFileService)
    {
        _ideBackgroundTaskApi = ideBackgroundTaskApi;
        _ideComponentRenderers = ideComponentRenderers;
        _backgroundTaskService = backgroundTaskService;
        _dialogService = dialogService;
        _inputFileService = inputFileService;
    }

    public void RequestInputFileStateForm(
        string message,
        Func<AbsolutePath, Task> onAfterSubmitFunc,
        Func<AbsolutePath, Task<bool>> selectionIsValidFunc,
        ImmutableArray<InputFilePattern> inputFilePatterns)
    {
        _backgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            BackgroundTaskFacts.ContinuousQueueKey,
            "Request InputFileState Form",
            async () => await HandleRequestInputFileStateFormActionAsync(
                    message,
                    onAfterSubmitFunc,
                    selectionIsValidFunc,
                    inputFilePatterns)
                .ConfigureAwait(false));
    }

    private Task HandleRequestInputFileStateFormActionAsync(
        string message,
        Func<AbsolutePath, Task> onAfterSubmitFunc,
        Func<AbsolutePath, Task<bool>> selectionIsValidFunc,
        ImmutableArray<InputFilePattern> inputFilePatternsList)
    {
        _inputFileService.ReduceStartInputFileStateFormAction(
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

        return Task.CompletedTask;
    }
}
