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
    private readonly LuthetusCommonApi _commonApi;

    public AppOptionsService(LuthetusCommonApi commonApi)
    {
        _commonApi = commonApi;
    }
    
    private AppOptionsState _appOptionsState = new();

#if DEBUG
    public string StorageKey => "luthetus-common_theme-storage-key-debug"; 
#else
    public string StorageKey => "luthetus-common_theme-storage-key";
#endif

    public string ThemeCssClassString => _commonApi.ThemeApi.GetThemeState().ThemeList.FirstOrDefault(
        x => x.Key == GetAppOptionsState().Options.ThemeKey)
        ?.CssClassString
            ?? ThemeFacts.VisualStudioDarkThemeClone.CssClassString;

    public string? FontFamilyCssStyleString
    {
        get
        {
            if (GetAppOptionsState().Options.FontFamily is null)
                return null;

            return $"font-family: {GetAppOptionsState().Options.FontFamily};";
        }
    }

    public string FontSizeCssStyleString
    {
        get
        {
            var fontSizeInPixels = GetAppOptionsState().Options.FontSizeInPixels;
            var fontSizeInPixelsCssValue = fontSizeInPixels.ToCssValue();

            return $"font-size: {fontSizeInPixelsCssValue}px;";
        }
    }
    
    public bool ShowPanelTitles => GetAppOptionsState().Options.ShowPanelTitles;
    
    public string ShowPanelTitlesCssClass => GetAppOptionsState().Options.ShowPanelTitles
    	? string.Empty
    	: "luth_ide_section-no-title";

    public string ColorSchemeCssStyleString
    {
        get
        {
	        var activeTheme = _commonApi.ThemeApi.GetThemeState().ThemeList.FirstOrDefault(
		        x => x.Key == GetAppOptionsState().Options.ThemeKey)
		        	?? ThemeFacts.VisualStudioDarkThemeClone;
		        
		    var cssStyleStringBuilder = new StringBuilder("color-scheme: ");
		    
		    if (activeTheme.ThemeColorKind == ThemeColorKind.Dark)
		    	cssStyleStringBuilder.Append("dark");
			else if (activeTheme.ThemeColorKind == ThemeColorKind.Light)
		    	cssStyleStringBuilder.Append("light");
			else
		    	cssStyleStringBuilder.Append("dark");
		    
		    cssStyleStringBuilder.Append(';');

            return cssStyleStringBuilder.ToString();
        }
    }

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
        var optionsJsonString = await _commonApi.StorageApi.GetValue(StorageKey).ConfigureAwait(false) as string;

        if (string.IsNullOrWhiteSpace(optionsJsonString))
            return;

        var optionsJson = JsonSerializer.Deserialize<CommonOptionsJsonDto>(optionsJsonString);

        if (optionsJson is null)
            return;

        if (optionsJson.ThemeKey is not null)
        {
            var matchedTheme = _commonApi.ThemeApi.GetThemeState().ThemeList.FirstOrDefault(
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
        IStorageService.WriteToLocalStorage(
            _commonApi.BackgroundTaskApi,
            _commonApi.StorageApi,
            StorageKey,
			new CommonOptionsJsonDto(GetAppOptionsState().Options));
    }
}