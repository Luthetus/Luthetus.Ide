using Microsoft.AspNetCore.Components;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.Dropdowns.States;

namespace Luthetus.Common.RazorLib.Dropdowns.Displays;

public partial class DropdownInitializer : FluxorComponent
{
	[Inject]
    private IState<DropdownState> DropdownStateWrap { get; set; } = null!;
}