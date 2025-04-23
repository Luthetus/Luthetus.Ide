using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

namespace Luthetus.TextEditor.RazorLib.Events.Models;

public struct OnScrollHorizontal
{
    public OnScrollHorizontal(
        double scrollLeft,
		TextEditorComponentData componentData,
        Key<TextEditorViewModel> viewModelKey)
    {
		ComponentData = componentData;

        ScrollLeft = scrollLeft;
        ViewModelKey = viewModelKey;
    }

    public double ScrollLeft { get; }
    public Key<TextEditorViewModel> ViewModelKey { get; }
    public TextEditorComponentData ComponentData { get; }

    public async ValueTask HandleEvent(CancellationToken cancellationToken)
    {
    	var editContext = new TextEditorEditContext(ComponentData.TextEditorViewModelSlimDisplay.TextEditorService);
    
        var viewModelModifier = editContext.GetViewModelModifier(ViewModelKey);
        if (viewModelModifier is null)
            return;

        editContext.TextEditorService.ViewModelApi.SetScrollPositionLeft(
        	editContext,
    		viewModelModifier,
        	ScrollLeft);
        	
        await editContext.TextEditorService
        	.FinalizePost(editContext)
        	.ConfigureAwait(false);
    }
}
