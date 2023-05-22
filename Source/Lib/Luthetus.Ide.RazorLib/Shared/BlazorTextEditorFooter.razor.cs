using BlazorCommon.RazorLib.Dimensions;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Ide.ClassLib.Store.FooterCase;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.Shared;

public partial class BlazorTextEditorFooter : FluxorComponent
{
    [Inject]
    private IState<FooterState> FooterStateWrap { get; set; } = null!;

    [Parameter, EditorRequired]
    public ElementDimensions FooterElementDimensions { get; set; } = null!;
}