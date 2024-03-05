using Luthetus.Common.RazorLib.Reactives.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals.UiEvent;

public class ThrottleEventOnMouseMove : IThrottleEvent
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
                _uiEventsDelay,
                0,
                (throttleEvent, _) =>
                {
                    TextEditorService.Post(
                        nameof(HandleContentOnMouseMove),
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

                            var rowAndColumnIndex = await CalculateRowAndColumnIndex(mouseEventArgs).ConfigureAwait(false);

                            primaryCursorModifier.RowIndex = rowAndColumnIndex.rowIndex;
                            primaryCursorModifier.ColumnIndex = rowAndColumnIndex.columnIndex;
                            primaryCursorModifier.PreferredColumnIndex = rowAndColumnIndex.columnIndex;

                            CursorDisplay?.PauseBlinkAnimation();

                            primaryCursorModifier.SelectionEndingPositionIndex = modelModifier.GetPositionIndex(primaryCursorModifier);
                        });

                    return Task.CompletedTask;
                },
                tuple => tuple.RecentEvent));

        throw new NotImplementedException();
    }
}
