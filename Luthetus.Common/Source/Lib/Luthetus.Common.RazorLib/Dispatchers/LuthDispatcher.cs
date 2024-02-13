using Fluxor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.Common.RazorLib.Dispatchers;

public class LuthDispatcher : IDispatcher
{
    public event EventHandler<ActionDispatchedEventArgs> ActionDispatched;

    public void Dispatch(object action)
    {
        throw new NotImplementedException();
    }
}
