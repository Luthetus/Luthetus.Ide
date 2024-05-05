using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Ide.RazorLib.Gits.States;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.Gits.Displays;

/// <summary>
/// Long term goal is to support any arbitrary version control system.
/// For now, implement a Git UI, this lets us get a feel for what the interface should be.
/// </summary>
public partial class GitDisplay : FluxorComponent
{
    [Inject]
    private IState<GitState> GitStateWrap { get; set; } = null!;
}