using System.Text.Json;
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
using Luthetus.TextEditor.RazorLib.JavaScriptObjects.Models;

using Luthetus.Common.RazorLib.BackgroundTasks.Models;

namespace Luthetus.TextEditor.RazorLib.Options.Models;

public sealed class TextEditorOptionsApi
{
    private readonly TextEditorService _textEditorService;
    private readonly LuthetusTextEditorConfig _textEditorConfig;
    private readonly IStorageService _storageService;
    private readonly IDialogService _dialogService;
    private readonly IContextService _contextService;
    private readonly IThemeService _themeService;
    private readonly CommonBackgroundTaskApi _commonBackgroundTaskApi;

    public TextEditorOptionsApi(
        TextEditorService textEditorService,
        LuthetusTextEditorConfig textEditorConfig,
        IStorageService storageService,
        IDialogService dialogService,
        IContextService contextService,
        IThemeService themeService,
        CommonBackgroundTaskApi commonBackgroundTaskApi)
    {
        _textEditorService = textEditorService;
        _textEditorConfig = textEditorConfig;
        _storageService = storageService;
        _dialogService = dialogService;
        _contextService = contextService;
        _themeService = themeService;
        _commonBackgroundTaskApi = commonBackgroundTaskApi;
    }
    
    private TextEditorOptionsState _textEditorOptionsState = new();

    private IDialog? _findAllDialog;

	public event Action? StaticStateChanged;
	public event Action? NeedsMeasured;
    public event Action? MeasuredStateChanged;

	public TextEditorOptionsState GetTextEditorOptionsState() => _textEditorOptionsState;

    public TextEditorOptions GetOptions()
    {
        return _textEditorService.OptionsApi.GetTextEditorOptionsState().Options;
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
    	var inState = GetTextEditorOptionsState();
    
        _textEditorOptionsState = new TextEditorOptionsState
        {
            Options = inState.Options with
            {
                CommonOptions = inState.Options.CommonOptions with
                {
                    ThemeKey = theme.Key
                },
                RenderStateKey = Key<RenderState>.NewKey(),
            },
        };
        
        // I'm optimizing all the expression bound properties that construct
		// a string, and specifically the ones that are rendered in the UI many times.
		//
		// Can probably use 'theme' variable here but
		// I don't want to touch that right now -- incase there are unexpected consequences.
        var usingThemeCssClassString = _themeService.GetThemeState().ThemeList
        	.FirstOrDefault(x => x.Key == GetTextEditorOptionsState().Options.CommonOptions.ThemeKey)
        	?.CssClassString
            ?? ThemeFacts.VisualStudioDarkThemeClone.CssClassString;
        _textEditorService.ThemeCssClassString = usingThemeCssClassString;
        
        StaticStateChanged?.Invoke();

        if (updateStorage)
            WriteToStorage();
    }

    public void SetShowWhitespace(bool showWhitespace, bool updateStorage = true)
    {
    	var inState = GetTextEditorOptionsState();
    
        _textEditorOptionsState = new TextEditorOptionsState
        {
            Options = inState.Options with
            {
                ShowWhitespace = showWhitespace,
                RenderStateKey = Key<RenderState>.NewKey(),
            },
        };
        StaticStateChanged?.Invoke();

        if (updateStorage)
            WriteToStorage();
    }

    public void SetUseMonospaceOptimizations(bool useMonospaceOptimizations, bool updateStorage = true)
    {
    	var inState = GetTextEditorOptionsState();
    
		_textEditorOptionsState = new TextEditorOptionsState
        {
            Options = inState.Options with
            {
                UseMonospaceOptimizations = useMonospaceOptimizations,
                RenderStateKey = Key<RenderState>.NewKey(),
            },
        };
        StaticStateChanged?.Invoke();
        
        if (updateStorage)
            WriteToStorage();
    }

    public void SetShowNewlines(bool showNewlines, bool updateStorage = true)
    {
    	var inState = GetTextEditorOptionsState();
    
		_textEditorOptionsState = new TextEditorOptionsState
        {
            Options = inState.Options with
            {
                ShowNewlines = showNewlines,
                RenderStateKey = Key<RenderState>.NewKey(),
            },
        };
        StaticStateChanged?.Invoke();
        
        if (updateStorage)
            WriteToStorage();
    }

