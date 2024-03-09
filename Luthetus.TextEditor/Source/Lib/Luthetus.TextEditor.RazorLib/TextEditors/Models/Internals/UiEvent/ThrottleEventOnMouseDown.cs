using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals.UiEvent;

public class ThrottleEventOnMouseDown : IThrottleEvent
{
    private readonly TextEditorViewModelDisplay.TextEditorEvents _events;

    public ThrottleEventOnMouseDown(
        MouseEventArgs mouseEventArgs,
        TextEditorViewModelDisplay.TextEditorEvents events,
        ResourceUri resourceUri,
        Key<TextEditorViewModel> viewModelKey)
    {
        _events = events;
        
        MouseEventArgs = mouseEventArgs;
        ResourceUri = resourceUri;
        ViewModelKey = viewModelKey;
    }

    public MouseEventArgs MouseEventArgs { get; }
    public ResourceUri ResourceUri { get; }
    public Key<TextEditorViewModel> ViewModelKey { get; }

    public TimeSpan ThrottleTimeSpan => _events.ThrottleDelayDefault;

    public IThrottleEvent? BatchOrDefault(IThrottleEvent moreRecentEvent)
    {
        return moreRecentEvent;
    }

    public Task HandleEvent(CancellationToken cancellationToken)
    {
        _events.TextEditorService.Post(
            nameof(ThrottleEventOnMouseDown),
            async editContext =>
            {
                var modelModifier = editContext.GetModelModifier(ResourceUri, true);
                var viewModelModifier = editContext.GetViewModelModifier(ViewModelKey);
                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                    return;

                var hasSelectedText = TextEditorSelectionHelper.HasSelectedText(primaryCursorModifier);

                if ((MouseEventArgs.Buttons & 1) != 1 && hasSelectedText)
                    return; // Not pressing the left mouse button so assume ContextMenu is desired result.

                await _events.CursorSetShouldDisplayMenuAsyncFunc.Invoke(TextEditorMenuKind.None, false).ConfigureAwait(false);

                // Remember the current cursor position prior to doing anything
                var inRowIndex = primaryCursorModifier.RowIndex;
                var inColumnIndex = primaryCursorModifier.ColumnIndex;

                // Move the cursor position
                var rowAndColumnIndex = await _events.CalculateRowAndColumnIndex(MouseEventArgs).ConfigureAwait(false);
                primaryCursorModifier.RowIndex = rowAndColumnIndex.rowIndex;
                primaryCursorModifier.ColumnIndex = rowAndColumnIndex.columnIndex;
                primaryCursorModifier.PreferredColumnIndex = rowAndColumnIndex.columnIndex;

                _events.CursorPauseBlinkAnimationAction.Invoke();

                var cursorPositionIndex = modelModifier.GetPositionIndex(new TextEditorCursor(
                    rowAndColumnIndex.rowIndex,
                    rowAndColumnIndex.columnIndex,
                    true));

                if (MouseEventArgs.ShiftKey)
                {
                    if (!hasSelectedText)
                    {
                        // If user does not yet have a selection then place the text selection anchor were they were
                        primaryCursorModifier.SelectionAnchorPositionIndex = modelModifier
                            .GetPositionIndex(inRowIndex, inColumnIndex);
                    }

                    // If user ALREADY has a selection then do not modify the text selection anchor
                }
                else
                {
                    primaryCursorModifier.SelectionAnchorPositionIndex = cursorPositionIndex;
                }

                primaryCursorModifier.SelectionEndingPositionIndex = cursorPositionIndex;
            });


        return Task.CompletedTask;
    }
}
