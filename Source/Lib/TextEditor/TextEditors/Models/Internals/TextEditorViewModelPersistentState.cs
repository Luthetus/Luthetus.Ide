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
	    TextEditorService textEditorService,
	    Category category,
	    Action<TextEditorModel>? onSaveRequested,
	    Func<TextEditorModel, string>? getTabDisplayNameFunc,
	    List<Key<TextEditorPresentationModel>> firstPresentationLayerKeysList,
	    List<Key<TextEditorPresentationModel>> lastPresentationLayerKeysList,
	    DynamicViewModelAdapterTextEditor dynamicViewModelAdapter,
	    string bodyElementId,
	    string primaryCursorContentId,
	    string gutterElementId,
	    string findOverlayId,
	    bool showFindOverlay,
	    string replaceValueInFindOverlay,
	    bool showReplaceButtonInFindOverlay,
	    string findOverlayValue,
	    bool findOverlayValueExternallyChangedMarker,
	    MenuKind menuKind,
	    TooltipViewModel tooltipViewModel,
	    bool shouldRevealCursor,
		VirtualAssociativityKind virtualAssociativityKind)
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
	    
	    ShowFindOverlay = showFindOverlay;
	    ReplaceValueInFindOverlay = replaceValueInFindOverlay;
	    ShowReplaceButtonInFindOverlay = showReplaceButtonInFindOverlay;
	    FindOverlayValue = findOverlayValue;
	    FindOverlayValueExternallyChangedMarker = findOverlayValueExternallyChangedMarker;
	    
	    MenuKind = menuKind;
	    TooltipViewModel = tooltipViewModel;

	    ShouldRevealCursor = shouldRevealCursor;
		VirtualAssociativityKind = virtualAssociativityKind;
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
	public TextEditorService TextEditorService { get; set; }
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
	
	/// <summary>
    /// The find overlay refers to hitting the keymap { Ctrl + f } when browser focus is within a text editor.
    /// </summary>
    public bool ShowFindOverlay { get; set; }
    public bool ShowReplaceButtonInFindOverlay { get; set; }
    /// <summary>
    /// The find overlay refers to hitting the keymap { Ctrl + f } when browser focus is within a text editor.
    /// This property is what the find overlay input element binds to.
    /// </summary>
    public string FindOverlayValue { get; set; }
    /// <summary>
    /// If the user presses the keybind to show the FindOverlayDisplay while focused on the Text Editor,
    /// check if the user has a text selection.
    ///
    /// If they do have a text selection, then populate the FindOverlayDisplay with their selection.
    ///
    /// The issue arises however, how does one know whether FindOverlayValue changed due to
    /// the input element itself being typed into, versus some 'background action'.
    ///
    /// Because the UI already will update properly if the input element itself is interacted with.
    ///
    /// We only need to solve the case where it was a 'background action'.
    ///
    /// So, if this bool toggles to a different value than what the UI last saw,
    /// then the UI is to set the input element's value equal to the 'FindOverlayValue'
    /// because a 'background action' modified the value.
    /// </summary>
    public bool FindOverlayValueExternallyChangedMarker { get; set; }
    public string ReplaceValueInFindOverlay { get; set; }
    
    /// <summary>
	/// This property determines the menu that is shown in the text editor.
	///
	/// For example, when this property is <see cref="MenuKind.AutoCompleteMenu"/>,
	/// then the autocomplete menu is displayed in the text editor.
	/// </summary>
    public MenuKind MenuKind { get; set; }
	/// <summary>
	/// This property determines the tooltip that is shown in the text editor.
	/// </summary>
    public TooltipViewModel? TooltipViewModel { get; set; }
    
    public bool ShouldRevealCursor { get; set; }
    public VirtualAssociativityKind VirtualAssociativityKind { get; set; } = VirtualAssociativityKind.None;
}
