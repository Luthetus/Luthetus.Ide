using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.Htmls.Models;
using static Luthetus.Ide.RazorLib.InputFiles.States.InputFileState;

namespace Luthetus.Ide.RazorLib.InputFiles.Models;

public class LuthetusIdeInputFileBackgroundTaskApi
{
    private readonly LuthetusIdeBackgroundTaskApi _ideBackgroundTaskApi;
    private readonly ILuthetusIdeComponentRenderers _ideComponentRenderers;
    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly IDispatcher _dispatcher;

    public LuthetusIdeInputFileBackgroundTaskApi(
        LuthetusIdeBackgroundTaskApi ideBackgroundTaskApi,
        ILuthetusIdeComponentRenderers ideComponentRenderers,
        IBackgroundTaskService backgroundTaskService,
        IDispatcher dispatcher)
    {
        _ideBackgroundTaskApi = ideBackgroundTaskApi;
        _ideComponentRenderers = ideComponentRenderers;
        _backgroundTaskService = backgroundTaskService;
        _dispatcher = dispatcher;
    }

    public void RequestInputFileStateForm(
        string message,
        Func<IAbsolutePath?, Task> onAfterSubmitFunc,
        Func<IAbsolutePath?, Task<bool>> selectionIsValidFunc,
        ImmutableArray<InputFilePattern> inputFilePatterns)
    {
        _backgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            ContinuousBackgroundTaskWorker.GetQueueKey(),
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
        Func<IAbsolutePath?, Task> onAfterSubmitFunc,
        Func<IAbsolutePath?, Task<bool>> selectionIsValidFunc,
        ImmutableArray<InputFilePattern> inputFilePatternsList)
    {
        _dispatcher.Dispatch(new StartInputFileStateFormAction(
            message,
            onAfterSubmitFunc,
            selectionIsValidFunc,
            inputFilePatternsList));

        var inputFileDialog = new DialogViewModel(
            DialogFacts.InputFileDialogKey,
            "Input File",
            _ideComponentRenderers.InputFileRendererType,
            null,
            HtmlFacts.Classes.DIALOG_PADDING_0,
            true,
            null);

        _dispatcher.Dispatch(new DialogState.RegisterAction(inputFileDialog));

        return Task.CompletedTask;
    }
}
