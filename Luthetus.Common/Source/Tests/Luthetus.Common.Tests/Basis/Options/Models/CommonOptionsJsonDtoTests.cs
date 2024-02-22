using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.Options.States;
using Luthetus.Common.RazorLib.Themes.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.Tests.Basis.Options.Models;

/// <summary>
/// <see cref="CommonOptionsJsonDto"/>
/// </summary>
public class CommonOptionsJsonDtoTests
{
    /// <summary>
    /// <see cref="CommonOptionsJsonDto(int?, int?, Key{ThemeRecord}?, string?)"/>
    /// </summary>
    [Fact]
    public void ConstructorA()
    {
        var fontSizeInPixels = 20;
        var iconSizeInPixels = 16;
        var themeKey = ThemeFacts.VisualStudioDarkThemeClone.Key;
        var fontFamily = (string?)null;

        var commonOptionsJsonDto = new CommonOptionsJsonDto(
            fontSizeInPixels,
            iconSizeInPixels,
            themeKey,
            fontFamily);

        Assert.Equal(fontSizeInPixels, commonOptionsJsonDto.FontSizeInPixels);
        Assert.Equal(iconSizeInPixels, commonOptionsJsonDto.IconSizeInPixels);
        Assert.Equal(themeKey, commonOptionsJsonDto.ThemeKey);
        Assert.Equal(fontFamily, commonOptionsJsonDto.FontFamily);
    }
    
    /// <summary>
    /// <see cref="CommonOptionsJsonDto()"/>
    /// </summary>
    [Fact]
    public void ConstructorB()
    {
        var commonOptionsJsonDto = new CommonOptionsJsonDto();

        Assert.Null(commonOptionsJsonDto.FontSizeInPixels);
        Assert.Null(commonOptionsJsonDto.IconSizeInPixels);
        Assert.Null(commonOptionsJsonDto.ThemeKey);
        Assert.Null(commonOptionsJsonDto.FontFamily);
    }

    /// <summary>
    /// <see cref="CommonOptionsJsonDto(CommonOptions)"/>
    /// </summary>
    [Fact]
    public void ConstructorC()
    {
        var commonOptions = AppOptionsState.DefaultCommonOptions;
        var commonOptionsJsonDto = new CommonOptionsJsonDto(commonOptions);

        Assert.Equal(commonOptions.FontSizeInPixels, commonOptionsJsonDto.FontSizeInPixels);
        Assert.Equal(commonOptions.IconSizeInPixels, commonOptionsJsonDto.IconSizeInPixels);
        Assert.Equal(commonOptions.ThemeKey, commonOptionsJsonDto.ThemeKey);
        Assert.Equal(commonOptions.FontFamily, commonOptionsJsonDto.FontFamily);
    }
}