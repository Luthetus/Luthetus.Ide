using Fluxor;
using Luthetus.Common.RazorLib.Dialog.Models;
using Luthetus.Common.RazorLib.Dialog.States;
using Luthetus.Ide.RazorLib.HtmlCase.Models;
using static Luthetus.Ide.RazorLib.InputFileCase.States.InputFileState;

namespace Luthetus.Ide.RazorLib.InputFileCase.States;

public partial record InputFileSync
{
    public Task HandleRequestInputFileStateFormAction(
        RequestInputFileStateFormAction requestInputFileStateFormAction)
    {
        if (_luthetusIdeComponentRenderers.InputFileRendererType is not null)
        {
            Dispatcher.Dispatch(new StartInputFileStateFormAction(
                requestInputFileStateFormAction));

            var inputFileDialog = new DialogRecord(
                DialogFacts.InputFileDialogKey,
                "Input File",
                _luthetusIdeComponentRenderers.InputFileRendererType,
                null,
                HtmlFacts.Classes.DIALOG_PADDING_0)
            {
                IsResizable = true
            };

            Dispatcher.Dispatch(new DialogRegistry.RegisterAction(
                inputFileDialog));
        }

        return Task.CompletedTask;
    }
}