using Fluxor;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.Common.RazorLib.Themes.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.Options.States;
using static Luthetus.Common.RazorLib.Contexts.States.ContextState;
using System.Text.Json;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;

namespace Luthetus.TextEditor.RazorLib.Options.Models;

public class TextEditorOptionsApi : ITextEditorOptionsApi
{
    private readonly ITextEditorService _textEditorService;
    private readonly LuthetusTextEditorConfig _textEditorConfig;
    private readonly IStorageService _storageService;
    private readonly LuthetusCommonBackgroundTaskApi _commonBackgroundTaskApi;
    private readonly IDispatcher _dispatcher;

    public TextEditorOptionsApi(
        ITextEditorService textEditorService,
        LuthetusTextEditorConfig textEditorConfig,
        IStorageService storageService,
        LuthetusCommonBackgroundTaskApi commonBackgroundTaskApi,
        IDispatcher dispatcher)
    {
        _textEditorService = textEditorService;
        _textEditorConfig = textEditorConfig;
        _storageService = storageService;
        _commonBackgroundTaskApi = commonBackgroundTaskApi;
        _dispatcher = dispatcher;
    }

    private IDialog? _findAllDialog;

    public TextEditorOptions GetOptions()
    {
        return _textEditorService.OptionsStateWrap.Value.Options;
    }

    public void ShowSettingsDialog(bool? isResizableOverride = null, string? cssClassString = null)
    {
        var settingsDialog = new DialogViewModel(
            Key<IDynamicViewModel>.NewKey(),
            "Text Editor Settings",
            _textEditorConfig.SettingsDialogConfig.ComponentRendererType,
            null,
            cssClassString,
            isResizableOverride ?? _textEditorConfig.SettingsDialogConfig.ComponentIsResizable);

        _dispatcher.Dispatch(new DialogState.RegisterAction(settingsDialog));
    }

    public void ShowFindAllDialog(bool? isResizableOverride = null, string? cssClassString = null)
    {
        _findAllDialog ??= new DialogViewModel(
            Key<IDynamicViewModel>.NewKey(),
            "Find All",
            _textEditorConfig.FindAllDialogConfig.ComponentRendererType,
            null,
            cssClassString,
            isResizableOverride ?? _textEditorConfig.FindAllDialogConfig.ComponentIsResizable);

        _dispatcher.Dispatch(new DialogState.RegisterAction(_findAllDialog));
    }

    public Task SetTheme(ThemeRecord theme, bool updateStorage = true)
    {
        _dispatcher.Dispatch(new TextEditorOptionsState.SetThemeAction(theme));

        if (updateStorage)
            return WriteToStorage();

        return Task.CompletedTask;
    }

    public Task SetShowWhitespace(bool showWhitespace, bool updateStorage = true)
    {
        _dispatcher.Dispatch(new TextEditorOptionsState.SetShowWhitespaceAction(showWhitespace));

        if (updateStorage)
            return WriteToStorage();

        return Task.CompletedTask;
    }

    public Task SetUseMonospaceOptimizations(bool useMonospaceOptimizations, bool updateStorage = true)
    {
        _dispatcher.Dispatch(new TextEditorOptionsState.SetUseMonospaceOptimizationsAction(useMonospaceOptimizations));

        if (updateStorage)
            return WriteToStorage();

        return Task.CompletedTask;
    }

    public Task SetShowNewlines(bool showNewlines, bool updateStorage = true)
    {
        _dispatcher.Dispatch(new TextEditorOptionsState.SetShowNewlinesAction(showNewlines));

        if (updateStorage)
            return WriteToStorage();

        return Task.CompletedTask;
    }

    public Task SetKeymap(Keymap keymap, bool updateStorage = true)
    {
        _dispatcher.Dispatch(new TextEditorOptionsState.SetKeymapAction(keymap));

        var activeKeymap = _textEditorService.OptionsStateWrap.Value.Options.Keymap;

        if (activeKeymap is not null)
        {
            _dispatcher.Dispatch(new SetContextKeymapAction(
                ContextFacts.TextEditorContext.ContextKey,
                activeKeymap));
        }

        if (updateStorage)
            return WriteToStorage();

        return Task.CompletedTask;
    }

