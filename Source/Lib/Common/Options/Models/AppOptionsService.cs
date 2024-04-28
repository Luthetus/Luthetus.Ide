using Fluxor;
using System.Text.Json;
using Luthetus.Common.RazorLib.Storages.States;
using Luthetus.Common.RazorLib.Options.States;
using Luthetus.Common.RazorLib.Themes.States;
using Luthetus.Common.RazorLib.Themes.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Storages.Models;

namespace Luthetus.Common.RazorLib.Options.Models;

public class AppOptionsService : IAppOptionsService
{
    private readonly IDispatcher _dispatcher;
    private readonly IStorageService _storageService;
    private readonly StorageSync _storageSync;

    public AppOptionsService(
        IState<AppOptionsState> appOptionsStateWrap,
        IState<ThemeState> themeStateWrap,
        IDispatcher dispatcher,
        IStorageService storageService,
        StorageSync storageSync)
    {
        AppOptionsStateWrap = appOptionsStateWrap;
        ThemeStateWrap = themeStateWrap;
        _dispatcher = dispatcher;
        _storageService = storageService;
        _storageSync = storageSync;
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

        if (optionsJson.IconSizeInPixels is not null)
            SetIconSize(optionsJson.IconSizeInPixels.Value, false);
    }

    public void WriteToStorage()
    {
        _storageSync.WriteToLocalStorage(
            StorageKey,
            new CommonOptionsJsonDto(AppOptionsStateWrap.Value.Options));
    }
}