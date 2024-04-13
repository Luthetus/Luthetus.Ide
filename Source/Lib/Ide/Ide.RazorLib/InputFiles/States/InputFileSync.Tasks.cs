using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.Htmls.Models;
using Luthetus.Ide.RazorLib.InputFiles.Models;
using System.Collections.Immutable;
using static Luthetus.Ide.RazorLib.InputFiles.States.InputFileState;

namespace Luthetus.Ide.RazorLib.InputFiles.States;

public partial record InputFileSync
{
    private Task HandleRequestInputFileStateFormActionAsync(
        string message,
        Func<IAbsolutePath?, Task> onAfterSubmitFunc,
        Func<IAbsolutePath?, Task<bool>> selectionIsValidFunc,
        ImmutableArray<InputFilePattern> inputFilePatternsList)
    {
        Dispatcher.Dispatch(new StartInputFileStateFormAction(
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
			true);

        Dispatcher.Dispatch(new DialogState.RegisterAction(inputFileDialog));

        return Task.CompletedTask;
    }
}