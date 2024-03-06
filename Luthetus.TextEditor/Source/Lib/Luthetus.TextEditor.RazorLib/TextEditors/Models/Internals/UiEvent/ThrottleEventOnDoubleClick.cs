using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals.UiEvent;

public class ThrottleEventOnDoubleClick : IThrottleEvent
{
    private readonly Func<MouseEventArgs, Task<(int rowIndex, int columnIndex)>> _calculateRowAndColumnIndexFunc;
    private readonly ThrottleController _throttleControllerUiEvents;
    private readonly TimeSpan _uiEventsDelay;
    private readonly ITextEditorService _textEditorService;

    public ThrottleEventOnDoubleClick(
        MouseEventArgs mouseEventArgs,
        Func<MouseEventArgs, Task<(int rowIndex, int columnIndex)>> calculateRowAndColumnIndexFunc,
        ThrottleController throttleControllerUiEvents,
        TimeSpan uiEventsDelay,
        ResourceUri resourceUri,
        Key<TextEditorViewModel> viewModelKey,
        ITextEditorService textEditorService)
    {
        _calculateRowAndColumnIndexFunc = calculateRowAndColumnIndexFunc;
        _throttleControllerUiEvents = throttleControllerUiEvents;
        _uiEventsDelay = uiEventsDelay;
        _textEditorService = textEditorService;
 
        MouseEventArgs = mouseEventArgs;
        ResourceUri = resourceUri;
        ViewModelKey = viewModelKey;
    }

    public MouseEventArgs MouseEventArgs { get; }
    public ResourceUri ResourceUri { get; }
    public Key<TextEditorViewModel> ViewModelKey { get; }

    public TimeSpan ThrottleTimeSpan => TimeSpan.Zero;

    public IThrottleEvent? BatchOrDefault(IThrottleEvent moreRecentEvent)
    {
        return moreRecentEvent;
    }

    public Task HandleEvent(CancellationToken cancellationToken)
    {
        _textEditorService.Post(
            nameof(ThrottleEventOnDoubleClick),
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

                if (MouseEventArgs.ShiftKey)
                    return; // Do not expand selection if user is holding shift

                var rowAndColumnIndex = await _calculateRowAndColumnIndexFunc(MouseEventArgs).ConfigureAwait(false);

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
    }
}
