using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTaskCase.Models;
using Luthetus.Common.RazorLib.Dialog.Models;
using Luthetus.Common.RazorLib.Dialog.States;
using Luthetus.Common.RazorLib.FileSystem.Models;
using Luthetus.Common.RazorLib.KeyCase.Models;
using Luthetus.Ide.RazorLib.HtmlCase.Models;
using Luthetus.Ide.RazorLib.InputFileCase.Models;
using System.Collections.Immutable;
using static Luthetus.Ide.RazorLib.InputFileCase.States.InputFileState;

namespace Luthetus.Ide.RazorLib.InputFileCase.States;

public partial record InputFileSync
{
    public void RequestInputFileStateForm(
        string message,
        Func<IAbsolutePath?, Task> onAfterSubmitFunc,
        Func<IAbsolutePath?, Task<bool>> selectionIsValidFunc,
        ImmutableArray<InputFilePattern> inputFilePatterns)
    {
        BackgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.Queue.Key,
            "SetDotNetSolutionAsync",
            async () => await HandleRequestInputFileStateFormActionAsync(
                message,
                onAfterSubmitFunc,
                selectionIsValidFunc,
                inputFilePatterns));
    }

    private Task HandleRequestInputFileStateFormActionAsync(
        string message,
        Func<IAbsolutePath?, Task> onAfterSubmitFunc,
        Func<IAbsolutePath?, Task<bool>> selectionIsValidFunc,
        ImmutableArray<InputFilePattern> inputFilePatterns)
    {
        Dispatcher.Dispatch(new StartInputFileStateFormAction(
            message,
            onAfterSubmitFunc,
            selectionIsValidFunc,
            inputFilePatterns));

        var inputFileDialog = new DialogRecord(
            DialogFacts.InputFileDialogKey,
            "Input File",
            _luthetusIdeComponentRenderers.InputFileRendererType,
            null,
            HtmlFacts.Classes.DIALOG_PADDING_0)
        {
            IsResizable = true
        };

        Dispatcher.Dispatch(new DialogState.RegisterAction(
            inputFileDialog));

        return Task.CompletedTask;
    }
}