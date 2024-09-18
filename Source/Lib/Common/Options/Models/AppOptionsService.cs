using System.Text.Json;
using System.Text;
using Fluxor;
using Luthetus.Common.RazorLib.Options.States;
using Luthetus.Common.RazorLib.Themes.States;
using Luthetus.Common.RazorLib.Themes.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Common.RazorLib.Options.Models;

public class AppOptionsService : IAppOptionsService
{
    private readonly IDispatcher _dispatcher;
    private readonly IStorageService _storageService;
    private readonly CommonBackgroundTaskApi _commonBackgroundTaskApi;
    private readonly IBackgroundTaskService _backgroundTaskService;

    public AppOptionsService(
        IState<AppOptionsState> appOptionsStateWrap,
        IState<ThemeState> themeStateWrap,
        IDispatcher dispatcher,
        IStorageService storageService,
        CommonBackgroundTaskApi commonBackgroundTaskApi,
        IBackgroundTaskService backgroundTaskService)
    {
        AppOptionsStateWrap = appOptionsStateWrap;
        ThemeStateWrap = themeStateWrap;
        _dispatcher = dispatcher;
        _storageService = storageService;
        _commonBackgroundTaskApi = commonBackgroundTaskApi;
        _backgroundTaskService = backgroundTaskService;
    }

    public IState<AppOptionsState> AppOptionsStateWrap { get; }
    public IState<ThemeState> ThemeStateWrap { get; }

#if DEBUG
    public string StorageKey => "luthetus-common_theme-storage-key-debug"; 
#else
    public string StorageKey => "luthetus-common_theme-storage-key";
#endif

    public string ThemeCssClassString => ThemeStateWrap.Value.ThemeList.FirstOrDefault(
        x => x.Key == AppOptionsStateWrap.Value.Options.ThemeKey)
        ?.CssClassString
            ?? ThemeFacts.VisualStudioDarkThemeClone.CssClassString;

    public string? FontFamilyCssStyleString
    {
        get
        {
            if (AppOptionsStateWrap.Value.Options.FontFamily is null)
                return null;

            return $"font-family: {AppOptionsStateWrap.Value.Options.FontFamily};";
        }
    }

    public string FontSizeCssStyleString
    {
        get
        {
            var fontSizeInPixels = AppOptionsStateWrap.Value.Options.FontSizeInPixels;
            var fontSizeInPixelsCssValue = fontSizeInPixels.ToCssValue();

            return $"font-size: {fontSizeInPixelsCssValue}px;";
        }
    }
    
    public bool ShowPanelTitles => AppOptionsStateWrap.Value.Options.ShowPanelTitles;
    
    public string ShowPanelTitlesCssClass => AppOptionsStateWrap.Value.Options.ShowPanelTitles
    	? string.Empty
    	: "luth_ide_section-no-title";

    public string ColorSchemeCssStyleString
    {
        get
        {
	        var activeTheme = ThemeStateWrap.Value.ThemeList.FirstOrDefault(
		        x => x.Key == AppOptionsStateWrap.Value.Options.ThemeKey)
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

    public void SetActiveThemeRecordKey(Key<ThemeRecord> themeKey, bool updateStorage = true)
    {
        _dispatcher.Dispatch(new AppOptionsState.WithAction(inState => inState with
        {
            Options = inState.Options with
            {
                ThemeKey = themeKey
            }
        }));

        if (updateStorage)
            WriteToStorage();
    }

    public void SetTheme(ThemeRecord theme, bool updateStorage = true)
    {
        _dispatcher.Dispatch(new AppOptionsState.WithAction(inState => inState with
        {
            Options = inState.Options with
            {
                ThemeKey = theme.Key
            }
        }));

        if (updateStorage)
            WriteToStorage();
    }

    public void SetFontFamily(string? fontFamily, bool updateStorage = true)
    {
        _dispatcher.Dispatch(new AppOptionsState.WithAction(inState => inState with
        {
            Options = inState.Options with
            {
                FontFamily = fontFamily
            }
        }));

        if (updateStorage)
            WriteToStorage();
    }

    public void SetFontSize(int fontSizeInPixels, bool updateStorage = true)
    {
        _dispatcher.Dispatch(new AppOptionsState.WithAction(inState => inState with
        {
            Options = inState.Options with
            {
                FontSizeInPixels = fontSizeInPixels
            }
        }));

        if (updateStorage)
            WriteToStorage();
    }

    public void SetResizeHandleWidth(int resizeHandleWidthInPixels, bool updateStorage = true)
    {
        _dispatcher.Dispatch(new AppOptionsState.WithAction(inState => inState with
        {
            Options = inState.Options with
            {
                ResizeHandleWidthInPixels = resizeHandleWidthInPixels
            }
        }));

        if (updateStorage)
            WriteToStorage();
    }

    public void SetResizeHandleHeight(int resizeHandleHeightInPixels, bool updateStorage = true)
    {
        _dispatcher.Dispatch(new AppOptionsState.WithAction(inState => inState with
        {
            Options = inState.Options with
            {
                ResizeHandleHeightInPixels = resizeHandleHeightInPixels
            }
        }));

        if (updateStorage)
            WriteToStorage();
    }

    public void SetIconSize(int iconSizeInPixels, bool updateStorage = true)
    {
        _dispatcher.Dispatch(new AppOptionsState.WithAction(inState => inState with
        {
            Options = inState.Options with
            {
                IconSizeInPixels = iconSizeInPixels
            }
        }));

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
            var matchedTheme = ThemeStateWrap.Value.ThemeList.FirstOrDefault(
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
        _commonBackgroundTaskApi.Storage.WriteToLocalStorage(
            StorageKey,
            new CommonOptionsJsonDto(AppOptionsStateWrap.Value.Options));
    }
}