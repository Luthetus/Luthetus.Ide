using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Options.Models;

namespace Luthetus.Common.RazorLib.Icons.Models;

public class IconBase : ComponentBase
{
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;

    [CascadingParameter(Name = "LuthetusCommonIconWidthOverride")]
    public int? LuthetusCommonIconWidthOverride { get; set; }
    [CascadingParameter(Name = "LuthetusCommonIconHeightOverride")]
    public int? LuthetusCommonIconHeightOverride { get; set; }

    [Parameter]
    public string CssStyleString { get; set; } = string.Empty;

    public int WidthInPixels => LuthetusCommonIconWidthOverride ??
        AppOptionsService.GetAppOptionsState().Options.IconSizeInPixels;

    public int HeightInPixels => LuthetusCommonIconHeightOverride ??
        AppOptionsService.GetAppOptionsState().Options.IconSizeInPixels;
}