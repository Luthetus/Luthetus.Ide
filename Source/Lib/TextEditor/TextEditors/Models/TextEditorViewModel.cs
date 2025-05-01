using Microsoft.JSInterop;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.Virtualizations.Models;
using Luthetus.TextEditor.RazorLib.JavaScriptObjects.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.Groups.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

/// <summary>
/// Do not mutate state on this type unless you
/// have a TextEditorEditContext.
///
/// TODO: 2 interfaces, 1 mutable one readonly?
///
/// Stores the state of the user interface. For example, the user's <see cref="TextEditorCursor"/> instances are stored here.<br/><br/>
/// 
/// Each <see cref="TextEditorViewModel"/> has a unique underlying <see cref="TextEditorModel"/>. Therefore, if one has a
/// <see cref="TextEditorModel"/> of a text file named "myHomework.txt", then arbitrary amount of <see cref="TextEditorViewModel"/>(s) can reference that <see cref="TextEditorModel"/>.<br/><br/>
/// 
/// For example, maybe one has a main text editor, but also a peek window open of the same underlying <see cref="TextEditorModel"/>.
/// The main text editor is one <see cref="TextEditorViewModel"/> and the peek window is a separate <see cref="TextEditorViewModel"/>.
/// Both of those <see cref="TextEditorViewModel"/>(s) are referencing the same <see cref="TextEditorModel"/>.
/// Therefore typing into the peek window will also result in the main text editor re-rendering with the updated text and vice versa.
/// </summary>
public sealed class TextEditorViewModel : IDisposable
{
	public TextEditorViewModel(
        Key<TextEditorViewModel> viewModelKey,
        ResourceUri resourceUri,
        ITextEditorService textEditorService,
        IPanelService panelService,
        IDialogService dialogService,
        CommonBackgroundTaskApi commonBackgroundTaskApi,
        VirtualizationGrid virtualizationResult,
		TextEditorDimensions textEditorDimensions,
		ScrollbarDimensions scrollbarDimensions,
        bool displayCommandBar,
        Category category)
    {
        ViewModelKey = viewModelKey;
        ResourceUri = resourceUri;
        TextEditorService = textEditorService;
        VirtualizationResult = virtualizationResult;
		TextEditorDimensions = textEditorDimensions;
		ScrollbarDimensions = scrollbarDimensions;
        CharAndLineMeasurements = textEditorService.OptionsApi.GetOptions().CharAndLineMeasurements;
        ShowCommandBar = displayCommandBar;
        Category = category;
        
        FirstPresentationLayerKeysList = new();
        LastPresentationLayerKeysList = new();
        
        CommandBarValue = string.Empty;
        FindOverlayValue = string.Empty;

        PrimaryCursor = new TextEditorCursor(true);

        DisplayTracker = new(
            textEditorService,
            resourceUri,
            viewModelKey);
        
        DynamicViewModelAdapter = new DynamicViewModelAdapterTextEditor(
            ViewModelKey,
            TextEditorService,
            panelService,
            dialogService,
            commonBackgroundTaskApi);
	
		BodyElementId = $"luth_te_text-editor-content_{ViewModelKey.Guid}";
	    PrimaryCursorContentId = $"luth_te_text-editor-content_{ViewModelKey.Guid}_primary-cursor";
	    GutterElementId = $"luth_te_text-editor-gutter_{ViewModelKey.Guid}";
	    FindOverlayId = $"luth_te_find-overlay_{ViewModelKey.Guid}";
	}
	
	public TextEditorViewModel(TextEditorViewModel other)
	{
	    PrimaryCursor = other.PrimaryCursor;
	    DisplayTracker = other.DisplayTracker;
	    ViewModelKey = other.ViewModelKey;
	    ResourceUri = other.ResourceUri;
	    TextEditorService = other.TextEditorService;
	    VirtualizationResult = other.VirtualizationResult;
		TextEditorDimensions = other.TextEditorDimensions;
		ScrollbarDimensions = other.ScrollbarDimensions;
	    CharAndLineMeasurements = other.CharAndLineMeasurements;
	    ShowCommandBar = other.ShowCommandBar;
	    MenuKind = other.MenuKind;
	    TooltipViewModel = other.TooltipViewModel;
	    Category = other.Category;
	    ShowFindOverlay = other.ShowFindOverlay;
	    ReplaceValueInFindOverlay = other.ReplaceValueInFindOverlay;
	    ShowReplaceButtonInFindOverlay = other.ShowReplaceButtonInFindOverlay;
	    OnSaveRequested = other.OnSaveRequested;
	    GetTabDisplayNameFunc = other.GetTabDisplayNameFunc;
	    FirstPresentationLayerKeysList = other.FirstPresentationLayerKeysList;
	    LastPresentationLayerKeysList = other.LastPresentationLayerKeysList;
	    CommandBarValue = other.CommandBarValue;
	    FindOverlayValue = other.FindOverlayValue;
	    FindOverlayValueExternallyChangedMarker = other.FindOverlayValueExternallyChangedMarker;
	    ShouldSetFocusAfterNextRender = other.ShouldSetFocusAfterNextRender;
	    ShouldRevealCursor = other.ShouldRevealCursor;
	    CursorIsIntersecting = other.CursorIsIntersecting;
	    DynamicViewModelAdapter = other.DynamicViewModelAdapter;
		AllCollapsePointList = other.AllCollapsePointList;
		VirtualizedCollapsePointList = other.VirtualizedCollapsePointList;
		HiddenLineIndexHashSet = other.HiddenLineIndexHashSet;
		InlineUiList = other.InlineUiList;
		UiOutdated = other.UiOutdated;
	    
	    BodyElementId = other.BodyElementId;
	    PrimaryCursorContentId = other.PrimaryCursorContentId;
	    GutterElementId = other.GutterElementId;
	    FindOverlayId = other.FindOverlayId;
	    
	    /*
	    // Don't copy these properties
	    ScrollWasModified { get; set; }
	    ShouldReloadVirtualizationResult { get; set; }
	    */
	}

