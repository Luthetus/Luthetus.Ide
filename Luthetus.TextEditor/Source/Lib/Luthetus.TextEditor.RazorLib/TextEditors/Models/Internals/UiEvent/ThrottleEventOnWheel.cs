using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals.UiEvent;

public class ThrottleEventOnWheel : IThrottleEvent
{
    private readonly TextEditorViewModelDisplay.TextEditorEvents _events;

    public ThrottleEventOnWheel(
        WheelEventArgs wheelEventArgs,
        TextEditorViewModelDisplay.TextEditorEvents events,
        Key<TextEditorViewModel> viewModelKey)
    {
        _events = events;

        WheelEventArgs = wheelEventArgs;
        ViewModelKey = viewModelKey;
    }

    public WheelEventArgs WheelEventArgs { get; }
    public Key<TextEditorViewModel> ViewModelKey { get; }

    public TimeSpan ThrottleTimeSpan => _events.ThrottleDelayDefault;

    public IThrottleEvent? BatchOrDefault(IThrottleEvent oldEvent)
    {
        if (oldEvent is ThrottleEventOnWheel oldEventOnWheel)
        {
            return new ThrottleEventOnWheelBatch(
                new List<WheelEventArgs>()
                {
                    oldEventOnWheel.WheelEventArgs,
                    WheelEventArgs
                },
                _events,
                ViewModelKey);
        }

        if (oldEvent is ThrottleEventOnWheelBatch oldEventOnWheelBatch)
        {
            oldEventOnWheelBatch.WheelEventArgsList.Add(WheelEventArgs);
            return oldEventOnWheelBatch;
        }

        return null;
    }

    public Task HandleEvent(CancellationToken cancellationToken)
    {
        _events.TextEditorService.Post(
            nameof(ThrottleEventOnWheel),
            editContext =>
            {
                var viewModelModifier = editContext.GetViewModelModifier(ViewModelKey);

                if (viewModelModifier is null)
                    return Task.CompletedTask;

                if (WheelEventArgs.ShiftKey)
                    viewModelModifier.ViewModel.MutateScrollHorizontalPositionByPixels(WheelEventArgs.DeltaY);
                else
                    viewModelModifier.ViewModel.MutateScrollVerticalPositionByPixels(WheelEventArgs.DeltaY);

                return Task.CompletedTask;
            });

        return Task.CompletedTask;
    }
}
