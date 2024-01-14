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
        Assert.Equal(ThemeState.DefaultThemeRecordsList, themeState.ThemeList);
    }

    /// <summary>
    /// <see cref="ThemeState.DefaultThemeRecordsList"/>
    /// </summary>
    [Fact]
    public void DefaultThemeRecordsList()
    {
        var themeState = new ThemeState();
        Assert.Equal(ThemeState.DefaultThemeRecordsList, themeState.ThemeList);

        var sampleThemeRecord = new ThemeRecord(
            Key<ThemeRecord>.NewKey(),
            "Test Dark Theme",
            "test_dark-theme",
            ThemeContrastKind.Default,
            ThemeColorKind.Dark,
            new ThemeScope[] { ThemeScope.App, ThemeScope.TextEditor }.ToImmutableList());

        var outThemeList = themeState.ThemeList.Add(sampleThemeRecord);

        themeState = themeState with
        {
            ThemeList = outThemeList
        };

        Assert.Contains(themeState.ThemeList, x => x == sampleThemeRecord);
    }
}