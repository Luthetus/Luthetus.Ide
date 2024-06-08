using Fluxor;
using Luthetus.Common.RazorLib.Themes.States;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.Diffs.Models;
using Luthetus.TextEditor.RazorLib.Diffs.States;
using Luthetus.TextEditor.RazorLib.FindAlls.States;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.TextEditor.RazorLib.Groups.States;
using Luthetus.TextEditor.RazorLib.Options.Models;
using Luthetus.TextEditor.RazorLib.Options.States;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using Luthetus.TextEditor.RazorLib.JsRuntimes.Models;
using Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.Autocompletes.Models;

namespace Luthetus.TextEditor.RazorLib;

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

    public IState<TextEditorState> TextEditorStateWrap { get; }
    public IState<TextEditorGroupState> GroupStateWrap { get; }
    public IState<TextEditorDiffState> DiffStateWrap { get; }
    public IState<ThemeState> ThemeStateWrap { get; }
    public IState<TextEditorOptionsState> OptionsStateWrap { get; }
    public IState<TextEditorFindAllState> FindAllStateWrap { get; }

	public LuthetusTextEditorJavaScriptInteropApi JsRuntimeTextEditorApi { get; }
	public IAutocompleteIndexer AutocompleteIndexer { get; }
	public IAutocompleteService AutocompleteService { get; }

    /// <summary>
    /// This method will create an instance of <see cref="BackgroundTasks.Models.SimpleBatchTextEditorTask"/>,
    /// and then invoke <see cref="Post(ITextEditorTask)"/><br/><br/>
    /// --- <see cref="BackgroundTasks.Models.SimpleBatchTextEditorTask"/>.cs inheritdoc:<br/><br/>
    /// <inheritdoc cref="BackgroundTasks.Models.SimpleBatchTextEditorTask"/>
    /// </summary>
    public Task PostSimpleBatch(
        string name,
        TextEditorEdit textEditorEdit,
        TimeSpan? throttleTimeSpan = null);

    /// <summary>
    /// This method will create an instance of <see cref="BackgroundTasks.Models.TakeMostRecentTextEditorTask"/>,
    /// and then invoke <see cref="Post(ITextEditorTask)"/><br/><br/>
    /// --- <see cref="BackgroundTasks.Models.TakeMostRecentTextEditorTask"/>.cs inheritdoc:<br/><br/>
    /// <inheritdoc cref="BackgroundTasks.Models.TakeMostRecentTextEditorTask"/>
    /// </summary>
    public Task PostTakeMostRecent(
        string name,
		ResourceUri resourceUri,
        Key<TextEditorViewModel> viewModelKey,
        TextEditorEdit textEditorEdit,
        TimeSpan? throttleTimeSpan = null);

    /// <summary>
    /// This method creates a <see cref="TextEditorServiceTask"/>
    /// that will encapsulate the provided innerTask.
    /// When the queue invokes the encapsulating <see cref="TextEditorServiceTask"/>,
    /// then the provided innerTask's <see cref="ITextEditorTask.InvokeWithEditContext(IEditContext)"/> will be invoked in turn.
    /// When the innerTask is finished, the encapsulating <see cref="TextEditorServiceTask"/>
    /// will update any state that was modified, and trigger re-renders for the UI.
    /// </summary>
    public Task Post(ITextEditorTask textEditorTask);

	public Task FinalizePost(IEditContext editContext);
}
