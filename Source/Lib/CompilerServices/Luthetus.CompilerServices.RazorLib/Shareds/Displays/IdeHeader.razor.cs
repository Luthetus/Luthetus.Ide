using Microsoft.AspNetCore.Components;
using Fluxor;
using Luthetus.CompilerServices.RazorLib.DotNetSolutions.Displays;
using Luthetus.CompilerServices.RazorLib.DotNetSolutions.States;

namespace Luthetus.CompilerServices.RazorLib.Shareds.Displays;

public partial class IdeHeader : ComponentBase
{
	[Inject]
	private IState<DotNetSolutionState> DotNetSolutionStateWrap { get; set; } = null!;
}