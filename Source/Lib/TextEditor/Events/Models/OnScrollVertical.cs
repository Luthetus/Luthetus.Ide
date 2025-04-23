using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

namespace Luthetus.TextEditor.RazorLib.Events.Models;

public struct OnScrollVertical
{
    public OnScrollVertical(
        double scrollTop,
		TextEditorComponentData componentData,
        Key<TextEditorViewModel> viewModelKey)
    {
		ComponentData = componentData;

        ScrollTop = scrollTop;
        ViewModelKey = viewModelKey;
    }

    public double ScrollTop { get; }
    public Key<TextEditorViewModel> ViewModelKey { get; }
    public TextEditorComponentData ComponentData { get; }

    public async ValueTask HandleEvent(CancellationToken cancellationToken)
    {
    	var editContext = new TextEditorEditContext(ComponentData.TextEditorViewModelSlimDisplay.TextEditorService);
    
        var viewModelModifier = editContext.GetViewModelModifier(ViewModelKey);
        if (viewModelModifier is null)
            return;

        editContext.TextEditorService.ViewModelApi.SetScrollPositionTop(
        	editContext,
    		viewModelModifier,
        	ScrollTop);
        	
        await editContext.TextEditorService
        	.FinalizePost(editContext)
        	.ConfigureAwait(false);
    }
}
