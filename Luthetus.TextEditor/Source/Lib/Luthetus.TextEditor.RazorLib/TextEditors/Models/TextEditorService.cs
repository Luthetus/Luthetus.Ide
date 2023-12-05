using Fluxor;
using Luthetus.Common.RazorLib.Storages.States;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.Common.RazorLib.Themes.States;
using Luthetus.Common.RazorLib.Themes.Models;
using Luthetus.TextEditor.RazorLib.Diffs.States;
using Luthetus.TextEditor.RazorLib.Finds.States;
using Luthetus.TextEditor.RazorLib.Groups.States;
using Luthetus.TextEditor.RazorLib.Options.States;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using Microsoft.JSInterop;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using static Luthetus.TextEditor.RazorLib.TextEditors.Models.ITextEditorService;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

public class TextEditorService : ITextEditorService
{
    private readonly IDispatcher _dispatcher;
    private readonly LuthetusTextEditorOptions _luthetusTextEditorOptions;
    private readonly IStorageService _storageService;
    // TODO: Perhaps do not reference IJSRuntime but instead wrap it in a 'IUiProvider' or something like that. The 'IUiProvider' would then expose methods that allow the TextEditorViewModel to adjust the scrollbars. 
    private readonly IJSRuntime _jsRuntime;
    private readonly StorageSync _storageSync;

    public TextEditorService(
        IState<TextEditorModelState> modelStateWrap,
        IState<TextEditorViewModelState> viewModelStateWrap,
        IState<TextEditorGroupState> groupStateWrap,
        IState<TextEditorDiffState> diffStateWrap,
        IState<ThemeState> themeStateWrap,
        IState<TextEditorOptionsState> optionsStateWrap,
        IState<TextEditorFindProviderState> findProviderStateWrap,
        LuthetusTextEditorOptions luthetusTextEditorOptions,
        IStorageService storageService,
        IJSRuntime jsRuntime,
        StorageSync storageSync,
        IDispatcher dispatcher)
    {
        ModelStateWrap = modelStateWrap;
        ViewModelStateWrap = viewModelStateWrap;
        GroupStateWrap = groupStateWrap;
        DiffStateWrap = diffStateWrap;
        ThemeStateWrap = themeStateWrap;
        OptionsStateWrap = optionsStateWrap;
        FindProviderStateWrap = findProviderStateWrap;

        _luthetusTextEditorOptions = luthetusTextEditorOptions;
        _storageService = storageService;
        _jsRuntime = jsRuntime;
        _storageSync = storageSync;
        _dispatcher = dispatcher;

        Model = new ModelApi(this, _dispatcher);
        ViewModel = new ViewModelApi(this, _jsRuntime, _dispatcher);
        Group = new GroupApi(this, _dispatcher);
        Diff = new DiffApi(this, _dispatcher);
        Options = new OptionsApi(this, _luthetusTextEditorOptions, _storageService, _storageSync, _dispatcher);
        FindProvider = new FindProviderApi(this, _dispatcher);
    }

    public IState<TextEditorModelState> ModelStateWrap { get; }
    public IState<TextEditorViewModelState> ViewModelStateWrap { get; }
    public IState<TextEditorGroupState> GroupStateWrap { get; }
    public IState<TextEditorDiffState> DiffStateWrap { get; }
    public IState<ThemeState> ThemeStateWrap { get; }
    public IState<TextEditorOptionsState> OptionsStateWrap { get; }
    public IState<TextEditorFindProviderState> FindProviderStateWrap { get; }

    
#if DEBUG
    public string StorageKey => "luth_te_text-editor-options-debug";
#else
    public string StorageKey => "luth_te_text-editor-options";
#endif

    public string ThemeCssClassString => ThemeStateWrap.Value.ThemeBag.FirstOrDefault(
        x => x.Key == OptionsStateWrap.Value.Options.CommonOptions.ThemeKey)
        ?.CssClassString
            ?? ThemeFacts.VisualStudioDarkThemeClone.CssClassString;

    public IModelApi Model { get; }
    public IViewModelApi ViewModel { get; }
    public IGroupApi Group { get; }
    public IDiffApi Diff { get; }
    public IOptionsApi Options { get; }
    public IFindProviderApi FindProvider { get; }
}