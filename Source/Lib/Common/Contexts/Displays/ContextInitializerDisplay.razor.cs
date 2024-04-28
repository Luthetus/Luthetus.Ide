using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.Contexts.States;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Common.RazorLib.Contexts.Displays;

public partial class ContextInitializerDisplay : FluxorComponent
{
    [Inject]
    private IState<ContextState> ContextStateWrap { get; set; } = null!;
}
