using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Common.RazorLib.Icons.Models;

public class IconBase : ComponentBase
{
    [Inject]
    private LuthetusCommonApi CommonApi { get; set; } = null!;

    [CascadingParameter(Name = "LuthetusCommonIconWidthOverride")]
    public int? LuthetusCommonIconWidthOverride { get; set; }
    [CascadingParameter(Name = "LuthetusCommonIconHeightOverride")]
    public int? LuthetusCommonIconHeightOverride { get; set; }

    [Parameter]
    public string CssStyleString { get; set; } = string.Empty;

    public int WidthInPixels => LuthetusCommonIconWidthOverride ??
		CommonApi.AppOptionApi.GetAppOptionsState().Options.IconSizeInPixels;

    public int HeightInPixels => LuthetusCommonIconHeightOverride ??
		CommonApi.AppOptionApi.GetAppOptionsState().Options.IconSizeInPixels;
}