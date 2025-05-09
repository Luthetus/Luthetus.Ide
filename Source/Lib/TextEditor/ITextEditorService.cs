using System.Text;
using Microsoft.JSInterop;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.Common.RazorLib.Themes.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.JsRuntimes.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.Lines.Models;
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
using Luthetus.TextEditor.RazorLib.Cursors.Models;

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
    public string ThemeCssClassString { get; set; }

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
	
	/// <summary>
	/// Capture a reference to this if re-using it within the same "context" otherwise it will change out from under you.
	/// 
	/// TODO: Make this private and make a method 'GetTextEditorState()' that returns the private field...
	/// ...because it is more self documenting that there is something "odd" going on.
	/// </summary>
	public TextEditorState TextEditorState { get; }
	
	public TextEditorWorkerUi WorkerUi { get; }
	public TextEditorWorkerArbitrary WorkerArbitrary { get; }
	
	/// <summary>TODO: Delete this, this is a hack so I have it in scope for the new TextEditorWorker code.</summary>
	public IBackgroundTaskService BackgroundTaskService { get; }
	
	/// <summary>
	/// Do not touch this property, it is used for the VirtualizationGrid and VirtualizationLine.
	/// </summary>
	public StringBuilder __StringBuilder { get; }
	
	/// <summary>
	/// Do not touch this property, it is used for the TextEditorEditContext.
	/// </summary>
	public TextEditorCursorModifier __CursorModifier { get; }
	/// <summary>
	/// Do not touch this property, it is used for the TextEditorEditContext.
	/// </summary>
	public bool __IsAvailableCursorModifier { get; set; }
	
	/// <summary>
	/// Do not touch this property, it is used for the TextEditorEditContext.
	/// </summary>
    public Dictionary<Key<TextEditorViewModel>, ResourceUri?> __ViewModelToModelResourceUriCache { get; }
    /// <summary>
	/// Do not touch this property, it is used for the TextEditorEditContext.
	/// </summary>
    public Dictionary<Key<TextEditorViewModel>, CursorModifierBagTextEditor> __CursorModifierBagCache { get; }
    /// <summary>
	/// Do not touch this property, it is used for the TextEditorEditContext.
	/// </summary>
    public Dictionary<Key<TextEditorDiffModel>, TextEditorDiffModelModifier?> __DiffModelCache { get; }
    
    /// <summary>
	/// Do not touch this property, it is used for the TextEditorEditContext.
	/// </summary>
	public List<TextEditorModel> __ModelList { get; }   
    /// <summary>
	/// Do not touch this property, it is used for the TextEditorEditContext.
	/// </summary>
    public List<TextEditorViewModel> __ViewModelList { get; }
    
    /// <summary>
	/// Do not touch this property, it is used for the 'TextEditorModel.InsertMetadata(...)' method.
	/// </summary>
    public List<LineEnd> __LocalLineEndList { get; }
    /// <summary>
	/// Do not touch this property, it is used for the 'TextEditorModel.InsertMetadata(...)' method.
	/// </summary>
    public List<int> __LocalTabPositionList { get; }
    /// <summary>
	/// Do not touch this property, it is used for the 'TextEditorModel.InsertMetadata(...)' method.
	/// </summary>
    public TextEditorViewModelLiason __TextEditorViewModelLiason { get; }
	
	public event Action? TextEditorStateChanged;
        
	/// <summmary>
	/// This method writes any mutated data within the <see cref="ITextEditorWork.EditContext"/>
	/// to the <see cref="TextEditorState"/>, and afterwards causes a UI render.
	/// </summary>
	public ValueTask FinalizePost(TextEditorEditContext editContext);
	
	public Task OpenInEditorAsync(
		TextEditorEditContext editContext,
		string absolutePath,
		bool shouldSetFocusToEditor,
		int? cursorPositionIndex,
		Category category,
		Key<TextEditorViewModel> preferredViewModelKey);
		
	public Task OpenInEditorAsync(
		TextEditorEditContext editContext,
		string absolutePath,
		bool shouldSetFocusToEditor,
		int? lineIndex,
		int? columnIndex,
		Category category,
		Key<TextEditorViewModel> preferredViewModelKey);
		
	public void RegisterModel(TextEditorEditContext editContext, TextEditorModel model);
	public void DisposeModel(TextEditorEditContext editContext, ResourceUri resourceUri);
	public void SetModel(TextEditorEditContext editContext, TextEditorModel modelModifier);
	
	public void RegisterViewModel(
	    TextEditorEditContext editContext,
	    TextEditorViewModel viewModel);
	
	public void DisposeViewModel(
		TextEditorEditContext editContext,
		Key<TextEditorViewModel> viewModelKey);
	
	public void SetModelAndViewModelRange(TextEditorEditContext editContext);
}
