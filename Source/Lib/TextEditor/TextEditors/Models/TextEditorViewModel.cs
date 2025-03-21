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
public sealed record TextEditorViewModel : IDisposable
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
        CharAndLineMeasurements charAndLineMeasurements,
        bool displayCommandBar,
        Category category)
    {
        ViewModelKey = viewModelKey;
        ResourceUri = resourceUri;
        TextEditorService = textEditorService;
        VirtualizationResult = virtualizationResult;
		TextEditorDimensions = textEditorDimensions;
		ScrollbarDimensions = scrollbarDimensions;
        CharAndLineMeasurements = charAndLineMeasurements;
        ShowCommandBar = displayCommandBar;
        Category = category;

        var primaryCursor = new TextEditorCursor(true);

        CursorList = new List<TextEditorCursor>()
        {
            primaryCursor
        };

        DisplayTracker = new(
            textEditorService,
            resourceUri,
            viewModelKey);

        UnsafeState = new();
        
        DynamicViewModelAdapter = new DynamicViewModelAdapterTextEditor(
            ViewModelKey,
            TextEditorService,
            panelService,
            dialogService,
            commonBackgroundTaskApi);
	}

    /// <summary>
    /// The first entry of <see cref="CursorList"/> should be the PrimaryCursor
    /// </summary>
    public TextEditorCursor PrimaryCursor => CursorList.First();
    /// <summary>
    /// The first entry of <see cref="CursorList"/> should be the PrimaryCursor
    /// </summary>
    public List<TextEditorCursor> CursorList { get; init; }
    /// <summary>
    /// This tracks which view models are actively rendered from Blazor's perspective. Thus, using this allows lazy recalculation
    /// of view model state when an underlying model changes.
    /// </summary>
    public DisplayTracker DisplayTracker { get; }
    /// <summary>
    /// The main unique identifier for a <see cref="TextEditorViewModel"/>, used in many API.
    /// </summary>
    public Key<TextEditorViewModel> ViewModelKey { get; init; }
    /// <summary>
    /// The unique identifier for a <see cref="TextEditorModel"/>. The model is to say a representation of the file on a filesystem.
    /// The contents and such. Whereas the viewmodel is to track state regarding a rendered editor for that file, for example the cursor position.
    /// </summary>
    public ResourceUri ResourceUri { get; init; }
    /// <summary>
    /// Most API invocation (if not all) occurs through the <see cref="ITextEditorService"/>
    /// </summary>
    public ITextEditorService TextEditorService { get; init; }
    /// <summary>
    /// Given the dimensions of the rendered text editor, this provides a subset of the file's content, such that "only what is
    /// visible when rendered" is in this. There is some padding of offscreen content so that scrolling is smoother.
    /// </summary>
    public VirtualizationGrid VirtualizationResult { get; init; }
	public TextEditorDimensions TextEditorDimensions { get; init; }
	public ScrollbarDimensions ScrollbarDimensions { get; init; }
	/// <summary>
	/// TODO: Rename 'CharAndLineMeasurements' to 'CharAndLineDimensions'...
	///       ...as to bring it inline with 'TextEditorDimensions' and 'ScrollbarDimensions'.
	/// </summary>
    public CharAndLineMeasurements CharAndLineMeasurements { get; init; }
    /// <summary>
    /// The command bar is referring to the <see cref="Keymaps.Models.Vims.TextEditorKeymapVim"/>.
    /// </summary>
    public bool ShowCommandBar { get; init; }
	/// <summary>
	/// This property determines the menu that is shown in the text editor.
	///
	/// For example, when this property is <see cref="MenuKind.AutoCompleteMenu"/>,
	/// then the autocomplete menu is displayed in the text editor.
	/// </summary>
    public MenuKind MenuKind { get; init; }
	/// <summary>
	/// This property determines the tooltip that is shown in the text editor.
	/// </summary>
    public TooltipViewModel? TooltipViewModel { get; init; }
    /// <summary>
    /// <inheritdoc cref="Models.Category"/>
    /// </summary>
    public Category Category { get; }
    /// <summary>
    /// The find overlay refers to hitting the keymap { Ctrl + f } when browser focus is within a text editor.
    /// </summary>
    public bool ShowFindOverlay { get; init; }
    /// <summary>
    /// If one hits the keymap { Ctrl + s } when browser focus is within a text editor.
    /// </summary>
    public Action<ITextEditorModel>? OnSaveRequested { get; init; }
    /// <summary>
    /// When a view model is rendered within a <see cref="TextEditorGroup"/>, this Func can be used to render a more friendly tab name, than the resource uri path.
    /// </summary>
    public Func<TextEditorModel, string>? GetTabDisplayNameFunc { get; init; }
    /// <summary>
    /// <see cref="FirstPresentationLayerKeysList"/> is painted prior to any internal workings of the text editor.<br/><br/>
    /// Therefore the selected text background is rendered after anything in the <see cref="FirstPresentationLayerKeysList"/>.<br/><br/>
    /// When using the <see cref="FirstPresentationLayerKeysList"/> one might find their css overriden by for example, text being selected.
    /// </summary>
    public List<Key<TextEditorPresentationModel>> FirstPresentationLayerKeysList { get; init; } = new();
    /// <summary>
    /// <see cref="LastPresentationLayerKeysList"/> is painted after any internal workings of the text editor.<br/><br/>
    /// Therefore the selected text background is rendered before anything in the <see cref="LastPresentationLayerKeysList"/>.<br/><br/>
    /// When using the <see cref="LastPresentationLayerKeysList"/> one might find the selected text background
    /// not being rendered with the text selection css if it were overriden by something in the <see cref="LastPresentationLayerKeysList"/>.
    /// </summary>
    public List<Key<TextEditorPresentationModel>> LastPresentationLayerKeysList { get; init; } = new();
    /// <summary>
    /// The command bar is referring to the <see cref="Keymaps.Models.Vims.TextEditorKeymapVim"/>.
    /// In the command line program "Vim" one can hit 'colon' to bring up a command bar.
    /// This property is what the input element binds to
    /// </summary>
    public string CommandBarValue { get; set; } = string.Empty;
    /// <summary>
    /// The find overlay refers to hitting the keymap { Ctrl + f } when browser focus is within a text editor.
    /// This property is what the find overlay input element binds to.
    /// </summary>
    public string FindOverlayValue { get; set; } = string.Empty;
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
    public bool FindOverlayValueExternallyChangedMarker { get; init; }
	/// <inheritdoc cref="ViewModelUnsafeState"/>
    public ViewModelUnsafeState UnsafeState { get; }
    /// <summary>
    /// This property contains all data, and logic, necessary to render a text editor from within a dialog,
    /// a panel tab, or a text editor group tab.
    /// </summary>
    public DynamicViewModelAdapterTextEditor DynamicViewModelAdapter { get; init; }
	/// <summary>
	/// This property is intended to be used for displaying 'code lens' comments.
	/// For example, above a property perhaps the text "3 references".
	/// </summary>
    public List<WidgetBlock> WidgetBlockList { get; init; } = null;
    /// <summary>
    /// This property is intended to be used for displaying 'inline hints'.
    /// For example, if the type of a lambda's parameter is not deemed obvious,
    /// one could inline the parameter's type. This inline hint wouldn't be actual text in the document.
    /// </summary>
    public List<WidgetInline> WidgetInlineList { get; init; } = null;
    public List<WidgetOverlay> WidgetOverlayList { get; init; } = null;

	public string BodyElementId => $"luth_te_text-editor-content_{ViewModelKey.Guid}";
    public string PrimaryCursorContentId => $"luth_te_text-editor-content_{ViewModelKey.Guid}_primary-cursor";
    public string GutterElementId => $"luth_te_text-editor-gutter_{ViewModelKey.Guid}";
    public string FindOverlayId => $"luth_te_find-overlay_{ViewModelKey.Guid}";

    public ValueTask FocusAsync()
    {
        return TextEditorService.ViewModelApi.FocusPrimaryCursorAsync(PrimaryCursorContentId);
    }

    public void Dispose()
    {
        DisplayTracker.Dispose();
    }
}
