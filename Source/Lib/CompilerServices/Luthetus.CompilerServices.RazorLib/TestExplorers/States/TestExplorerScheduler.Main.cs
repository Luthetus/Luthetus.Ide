using Fluxor;
using Luthetus.CompilerServices.RazorLib.DotNetSolutions.States;
using Luthetus.CompilerServices.RazorLib.CommandLines.Models;

namespace Luthetus.CompilerServices.RazorLib.TestExplorers.States;

public partial class TestExplorerScheduler
{
	private readonly DotNetCliOutputParser _dotNetCliOutputParser;
    private readonly IState<DotNetSolutionState> _dotNetSolutionStateWrap;

	public TestExplorerScheduler(
		DotNetCliOutputParser dotNetCliOutputParser,
		IState<DotNetSolutionState> dotNetSolutionStateWrap)
	{
		_dotNetCliOutputParser = dotNetCliOutputParser;
    	_dotNetSolutionStateWrap = dotNetSolutionStateWrap;
	}
}
