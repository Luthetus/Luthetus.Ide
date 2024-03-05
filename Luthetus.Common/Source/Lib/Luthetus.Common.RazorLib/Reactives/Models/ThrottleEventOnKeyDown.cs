using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.Common.RazorLib.Reactives.Models;

public class ThrottleEventOnKeyDown : IThrottleEvent
{
    public ThrottleEventOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
        KeyboardEventArgs = keyboardEventArgs;
    }

    public TimeSpan ThrottleTimeSpan => TimeSpan.Zero;
    public KeyboardEventArgs KeyboardEventArgs { get; }

    public IThrottleEvent? BatchOrDefault(IThrottleEvent moreRecentEvent)
    {
        if (moreRecentEvent is ThrottleEventOnKeyDown moreRecentEventOnKeyDown)
        {
            return new ThrottleEventOnKeyDownBatch(new() 
            { 
                moreRecentEventOnKeyDown.KeyboardEventArgs,
                KeyboardEventArgs
            });
        }
        
        if (moreRecentEvent is ThrottleEventOnKeyDownBatch moreRecentEventOnKeyDownBatch)
        {
            moreRecentEventOnKeyDownBatch.KeyboardEventArgsList.Add(KeyboardEventArgs);
            return moreRecentEventOnKeyDownBatch;
        }

        return null;
    }

    public Task HandleEvent(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
