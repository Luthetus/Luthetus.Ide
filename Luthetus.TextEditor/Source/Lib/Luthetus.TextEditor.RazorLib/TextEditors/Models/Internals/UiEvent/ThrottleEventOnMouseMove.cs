using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals.UiEvent;

public class ThrottleEventOnMouseMove : IThrottleEvent
{
    private readonly Func<MouseEventArgs, Task<(int rowIndex, int columnIndex)>> _calculateRowAndColumnIndexFunc;
    private readonly Action _cursorPauseBlinkAnimationAction;
    private readonly ThrottleController _throttleControllerUiEvents;
    private readonly TimeSpan _uiEventsDelay;
    private readonly ITextEditorService _textEditorService;

    public ThrottleEventOnMouseMove(
        MouseEventArgs mouseEventArgs,
        Func<MouseEventArgs, Task<(int rowIndex, int columnIndex)>> calculateRowAndColumnIndexFunc,
        Action cursorPauseBlinkAnimationAction,
        ThrottleController throttleControllerUiEvents,
        TimeSpan uiEventsDelay,
        ResourceUri resourceUri,
        Key<TextEditorViewModel> viewModelKey,
        ITextEditorService textEditorService)
    {
        _calculateRowAndColumnIndexFunc = calculateRowAndColumnIndexFunc;
        _cursorPauseBlinkAnimationAction = cursorPauseBlinkAnimationAction;
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

    public TimeSpan ThrottleTimeSpan => _uiEventsDelay;

    public IThrottleEvent? BatchOrDefault(IThrottleEvent moreRecentEvent)
    {
        return moreRecentEvent;
    }

    public Task HandleEvent(CancellationToken cancellationToken)
    {
        _textEditorService.Post(
            nameof(ThrottleEventOnMouseMove),
            async editContext =>
            {
                var modelModifier = editContext.GetModelModifier(ResourceUri, true);
                var viewModelModifier = editContext.GetViewModelModifier(ViewModelKey);

                if (modelModifier is null || viewModelModifier is null)
                    return;

                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier.ViewModel);
                var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (cursorModifierBag is null || primaryCursorModifier is null)
                    return;

                var rowAndColumnIndex = await _calculateRowAndColumnIndexFunc.Invoke(MouseEventArgs).ConfigureAwait(false);

                primaryCursorModifier.RowIndex = rowAndColumnIndex.rowIndex;
                primaryCursorModifier.ColumnIndex = rowAndColumnIndex.columnIndex;
                primaryCursorModifier.PreferredColumnIndex = rowAndColumnIndex.columnIndex;

                _cursorPauseBlinkAnimationAction.Invoke();

                primaryCursorModifier.SelectionEndingPositionIndex = modelModifier.GetPositionIndex(primaryCursorModifier);
            });

        return Task.CompletedTask;
    }
}
