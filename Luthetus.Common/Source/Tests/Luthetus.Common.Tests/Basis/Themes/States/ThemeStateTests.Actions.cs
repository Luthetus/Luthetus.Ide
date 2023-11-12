using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Themes.Models;
using Luthetus.Common.RazorLib.Themes.States;
using System.Collections.Immutable;

namespace Luthetus.Common.Tests.Basis.Themes.States;

/// <summary>
/// <see cref="ThemeState"/>
/// </summary>
public class ThemeStateActionsTests
{
    /// <summary>
    /// <see cref="ThemeState.RegisterAction"/>
    /// </summary>
    [Fact]
    public void RegisterAction()
    {
        InitializeThemeStateActionsTests(out var themeRecord);

        var registerAction = new ThemeState.RegisterAction(themeRecord);
        Assert.Equal(themeRecord, registerAction.Theme);
    }

    /// <summary>
    /// <see cref="ThemeState.DisposeAction"/>
    /// </summary>
    [Fact]
    public void DisposeAction()
    {
        InitializeThemeStateActionsTests(out var themeRecord);

        var disposeAction = new ThemeState.DisposeAction(themeRecord.Key);
        Assert.Equal(themeRecord.Key, disposeAction.ThemeKey);
    }

    private void InitializeThemeStateActionsTests(
        out ThemeRecord sampleThemeRecord)
    {
        sampleThemeRecord = new ThemeRecord(
            Key<ThemeRecord>.NewKey(),
            "Test Dark Theme",
            "test_dark-theme",
            ThemeContrastKind.Default,
            ThemeColorKind.Dark,
            new ThemeScope[] { ThemeScope.App, ThemeScope.TextEditor }.ToImmutableList());
    }
}