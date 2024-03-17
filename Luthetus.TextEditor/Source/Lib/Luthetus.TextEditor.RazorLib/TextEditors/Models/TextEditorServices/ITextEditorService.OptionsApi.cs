using Fluxor;
using System.Text.Json;
using Luthetus.Common.RazorLib.Storages.States;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Themes.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.Options.Models;
using Luthetus.TextEditor.RazorLib.Options.States;
using Luthetus.TextEditor.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using static Luthetus.Common.RazorLib.Contexts.States.ContextState;
using Luthetus.Common.RazorLib.Dynamics.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;

public partial interface ITextEditorService
{
    public interface ITextEditorOptionsApi
    {
        public void SetCursorWidth(double cursorWidthInPixels);
        public void SetFontFamily(string? fontFamily);
        public void SetFontSize(int fontSizeInPixels);
        public Task SetFromLocalStorageAsync();
        public void SetHeight(int? heightInPixels);
        public void SetKeymap(Keymap keymap);
        public void SetShowNewlines(bool showNewlines);
        public void SetUseMonospaceOptimizations(bool useMonospaceOptimizations);
        public void SetShowWhitespace(bool showWhitespace);
        /// <summary>This is setting the TextEditor's theme specifically. This is not to be confused with the AppOptions Themes which get applied at an application level. <br /><br /> This allows for a "DarkTheme-Application" that has a "LightTheme-TextEditor"</summary>
        public void SetTheme(ThemeRecord theme);
        public void ShowSettingsDialog(bool? isResizableOverride = null, string? cssClassString = null);
        public void ShowFindAllDialog(bool? isResizableOverride = null, string? cssClassString = null);
        public void WriteToStorage();
        public void SetRenderStateKey(Key<RenderState> renderStateKey);

        /// <summary>
        /// One should store the result of invoking this method in a variable, then reference that variable.
        /// If one continually invokes this, there is no guarantee that the data had not changed
        /// since the previous invocation.
        /// </summary>
        public TextEditorOptions GetOptions();
    }

    public class TextEditorOptionsApi : ITextEditorOptionsApi
    {
        private readonly ITextEditorService _textEditorService;
        private readonly LuthetusTextEditorConfig _textEditorConfig;
        private readonly IStorageService _storageService;
        private readonly StorageSync _storageSync;
        private readonly IDispatcher _dispatcher;

        public TextEditorOptionsApi(
            ITextEditorService textEditorService,
            LuthetusTextEditorConfig textEditorConfig,
            IStorageService storageService,
            StorageSync storageSync,
            IDispatcher dispatcher)
        {
            _textEditorService = textEditorService;
            _textEditorConfig = textEditorConfig;
            _storageService = storageService;
            _storageSync = storageSync;
            _dispatcher = dispatcher;
        }

        private IDialog? _findAllDialog;

        public void WriteToStorage()
        {
            _storageSync.WriteToLocalStorage(
                _textEditorService.StorageKey,
                new TextEditorOptionsJsonDto(_textEditorService.OptionsStateWrap.Value.Options));
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

        public void SetTheme(ThemeRecord theme)
        {
            _dispatcher.Dispatch(new TextEditorOptionsState.SetThemeAction(theme));
            WriteToStorage();
        }

        public void SetShowWhitespace(bool showWhitespace)
        {
            _dispatcher.Dispatch(new TextEditorOptionsState.SetShowWhitespaceAction(showWhitespace));
            WriteToStorage();
        }

        public void SetUseMonospaceOptimizations(bool useMonospaceOptimizations)
        {
            _dispatcher.Dispatch(new TextEditorOptionsState.SetUseMonospaceOptimizationsAction(useMonospaceOptimizations));
            WriteToStorage();
        }

        public void SetShowNewlines(bool showNewlines)
        {
            _dispatcher.Dispatch(new TextEditorOptionsState.SetShowNewlinesAction(showNewlines));
            WriteToStorage();
        }

        public void SetKeymap(Keymap keymap)
        {
            _dispatcher.Dispatch(new TextEditorOptionsState.SetKeymapAction(keymap));

            var activeKeymap = _textEditorService.OptionsStateWrap.Value.Options.Keymap;

            if (activeKeymap is not null)
            {
                _dispatcher.Dispatch(new SetContextKeymapAction(
                    ContextFacts.TextEditorContext.ContextKey,
                    activeKeymap));
            }

            WriteToStorage();
        }

        public void SetHeight(int? heightInPixels)
        {
            _dispatcher.Dispatch(new TextEditorOptionsState.SetHeightAction(heightInPixels));
            WriteToStorage();
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

                SetTheme(matchedTheme ?? ThemeFacts.VisualStudioDarkThemeClone);
            }

            if (optionsJson.Keymap is not null)
            {
                var matchedKeymap = TextEditorKeymapFacts.AllKeymapsList.FirstOrDefault(
                    x => x.Key == optionsJson.Keymap.Key);

                SetKeymap(matchedKeymap ?? TextEditorKeymapFacts.DefaultKeymap);

                var activeKeymap = _textEditorService.OptionsStateWrap.Value.Options.Keymap;

                if (activeKeymap is not null)
                {
                    _dispatcher.Dispatch(new SetContextKeymapAction(
                        ContextFacts.TextEditorContext.ContextKey,
                        activeKeymap));
                }
            }

            if (optionsJson.CommonOptionsJsonDto?.FontSizeInPixels is not null)
                SetFontSize(optionsJson.CommonOptionsJsonDto.FontSizeInPixels.Value);

            if (optionsJson.CursorWidthInPixels is not null)
                SetCursorWidth(optionsJson.CursorWidthInPixels.Value);

            if (optionsJson.TextEditorHeightInPixels is not null)
                SetHeight(optionsJson.TextEditorHeightInPixels.Value);

            if (optionsJson.ShowNewlines is not null)
                SetShowNewlines(optionsJson.ShowNewlines.Value);

            // TODO: OptionsSetUseMonospaceOptimizations will always get set to false (default for bool)
            // for a first time user. This leads to a bad user experience since the proportional
            // font logic is still being optimized. Therefore don't read in UseMonospaceOptimizations
            // from local storage.
            //
            // OptionsSetUseMonospaceOptimizations(options.UseMonospaceOptimizations);

            if (optionsJson.ShowWhitespace is not null)
                SetShowWhitespace(optionsJson.ShowWhitespace.Value);
        }

        public void SetFontSize(int fontSizeInPixels)
        {
            _dispatcher.Dispatch(new TextEditorOptionsState.SetFontSizeAction(fontSizeInPixels));
            WriteToStorage();
        }

        public void SetFontFamily(string? fontFamily)
        {
            _dispatcher.Dispatch(new TextEditorOptionsState.SetFontFamilyAction(fontFamily));
            WriteToStorage();
        }

        public void SetCursorWidth(double cursorWidthInPixels)
        {
            _dispatcher.Dispatch(new TextEditorOptionsState.SetCursorWidthAction(cursorWidthInPixels));
            WriteToStorage();
        }

        public void SetRenderStateKey(Key<RenderState> renderStateKey)
        {
            _dispatcher.Dispatch(new TextEditorOptionsState.SetRenderStateKeyAction(renderStateKey));
        }

        public TextEditorOptions GetOptions()
        {
            return _textEditorService.OptionsStateWrap.Value.Options;
        }
    }
}