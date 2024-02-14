using Fluxor;
using Luthetus.Common.RazorLib.Themes.States;
using Luthetus.TextEditor.RazorLib.Diffs.States;
using Luthetus.TextEditor.RazorLib.FindAlls.States;
using Luthetus.TextEditor.RazorLib.Groups.States;
using Luthetus.TextEditor.RazorLib.Options.States;
using Luthetus.TextEditor.RazorLib.TextEditors.States;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;

public partial interface ITextEditorService
{
    /// <summary>This is used when interacting with the <see cref="IStorageService"/> to set and get data.</summary>
    public string StorageKey { get; }
    public string ThemeCssClassString { get; }

    public ITextEditorModelApi ModelApi { get; }
    public ITextEditorViewModelApi ViewModelApi { get; }
    public ITextEditorGroupApi GroupApi { get; }
    public ITextEditorDiffApi DiffApi { get; }
    public ITextEditorOptionsApi OptionsApi { get; }
    public ITextEditorSearchEngineApi SearchEngineApi { get; }

    public IState<TextEditorModelState> ModelStateWrap { get; }
    public IState<TextEditorViewModelState> ViewModelStateWrap { get; }
    public IState<TextEditorGroupState> GroupStateWrap { get; }
    public IState<TextEditorDiffState> DiffStateWrap { get; }
    public IState<ThemeState> ThemeStateWrap { get; }
    public IState<TextEditorOptionsState> OptionsStateWrap { get; }
    public IState<TextEditorSearchEngineState> SearchEngineStateWrap { get; }

    /// <summary>
    /// The intent of this method is to replicate functionality of the <see cref="SynchronizationContext"/>
    /// class. Here the replication is quite roundabout, perhaps I should use <see cref="SynchronizationContext"/> directly,
    /// but I need to learn more about it. (2023-12-21)
    /// </summary>
    public void Post(string taskDisplayName, TextEditorEdit textEditorEdit);
}
