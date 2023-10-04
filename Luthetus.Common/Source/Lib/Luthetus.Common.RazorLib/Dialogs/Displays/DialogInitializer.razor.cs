using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Dialogs.States;

namespace Luthetus.Common.RazorLib.Dialogs.Displays;

public partial class DialogInitializer : FluxorComponent
{
    [Inject]
    private IState<DialogState> DialogStateWrap { get; set; } = null!;
}