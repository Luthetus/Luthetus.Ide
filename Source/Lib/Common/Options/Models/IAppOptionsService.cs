using Fluxor;
using Luthetus.Common.RazorLib.Options.States;
using Luthetus.Common.RazorLib.Themes.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Storages.Models;

namespace Luthetus.Common.RazorLib.Options.Models;

public interface IAppOptionsService
{
    /// <summary>
    /// This is used when interacting with the <see cref="IStorageService"/> to set and get data.
    /// </summary>
    public string StorageKey { get; }
    public IState<AppOptionsState> AppOptionsStateWrap { get; }
    public string ThemeCssClassString { get; }
    public string? FontFamilyCssStyleString { get; }
    public string FontSizeCssStyleString { get; }

    public Task SetActiveThemeRecordKey(Key<ThemeRecord> themeKey, bool updateStorage = true);
    public Task SetTheme(ThemeRecord theme, bool updateStorage = true);
    public Task SetFontFamily(string? fontFamily, bool updateStorage = true);
    public Task SetFontSize(int fontSizeInPixels, bool updateStorage = true);
    public Task SetIconSize(int iconSizeInPixels, bool updateStorage = true);
    public Task SetFromLocalStorageAsync();
    public Task WriteToStorage();
}