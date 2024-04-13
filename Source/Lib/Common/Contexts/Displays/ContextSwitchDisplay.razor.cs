using Fluxor;
using Luthetus.Common.RazorLib.Contexts.States;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Common.RazorLib.Contexts.Displays;

public partial class ContextSwitchDisplay : ComponentBase
{
	[Inject]
	private IState<ContextState> ContextStateWrap { get; set; } = null!;
}