using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

/// <summary>
/// This type reduces the amount of properties that need to be copied from one TextEditorViewModel instance to another
/// by chosing to have some of the state shared between instances.
/// </summary>
public class TextEditorViewModelPersistentState
{
	public TextEditorViewModelPersistentState(
		DisplayTracker displayTracker,
	    Key<TextEditorViewModel> viewModelKey,
	    ResourceUri resourceUri,
	    ITextEditorService textEditorService,
	    Category category,
	    Action<TextEditorModel>? onSaveRequested,
	    Func<TextEditorModel, string>? getTabDisplayNameFunc,
	    List<Key<TextEditorPresentationModel>> firstPresentationLayerKeysList,
	    List<Key<TextEditorPresentationModel>> lastPresentationLayerKeysList,
	    DynamicViewModelAdapterTextEditor dynamicViewModelAdapter,
	    string bodyElementId,
	    string primaryCursorContentId,
	    string gutterElementId,
	    string findOverlayId)
	{
		DisplayTracker = displayTracker;
	    ViewModelKey = viewModelKey;
	    ResourceUri = resourceUri;
	    TextEditorService = textEditorService;
	    Category = category;
	    OnSaveRequested = onSaveRequested;
	    GetTabDisplayNameFunc = getTabDisplayNameFunc;
	    FirstPresentationLayerKeysList = firstPresentationLayerKeysList;
	    LastPresentationLayerKeysList = lastPresentationLayerKeysList;
	    DynamicViewModelAdapter = dynamicViewModelAdapter;
	    BodyElementId = bodyElementId;
	    PrimaryCursorContentId = primaryCursorContentId;
	    GutterElementId = gutterElementId;
	    FindOverlayId = findOverlayId;
	}
	
    
    /// <summary>
	/// This tracks which view models are actively rendered from Blazor's perspective. Thus, using this allows lazy recalculation
	/// of view model state when an underlying model changes.
	/// </summary>
	public DisplayTracker DisplayTracker { get; }
	/// <summary>
	/// The main unique identifier for a <see cref="TextEditorViewModel"/>, used in many API.
	/// </summary>
	public Key<TextEditorViewModel> ViewModelKey { get; set; }
	/// <summary>
	/// The unique identifier for a <see cref="TextEditorModel"/>. The model is to say a representation of the file on a filesystem.
	/// The contents and such. Whereas the viewmodel is to track state regarding a rendered editor for that file, for example the cursor position.
	/// </summary>
	public ResourceUri ResourceUri { get; set; }
	/// <summary>
	/// Most API invocation (if not all) occurs through the <see cref="ITextEditorService"/>
	/// </summary>
	public ITextEditorService TextEditorService { get; set; }
	/// <summary>
	/// <inheritdoc cref="Models.Category"/>
	/// </summary>
	public Category Category { get; set; }
	/// <summary>
	/// If one hits the keymap { Ctrl + s } when browser focus is within a text editor.
	/// </summary>
	public Action<TextEditorModel>? OnSaveRequested { get; set; }
	/// <summary>
	/// When a view model is rendered within a <see cref="TextEditorGroup"/>, this Func can be used to render a more friendly tab name, than the resource uri path.
	/// </summary>
	public Func<TextEditorModel, string>? GetTabDisplayNameFunc { get; set; }
	/// <summary>
	/// <see cref="FirstPresentationLayerKeysList"/> is painted prior to any internal workings of the text editor.<br/><br/>
	/// Therefore the selected text background is rendered after anything in the <see cref="FirstPresentationLayerKeysList"/>.<br/><br/>
	/// When using the <see cref="FirstPresentationLayerKeysList"/> one might find their css overriden by for example, text being selected.
	/// </summary>
	public List<Key<TextEditorPresentationModel>> FirstPresentationLayerKeysList { get; set; }
	/// <summary>
	/// <see cref="LastPresentationLayerKeysList"/> is painted after any internal workings of the text editor.<br/><br/>
	/// Therefore the selected text background is rendered before anything in the <see cref="LastPresentationLayerKeysList"/>.<br/><br/>
	/// When using the <see cref="LastPresentationLayerKeysList"/> one might find the selected text background
	/// not being rendered with the text selection css if it were overriden by something in the <see cref="LastPresentationLayerKeysList"/>.
	/// </summary>
	public List<Key<TextEditorPresentationModel>> LastPresentationLayerKeysList { get; set; }
	/// <summary>
	/// This property contains all data, and logic, necessary to render a text editor from within a dialog,
	/// a panel tab, or a text editor group tab.
	/// </summary>
	public DynamicViewModelAdapterTextEditor DynamicViewModelAdapter { get; set; }
	public string BodyElementId { get; }
	public string PrimaryCursorContentId { get; }
	public string GutterElementId { get; }
	public string FindOverlayId { get; }
}
