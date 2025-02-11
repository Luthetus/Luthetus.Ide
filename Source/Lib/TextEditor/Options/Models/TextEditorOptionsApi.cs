using System.Text.Json;
using Fluxor;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.Common.RazorLib.Themes.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.Options.States;

using Luthetus.Common.RazorLib.BackgroundTasks.Models;

namespace Luthetus.TextEditor.RazorLib.Options.Models;

public class TextEditorOptionsApi : ITextEditorOptionsApi
{
    private readonly ITextEditorService _textEditorService;
    private readonly LuthetusTextEditorConfig _textEditorConfig;
    private readonly IStorageService _storageService;
    private readonly IDialogService _dialogService;
    private readonly IContextService _contextService;
    private readonly CommonBackgroundTaskApi _commonBackgroundTaskApi;
    private readonly IDispatcher _dispatcher;

    public TextEditorOptionsApi(
        ITextEditorService textEditorService,
        LuthetusTextEditorConfig textEditorConfig,
        IStorageService storageService,
        IDialogService dialogService,
        IContextService contextService,
        CommonBackgroundTaskApi commonBackgroundTaskApi,
        IDispatcher dispatcher)
    {
        _textEditorService = textEditorService;
        _textEditorConfig = textEditorConfig;
        _storageService = storageService;
        _dialogService = dialogService;
        _contextService = contextService;
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
        // TODO: determine the actively focused element at time of invocation,
        //       then restore focus to that element when this dialog is closed.
        var settingsDialog = new DialogViewModel(
            Key<IDynamicViewModel>.NewKey(),
            "Text Editor Settings",
            _textEditorConfig.SettingsDialogConfig.ComponentRendererType,
            null,
            cssClassString,
            isResizableOverride ?? _textEditorConfig.SettingsDialogConfig.ComponentIsResizable,
            null);

        _dialogService.ReduceRegisterAction(settingsDialog);
    }

    public void ShowFindAllDialog(bool? isResizableOverride = null, string? cssClassString = null)
    {
        // TODO: determine the actively focused element at time of invocation,
        //       then restore focus to that element when this dialog is closed.
        _findAllDialog ??= new DialogViewModel(
            Key<IDynamicViewModel>.NewKey(),
            "Find All",
            _textEditorConfig.FindAllDialogConfig.ComponentRendererType,
            null,
            cssClassString,
            isResizableOverride ?? _textEditorConfig.FindAllDialogConfig.ComponentIsResizable,
            null);

        _dialogService.ReduceRegisterAction(_findAllDialog);
    }

    public void SetTheme(ThemeRecord theme, bool updateStorage = true)
    {
        _dispatcher.Dispatch(new TextEditorOptionsState.SetThemeAction(theme));

        if (updateStorage)
            WriteToStorage();
    }

    public void SetShowWhitespace(bool showWhitespace, bool updateStorage = true)
    {
        _dispatcher.Dispatch(new TextEditorOptionsState.SetShowWhitespaceAction(showWhitespace));

        if (updateStorage)
            WriteToStorage();
    }

    public void SetUseMonospaceOptimizations(bool useMonospaceOptimizations, bool updateStorage = true)
    {
        _dispatcher.Dispatch(new TextEditorOptionsState.SetUseMonospaceOptimizationsAction(useMonospaceOptimizations));

        if (updateStorage)
            WriteToStorage();
    }

    public void SetShowNewlines(bool showNewlines, bool updateStorage = true)
    {
        _dispatcher.Dispatch(new TextEditorOptionsState.SetShowNewlinesAction(showNewlines));

        if (updateStorage)
            WriteToStorage();
    }

    public void SetKeymap(Keymap keymap, bool updateStorage = true)
    {
        _dispatcher.Dispatch(new TextEditorOptionsState.SetKeymapAction(keymap));

        var activeKeymap = _textEditorService.OptionsStateWrap.Value.Options.Keymap;

        if (activeKeymap is not null)
        {
            _contextService.ReduceSetContextKeymapAction(
                ContextFacts.TextEditorContext.ContextKey,
                activeKeymap);
        }

        if (updateStorage)
            WriteToStorage();
    }

    public void SetHeight(int? heightInPixels, bool updateStorage = true)
    {
        _dispatcher.Dispatch(new TextEditorOptionsState.SetHeightAction(heightInPixels));

        if (updateStorage)
            WriteToStorage();
    }

    public void SetFontSize(int fontSizeInPixels, bool updateStorage = true)
    {
        _dispatcher.Dispatch(new TextEditorOptionsState.SetFontSizeAction(fontSizeInPixels));

        if (updateStorage)
            WriteToStorage();
    }

    public void SetFontFamily(string? fontFamily, bool updateStorage = true)
    {
        _dispatcher.Dispatch(new TextEditorOptionsState.SetFontFamilyAction(fontFamily));

        if (updateStorage)
            WriteToStorage();
    }

    public void SetCursorWidth(double cursorWidthInPixels, bool updateStorage = true)
    {
        _dispatcher.Dispatch(new TextEditorOptionsState.SetCursorWidthAction(cursorWidthInPixels));

        if (updateStorage)
            WriteToStorage();
    }

    public void SetRenderStateKey(Key<RenderState> renderStateKey)
    {
        _dispatcher.Dispatch(new TextEditorOptionsState.SetRenderStateKeyAction(renderStateKey));
    }

    public void WriteToStorage()
    {
        _commonBackgroundTaskApi.Storage.WriteToLocalStorage(
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
            var matchedTheme = _textEditorService.ThemeService.GetThemeState().ThemeList.FirstOrDefault(
                x => x.Key == optionsJson.CommonOptionsJsonDto.ThemeKey);

            SetTheme(matchedTheme ?? ThemeFacts.VisualStudioDarkThemeClone, false);
        }

        if (optionsJson.Keymap is not null)
        {
            var matchedKeymap = TextEditorKeymapFacts.AllKeymapsList.FirstOrDefault(
                x => x.Key == optionsJson.Keymap.Key);

            SetKeymap(matchedKeymap ?? TextEditorKeymapFacts.DefaultKeymap, false);

            var activeKeymap = _textEditorService.OptionsStateWrap.Value.Options.Keymap;

            if (activeKeymap is not null)
            {
                _contextService.ReduceSetContextKeymapAction(
                    ContextFacts.TextEditorContext.ContextKey,
                    activeKeymap);
            }
        }

        if (optionsJson.CommonOptionsJsonDto?.FontSizeInPixels is not null)
            SetFontSize(optionsJson.CommonOptionsJsonDto.FontSizeInPixels.Value, false);

        if (optionsJson.CursorWidthInPixels is not null)
            SetCursorWidth(optionsJson.CursorWidthInPixels.Value, false);

        if (optionsJson.TextEditorHeightInPixels is not null)
            SetHeight(optionsJson.TextEditorHeightInPixels.Value, false);

        if (optionsJson.ShowNewlines is not null)
            SetShowNewlines(optionsJson.ShowNewlines.Value, false);

        // TODO: OptionsSetUseMonospaceOptimizations will always get set to false (default for bool)
        // for a first time user. This leads to a bad user experience since the proportional
        // font logic is still being optimized. Therefore don't read in UseMonospaceOptimizations
        // from local storage.
        //
        // OptionsSetUseMonospaceOptimizations(options.UseMonospaceOptimizations);

        if (optionsJson.ShowWhitespace is not null)
            SetShowWhitespace(optionsJson.ShowWhitespace.Value, false);
    }
}