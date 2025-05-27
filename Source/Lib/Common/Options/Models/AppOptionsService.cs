using System.Text.Json;
using System.Text;
using Luthetus.Common.RazorLib.Themes.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Common.RazorLib.Options.Models;

public class AppOptionsService : IAppOptionsService
{
    private readonly IStorageService _storageService;
    private readonly BackgroundTaskService _backgroundTaskService;

    public AppOptionsService(
		IThemeService themeService,
        IStorageService storageService,
        BackgroundTaskService backgroundTaskService)
    {
        ThemeService = themeService;
        _storageService = storageService;
        _backgroundTaskService = backgroundTaskService;
    }
    
    private AppOptionsState _appOptionsState = new();

    public IThemeService ThemeService { get; }

    public CommonBackgroundTaskApi CommonBackgroundTaskApi { get; set; }

#if DEBUG
    public string StorageKey => "luthetus-common_theme-storage-key-debug"; 
#else
    public string StorageKey => "luthetus-common_theme-storage-key";
#endif

	public string ThemeCssClassString { get; set; } = ThemeFacts.VisualStudioDarkThemeClone.CssClassString;

    public string? FontFamilyCssStyleString { get; set; }

    public string FontSizeCssStyleString { get; set; }
    
    public bool ShowPanelTitles => GetAppOptionsState().Options.ShowPanelTitles;
    
    public string ShowPanelTitlesCssClass => GetAppOptionsState().Options.ShowPanelTitles
    	? string.Empty
    	: "luth_ide_section-no-title";

    public string ColorSchemeCssStyleString { get; set; }

	public event Action? AppOptionsStateChanged;
	
	public AppOptionsState GetAppOptionsState() => _appOptionsState;

    public void SetActiveThemeRecordKey(Key<ThemeRecord> themeKey, bool updateStorage = true)
    {
    	var inState = GetAppOptionsState();
    	
        _appOptionsState = inState with
        {
            Options = inState.Options with
            {
                ThemeKey = themeKey
            }
        };
        
        HandleThemeChange();
        
        AppOptionsStateChanged?.Invoke();

        if (updateStorage)
            WriteToStorage();
    }

    public void SetTheme(ThemeRecord theme, bool updateStorage = true)
    {
        var inState = GetAppOptionsState();
    	
        _appOptionsState = inState with
        {
            Options = inState.Options with
            {
                ThemeKey = theme.Key
            }
        };
        
        HandleThemeChange();
        
        AppOptionsStateChanged?.Invoke();

        if (updateStorage)
            WriteToStorage();
    }

    public void SetFontFamily(string? fontFamily, bool updateStorage = true)
    {
        var inState = GetAppOptionsState();
    	
        _appOptionsState = inState with
        {
            Options = inState.Options with
            {
                FontFamily = fontFamily
            }
        };
        
        // I'm optimizing all the expression bound properties that construct
        // a string, and specifically the ones that are rendered in the UI many times.
        //
        // Can probably use 'fontFamily' variable here but
        // I don't want to touch that right now -- incase there are unexpected consequences.
        var usingFontFamily = GetAppOptionsState().Options.FontFamily;
        if (usingFontFamily is null)
        	FontFamilyCssStyleString = null;
        else
        	FontFamilyCssStyleString = $"font-family: {usingFontFamily};";

        AppOptionsStateChanged?.Invoke();

        if (updateStorage)
            WriteToStorage();
    }

    public void SetFontSize(int fontSizeInPixels, bool updateStorage = true)
    {
        var inState = GetAppOptionsState();
    	
        _appOptionsState = inState with
        {
            Options = inState.Options with
            {
                FontSizeInPixels = fontSizeInPixels
            }
        };
        
        // I'm optimizing all the expression bound properties that construct
        // a string, and specifically the ones that are rendered in the UI many times.
        //
        // Can probably use 'fontSizeInPixels' variable here but
        // I don't want to touch that right now -- incase there are unexpected consequences.
    	var usingFontSizeInPixels = GetAppOptionsState().Options.FontSizeInPixels;
        var usingFontSizeInPixelsCssValue = usingFontSizeInPixels.ToCssValue();
    	FontSizeCssStyleString = $"font-size: {usingFontSizeInPixelsCssValue}px;";
        
        AppOptionsStateChanged?.Invoke();

        if (updateStorage)
            WriteToStorage();
    }

