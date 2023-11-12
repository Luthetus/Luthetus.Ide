using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Themes.Models;
using Luthetus.Common.RazorLib.Themes.States;
using System.Collections.Immutable;

namespace Luthetus.Common.Tests.Basis.Themes.States;

/// <summary>
/// <see cref="ThemeState"/>
/// </summary>
public class ThemeStateMainTests
{
    /// <summary>
    /// <see cref="ThemeState()"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var themeState = new ThemeState();
        Assert.Equal(ThemeState.DefaultThemeRecordsBag, themeState.ThemeBag);
    }

    /// <summary>
    /// <see cref="ThemeState.DefaultThemeRecordsBag"/>
    /// </summary>
    [Fact]
    public void DefaultThemeRecordsBag()
    {
        var themeState = new ThemeState();
        Assert.Equal(ThemeState.DefaultThemeRecordsBag, themeState.ThemeBag);

        var sampleThemeRecord = new ThemeRecord(
            Key<ThemeRecord>.NewKey(),
            "Test Dark Theme",
            "test_dark-theme",
            ThemeContrastKind.Default,
            ThemeColorKind.Dark,
            new ThemeScope[] { ThemeScope.App, ThemeScope.TextEditor }.ToImmutableList());

        var outThemeBag = themeState.ThemeBag.Add(sampleThemeRecord);

        themeState = themeState with
        {
            ThemeBag = outThemeBag
        };

        Assert.Contains(themeState.ThemeBag, x => x == sampleThemeRecord);
    }
}