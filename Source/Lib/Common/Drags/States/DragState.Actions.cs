namespace Luthetus.Common.RazorLib.Drags.Displays;

public partial record DragState
{
    public record WithAction(Func<DragState, DragState> WithFunc);
}