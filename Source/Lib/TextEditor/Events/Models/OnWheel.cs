using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;

namespace Luthetus.TextEditor.RazorLib.Events.Models;

public struct OnWheel : ITextEditorWork
{
    public OnWheel(
        WheelEventArgs wheelEventArgs,
		TextEditorComponentData componentData,
        Key<TextEditorViewModel> viewModelKey)
    {
		ComponentData = componentData;

        WheelEventArgs = wheelEventArgs;
        ViewModelKey = viewModelKey;
    }

    public Key<IBackgroundTask> BackgroundTaskKey => Key<IBackgroundTask>.Empty;
    public Key<IBackgroundTaskQueue> QueueKey { get; } = BackgroundTaskFacts.ContinuousQueueKey;
    public bool EarlyBatchEnabled { get; set; } = true;
    public bool __TaskCompletionSourceWasCreated { get; set; }
    // TODO: I'm uncomfortable as to whether "luth_{nameof(Abc123)}" is a constant interpolated string so I'm just gonna hardcode it.
    public string Name => "luth_OnWheel";
    public WheelEventArgs WheelEventArgs { get; }
    public Key<TextEditorViewModel> ViewModelKey { get; }
    public TextEditorComponentData ComponentData { get; }

	public ITextEditorEditContext? EditContext { get; private set; }

    public IBackgroundTask? EarlyBatchOrDefault(IBackgroundTask oldEvent)
    {
		// Horizontal mouse wheel was not working on Linux.
		// Prior to the fix that was just made, only the 'WheelEventArgs.DeltaY'
		// was being used.
		//
		// Reason being, on Windows, regardless of if 'WheelEventArgs.ShiftKey' was true or false,
		// the 'WheelEventArgs.DeltaY' was always being provided as a non-zero value.
		//
		// But, on Linux, when not holding the shift key: 'WheelEventArgs.DeltaY' is set,
		// and 'WheelEventArgs.DeltaX' is 0.
		// The vice versa though, if one does hold the shift key, 'WheelEventArgs.DeltaY' is zero,
		// and 'WheelEventArgs.DeltaX' is set.
		//
		// If I end up breaking Windows with this change (to use ether 'WheelEventArgs.DeltaY'
		// or 'WheelEventArgs.DeltaX' dependent on 'WheelEventArgs.ShiftKey',
		// then... I'm tired and forgot what I was typing. (2024-05-28)


        // If the two individuals, or a batch and an individual are both positive,
        // then batch them, etc... for negative and 0

        if (oldEvent.Name == Name)
        {
        	var oldEventOnWheel = (OnWheel)oldEvent;
        	
			if (oldEventOnWheel.WheelEventArgs.ShiftKey && WheelEventArgs.ShiftKey)
			{
	            if (oldEventOnWheel.WheelEventArgs.DeltaX > 0 &&
	                WheelEventArgs.DeltaX > 0)
	            {
	                return new OnWheelBatch(
	                    new List<WheelEventArgs>()
	                    {
	                        oldEventOnWheel.WheelEventArgs,
	                        WheelEventArgs
	                    },
						ComponentData,
	                    ViewModelKey);
	            }
	            else if (oldEventOnWheel.WheelEventArgs.DeltaX < 0 &&
	                     WheelEventArgs.DeltaX < 0)
	            {
	                return new OnWheelBatch(
	                    new List<WheelEventArgs>()
	                    {
	                        oldEventOnWheel.WheelEventArgs,
	                        WheelEventArgs
	                    },
						ComponentData,
	                    ViewModelKey);
	            }
	            else if (oldEventOnWheel.WheelEventArgs.DeltaX == 0 &&
	                     WheelEventArgs.DeltaX == 0)
	            {
	                return new OnWheelBatch(
	                    new List<WheelEventArgs>()
	                    {
	                        oldEventOnWheel.WheelEventArgs,
	                        WheelEventArgs
	                    },
						ComponentData,
	                    ViewModelKey);
	            }
			}
			else if (!oldEventOnWheel.WheelEventArgs.ShiftKey && !WheelEventArgs.ShiftKey)
			{
	            if (oldEventOnWheel.WheelEventArgs.DeltaY > 0 &&
	                WheelEventArgs.DeltaY > 0)
	            {
	                return new OnWheelBatch(
	                    new List<WheelEventArgs>()
	                    {
	                        oldEventOnWheel.WheelEventArgs,
	                        WheelEventArgs
	                    },
						ComponentData,
	                    ViewModelKey);
	            }
	            else if (oldEventOnWheel.WheelEventArgs.DeltaY < 0 &&
	                     WheelEventArgs.DeltaY < 0)
	            {
	                return new OnWheelBatch(
	                    new List<WheelEventArgs>()
	                    {
	                        oldEventOnWheel.WheelEventArgs,
	                        WheelEventArgs
	                    },
						ComponentData,
	                    ViewModelKey);
	            }
	            else if (oldEventOnWheel.WheelEventArgs.DeltaY == 0 &&
	                     WheelEventArgs.DeltaY == 0)
	            {
	                return new OnWheelBatch(
	                    new List<WheelEventArgs>()
	                    {
	                        oldEventOnWheel.WheelEventArgs,
	                        WheelEventArgs
	                    },
						ComponentData,
	                    ViewModelKey);
	            }
			}
        }
        else if (oldEvent is OnWheelBatch oldEventOnWheelBatch)
        {
			if (oldEventOnWheelBatch.WheelEventArgsList.Last().ShiftKey && WheelEventArgs.ShiftKey)
			{
				if (oldEventOnWheelBatch.WheelEventArgsList.Last().DeltaX > 0 &&
                WheelEventArgs.DeltaX > 0)
	            {
	                oldEventOnWheelBatch.WheelEventArgsList.Add(WheelEventArgs);
	                return oldEventOnWheelBatch;
	            }
	            else if (oldEventOnWheelBatch.WheelEventArgsList.Last().DeltaX < 0 &&
	                     WheelEventArgs.DeltaX < 0)
	            {
	                oldEventOnWheelBatch.WheelEventArgsList.Add(WheelEventArgs);
	                return oldEventOnWheelBatch;
	            }
	            else if (oldEventOnWheelBatch.WheelEventArgsList.Last().DeltaX == 0 &&
	                     WheelEventArgs.DeltaX == 0)
	            {
	                oldEventOnWheelBatch.WheelEventArgsList.Add(WheelEventArgs);
	                return oldEventOnWheelBatch;
	            }
			}
   	     else if (!oldEventOnWheelBatch.WheelEventArgsList.Last().ShiftKey && !WheelEventArgs.ShiftKey)
			{
				if (oldEventOnWheelBatch.WheelEventArgsList.Last().DeltaY > 0 &&
                WheelEventArgs.DeltaY > 0)
	            {
	                oldEventOnWheelBatch.WheelEventArgsList.Add(WheelEventArgs);
	                return oldEventOnWheelBatch;
	            }
	            else if (oldEventOnWheelBatch.WheelEventArgsList.Last().DeltaY < 0 &&
	                     WheelEventArgs.DeltaY < 0)
	            {
	                oldEventOnWheelBatch.WheelEventArgsList.Add(WheelEventArgs);
	                return oldEventOnWheelBatch;
	            }
	            else if (oldEventOnWheelBatch.WheelEventArgsList.Last().DeltaY == 0 &&
	                     WheelEventArgs.DeltaY == 0)
	            {
	                oldEventOnWheelBatch.WheelEventArgsList.Add(WheelEventArgs);
	                return oldEventOnWheelBatch;
	            }
			}
        }

        return null;
    }
    
