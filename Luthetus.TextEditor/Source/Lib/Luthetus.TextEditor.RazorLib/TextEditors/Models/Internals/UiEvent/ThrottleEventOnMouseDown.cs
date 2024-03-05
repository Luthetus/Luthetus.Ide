using Luthetus.Common.RazorLib.Reactives.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals.UiEvent;

public class ThrottleEventOnMouseDown : IThrottleEvent
{
    public TimeSpan ThrottleTimeSpan => throw new NotImplementedException();

    public IThrottleEvent? BatchOrDefault(IThrottleEvent moreRecentEvent)
    {
        throw new NotImplementedException();
    }

    public Task HandleEvent(CancellationToken cancellationToken)
    {
        _throttleControllerUiEvents.EnqueueEvent(new ThrottleEvent<byte>(
            0,
            TimeSpan.Zero,
            0,
            (throttleEvent, _) =>
            {
                TextEditorService.Post(
                    nameof(HandleContentOnMouseDown),
                    async editContext =>
                    {
                        var modelModifier = editContext.GetModelModifier(modelResourceUri, true);
                        var viewModelModifier = editContext.GetViewModelModifier(viewModelKey.Value);

                        if (modelModifier is null || viewModelModifier is null)
                            return;

                        var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier.ViewModel);
                        var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                        if (cursorModifierBag is null || primaryCursorModifier is null)
                            return;

                        var hasSelectedText = TextEditorSelectionHelper.HasSelectedText(primaryCursorModifier);

                        if ((mouseEventArgs.Buttons & 1) != 1 && hasSelectedText)
                            return; // Not pressing the left mouse button so assume ContextMenu is desired result.

                        CursorDisplay?.SetShouldDisplayMenuAsync(TextEditorMenuKind.None, false);

                        // Remember the current cursor position prior to doing anything
                        var inRowIndex = primaryCursorModifier.RowIndex;
                        var inColumnIndex = primaryCursorModifier.ColumnIndex;

                        // Move the cursor position
                        var rowAndColumnIndex = await CalculateRowAndColumnIndex(mouseEventArgs).ConfigureAwait(false);
                        primaryCursorModifier.RowIndex = rowAndColumnIndex.rowIndex;
                        primaryCursorModifier.ColumnIndex = rowAndColumnIndex.columnIndex;
                        primaryCursorModifier.PreferredColumnIndex = rowAndColumnIndex.columnIndex;

                        CursorDisplay?.PauseBlinkAnimation();

                        var cursorPositionIndex = modelModifier.GetPositionIndex(new TextEditorCursor(
                            rowAndColumnIndex.rowIndex,
                            rowAndColumnIndex.columnIndex,
                            true));

                        if (mouseEventArgs.ShiftKey)
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

                        _thinksLeftMouseButtonIsDown = true;
                    });


                return Task.CompletedTask;
            },
            tuple => tuple.RecentEvent));

        throw new NotImplementedException();
    }
}
