using System.Collections.Immutable;
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
	private readonly LuthetusCommonApi _commonApi;
    private readonly IdeBackgroundTaskApi _ideBackgroundTaskApi;
    private readonly IIdeComponentRenderers _ideComponentRenderers;
    private readonly IInputFileService _inputFileService;

    public InputFileIdeApi(
    	LuthetusCommonApi commonApi,
        IdeBackgroundTaskApi ideBackgroundTaskApi,
        IIdeComponentRenderers ideComponentRenderers,
        IInputFileService inputFileService)
    {
        _commonApi = commonApi;
        _ideBackgroundTaskApi = ideBackgroundTaskApi;
        _ideComponentRenderers = ideComponentRenderers;
        _inputFileService = inputFileService;
    }

    public void RequestInputFileStateForm(
        string message,
        Func<AbsolutePath, Task> onAfterSubmitFunc,
        Func<AbsolutePath, Task<bool>> selectionIsValidFunc,
        ImmutableArray<InputFilePattern> inputFilePatterns)
    {
        _commonApi.BackgroundTaskApi.Enqueue(
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

        _commonApi.DialogApi.ReduceRegisterAction(inputFileDialog);

        return Task.CompletedTask;
    }
}