    public IBackgroundTask? LateBatchOrDefault(IBackgroundTask oldEvent)
    {
    	return null;
    }

    public async ValueTask HandleEvent(CancellationToken cancellationToken)
    {
    	EditContext = new TextEditorService.TextEditorEditContext(
            ComponentData.TextEditorViewModelDisplay.TextEditorService,
            TextEditorService.AuthenticatedActionKey);
    
        var viewModelModifier = EditContext.GetViewModelModifier(ViewModelKey);
        if (viewModelModifier is null)
            return;

		// TODO: Why was this made as 'if' 'else' whereas the OnWheelBatch...
		//       ...is doing 'if' 'if'.
		//       |
		//       The OnWheelBatch doesn't currently batch horizontal with vertical
		//       the OnWheel events have to be the same axis to batch.
        if (WheelEventArgs.ShiftKey)
        {
            EditContext.TextEditorService.ViewModelApi.MutateScrollHorizontalPosition(
            	EditContext,
		        viewModelModifier,
		        WheelEventArgs.DeltaX);
        }
        else
        {
            EditContext.TextEditorService.ViewModelApi.MutateScrollVerticalPosition(
            	EditContext,
		        viewModelModifier,
            	WheelEventArgs.DeltaY);
        }
        
        await EditContext.TextEditorService
        	.FinalizePost(EditContext)
        	.ConfigureAwait(false);
        	
        // await Task.Delay(ThrottleFacts.TwentyFour_Frames_Per_Second).ConfigureAwait(false);
    }
}
