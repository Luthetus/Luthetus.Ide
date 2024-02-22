using Fluxor;

namespace Luthetus.Common.RazorLib.Dispatchers;

public class LuthDispatcher : IDispatcher
{
    public event EventHandler<ActionDispatchedEventArgs> ActionDispatched;

    public void Dispatch(object action)
    {
        throw new NotImplementedException();
    }
}
