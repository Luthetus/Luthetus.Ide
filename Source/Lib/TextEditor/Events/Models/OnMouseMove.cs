using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;

namespace Luthetus.TextEditor.RazorLib.Events.Models;

public class OnMouseMove : ITextEditorTask
{
    public OnMouseMove(
        MouseEventArgs mouseEventArgs,
		TextEditorComponentData componentData,
        ResourceUri resourceUri,
        Key<TextEditorViewModel> viewModelKey)
    {
		ComponentData = componentData;

        MouseEventArgs = mouseEventArgs;
        ResourceUri = resourceUri;
        ViewModelKey = viewModelKey;
    }

    public Key<BackgroundTask> BackgroundTaskKey { get; } = Key<BackgroundTask>.NewKey();
    public Key<BackgroundTaskQueue> QueueKey { get; } = ContinuousBackgroundTaskWorker.GetQueueKey();
    public string Name { get; } = nameof(OnMouseMove);
    public Task? WorkProgress { get; }
    public MouseEventArgs MouseEventArgs { get; }
    public ResourceUri ResourceUri { get; }
    public Key<TextEditorViewModel> ViewModelKey { get; }
	public TextEditorComponentData ComponentData { get; }

	public IEditContext EditContext { get; set; }

    public TimeSpan ThrottleTimeSpan => TextEditorComponentData.ThrottleDelayDefault;

    public IBackgroundTask? BatchOrDefault(IBackgroundTask oldEvent)
    {
        if (oldEvent is OnMouseMove)
		{
			// Replace the upstream event with this one,
			// because unhandled-consecutive events of this type are redundant.
			return this;
		}
        
		// Keep both events, because they are not able to be batched.
		return null;
    }

    public async Task HandleEvent(CancellationToken cancellationToken)
    {
		try
		{
            var modelModifier = EditContext.GetModelModifier(ResourceUri, true);
            var viewModelModifier = EditContext.GetViewModelModifier(ViewModelKey);
            var cursorModifierBag = EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = EditContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return;

			// Labeling any IEditContext -> JavaScript interop or Blazor StateHasChanged.
			// Reason being, these are likely to be huge optimizations (2024-05-29).
            var rowAndColumnIndex = await EventUtils.CalculateRowAndColumnIndex(
					ResourceUri,
					ViewModelKey,
					MouseEventArgs,
					ComponentData,
					EditContext)
				.ConfigureAwait(false);

            primaryCursorModifier.LineIndex = rowAndColumnIndex.rowIndex;
            primaryCursorModifier.ColumnIndex = rowAndColumnIndex.columnIndex;
            primaryCursorModifier.PreferredColumnIndex = rowAndColumnIndex.columnIndex;

			EditContext.TextEditorService.ViewModelApi.SetCursorShouldBlink(false);

            primaryCursorModifier.SelectionEndingPositionIndex = modelModifier.GetPositionIndex(primaryCursorModifier);
		}
		finally
		{
			await EditContext.TextEditorService.FinalizePost(EditContext);
		}
    }
}
