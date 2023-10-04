using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.Options.States;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Common.RazorLib.Icons.Models;

public class IconBase : FluxorComponent
{
    [Inject]
    private IState<AppOptionsState> AppOptionsStateWrap { get; set; } = null!;

    [CascadingParameter(Name = "LuthetusCommonIconWidthOverride")]
    public int? LuthetusCommonIconWidthOverride { get; set; }
    [CascadingParameter(Name = "LuthetusCommonIconHeightOverride")]
    public int? LuthetusCommonIconHeightOverride { get; set; }

    [Parameter]
    public string CssStyleString { get; set; } = string.Empty;

    public int WidthInPixels => LuthetusCommonIconWidthOverride ??
        AppOptionsStateWrap.Value.Options.IconSizeInPixels;

    public int HeightInPixels => LuthetusCommonIconHeightOverride ??
        AppOptionsStateWrap.Value.Options.IconSizeInPixels;
}