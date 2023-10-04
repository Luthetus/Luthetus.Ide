using Fluxor;
using Luthetus.Common.RazorLib.Themes.States;
using Luthetus.TextEditor.RazorLib.Diffs.States;
using Luthetus.TextEditor.RazorLib.Finds.States;
using Luthetus.TextEditor.RazorLib.Groups.States;
using Luthetus.TextEditor.RazorLib.Options.States;
using Luthetus.TextEditor.RazorLib.TextEditors.States;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

public partial interface ITextEditorService
{
    /// <summary>This is used when interacting with the <see cref="IStorageService"/> to set and get data.</summary>
    public string StorageKey { get; }
    public string ThemeCssClassString { get; }

    public IModelApi Model { get; }
    public IViewModelApi ViewModel { get; }
    public IGroupApi Group { get; }
    public IDiffApi Diff { get; }
    public IOptionsApi Options { get; }
    public IFindProviderApi FindProvider { get; }

    public IState<TextEditorModelState> ModelStateWrap { get; }
    public IState<TextEditorViewModelState> ViewModelStateWrap { get; }
    public IState<TextEditorGroupState> GroupStateWrap { get; }
    public IState<TextEditorDiffState> DiffStateWrap { get; }
    public IState<ThemeState> ThemeStateWrap { get; }
    public IState<TextEditorOptionsState> OptionsStateWrap { get; }
    public IState<TextEditorFindProviderState> FindProviderStateWrap { get; }
}