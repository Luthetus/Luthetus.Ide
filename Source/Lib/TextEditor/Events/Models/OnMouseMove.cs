using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

namespace Luthetus.TextEditor.RazorLib.Events.Models;

public struct OnMouseMove
{
    public OnMouseMove(
        MouseEventArgs mouseEventArgs,
		TextEditorComponentData componentData,
        Key<TextEditorViewModel> viewModelKey)
    {
    	MouseEventArgs = mouseEventArgs;
		ComponentData = componentData;
        ViewModelKey = viewModelKey;
    }

    public MouseEventArgs MouseEventArgs { get; }
    public Key<TextEditorViewModel> ViewModelKey { get; }
	public TextEditorComponentData ComponentData { get; }

    public async ValueTask HandleEvent(CancellationToken cancellationToken)
    {
    	var editContext = new TextEditorEditContext(ComponentData.TextEditorViewModelDisplay.TextEditorService);
    
        var viewModel = editContext.GetViewModelModifier(ViewModelKey);
        var modelModifier = editContext.GetModelModifier(viewModel.ResourceUri, true);
        var cursorModifierBag = editContext.GetCursorModifierBag(viewModel);
        var primaryCursorModifier = cursorModifierBag.CursorModifier;

        if (modelModifier is null || viewModel is null || !cursorModifierBag.ConstructorWasInvoked || primaryCursorModifier is null)
            return;

		// Labeling any ITextEditorEditContext -> JavaScript interop or Blazor StateHasChanged.
		// Reason being, these are likely to be huge optimizations (2024-05-29).
        var rowAndColumnIndex = await EventUtils.CalculateRowAndColumnIndex(
				viewModel.ResourceUri,
				ViewModelKey,
				MouseEventArgs,
				ComponentData,
				editContext)
			.ConfigureAwait(false);

        primaryCursorModifier.LineIndex = rowAndColumnIndex.rowIndex;
        primaryCursorModifier.ColumnIndex = rowAndColumnIndex.columnIndex;
        primaryCursorModifier.PreferredColumnIndex = rowAndColumnIndex.columnIndex;

		// editContext.TextEditorService.ViewModelApi.SetCursorShouldBlink(false);

        primaryCursorModifier.SelectionEndingPositionIndex = modelModifier.GetPositionIndex(primaryCursorModifier);
	
		editContext.TextEditorService.ViewModelApi.SetCursorShouldBlink(false);
	
		await editContext.TextEditorService
			.FinalizePost(editContext)
			.ConfigureAwait(false);
    }
}
