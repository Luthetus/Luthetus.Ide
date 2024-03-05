using Luthetus.Common.RazorLib.Reactives.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals.UiEvent;

public class ThrottleEventOnDoubleClick : IThrottleEvent
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
                    nameof(ReceiveOnDoubleClick),
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

                        if (mouseEventArgs.ShiftKey)
                            return; // Do not expand selection if user is holding shift

                        var rowAndColumnIndex = await CalculateRowAndColumnIndex(mouseEventArgs).ConfigureAwait(false);

                        var lowerColumnIndexExpansion = modelModifier.GetColumnIndexOfCharacterWithDifferingKind(
                            rowAndColumnIndex.rowIndex,
                            rowAndColumnIndex.columnIndex,
                            true);

                        lowerColumnIndexExpansion = lowerColumnIndexExpansion == -1
                            ? 0
                            : lowerColumnIndexExpansion;

                        var higherColumnIndexExpansion = modelModifier.GetColumnIndexOfCharacterWithDifferingKind(
                            rowAndColumnIndex.rowIndex,
                            rowAndColumnIndex.columnIndex,
                            false);

                        higherColumnIndexExpansion = higherColumnIndexExpansion == -1
                                ? modelModifier.GetLengthOfRow(rowAndColumnIndex.rowIndex)
                                : higherColumnIndexExpansion;

                        // Move user's cursor position to the higher expansion
                        {
                            primaryCursorModifier.RowIndex = rowAndColumnIndex.rowIndex;
                            primaryCursorModifier.ColumnIndex = higherColumnIndexExpansion;
                            primaryCursorModifier.PreferredColumnIndex = rowAndColumnIndex.columnIndex;
                        }

                        // Set text selection ending to higher expansion
                        {
                            var cursorPositionOfHigherExpansion = modelModifier.GetPositionIndex(
                                rowAndColumnIndex.rowIndex,
                                higherColumnIndexExpansion);

                            primaryCursorModifier.SelectionEndingPositionIndex = cursorPositionOfHigherExpansion;
                        }

                        // Set text selection anchor to lower expansion
                        {
                            var cursorPositionOfLowerExpansion = modelModifier.GetPositionIndex(
                                rowAndColumnIndex.rowIndex,
                                lowerColumnIndexExpansion);

                            primaryCursorModifier.SelectionAnchorPositionIndex = cursorPositionOfLowerExpansion;
                        }
                    });

                return Task.CompletedTask;
            },
            tuple => tuple.RecentEvent));

        throw new NotImplementedException();
    }
}
