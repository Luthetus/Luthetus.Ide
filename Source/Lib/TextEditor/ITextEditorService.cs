using Fluxor;
using Luthetus.Common.RazorLib.Themes.States;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.Common.RazorLib.Dimensions.States;
using Luthetus.Common.RazorLib.JsRuntimes.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.Diffs.Models;
using Luthetus.TextEditor.RazorLib.Diffs.States;
using Luthetus.TextEditor.RazorLib.FindAlls.States;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.TextEditor.RazorLib.Groups.States;
using Luthetus.TextEditor.RazorLib.Options.Models;
using Luthetus.TextEditor.RazorLib.Options.States;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using Luthetus.TextEditor.RazorLib.JsRuntimes.Models;
using Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.Autocompletes.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;

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
	public IState<AppDimensionState> AppDimensionStateWrap { get; }

	public LuthetusTextEditorJavaScriptInteropApi JsRuntimeTextEditorApi { get; }
	public LuthetusCommonJavaScriptInteropApi JsRuntimeCommonApi { get; }
	public IAutocompleteIndexer AutocompleteIndexer { get; }
	public IAutocompleteService AutocompleteService { get; }
	public LuthetusTextEditorConfig TextEditorConfig { get; }
        
    /// <summary>
    /// This method will create an instance of <see cref="UniqueTextEditorTask"/>,
    /// and then invoke IBackgroundTaskService.Enqueue(IBackgroundTask backgroundTask)<br/><br/>
    /// </summary>
    public void PostUnique(
        string name,
        Func<ITextEditorEditContext, ValueTask> textEditorFunc);
        
    /// <summary>
    /// This method will create an instance of <see cref="RedundantTextEditorTask"/>,
    /// and then invoke IBackgroundTaskService.Enqueue(IBackgroundTask backgroundTask)<br/><br/>
    /// </summary>
    public void PostRedundant(
        string name,
		ResourceUri resourceUri,
        Key<TextEditorViewModel> viewModelKey,
        Func<ITextEditorEditContext, ValueTask> textEditorFunc);

    /// <summary>
    /// This method simply invokes IBackgroundTaskService.Enqueue(IBackgroundTask backgroundTask)
    /// with the same parameter.
    /// </summary>
    public void Post(ITextEditorWork textEditorWork);
    
    /// <summary>
    /// This method simply invokes IBackgroundTaskService.EnqueueAsync(IBackgroundTask backgroundTask)
    /// with the same parameter.
    ///
    /// This async version will block until the background task is completed.
    /// </summary>
    public Task PostAsync(ITextEditorWork textEditorWork);

	/// <summmary>
	/// This method writes any mutated data within the <see cref="ITextEditorWork.EditContext"/>
	/// to the <see cref="TextEditorState"/>, and afterwards causes a UI render.
	/// </summary>
	public ValueTask FinalizePost(ITextEditorEditContext editContext);
	
	public Task OpenInEditorAsync(
		string absolutePath,
		bool shouldSetFocusToEditor,
		int? cursorPositionIndex,
		Category category,
		Key<TextEditorViewModel> preferredViewModelKey);
		
	public Task OpenInEditorAsync(
		string absolutePath,
		bool shouldSetFocusToEditor,
		int? lineIndex,
		int? columnIndex,
		Category category,
		Key<TextEditorViewModel> preferredViewModelKey);
}
