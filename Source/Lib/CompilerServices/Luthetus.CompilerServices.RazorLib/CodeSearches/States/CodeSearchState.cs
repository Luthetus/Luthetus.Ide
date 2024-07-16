using Luthetus.Ide.RazorLib.DotNetSolutions.States;

namespace Luthetus.CompilerServices.RazorLib.CodeSearches.States;

public partial record CodeSearchState
{
    private readonly IState<DotNetSolutionState> _dotNetSolutionStateWrap;
    
	public Effector(IState<DotNetSolutionState> dotNetSolutionStateWrap)
	{
		_dotNetSolutionStateWrap = dotNetSolutionStateWrap;
	}
}
