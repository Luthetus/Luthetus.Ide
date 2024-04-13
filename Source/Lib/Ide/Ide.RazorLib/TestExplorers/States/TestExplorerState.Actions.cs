namespace Luthetus.Ide.RazorLib.TestExplorers.States;

public partial record TestExplorerState
{
    public record WithAction(Func<TestExplorerState, TestExplorerState> WithFunc);
}