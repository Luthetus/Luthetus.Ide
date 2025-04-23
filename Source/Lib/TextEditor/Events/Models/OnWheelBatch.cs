using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

namespace Luthetus.TextEditor.RazorLib.Events.Models;

public struct OnWheelBatch
{
    public OnWheelBatch(
        List<WheelEventArgs> wheelEventArgsList,
		TextEditorComponentData componentData,
        Key<TextEditorViewModel> viewModelKey)
    {
		ComponentData = componentData;

        WheelEventArgsList = wheelEventArgsList;
        ViewModelKey = viewModelKey;
    }

    public List<WheelEventArgs> WheelEventArgsList { get; }
    public Key<TextEditorViewModel> ViewModelKey { get; }
    public TextEditorComponentData ComponentData { get; }

    public async ValueTask HandleEvent(CancellationToken cancellationToken)
    {
    	var editContext = new TextEditorEditContext(ComponentData.TextEditorViewModelSlimDisplay.TextEditorService);
    
        var viewModelModifier = editContext.GetViewModelModifier(ViewModelKey);

        if (viewModelModifier is null)
            return;

        double? horizontalMutateScrollPositionByPixels = null;
        double? verticalMutateScrollPositionByPixels = null;

        foreach (var wheelEventArgs in WheelEventArgsList)
        {
            if (wheelEventArgs.ShiftKey)
            {
                horizontalMutateScrollPositionByPixels ??= 0;
                horizontalMutateScrollPositionByPixels += wheelEventArgs.DeltaY;
            }
            else
            {
                verticalMutateScrollPositionByPixels ??= 0;
                verticalMutateScrollPositionByPixels += wheelEventArgs.DeltaY;
            }
        }

		// See this quoted comment from OnWheel.cs
		// =======================================
		// 	"TODO: Why was this made as 'if' 'else' whereas the OnWheelBatch...
		// 	      ...is doing 'if' 'if'.
		// 	      |
		// 	      The OnWheelBatch doesn't currently batch horizontal with vertical
		// 	      the OnWheel events have to be the same axis to batch."
        if (horizontalMutateScrollPositionByPixels is not null)
        {
            editContext.TextEditorService.ViewModelApi.MutateScrollHorizontalPosition(
                editContext,
		        viewModelModifier,
		        horizontalMutateScrollPositionByPixels.Value);
        }

        if (verticalMutateScrollPositionByPixels is not null)
        {
            editContext.TextEditorService.ViewModelApi.MutateScrollVerticalPosition(
                editContext,
		        viewModelModifier,
		        verticalMutateScrollPositionByPixels.Value);
        }
        
        await editContext.TextEditorService
        	.FinalizePost(editContext)
        	.ConfigureAwait(false);
    }
}
