using Microsoft.JSInterop;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.Common.RazorLib.Themes.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.JsRuntimes.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.Diffs.Models;
using Luthetus.TextEditor.RazorLib.FindAlls.Models;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.TextEditor.RazorLib.Options.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.JsRuntimes.Models;
using Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.Autocompletes.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;

namespace Luthetus.TextEditor.RazorLib;

/// <summary>
/// Post any background tasks via <see cref="TextEditorWorker"/>.
/// TODO: Make the TextEditorWorker logic more clear...
/// ...(more clear that you don't use IBackgroundTaskService for text editor related background tasks).
/// </summary>
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
    
    public IThemeService ThemeService { get; }
    public IAppDimensionService AppDimensionService { get; }
    public IFindAllService FindAllService { get; }

	public LuthetusTextEditorJavaScriptInteropApi JsRuntimeTextEditorApi { get; }
	public LuthetusCommonJavaScriptInteropApi JsRuntimeCommonApi { get; }
	public IAutocompleteIndexer AutocompleteIndexer { get; }
	public IAutocompleteService AutocompleteService { get; }
	public LuthetusTextEditorConfig TextEditorConfig { get; }
	
	public TextEditorState TextEditorState { get; }
	
	public TextEditorWorker TextEditorWorker { get; }
	
	/// <summary>TODO: Delete this, this is a hack so I have it in scope for the new TextEditorWorker code.</summary>
	public IBackgroundTaskService BackgroundTaskService { get; }
	
	public event Action? TextEditorStateChanged;
        
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
		
	public void ReduceRegisterModelAction(TextEditorModel model);
	public void ReduceDisposeModelAction(ResourceUri resourceUri);
	public void ReduceSetModelAction(ITextEditorEditContext editContext, TextEditorModelModifier modelModifier);
	
	public void ReduceRegisterViewModelAction(
	    Key<TextEditorViewModel> viewModelKey,
	    ResourceUri resourceUri,
	    Category category,
	    ITextEditorService textEditorService,
	    IDialogService dialogService,
	    IJSRuntime jsRuntime);
	
	public void ReduceRegisterViewModelExistingAction(TextEditorViewModel viewModel);
	
	public void ReduceDisposeViewModelAction(Key<TextEditorViewModel> viewModelKey);
	
	public void ReduceSetViewModelWithAction(
	    ITextEditorEditContext editContext,
	    Key<TextEditorViewModel> viewModelKey,
	    Func<TextEditorViewModel, TextEditorViewModel> withFunc);
	
	public void ReduceSetModelAndViewModelRangeAction(
	    ITextEditorEditContext editContext,
		Dictionary<ResourceUri, TextEditorModelModifier?>? modelModifierList,
		Dictionary<Key<TextEditorViewModel>, TextEditorViewModelModifier?>? viewModelModifierList);
}
