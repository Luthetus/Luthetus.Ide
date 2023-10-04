using Luthetus.Common.RazorLib.Keys.Models;
using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.Themes.Models;

public static class ThemeFacts
{
    public static readonly ThemeRecord VisualStudioLightThemeClone = new ThemeRecord(
        new Key<ThemeRecord>(Guid.Parse("3ea6a4a5-02b3-4b84-9d6f-e663465d3126")),
        "Visual Studio Light Clone",
        "luth_visual-studio-light-theme-clone",
        ThemeContrastKind.Default,
        ThemeColorKind.Light,
        new[] { ThemeScope.App, ThemeScope.TextEditor }.ToImmutableList());

    public static readonly ThemeRecord VisualStudioDarkThemeClone = new ThemeRecord(
        new Key<ThemeRecord>(Guid.Parse("8eaabd97-186d-40d0-a57b-5fec1c158902")),
        "Visual Studio Dark Clone",
        "luth_visual-studio-dark-theme-clone",
        ThemeContrastKind.Default,
        ThemeColorKind.Dark,
        new[] { ThemeScope.App, ThemeScope.TextEditor }.ToImmutableList());
}