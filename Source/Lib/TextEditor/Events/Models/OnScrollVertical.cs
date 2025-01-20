using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;

namespace Luthetus.TextEditor.RazorLib.Events.Models;

public struct OnScrollVertical : ITextEditorWork
{
    public OnScrollVertical(
        double scrollTop,
		TextEditorComponentData componentData,
        Key<TextEditorViewModel> viewModelKey)
    {
		ComponentData = componentData;

        ScrollTop = scrollTop;
        ViewModelKey = viewModelKey;
    }

    public Key<IBackgroundTask> BackgroundTaskKey => Key<IBackgroundTask>.Empty;
    public Key<IBackgroundTaskQueue> QueueKey { get; } = BackgroundTaskFacts.ContinuousQueueKey;
    public bool EarlyBatchEnabled { get; set; } = true;
    public bool __TaskCompletionSourceWasCreated { get; set; }
    // TODO: I'm uncomfortable as to whether "luth_{nameof(Abc123)}" is a constant interpolated string so I'm just gonna hardcode it.
    public string Name => "luth_OnScrollVertical";
    public double ScrollTop { get; }
    public Key<TextEditorViewModel> ViewModelKey { get; }
    public TextEditorComponentData ComponentData { get; }

	public ITextEditorEditContext? EditContext { get; private set; }

    public IBackgroundTask? EarlyBatchOrDefault(IBackgroundTask oldEvent)
    {
        if (oldEvent.Name == Name)
		{
			// Replace the upstream event with this one,
			// because unhandled-consecutive events of this type are redundant.
			return this;
		}
        
		// Keep both events, because they are not able to be batched.
		return null;
    }
    
    public IBackgroundTask? LateBatchOrDefault(IBackgroundTask oldEvent)
    {
    	return null;
    }

    public async ValueTask HandleEvent(CancellationToken cancellationToken)
    {
    	EditContext = new TextEditorEditContext(
            ComponentData.TextEditorViewModelDisplay.TextEditorService,
            TextEditorService.AuthenticatedActionKey);
    
        var viewModelModifier = EditContext.GetViewModelModifier(ViewModelKey);
        if (viewModelModifier is null)
            return;

        EditContext.TextEditorService.ViewModelApi.SetScrollPosition(
        	EditContext,
    		viewModelModifier,
        	null,
        	ScrollTop);
        	
        await EditContext.TextEditorService
        	.FinalizePost(EditContext)
        	.ConfigureAwait(false);
        	
        // await Task.Delay(ThrottleFacts.TwentyFour_Frames_Per_Second).ConfigureAwait(false);
    }
}
