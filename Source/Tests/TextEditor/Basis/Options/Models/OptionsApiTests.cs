using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Options.States;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.Common.RazorLib.Themes.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.Options.States;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;

namespace Luthetus.TextEditor.Tests.Basis.Options.Models;

/// <summary>
/// <see cref="ITextEditorService.TextEditorOptionsApi"/>
/// </summary>
public class OptionsApiTests
{
    /// <summary>
    /// <see cref="ITextEditorService.TextEditorOptionsApi.TextEditorOptionsApi(ITextEditorService, RazorLib.Installations.Models.LuthetusTextEditorConfig, Common.RazorLib.Storages.Models.IStorageService, Common.RazorLib.Storages.States.StorageSync, Fluxor.IDispatcher)"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        TestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        Assert.NotNull(textEditorService.OptionsApi);
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorOptionsApi.SetTheme(ThemeRecord)"/>
    /// </summary>
    [Fact]
    public void SetTheme()
    {
        TestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
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
    /// <see cref="ITextEditorService.TextEditorOptionsApi.SetShowWhitespace(bool)"/>
    /// </summary>
    [Fact]
    public void SetShowWhitespace()
    {
        TestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        Assert.False(textEditorService.OptionsApi.GetOptions().ShowWhitespace);
        textEditorService.OptionsApi.SetShowWhitespace(true);
        Assert.True(textEditorService.OptionsApi.GetOptions().ShowWhitespace);
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorOptionsApi.SetUseMonospaceOptimizations(bool)"/>
    /// </summary>
    [Fact]
    public void SetUseMonospaceOptimizations()
    {
        TestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        Assert.True(textEditorService.OptionsApi.GetOptions().UseMonospaceOptimizations);
        textEditorService.OptionsApi.SetUseMonospaceOptimizations(false);
        Assert.False(textEditorService.OptionsApi.GetOptions().UseMonospaceOptimizations);
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorOptionsApi.SetShowNewlines(bool)"/>
    /// </summary>
    [Fact]
    public void SetShowNewlines()
    {
        TestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        Assert.False(textEditorService.OptionsApi.GetOptions().ShowNewlines);
        textEditorService.OptionsApi.SetShowNewlines(true);
        Assert.True(textEditorService.OptionsApi.GetOptions().ShowNewlines);
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorOptionsApi.SetKeymap(Common.RazorLib.Keymaps.Models.Keymap)"/>
    /// </summary>
    [Fact]
    public void SetKeymap()
    {
        TestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
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
    /// <see cref="ITextEditorService.TextEditorOptionsApi.SetHeight(int?)"/>
    /// </summary>
    [Fact]
    public void SetHeight()
    {
        TestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        var textEditorHeightInPixels = 500;

        Assert.Null(textEditorService.OptionsApi.GetOptions().TextEditorHeightInPixels);

        textEditorService.OptionsApi.SetHeight(textEditorHeightInPixels);

        Assert.Equal(
            textEditorHeightInPixels,
            textEditorService.OptionsApi.GetOptions().TextEditorHeightInPixels!.Value);
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorOptionsApi.SetFontSize(int)"/>
    /// </summary>
    [Fact]
    public void SetFontSize()
    {
        TestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
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
    /// <see cref="ITextEditorService.TextEditorOptionsApi.SetFontFamily(string?)"/>
    /// </summary>
    [Fact]
    public void SetFontFamily()
    {
        TestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        Assert.Null(textEditorService.OptionsApi.GetOptions().CommonOptions.FontFamily);

        var fontFamily = "chiller";

        textEditorService.OptionsApi.SetFontFamily(fontFamily);

        Assert.Equal(
            fontFamily,
            textEditorService.OptionsApi.GetOptions().CommonOptions.FontFamily);
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorOptionsApi.SetCursorWidth(double)"/>
    /// </summary>
    [Fact]
    public void SetCursorWidth()
    {
        TestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
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
    /// <see cref="ITextEditorService.TextEditorOptionsApi.SetRenderStateKey(Key{RenderState})"/>
    /// </summary>
    [Fact]
    public void SetRenderStateKey()
    {
        TestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
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
    /// <see cref="ITextEditorService.TextEditorOptionsApi.SetFromLocalStorageAsync()"/>
    /// </summary>
    [Fact]
    public void SetFromLocalStorageAsync()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorOptionsApi.WriteToStorage()"/>
    /// </summary>
    [Fact]
    public void WriteToStorage()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorOptionsApi.ShowSettingsDialog(bool?, string?)"/>
    /// </summary>
    [Fact]
    public void ShowSettingsDialog()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorOptionsApi.ShowFindAllDialog(bool?, string?)"/>
    /// </summary>
    [Fact]
    public void ShowFindDialog()
    {
        throw new NotImplementedException();
    }
}