using Fluxor;
using Luthetus.Extensions.DotNet.DotNetSolutions.States;

namespace Luthetus.Extensions.DotNet.CodeSearches.States;

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
