using Luthetus.Common.RazorLib.Themes.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Common.RazorLib.Options.Models;

public interface IAppOptionsService
{
    /// <summary>
    /// This is used when interacting with the <see cref="IStorageService"/> to set and get data.
    /// </summary>
    public string StorageKey { get; }
    public string ThemeCssClassString { get; }
    public string? FontFamilyCssStyleString { get; }
    public string FontSizeCssStyleString { get; }
    public string ColorSchemeCssStyleString { get; }
    public bool ShowPanelTitles { get; }
    public string ShowPanelTitlesCssClass { get; }
    
    /// <summary>
    /// Very hacky property to avoid circular services while I work out the details of things.
    /// </summary>
    public CommonBackgroundTaskApi CommonBackgroundTaskApi { get; set; }
    
    public event Action? AppOptionsStateChanged;
	
	public AppOptionsState GetAppOptionsState();

    public void SetActiveThemeRecordKey(Key<ThemeRecord> themeKey, bool updateStorage = true);
    public void SetTheme(ThemeRecord theme, bool updateStorage = true);
    public void SetFontFamily(string? fontFamily, bool updateStorage = true);
    public void SetFontSize(int fontSizeInPixels, bool updateStorage = true);
    public void SetResizeHandleWidth(int resizeHandleWidthInPixels, bool updateStorage = true);
    public void SetResizeHandleHeight(int resizeHandleHeightInPixels, bool updateStorage = true);
    public void SetIconSize(int iconSizeInPixels, bool updateStorage = true);
    public Task SetFromLocalStorageAsync();
    public void WriteToStorage();
}