using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals.UiEvent;

public class ThrottleEventOnWheel : IThrottleEvent
{
    private readonly WheelEventArgs _wheelEventArgs;
    private readonly ThrottleController _throttleControllerUiEvents;
    private readonly TimeSpan _uiEventsDelay;
    private readonly ResourceUri _resourceUri;
    private readonly Key<TextEditorViewModel> _viewModelKey;
    private readonly ITextEditorService _textEditorService;

    public ThrottleEventOnWheel(
        WheelEventArgs wheelEventArgs,
        ThrottleController throttleControllerUiEvents,
        TimeSpan uiEventsDelay,
        ResourceUri resourceUri,
        Key<TextEditorViewModel> viewModelKey,
        ITextEditorService textEditorService)
    {
        _wheelEventArgs = wheelEventArgs;
        _throttleControllerUiEvents = throttleControllerUiEvents;
        _uiEventsDelay = uiEventsDelay;
        _resourceUri = resourceUri;
        _viewModelKey = viewModelKey;
        _textEditorService = textEditorService;
    }

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
                var viewModelModifier = editContext.GetViewModelModifier(_viewModelKey);

                if (viewModelModifier is null)
                    return Task.CompletedTask;

                if (_wheelEventArgs.ShiftKey)
                    viewModelModifier.ViewModel.MutateScrollHorizontalPositionByPixels(_wheelEventArgs.DeltaY);
                else
                    viewModelModifier.ViewModel.MutateScrollVerticalPositionByPixels(_wheelEventArgs.DeltaY);

                return Task.CompletedTask;
            });

        return Task.CompletedTask;
    }
}
