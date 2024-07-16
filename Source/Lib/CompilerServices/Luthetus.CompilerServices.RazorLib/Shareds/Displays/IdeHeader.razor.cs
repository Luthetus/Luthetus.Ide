using Microsoft.AspNetCore.Components;
using Luthetus.Ide.RazorLib.DotNetSolutions.Displays;
using Luthetus.Ide.RazorLib.DotNetSolutions.States;

namespace Luthetus.CompilerServices.RazorLib.Shareds.Displays;

public partial class IdeHeader : ComponentBase
{
	[Inject]
	private IState<DotNetSolutionState> DotNetSolutionStateWrap { get; set; } = null!;
}