    public void SetKeymap(ITextEditorKeymap keymap, bool updateStorage = true)
    {
    	var inState = GetTextEditorOptionsState();
    
        _textEditorOptionsState = new TextEditorOptionsState
        {
            Options = inState.Options with
            {
                Keymap = keymap,
                RenderStateKey = Key<RenderState>.NewKey(),
            },
        };
        StaticStateChanged?.Invoke();

        /*var activeKeymap = _textEditorService.OptionsApi.GetTextEditorOptionsState().Options.Keymap;

        if (activeKeymap is not null)
        {
            _contextService.SetContextKeymap(
                ContextFacts.TextEditorContext.ContextKey,
                activeKeymap);
        }

        if (updateStorage)
            WriteToStorage();*/
    }

    public void SetHeight(int? heightInPixels, bool updateStorage = true)
    {
    	var inState = GetTextEditorOptionsState();
    
        _textEditorOptionsState = new TextEditorOptionsState
        {
            Options = inState.Options with
            {
                TextEditorHeightInPixels = heightInPixels,
                RenderStateKey = Key<RenderState>.NewKey(),
            },
        };
        StaticStateChanged?.Invoke();

        if (updateStorage)
            WriteToStorage();
    }

    public void SetFontSize(int fontSizeInPixels, bool updateStorage = true)
    {
    	var inState = GetTextEditorOptionsState();
    
        _textEditorOptionsState = new TextEditorOptionsState
        {
            Options = inState.Options with
            {
                CommonOptions = inState.Options.CommonOptions with
                {
                    FontSizeInPixels = fontSizeInPixels
                },
                RenderStateKey = Key<RenderState>.NewKey(),
            },
        };
        NeedsMeasured?.Invoke();

        if (updateStorage)
            WriteToStorage();
    }

    public void SetFontFamily(string? fontFamily, bool updateStorage = true)
    {
    	var inState = GetTextEditorOptionsState();
    
        _textEditorOptionsState = new TextEditorOptionsState
        {
            Options = inState.Options with
            {
                CommonOptions = inState.Options.CommonOptions with
                {
                    FontFamily = fontFamily
                },
                RenderStateKey = Key<RenderState>.NewKey(),
            },
        };
        NeedsMeasured?.Invoke();

        if (updateStorage)
            WriteToStorage();
    }

    public void SetCursorWidth(double cursorWidthInPixels, bool updateStorage = true)
    {
    	var inState = GetTextEditorOptionsState();

        _textEditorOptionsState = new TextEditorOptionsState
        {
            Options = inState.Options with
            {
                CursorWidthInPixels = cursorWidthInPixels,
                RenderStateKey = Key<RenderState>.NewKey(),
            },
        };
        StaticStateChanged?.Invoke();

        if (updateStorage)
            WriteToStorage();
    }

    public void SetRenderStateKey(Key<RenderState> renderStateKey)
    {
    	var inState = GetTextEditorOptionsState();
    
        _textEditorOptionsState = new TextEditorOptionsState
        {
            Options = inState.Options with
            {
                RenderStateKey = renderStateKey
            },
        };
        StaticStateChanged?.Invoke();
    }
    
    public void SetCharAndLineMeasurements(TextEditorEditContext editContext, CharAndLineMeasurements charAndLineMeasurements)
    {
    	var inState = GetTextEditorOptionsState();

        _textEditorOptionsState = new TextEditorOptionsState
        {
            Options = inState.Options with
            {
                CharAndLineMeasurements = charAndLineMeasurements,
                RenderStateKey = Key<RenderState>.NewKey(),
            },
        };
        
    	MeasuredStateChanged?.Invoke();
    }

    public void WriteToStorage()
    {
        _commonBackgroundTaskApi.Enqueue(new CommonWorkArgs
        {
    		WorkKind = CommonWorkKind.WriteToLocalStorage,
        	WriteToLocalStorage_Key = _textEditorService.StorageKey,
            WriteToLocalStorage_Value = new TextEditorOptionsJsonDto(_textEditorService.OptionsApi.GetTextEditorOptionsState().Options),
        });
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

        /*if (optionsJson.Keymap is not null)
        {
            var matchedKeymap = TextEditorKeymapFacts.AllKeymapsList.FirstOrDefault(
                x => x.Key == optionsJson.Keymap.Key);

            SetKeymap(matchedKeymap ?? TextEditorKeymapFacts.DefaultKeymap, false);

            var activeKeymap = _textEditorService.OptionsApi.GetTextEditorOptionsState().Options.Keymap;

            if (activeKeymap is not null)
            {
                _contextService.SetContextKeymap(
                    ContextFacts.TextEditorContext.ContextKey,
                    activeKeymap);
            }
        }*/

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