    public void SetResizeHandleWidth(int resizeHandleWidthInPixels, bool updateStorage = true)
    {
        var inState = GetAppOptionsState();
    	
        _appOptionsState = inState with
        {
            Options = inState.Options with
            {
                ResizeHandleWidthInPixels = resizeHandleWidthInPixels
            }
        };
        
        AppOptionsStateChanged?.Invoke();

        if (updateStorage)
            WriteToStorage();
    }

    public void SetResizeHandleHeight(int resizeHandleHeightInPixels, bool updateStorage = true)
    {
        var inState = GetAppOptionsState();
    	
        _appOptionsState = inState with
        {
            Options = inState.Options with
            {
                ResizeHandleHeightInPixels = resizeHandleHeightInPixels
            }
        };
        
        AppOptionsStateChanged?.Invoke();

        if (updateStorage)
            WriteToStorage();
    }

    public void SetIconSize(int iconSizeInPixels, bool updateStorage = true)
    {
        var inState = GetAppOptionsState();
    	
        _appOptionsState = inState with
        {
            Options = inState.Options with
            {
                IconSizeInPixels = iconSizeInPixels
            }
        };
        
        AppOptionsStateChanged?.Invoke();

        if (updateStorage)
            WriteToStorage();
    }

    public async Task SetFromLocalStorageAsync()
    {
        var optionsJsonString = await _storageService.GetValue(StorageKey).ConfigureAwait(false) as string;

        if (string.IsNullOrWhiteSpace(optionsJsonString))
            return;

        var optionsJson = JsonSerializer.Deserialize<CommonOptionsJsonDto>(optionsJsonString);

        if (optionsJson is null)
            return;

        if (optionsJson.ThemeKey is not null)
        {
            var matchedTheme = ThemeService.GetThemeState().ThemeList.FirstOrDefault(
                x => x.Key == optionsJson.ThemeKey);

            SetTheme(matchedTheme ?? ThemeFacts.VisualStudioDarkThemeClone, false);
        }

        if (optionsJson.FontFamily is not null)
            SetFontFamily(optionsJson.FontFamily, false);

        if (optionsJson.FontSizeInPixels is not null)
            SetFontSize(optionsJson.FontSizeInPixels.Value, false);
            
        if (optionsJson.ResizeHandleWidthInPixels is not null)
            SetResizeHandleWidth(optionsJson.ResizeHandleWidthInPixels.Value, false);
            
        if (optionsJson.ResizeHandleHeightInPixels is not null)
            SetResizeHandleHeight(optionsJson.ResizeHandleHeightInPixels.Value, false);

        if (optionsJson.IconSizeInPixels is not null)
            SetIconSize(optionsJson.IconSizeInPixels.Value, false);
    }

    public void WriteToStorage()
    {
        CommonBackgroundTaskApi.Enqueue(new CommonWorkArgs
    	{
    		WorkKind = CommonWorkKind.WriteToLocalStorage,
    		WriteToLocalStorage_Key = StorageKey,
    		WriteToLocalStorage_Value = new CommonOptionsJsonDto(GetAppOptionsState().Options)
    	});
    }
    
    private void HandleThemeChange()
    {
        var usingTheme = ThemeService.GetThemeState().ThemeList
        	.FirstOrDefault(x => x.Key == GetAppOptionsState().Options.ThemeKey)
        	?? ThemeFacts.VisualStudioDarkThemeClone;
        
        ThemeCssClassString = usingTheme.CssClassString;
	    
	    var cssStyleStringBuilder = new StringBuilder("color-scheme: ");
	    if (usingTheme.ThemeColorKind == ThemeColorKind.Dark)
	    	cssStyleStringBuilder.Append("dark");
		else if (usingTheme.ThemeColorKind == ThemeColorKind.Light)
	    	cssStyleStringBuilder.Append("light");
		else
	    	cssStyleStringBuilder.Append("dark");
	    cssStyleStringBuilder.Append(';');
        ColorSchemeCssStyleString = cssStyleStringBuilder.ToString();
    }
}