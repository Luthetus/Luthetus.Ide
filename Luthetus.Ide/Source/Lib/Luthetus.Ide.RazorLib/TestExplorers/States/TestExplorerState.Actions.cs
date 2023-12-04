using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.Models;

namespace Luthetus.Ide.RazorLib.TestExplorers.States;

public partial record TestExplorerState
{
    public record WithAction(Func<TestExplorerState, TestExplorerState> WithFunc);
}