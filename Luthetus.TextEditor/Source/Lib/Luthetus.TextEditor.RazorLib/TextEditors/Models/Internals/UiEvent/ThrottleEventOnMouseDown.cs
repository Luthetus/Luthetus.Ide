using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals.UiEvent;

public class ThrottleEventOnMouseDown : IThrottleEvent
{
    private readonly MouseEventArgs _mouseEventArgs;
    private readonly Func<MouseEventArgs, Task<(int rowIndex, int columnIndex)>> _calculateRowAndColumnIndexFunc;
    private readonly Action _cursorPauseBlinkAnimationAction;
    private readonly Func<TextEditorMenuKind, bool, Task> _cursorSetShouldDisplayMenuAsyncFunc;
    private readonly ThrottleController _throttleControllerUiEvents;
    private readonly TimeSpan _uiEventsDelay;
    private readonly ResourceUri _resourceUri;
    private readonly Key<TextEditorViewModel> _viewModelKey;
    private readonly ITextEditorService _textEditorService;

    public ThrottleEventOnMouseDown(
        MouseEventArgs mouseEventArgs,
        Func<MouseEventArgs, Task<(int rowIndex, int columnIndex)>> calculateRowAndColumnIndexFunc,
        Action cursorPauseBlinkAnimationAction,
        Func<TextEditorMenuKind, bool, Task> cursorSetShouldDisplayMenuAsyncFunc,
        ThrottleController throttleControllerUiEvents,
        TimeSpan uiEventsDelay,
        ResourceUri resourceUri,
        Key<TextEditorViewModel> viewModelKey,
        ITextEditorService textEditorService)
    {
        _mouseEventArgs = mouseEventArgs;
        _calculateRowAndColumnIndexFunc = calculateRowAndColumnIndexFunc;
        _cursorPauseBlinkAnimationAction = cursorPauseBlinkAnimationAction;
        _cursorSetShouldDisplayMenuAsyncFunc = cursorSetShouldDisplayMenuAsyncFunc;
        _throttleControllerUiEvents = throttleControllerUiEvents;
        _uiEventsDelay = uiEventsDelay;
        _resourceUri = resourceUri;
        _viewModelKey = viewModelKey;
        _textEditorService = textEditorService;
    }

    public TimeSpan ThrottleTimeSpan => TimeSpan.Zero;

    public IThrottleEvent? BatchOrDefault(IThrottleEvent moreRecentEvent)
    {
        return moreRecentEvent;
    }

    public Task HandleEvent(CancellationToken cancellationToken)
    {
        _textEditorService.Post(
            nameof(ThrottleEventOnMouseDown),
            async editContext =>
            {
                var modelModifier = editContext.GetModelModifier(_resourceUri, true);
                var viewModelModifier = editContext.GetViewModelModifier(_viewModelKey);

                if (modelModifier is null || viewModelModifier is null)
                    return;

                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier.ViewModel);
                var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (cursorModifierBag is null || primaryCursorModifier is null)
                    return;

                var hasSelectedText = TextEditorSelectionHelper.HasSelectedText(primaryCursorModifier);

                if ((_mouseEventArgs.Buttons & 1) != 1 && hasSelectedText)
                    return; // Not pressing the left mouse button so assume ContextMenu is desired result.

                await _cursorSetShouldDisplayMenuAsyncFunc.Invoke(TextEditorMenuKind.None, false);

                // Remember the current cursor position prior to doing anything
                var inRowIndex = primaryCursorModifier.RowIndex;
                var inColumnIndex = primaryCursorModifier.ColumnIndex;

                // Move the cursor position
                var rowAndColumnIndex = await _calculateRowAndColumnIndexFunc.Invoke(_mouseEventArgs).ConfigureAwait(false);
                primaryCursorModifier.RowIndex = rowAndColumnIndex.rowIndex;
                primaryCursorModifier.ColumnIndex = rowAndColumnIndex.columnIndex;
                primaryCursorModifier.PreferredColumnIndex = rowAndColumnIndex.columnIndex;

                _cursorPauseBlinkAnimationAction();

                var cursorPositionIndex = modelModifier.GetPositionIndex(new TextEditorCursor(
                    rowAndColumnIndex.rowIndex,
                    rowAndColumnIndex.columnIndex,
                    true));

                if (_mouseEventArgs.ShiftKey)
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
