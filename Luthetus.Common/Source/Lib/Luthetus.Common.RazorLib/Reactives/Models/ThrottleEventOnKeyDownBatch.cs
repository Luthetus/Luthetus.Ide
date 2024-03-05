using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.Common.RazorLib.Reactives.Models;

public class ThrottleEventOnKeyDownBatch : IThrottleEvent
{
    public ThrottleEventOnKeyDownBatch(List<KeyboardEventArgs> keyboardEventArgsList)
    {
        KeyboardEventArgsList = keyboardEventArgsList;
    }

    public TimeSpan ThrottleTimeSpan => TimeSpan.Zero;
    public List<KeyboardEventArgs> KeyboardEventArgsList { get; }

    public IThrottleEvent? BatchOrDefault(IThrottleEvent moreRecentEvent)
    {
        if (moreRecentEvent is ThrottleEventOnKeyDown moreRecentEventOnKeyDown)
        {
            KeyboardEventArgsList.Insert(0, moreRecentEventOnKeyDown.KeyboardEventArgs);
            return this;
        }

        if (moreRecentEvent is ThrottleEventOnKeyDownBatch moreRecentEventOnKeyDownBatch)
        {
            moreRecentEventOnKeyDownBatch.KeyboardEventArgsList.AddRange(KeyboardEventArgsList);
            return moreRecentEventOnKeyDownBatch;
        }

        return null;
    }

    public Task HandleEvent(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
