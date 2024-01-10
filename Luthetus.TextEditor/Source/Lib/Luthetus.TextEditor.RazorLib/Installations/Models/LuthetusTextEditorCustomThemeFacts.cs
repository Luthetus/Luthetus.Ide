using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Themes.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.Installations.Models;

public class LuthetusTextEditorCustomThemeFacts
{
    public static readonly ThemeRecord LightTheme = new ThemeRecord(
        new Key<ThemeRecord>(Guid.Parse("8165209b-0cea-45b4-b6dd-e5661b319c73")),
        "Luthetus IDE Light Theme",
        "luth_light-theme",
        ThemeContrastKind.Default,
        ThemeColorKind.Light,
        new[] { ThemeScope.TextEditor }.ToImmutableList());

    public static readonly ThemeRecord DarkTheme = new ThemeRecord(
        new Key<ThemeRecord>(Guid.Parse("56d64327-03c2-48a3-b086-11b101826efb")),
        "Luthetus IDE Dark Theme",
        "luth_dark-theme",
        ThemeContrastKind.Default,
        ThemeColorKind.Dark,
        new[] { ThemeScope.TextEditor }.ToImmutableList());

    public static readonly ImmutableArray<ThemeRecord> AllCustomThemesList = new[]
    {
        LightTheme,
        DarkTheme
    }.ToImmutableArray();
}