using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Options.States;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.Common.RazorLib.Themes.Models;
using Luthetus.Common.RazorLib.Storages.States;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.Options.Models;
using Luthetus.TextEditor.RazorLib.Options.States;
using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Misc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Luthetus.TextEditor.Tests.Basis.Options.Models;

/// <summary>
/// <see cref="TextEditorOptionsApi"/>
/// </summary>
public class OptionsApiTests
{
    /// <summary>
    /// <see cref="TextEditorOptionsApi(ITextEditorService, LuthetusTextEditorConfig, IStorageService, StorageSync, IDispatcher)"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        InitializeOptionsApiTests(
            out var textEditorService,
            out var dispatcher,
            out var serviceProvider);

        Assert.NotNull(textEditorService.OptionsApi);
    }

    /// <summary>
    /// <see cref="TextEditorOptionsApi.SetTheme(ThemeRecord)"/>
    /// </summary>
    [Fact]
    public void SetTheme()
    {
        InitializeOptionsApiTests(
            out var textEditorService,
            out var dispatcher,
            out var serviceProvider);

        Assert.Equal(
            ThemeFacts.VisualStudioDarkThemeClone.Key,
            textEditorService.OptionsApi.GetOptions().CommonOptions.ThemeKey);

        textEditorService.OptionsApi.SetTheme(ThemeFacts.VisualStudioLightThemeClone);

        Assert.Equal(
            ThemeFacts.VisualStudioLightThemeClone.Key,
            textEditorService.OptionsApi.GetOptions().CommonOptions.ThemeKey);
    }

    /// <summary>
    /// <see cref="TextEditorOptionsApi.SetShowWhitespace(bool)"/>
    /// </summary>
    [Fact]
    public void SetShowWhitespace()
    {
        InitializeOptionsApiTests(
            out var textEditorService,
            out var dispatcher,
            out var serviceProvider);

        Assert.False(textEditorService.OptionsApi.GetOptions().ShowWhitespace);
        textEditorService.OptionsApi.SetShowWhitespace(true);
        Assert.True(textEditorService.OptionsApi.GetOptions().ShowWhitespace);
    }

    /// <summary>
    /// <see cref="TextEditorOptionsApi.SetUseMonospaceOptimizations(bool)"/>
    /// </summary>
    [Fact]
    public void SetUseMonospaceOptimizations()
    {
        InitializeOptionsApiTests(
            out var textEditorService,
            out var dispatcher,
            out var serviceProvider);

        Assert.True(textEditorService.OptionsApi.GetOptions().UseMonospaceOptimizations);
        textEditorService.OptionsApi.SetUseMonospaceOptimizations(false);
        Assert.False(textEditorService.OptionsApi.GetOptions().UseMonospaceOptimizations);
    }

    /// <summary>
    /// <see cref="TextEditorOptionsApi.SetShowNewlines(bool)"/>
    /// </summary>
    [Fact]
    public void SetShowNewlines()
    {
        InitializeOptionsApiTests(
            out var textEditorService,
            out var dispatcher,
            out var serviceProvider);

        Assert.False(textEditorService.OptionsApi.GetOptions().ShowNewlines);
        textEditorService.OptionsApi.SetShowNewlines(true);
        Assert.True(textEditorService.OptionsApi.GetOptions().ShowNewlines);
    }

    /// <summary>
    /// <see cref="TextEditorOptionsApi.SetKeymap(Keymap)"/>
    /// </summary>
    [Fact]
    public void SetKeymap()
    {
        InitializeOptionsApiTests(
            out var textEditorService,
            out var dispatcher,
            out var serviceProvider);

        Assert.Equal(
            TextEditorKeymapFacts.DefaultKeymap.Key,
            textEditorService.OptionsApi.GetOptions().Keymap.Key);

        textEditorService.OptionsApi.SetKeymap(TextEditorKeymapFacts.VimKeymap);

        Assert.Equal(
            TextEditorKeymapFacts.VimKeymap.Key,
            textEditorService.OptionsApi.GetOptions().Keymap.Key);
    }

    /// <summary>
    /// <see cref="TextEditorOptionsApi.SetHeight(int?)"/>
    /// </summary>
    [Fact]
    public void SetHeight()
    {
        InitializeOptionsApiTests(
            out var textEditorService,
            out var dispatcher,
            out var serviceProvider);

        var textEditorHeightInPixels = 500;

        Assert.Null(textEditorService.OptionsApi.GetOptions().TextEditorHeightInPixels);

        textEditorService.OptionsApi.SetHeight(textEditorHeightInPixels);

        Assert.Equal(
            textEditorHeightInPixels,
            textEditorService.OptionsApi.GetOptions().TextEditorHeightInPixels!.Value);
    }

    /// <summary>
    /// <see cref="TextEditorOptionsApi.SetFontSize(int)"/>
    /// </summary>
    [Fact]
    public void SetFontSize()
    {
        InitializeOptionsApiTests(
            out var textEditorService,
            out var dispatcher,
            out var serviceProvider);

        Assert.Equal(
            AppOptionsState.DEFAULT_FONT_SIZE_IN_PIXELS,
            textEditorService.OptionsApi.GetOptions().CommonOptions.FontSizeInPixels);

        var fontSizeInPixels = 1 + AppOptionsState.DEFAULT_FONT_SIZE_IN_PIXELS;

        textEditorService.OptionsApi.SetFontSize(fontSizeInPixels);

        Assert.Equal(
            fontSizeInPixels,
            textEditorService.OptionsApi.GetOptions().CommonOptions.FontSizeInPixels);
    }

    /// <summary>
    /// <see cref="TextEditorOptionsApi.SetFontFamily(string?)"/>
    /// </summary>
    [Fact]
    public void SetFontFamily()
    {
        InitializeOptionsApiTests(
            out var textEditorService,
            out var dispatcher,
            out var serviceProvider);

        Assert.Null(textEditorService.OptionsApi.GetOptions().CommonOptions.FontFamily);

        var fontFamily = "chiller";

        textEditorService.OptionsApi.SetFontFamily(fontFamily);

        Assert.Equal(
            fontFamily,
            textEditorService.OptionsApi.GetOptions().CommonOptions.FontFamily);
    }

    /// <summary>
    /// <see cref="TextEditorOptionsApi.SetCursorWidth(double)"/>
    /// </summary>
    [Fact]
    public void SetCursorWidth()
    {
        InitializeOptionsApiTests(
            out var textEditorService,
            out var dispatcher,
            out var serviceProvider);

        Assert.Equal(
            TextEditorOptionsState.DEFAULT_CURSOR_WIDTH_IN_PIXELS,
            textEditorService.OptionsApi.GetOptions().CursorWidthInPixels);

        var cursorWidthInPixels = 1 + TextEditorOptionsState.DEFAULT_CURSOR_WIDTH_IN_PIXELS;

        textEditorService.OptionsApi.SetCursorWidth(cursorWidthInPixels);

        Assert.Equal(
            cursorWidthInPixels,
            textEditorService.OptionsApi.GetOptions().CursorWidthInPixels);
    }

    /// <summary>
    /// <see cref="TextEditorOptionsApi.SetRenderStateKey(Key{RenderState})"/>
    /// </summary>
    [Fact]
    public void SetRenderStateKey()
    {
        InitializeOptionsApiTests(
            out var textEditorService,
            out var dispatcher,
            out var serviceProvider);

        var renderStateKey = Key<RenderState>.NewKey();

        Assert.NotEqual(
            renderStateKey,
            textEditorService.OptionsApi.GetOptions().RenderStateKey);

        textEditorService.OptionsApi.SetRenderStateKey(renderStateKey);

        Assert.Equal(
            renderStateKey,
            textEditorService.OptionsApi.GetOptions().RenderStateKey);
    }

    /// <summary>
    /// <see cref="TextEditorOptionsApi.SetFromLocalStorageAsync()"/>
    /// </summary>
    [Fact]
    public void SetFromLocalStorageAsync()
    {
        InitializeOptionsApiTests(
            out var textEditorService,
            out var dispatcher,
            out var serviceProvider);

        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorOptionsApi.WriteToStorage()"/>
    /// </summary>
    [Fact]
    public void WriteToStorage()
    {
        InitializeOptionsApiTests(
            out var textEditorService,
            out var dispatcher,
            out var serviceProvider);

        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorOptionsApi.ShowSettingsDialog(bool?, string?)"/>
    /// </summary>
    [Fact]
    public void ShowSettingsDialog()
    {
        InitializeOptionsApiTests(
            out var textEditorService,
            out var dispatcher,
            out var serviceProvider);

        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorOptionsApi.ShowFindAllDialog(bool?, string?)"/>
    /// </summary>
    [Fact]
    public void ShowFindDialog()
    {
        InitializeOptionsApiTests(
            out var textEditorService,
            out var dispatcher,
            out var serviceProvider);

        throw new NotImplementedException();
    }
    
    private void InitializeOptionsApiTests(
        out ITextEditorService textEditorService,
        out IDispatcher dispatcher,
        out ServiceProvider serviceProvider)
    {
        var backgroundTaskService = new BackgroundTaskServiceSynchronous();

        var serviceCollection = new ServiceCollection()
            .AddScoped<IJSRuntime, DoNothingJsRuntime>()
            .AddLuthetusTextEditor(new LuthetusHostingInformation(LuthetusHostingKind.UnitTesting, backgroundTaskService))
            .AddFluxor(options => options.ScanAssemblies(
                typeof(LuthetusCommonConfig).Assembly,
                typeof(LuthetusTextEditorConfig).Assembly));

        serviceProvider = serviceCollection.BuildServiceProvider();

        var store = serviceProvider.GetRequiredService<IStore>();
        store.InitializeAsync().Wait();

        dispatcher = serviceProvider.GetRequiredService<IDispatcher>();
        textEditorService = serviceProvider.GetRequiredService<ITextEditorService>();
    }
}