    public Task SetHeight(int? heightInPixels, bool updateStorage = true)
    {
        _dispatcher.Dispatch(new TextEditorOptionsState.SetHeightAction(heightInPixels));

        if (updateStorage)
            return WriteToStorage();

        return Task.CompletedTask;
    }

    public Task SetFontSize(int fontSizeInPixels, bool updateStorage = true)
    {
        _dispatcher.Dispatch(new TextEditorOptionsState.SetFontSizeAction(fontSizeInPixels));

        if (updateStorage)
            return WriteToStorage();

        return Task.CompletedTask;
    }

    public Task SetFontFamily(string? fontFamily, bool updateStorage = true)
    {
        _dispatcher.Dispatch(new TextEditorOptionsState.SetFontFamilyAction(fontFamily));

        if (updateStorage)
            return WriteToStorage();

        return Task.CompletedTask;
    }

    public Task SetCursorWidth(double cursorWidthInPixels, bool updateStorage = true)
    {
        _dispatcher.Dispatch(new TextEditorOptionsState.SetCursorWidthAction(cursorWidthInPixels));

        if (updateStorage)
            return WriteToStorage();

        return Task.CompletedTask;
    }

    public void SetRenderStateKey(Key<RenderState> renderStateKey)
    {
        _dispatcher.Dispatch(new TextEditorOptionsState.SetRenderStateKeyAction(renderStateKey));
    }

    public Task WriteToStorage()
    {
        return _commonBackgroundTaskApi.Storage.WriteToLocalStorage(
            _textEditorService.StorageKey,
            new TextEditorOptionsJsonDto(_textEditorService.OptionsStateWrap.Value.Options));
    }

    public async Task SetFromLocalStorageAsync()
    {
        var optionsJsonString = await _storageService.GetValue(_textEditorService.StorageKey).ConfigureAwait(false) as string;

        if (string.IsNullOrWhiteSpace(optionsJsonString))
            return;

        var optionsJson = JsonSerializer.Deserialize<TextEditorOptionsJsonDto>(optionsJsonString);

        if (optionsJson is null)
            return;

        if (optionsJson.CommonOptionsJsonDto?.ThemeKey is not null)
        {
            var matchedTheme = _textEditorService.ThemeStateWrap.Value.ThemeList.FirstOrDefault(
                x => x.Key == optionsJson.CommonOptionsJsonDto.ThemeKey);

            await SetTheme(matchedTheme ?? ThemeFacts.VisualStudioDarkThemeClone, false).ConfigureAwait(false);
        }

        if (optionsJson.Keymap is not null)
        {
            var matchedKeymap = TextEditorKeymapFacts.AllKeymapsList.FirstOrDefault(
                x => x.Key == optionsJson.Keymap.Key);

            await SetKeymap(matchedKeymap ?? TextEditorKeymapFacts.DefaultKeymap, false).ConfigureAwait(false);

            var activeKeymap = _textEditorService.OptionsStateWrap.Value.Options.Keymap;

            if (activeKeymap is not null)
            {
                _dispatcher.Dispatch(new SetContextKeymapAction(
                    ContextFacts.TextEditorContext.ContextKey,
                    activeKeymap));
            }
        }

        if (optionsJson.CommonOptionsJsonDto?.FontSizeInPixels is not null)
            await SetFontSize(optionsJson.CommonOptionsJsonDto.FontSizeInPixels.Value, false).ConfigureAwait(false);

        if (optionsJson.CursorWidthInPixels is not null)
            await SetCursorWidth(optionsJson.CursorWidthInPixels.Value, false).ConfigureAwait(false);

        if (optionsJson.TextEditorHeightInPixels is not null)
            await SetHeight(optionsJson.TextEditorHeightInPixels.Value, false).ConfigureAwait(false);

        if (optionsJson.ShowNewlines is not null)
            await SetShowNewlines(optionsJson.ShowNewlines.Value, false).ConfigureAwait(false);

        // TODO: OptionsSetUseMonospaceOptimizations will always get set to false (default for bool)
        // for a first time user. This leads to a bad user experience since the proportional
        // font logic is still being optimized. Therefore don't read in UseMonospaceOptimizations
        // from local storage.
        //
        // OptionsSetUseMonospaceOptimizations(options.UseMonospaceOptimizations);

        if (optionsJson.ShowWhitespace is not null)
            await SetShowWhitespace(optionsJson.ShowWhitespace.Value, false).ConfigureAwait(false);
    }
}