    public TextEditorCursor PrimaryCursor { get; set; }
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
    /// Given the dimensions of the rendered text editor, this provides a subset of the file's content, such that "only what is
    /// visible when rendered" is in this. There is some padding of offscreen content so that scrolling is smoother.
    /// </summary>
    public VirtualizationGrid VirtualizationResult { get; set; }
	public TextEditorDimensions TextEditorDimensions { get; set; }
	public ScrollbarDimensions ScrollbarDimensions { get; set; }
	/// <summary>
	/// TODO: Rename 'CharAndLineMeasurements' to 'CharAndLineDimensions'...
	///       ...as to bring it inline with 'TextEditorDimensions' and 'ScrollbarDimensions'.
	/// </summary>
    public CharAndLineMeasurements CharAndLineMeasurements { get; set; }
    /// <summary>
    /// The command bar is referring to the <see cref="Keymaps.Models.Vims.TextEditorKeymapVim"/>.
    /// </summary>
    public bool ShowCommandBar { get; set; }
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
    /// <summary>
    /// <inheritdoc cref="Models.Category"/>
    /// </summary>
    public Category Category { get; set; }
    /// <summary>
    /// The find overlay refers to hitting the keymap { Ctrl + f } when browser focus is within a text editor.
    /// </summary>
    public bool ShowFindOverlay { get; set; }
    public bool ShowReplaceButtonInFindOverlay { get; set; }
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
    /// The command bar is referring to the <see cref="Keymaps.Models.Vims.TextEditorKeymapVim"/>.
    /// In the command line program "Vim" one can hit 'colon' to bring up a command bar.
    /// This property is what the input element binds to
    /// </summary>
    public string CommandBarValue { get; set; }
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
    /// If one opens a file with the 'Enter' key, they might want focus to then be set on that
    /// newly opened file. However, perhaps one wants the 'Space' key to also open the file,
    /// but not set focus to it.
    /// </summary>
    public bool ShouldSetFocusAfterNextRender { get; set; }
    public bool ShouldRevealCursor { get; set; }
    /// <summary>
    /// Relates to whether the cursor is within the viewable area of the Text Editor on the UI
    /// </summary>
    public bool CursorIsIntersecting { get; set; }
    /// <summary>
    /// This property contains all data, and logic, necessary to render a text editor from within a dialog,
    /// a panel tab, or a text editor group tab.
    /// </summary>
    public DynamicViewModelAdapterTextEditor DynamicViewModelAdapter { get; set; }
    public List<CollapsePoint> AllCollapsePointList { get; set; } = new();
    /// <summary>
    /// TODO: This does not belong here move this to the 'TextEditorViewModelSlimDisplay.razor'.
    /// </summary>
    public List<CollapsePoint> VirtualizedCollapsePointList { get; set; } = new();
    public HashSet<int> HiddenLineIndexHashSet { get; set; } = new();
    /// <summary>
    /// For the time being, this similarly named list on the TextEditorModel will be added
    /// to alongside this one of the viewmodel.
    ///
    /// By adding to the model, the model's tab positioning logic can "just work" for the
    /// 3 dots thing.
    ///
    /// But, the 3 dots thing shouldn't be on the model long term since it is tied to
    /// the collapse/expand state and it was decided that
    /// separate viewmodels that reference the same underlying model
    /// should have different collapse/expand states.
    /// </summary>
    public List<(InlineUi InlineUi, string Tag)> InlineUiList { get; set; } = new();
    public bool UiOutdated { get; set; }
    
    public bool ScrollWasModified { get; set; }
	/// <summary>
	/// This property decides whether or not to re-calculate the virtualization result that gets displayed on the UI.
	/// </summary>
    public bool ShouldReloadVirtualizationResult { get; set; }

	public string BodyElementId { get; }
    public string PrimaryCursorContentId { get; }
    public string GutterElementId { get; }
    public string FindOverlayId { get; }

    public ValueTask FocusAsync()
    {
        return TextEditorService.ViewModelApi.FocusPrimaryCursorAsync(PrimaryCursorContentId);
    }

    public void Dispose()
    {
        DisplayTracker.Dispose();
    }
}
