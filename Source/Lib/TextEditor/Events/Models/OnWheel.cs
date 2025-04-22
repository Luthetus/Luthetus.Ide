using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

namespace Luthetus.TextEditor.RazorLib.Events.Models;

public struct OnWheel
{
    public OnWheel(
        WheelEventArgs wheelEventArgs,
		TextEditorComponentData componentData,
        Key<TextEditorViewModel> viewModelKey)
    {
		ComponentData = componentData;

        WheelEventArgs = wheelEventArgs;
        ViewModelKey = viewModelKey;
    }

    public WheelEventArgs WheelEventArgs { get; }
    public Key<TextEditorViewModel> ViewModelKey { get; }
    public TextEditorComponentData ComponentData { get; }

    public async ValueTask HandleEvent(CancellationToken cancellationToken)
    {
    	var editContext = new TextEditorEditContext(ComponentData.TextEditorViewModelSlimDisplay.TextEditorService);
    
        var viewModelModifier = editContext.GetViewModelModifier(ViewModelKey);
        if (viewModelModifier is null)
            return;

		// TODO: Why was this made as 'if' 'else' whereas the OnWheelBatch...
		//       ...is doing 'if' 'if'.
		//       |
		//       The OnWheelBatch doesn't currently batch horizontal with vertical
		//       the OnWheel events have to be the same axis to batch.
        if (WheelEventArgs.ShiftKey)
        {
            editContext.TextEditorService.ViewModelApi.MutateScrollHorizontalPosition(
            	editContext,
		        viewModelModifier,
		        WheelEventArgs.DeltaX);
        }
        else
        {
            editContext.TextEditorService.ViewModelApi.MutateScrollVerticalPosition(
            	editContext,
		        viewModelModifier,
            	WheelEventArgs.DeltaY);
        }
        
        await editContext.TextEditorService
        	.FinalizePost(editContext)
        	.ConfigureAwait(false);
    }
}
