using Luthetus.Common.RazorLib.Dialog.Models;
using Luthetus.Common.RazorLib.Dialog.States;
using Luthetus.Common.RazorLib.FileSystem.Models;
using Luthetus.Ide.RazorLib.HtmlCase.Models;
using Luthetus.Ide.RazorLib.InputFileCase.Models;
using System.Collections.Immutable;
using static Luthetus.Ide.RazorLib.InputFileCase.States.InputFileState;

namespace Luthetus.Ide.RazorLib.InputFileCase.States;

public partial record InputFileSync
{
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