using Fluxor;
using Luthetus.CompilerServices.RazorLib.DotNetSolutions.States;

namespace Luthetus.CompilerServices.RazorLib.CodeSearches.States;

public partial record CodeSearchState
{
	public class Effector
	{
	    private readonly IState<DotNetSolutionState> _dotNetSolutionStateWrap;
	    
		public Effector(IState<DotNetSolutionState> dotNetSolutionStateWrap)
		{
			_dotNetSolutionStateWrap = dotNetSolutionStateWrap;
		}
	}
}
