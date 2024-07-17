using Microsoft.AspNetCore.Components;
using Fluxor;
using Luthetus.Extensions.DotNet.DotNetSolutions.Displays;
using Luthetus.Extensions.DotNet.DotNetSolutions.States;

namespace Luthetus.Extensions.DotNet.Shareds.Displays;

public partial class IdeHeader : ComponentBase
{
	[Inject]
	private IState<DotNetSolutionState> DotNetSolutionStateWrap { get; set; } = null!;
}