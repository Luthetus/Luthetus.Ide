namespace Luthetus.Ide.Tests.Basis.TestExplorers.States;

public partial record TestExplorerStateActionsTests
{
    public record WithAction(Func<TestExplorerState, TestExplorerState> WithFunc);
}