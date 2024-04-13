using Fluxor;
using Luthetus.Common.RazorLib.Drags.Displays;

namespace Luthetus.Common.RazorLib.Drags.Models;

public interface IDragService
{
    public IState<DragState> DragStateWrap { get; }

    public void WithAction(Func<DragState, DragState> withFunc);
}