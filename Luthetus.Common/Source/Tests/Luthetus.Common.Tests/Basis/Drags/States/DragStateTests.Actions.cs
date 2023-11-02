namespace Luthetus.Common.RazorLib.Drags.Displays;

public partial record DragStateTests
{
    public record WithAction(Func<DragState, DragState> WithFunc);
}