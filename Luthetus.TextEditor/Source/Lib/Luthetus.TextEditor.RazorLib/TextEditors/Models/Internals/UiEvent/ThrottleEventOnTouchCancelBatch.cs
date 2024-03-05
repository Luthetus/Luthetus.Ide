using Luthetus.Common.RazorLib.Reactives.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals.UiEvent;

public class ThrottleEventOnTouchCancelBatch : IThrottleEvent
{
    public TimeSpan ThrottleTimeSpan => throw new NotImplementedException();

    public IThrottleEvent? BatchOrDefault(IThrottleEvent moreRecentEvent)
    {
        throw new NotImplementedException();
    }

    public Task HandleEvent(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
