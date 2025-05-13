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
///
/// Do not use object initializers because the cloning of the TextEditorViewModel
/// will redundantly take time to perform the object initialization.
/// Instead, add "object initialization" to the constructor of the original instance.
/// </summary>
public sealed class TextEditorViewModel : IDisposable
{
	public TextEditorViewModel(
        Key<TextEditorViewModel> viewModelKey,
        ResourceUri resourceUri,
        TextEditorService textEditorService,
        IPanelService panelService,
        IDialogService dialogService,
        CommonBackgroundTaskApi commonBackgroundTaskApi,
        VirtualizationGrid virtualizationResult,
		TextEditorDimensions textEditorDimensions,
		ScrollbarDimensions scrollbarDimensions,
		Category category)
    {
    	PersistentState = new TextEditorViewModelPersistentState(
			new DisplayTracker(
	            textEditorService,
	            resourceUri,
	            viewModelKey),
		    viewModelKey,
		    resourceUri,
		    textEditorService,
		    category,
		    onSaveRequested: null,
		    getTabDisplayNameFunc: null,
		    firstPresentationLayerKeysList: new(),
		    lastPresentationLayerKeysList: new(),
		    new DynamicViewModelAdapterTextEditor(
	            viewModelKey,
	            textEditorService,
	            panelService,
	            dialogService,
	            commonBackgroundTaskApi),
		    bodyElementId: $"luth_te_text-editor-content_{viewModelKey.Guid}",
		    primaryCursorContentId: $"luth_te_text-editor-content_{viewModelKey.Guid}_primary-cursor",
		    gutterElementId: $"luth_te_text-editor-gutter_{viewModelKey.Guid}",
		    findOverlayId: $"luth_te_find-overlay_{viewModelKey.Guid}",
		    showFindOverlay: false,
		    replaceValueInFindOverlay: string.Empty,
		    showReplaceButtonInFindOverlay: false,
		    findOverlayValue: string.Empty,
		    findOverlayValueExternallyChangedMarker: false,
		    menuKind: MenuKind.None,
	    	tooltipViewModel: null,
		    shouldRevealCursor: false,
			virtualAssociativityKind: VirtualAssociativityKind.None);
    
        VirtualizationResult = virtualizationResult;
		TextEditorDimensions = textEditorDimensions;
		ScrollbarDimensions = scrollbarDimensions;
        CharAndLineMeasurements = textEditorService.OptionsApi.GetOptions().CharAndLineMeasurements;
        
        PrimaryCursor = new TextEditorCursor(true);
        
        AllCollapsePointList = new();
		VirtualizedCollapsePointList = new();
		HiddenLineIndexHashSet = new();
		InlineUiList = new();
	}
	
	public TextEditorViewModel(TextEditorViewModel other)
	{
		PersistentState = other.PersistentState;
		
	    PrimaryCursor = other.PrimaryCursor;
	    VirtualizationResult = other.VirtualizationResult;
		TextEditorDimensions = other.TextEditorDimensions;
		ScrollbarDimensions = other.ScrollbarDimensions;
	    CharAndLineMeasurements = other.CharAndLineMeasurements;
		
		CreateCacheWasInvoked = other.CreateCacheWasInvoked;
		
		AllCollapsePointList = other.AllCollapsePointList;
		VirtualizedCollapsePointList = other.VirtualizedCollapsePointList;
		HiddenLineIndexHashSet = other.HiddenLineIndexHashSet;
		InlineUiList = other.InlineUiList;
	    
	    /*
	    // Don't copy these properties
	    ScrollWasModified { get; set; }
	    ShouldReloadVirtualizationResult { get; set; }
	    */
	}
	
	public TextEditorViewModelPersistentState PersistentState { get; set; }

    public TextEditorCursor PrimaryCursor { get; set; }
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
	
    public bool CreateCacheWasInvoked { get; set; }
    
    public bool ScrollWasModified { get; set; }
	/// <summary>
	/// This property decides whether or not to re-calculate the virtualization result that gets displayed on the UI.
	/// </summary>
    public bool ShouldReloadVirtualizationResult { get; set; }
    
    public List<CollapsePoint> AllCollapsePointList { get; set; }
	public List<CollapsePoint> VirtualizedCollapsePointList { get; set; }
	public bool HiddenLineIndexHashSetIsShallowCopy { get; set; }
	public HashSet<int> HiddenLineIndexHashSet { get; set; }
	public List<(InlineUi InlineUi, string Tag)> InlineUiList { get; set; }

    public ValueTask FocusAsync()
    {
        return PersistentState.TextEditorService.ViewModelApi.FocusPrimaryCursorAsync(PersistentState.PrimaryCursorContentId);
    }
    
    public void ApplyCollapsePointState(TextEditorEditContext editContext)
    {
    	HiddenLineIndexHashSet = new();
    	HiddenLineIndexHashSetIsShallowCopy = true;
    	
    	foreach (var collapsePoint in AllCollapsePointList)
		{
			if (!collapsePoint.IsCollapsed)
				continue;
			var firstToHideLineIndex = collapsePoint.AppendToLineIndex + 1;
			for (var lineOffset = 0; lineOffset < collapsePoint.EndExclusiveLineIndex - collapsePoint.AppendToLineIndex - 1; lineOffset++)
			{
				HiddenLineIndexHashSet.Add(firstToHideLineIndex + lineOffset);
			}
		}
    }

    public void Dispose()
    {
        PersistentState.DisplayTracker.Dispose();
    }
}
