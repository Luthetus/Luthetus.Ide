using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Options.States;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Common.RazorLib.Icons.Displays;

public partial class IconBadgeDisplay : FluxorComponent
{
    [Inject]
    public IState<AppOptionsState> AppOptionsStateWrap { get; set; } = null!;

    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = null!;

    [Parameter]
    public IconBadgeHorizontalPositionKind IconBadgeHorizontalPositionKind { get; set; } = IconBadgeHorizontalPositionKind.Right;
    [Parameter]
    public IconBadgeVerticalPositionKind IconBadgeVerticalPositionKind { get; set; } = IconBadgeVerticalPositionKind.Top;

    private string GetStyleString(AppOptionsState localAppOptionsState)
    {
        var minimumBadgeSize = 1.5;

        var iconSizeInPixels = localAppOptionsState.Options.IconSizeInPixels;

        var badgeSizeInPixels = iconSizeInPixels / 3.0;
        badgeSizeInPixels = Math.Max(minimumBadgeSize, badgeSizeInPixels);

        var badgeSizeInPixelsCssValue = badgeSizeInPixels.ToCssValue();

        string widthStyle = $"width: {badgeSizeInPixelsCssValue}px;";

        string heightStyle = $"height: {badgeSizeInPixelsCssValue}px;";

        string horizontalStyle;
        {
            var horizontalPropertyName = IconBadgeHorizontalPositionKind
                .ToString()
                .ToLower();

            var rightInPixels = badgeSizeInPixels - minimumBadgeSize;
            var rightInPixelsCssValue = rightInPixels.ToCssValue();

            horizontalStyle = $"{horizontalPropertyName}: {rightInPixelsCssValue}px;";
        }

        string verticalStyle;
        {
            var verticalPropertyName = IconBadgeVerticalPositionKind
                .ToString()
                .ToLower();

            verticalStyle = $"{verticalPropertyName}: {0}px;";
        }

        return $"{widthStyle} {heightStyle} {horizontalStyle} {verticalStyle}";
    }
}

