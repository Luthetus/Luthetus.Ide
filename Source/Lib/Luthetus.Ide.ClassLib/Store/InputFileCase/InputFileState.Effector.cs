using Luthetus.Ide.ClassLib.Html;
using Luthetus.Ide.ClassLib.ComponentRenderers;

namespace Luthetus.Ide.ClassLib.Store.InputFileCase;

public partial record InputFileState
{
    private class Effector
    {
        private readonly ILuthetusIdeComponentRenderers _luthetusIdeComponentRenderers;

        public Effector(
            ILuthetusIdeComponentRenderers luthetusIdeComponentRenderers)
        {
            _luthetusIdeComponentRenderers = luthetusIdeComponentRenderers;
        }
        
        [EffectMethod]
        public Task HandleRequestInputFileStateFormAction(
            RequestInputFileStateFormAction requestInputFileStateFormAction,
            IDispatcher dispatcher)
        {
            if (_luthetusIdeComponentRenderers.InputFileRendererType is not null)
            {
                dispatcher.Dispatch(
                    new StartInputFileStateFormAction(
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
            
                dispatcher.Dispatch(
                    new DialogRecordsCollection.RegisterAction(
                        inputFileDialog));
            }

            return Task.CompletedTask;
        }
    }
}