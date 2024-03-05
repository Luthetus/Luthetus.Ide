using Luthetus.Common.RazorLib.Reactives.Models;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals.UiEvent;

public class ThrottleEventOnWheel : IThrottleEvent
{
    public TimeSpan ThrottleTimeSpan => throw new NotImplementedException();

    public IThrottleEvent? BatchOrDefault(IThrottleEvent moreRecentEvent)
    {
        throw new NotImplementedException();
    }

    public Task HandleEvent(CancellationToken cancellationToken)
    {
        _throttleControllerUiEvents.EnqueueEvent(new ThrottleEvent<WheelEventArgs>(
            nameof(HandleOnWheel),
            _uiEventsDelay,
            0,
            (throttleEvent, _) =>
            {
                TextEditorService.Post(
                    nameof(HandleOnWheel),
                    async editContext =>
                    {
                        var viewModelModifier = editContext.GetViewModelModifier(viewModelKey.Value);

                        if (viewModelModifier is null)
                            return;

                        if (wheelEventArgs.ShiftKey)
                            viewModelModifier.ViewModel.MutateScrollHorizontalPositionByPixels(wheelEventArgs.DeltaY);
                        else
                            viewModelModifier.ViewModel.MutateScrollVerticalPositionByPixels(wheelEventArgs.DeltaY);
                    });

                return Task.CompletedTask;
            },
            tuple =>
            {
                if ()
                {

                }
                return tuple.RecentEvent;
            }));

        throw new NotImplementedException();
    }
}
