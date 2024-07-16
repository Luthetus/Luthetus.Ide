using Microsoft.AspNetCore.Components;
using Luthetus.Ide.RazorLib.DotNetSolutions.States;

namespace Luthetus.CompilerServices.RazorLib.CodeSearches.Displays;

public partial class CodeSearchDisplay : ComponentBase
{
	[Inject]
	private IState<DotNetSolutionState> DotNetSolutionStateWrap { get; set; } = null!;
}