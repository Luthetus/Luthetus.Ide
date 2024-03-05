using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals.UiEvent;

public class ThrottleEventOnWheel : IThrottleEvent
{
    private readonly ThrottleController _throttleControllerUiEvents;
    private readonly TimeSpan _uiEventsDelay;
    private readonly ITextEditorService _textEditorService;

    public ThrottleEventOnWheel(
        WheelEventArgs wheelEventArgs,
        ThrottleController throttleControllerUiEvents,
        TimeSpan uiEventsDelay,
        Key<TextEditorViewModel> viewModelKey,
        ITextEditorService textEditorService)
    {
        _throttleControllerUiEvents = throttleControllerUiEvents;
        _uiEventsDelay = uiEventsDelay;
        _textEditorService = textEditorService;

        WheelEventArgs = wheelEventArgs;
        ViewModelKey = viewModelKey;
    }

    public WheelEventArgs WheelEventArgs { get; }
    public Key<TextEditorViewModel> ViewModelKey { get; }

    public TimeSpan ThrottleTimeSpan => _uiEventsDelay;

    public IThrottleEvent? BatchOrDefault(IThrottleEvent moreRecentEvent)
    {
        return null;
    }

    public Task HandleEvent(CancellationToken cancellationToken)
    {
        _textEditorService.Post(
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
