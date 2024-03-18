using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals.UiEvent;

public class ThrottleEventOnMouseMove : ITextEditorTask
{
    private readonly TextEditorViewModelDisplay.TextEditorEvents _events;

    public ThrottleEventOnMouseMove(
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

    public Key<BackgroundTask> BackgroundTaskKey { get; } = Key<BackgroundTask>.NewKey();
    public Key<BackgroundTaskQueue> QueueKey { get; } = ContinuousBackgroundTaskWorker.GetQueueKey();
    public string Name { get; } = nameof(ThrottleEventOnMouseMove);
    public Task? WorkProgress { get; }
    public MouseEventArgs MouseEventArgs { get; }
    public ResourceUri ResourceUri { get; }
    public Key<TextEditorViewModel> ViewModelKey { get; }

    public TimeSpan ThrottleTimeSpan => _events.ThrottleDelayDefault;

    public async Task InvokeWithEditContext(ITextEditorEditContext editContext)
	{
		var modelModifier = editContext.GetModelModifier(ResourceUri, true);
        var viewModelModifier = editContext.GetViewModelModifier(ViewModelKey);
        var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
        var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

        if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
            return;

        var rowAndColumnIndex = await _events.CalculateRowAndColumnIndex(MouseEventArgs).ConfigureAwait(false);

        primaryCursorModifier.RowIndex = rowAndColumnIndex.rowIndex;
        primaryCursorModifier.ColumnIndex = rowAndColumnIndex.columnIndex;
        primaryCursorModifier.PreferredColumnIndex = rowAndColumnIndex.columnIndex;

        _events.CursorPauseBlinkAnimationAction.Invoke();

        primaryCursorModifier.SelectionEndingPositionIndex = modelModifier.GetPositionIndex(primaryCursorModifier);
	}

    public IBackgroundTask? BatchOrDefault(IBackgroundTask oldEvent)
    {
        return this;
    }

    public Task HandleEvent(CancellationToken cancellationToken)
    {
        throw new NotImplementedException($"{nameof(ITextEditorTask)} should not implement {nameof(HandleEvent)}" +
			"because they instead are contained within an 'IBackgroundTask' that came from the 'TextEditorService'");
    }
}
