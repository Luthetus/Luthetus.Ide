using Luthetus.Common.RazorLib.Reactives.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals.UiEvent;

public class ThrottleEventOnContextMenu : IThrottleEvent
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
