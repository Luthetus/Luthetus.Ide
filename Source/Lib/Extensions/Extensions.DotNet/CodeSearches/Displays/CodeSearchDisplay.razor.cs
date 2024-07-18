using Microsoft.AspNetCore.Components;
using Fluxor;
using Luthetus.Extensions.DotNet.DotNetSolutions.States;

namespace Luthetus.Extensions.DotNet.CodeSearches.Displays;

public partial class CodeSearchDisplay : ComponentBase
{
	[Inject]
	private IState<DotNetSolutionState> DotNetSolutionStateWrap { get; set; } = null!;
}