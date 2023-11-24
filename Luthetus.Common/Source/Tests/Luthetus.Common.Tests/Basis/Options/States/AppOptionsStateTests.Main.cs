using Luthetus.Common.RazorLib.Options.States;
using Luthetus.Common.RazorLib.Themes.Models;

namespace Luthetus.Common.Tests.Basis.Options.States;

public class AppOptionsStateMainTests
{
    /// <summary>
    /// <see cref="AppOptionsState()"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var appOptionsState = new AppOptionsState();
        Assert.Equal(AppOptionsState.DefaultCommonOptions, appOptionsState.Options);
    }

    /// <summary>
    /// <see cref="AppOptionsState.DefaultCommonOptions"/>
    /// </summary>
    [Fact]
    public void DefaultCommonOptions()
    {
        Assert.Equal(
            AppOptionsState.DEFAULT_FONT_SIZE_IN_PIXELS,
            AppOptionsState.DefaultCommonOptions.FontSizeInPixels);
        
        Assert.Equal(
            AppOptionsState.DEFAULT_ICON_SIZE_IN_PIXELS,
            AppOptionsState.DefaultCommonOptions.IconSizeInPixels);

        Assert.Equal(
            ThemeFacts.VisualStudioDarkThemeClone.Key,
            AppOptionsState.DefaultCommonOptions.ThemeKey);

        Assert.Null(AppOptionsState.DefaultCommonOptions.FontFamily);
    }
}