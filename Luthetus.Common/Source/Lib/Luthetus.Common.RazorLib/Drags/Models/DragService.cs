using Fluxor;
using Luthetus.Common.RazorLib.Drags.Displays;

namespace Luthetus.Common.RazorLib.Drags.Models;

public class DragService : IDragService
{
    private readonly IDispatcher _dispatcher;

    public DragService(
        IDispatcher dispatcher,
        IState<DragState> dragStateWrap)
    {
        _dispatcher = dispatcher;
        DragStateWrap = dragStateWrap;
    }

    public IState<DragState> DragStateWrap { get; }

    public void WithAction(Func<DragState, DragState> withFunc)
    {
        _dispatcher.Dispatch(new DragState.WithAction(
            withFunc));
    }
}