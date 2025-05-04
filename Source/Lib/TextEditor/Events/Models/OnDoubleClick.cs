using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

namespace Luthetus.TextEditor.RazorLib.Events.Models;

public struct OnDoubleClick
{
    public OnDoubleClick(
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
    	var editContext = new TextEditorEditContext(ComponentData.TextEditorViewModelSlimDisplay.TextEditorService);
    
        var viewModel = editContext.GetViewModelModifier(ViewModelKey);
        var modelModifier = editContext.GetModelModifier(viewModel.ResourceUri, isReadOnly: true);
        var cursorModifierBag = editContext.GetCursorModifierBag(viewModel);
        var primaryCursorModifier = cursorModifierBag.CursorModifier;

        if (modelModifier is null || viewModel is null || !cursorModifierBag.ConstructorWasInvoked || primaryCursorModifier is null)
            return;

        var hasSelectedText = TextEditorSelectionHelper.HasSelectedText(primaryCursorModifier);

        if ((MouseEventArgs.Buttons & 1) != 1 && hasSelectedText)
            return; // Not pressing the left mouse button so assume ContextMenu is desired result.

        if (MouseEventArgs.ShiftKey)
            return; // Do not expand selection if user is holding shift

		// Labeling any ITextEditorEditContext -> JavaScript interop or Blazor StateHasChanged.
		// Reason being, these are likely to be huge optimizations (2024-05-29).
        var lineAndColumnIndex = await EventUtils.CalculateLineAndColumnIndex(
				modelModifier,
				viewModel,
				MouseEventArgs,
				ComponentData,
				editContext)
			.ConfigureAwait(false);

        var lowerColumnIndexExpansion = modelModifier.GetColumnIndexOfCharacterWithDifferingKind(
            lineAndColumnIndex.LineIndex,
            lineAndColumnIndex.ColumnIndex,
            true);

        lowerColumnIndexExpansion = lowerColumnIndexExpansion == -1
            ? 0
            : lowerColumnIndexExpansion;

        var higherColumnIndexExpansion = modelModifier.GetColumnIndexOfCharacterWithDifferingKind(
            lineAndColumnIndex.LineIndex,
            lineAndColumnIndex.ColumnIndex,
            false);

        higherColumnIndexExpansion = higherColumnIndexExpansion == -1
            ? modelModifier.GetLineLength(lineAndColumnIndex.LineIndex)
            : higherColumnIndexExpansion;

        // Move user's cursor position to the higher expansion
        {
            primaryCursorModifier.LineIndex = lineAndColumnIndex.LineIndex;
            primaryCursorModifier.ColumnIndex = higherColumnIndexExpansion;
            primaryCursorModifier.PreferredColumnIndex = lineAndColumnIndex.ColumnIndex;
        }

        // Set text selection ending to higher expansion
        {
            var cursorPositionOfHigherExpansion = modelModifier.GetPositionIndex(
                lineAndColumnIndex.LineIndex,
                higherColumnIndexExpansion);

            primaryCursorModifier.SelectionEndingPositionIndex = cursorPositionOfHigherExpansion;
        }

        // Set text selection anchor to lower expansion
        {
            var cursorPositionOfLowerExpansion = modelModifier.GetPositionIndex(
                lineAndColumnIndex.LineIndex,
                lowerColumnIndexExpansion);

            primaryCursorModifier.SelectionAnchorPositionIndex = cursorPositionOfLowerExpansion;
        }
        
        await editContext.TextEditorService
			.FinalizePost(editContext)
			.ConfigureAwait(false);
    }
}
