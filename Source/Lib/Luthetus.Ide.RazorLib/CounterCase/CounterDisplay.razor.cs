using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Ide.ClassLib.Store.CounterCase;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.CounterCase;

public partial class CounterDisplay : FluxorComponent
{
    [Inject]
    private IState<CounterState> CounterStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    private void DispatchIncrementCounterStateAction()
    {
        Dispatcher.Dispatch(new IncrementCounterStateAction());
    }
}