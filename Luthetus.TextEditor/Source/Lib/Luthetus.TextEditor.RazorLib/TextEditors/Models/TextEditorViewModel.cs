using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Virtualizations.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

/// <summary>
/// Stores the state of the user interface.
/// <br/><br/>
/// For example, the user's <see cref="TextEditorCursor"/> instances are stored here.
/// <br/><br/>
/// Each <see cref="TextEditorViewModel"/> has a unique underlying <see cref="TextEditorModel"/>.
/// <br/><br/>
/// Therefore, if one has a <see cref="TextEditorModel"/> of a text file named "myHomework.txt", then arbitrary amount of <see cref="TextEditorViewModel"/>(s) can reference that <see cref="TextEditorModel"/>.
/// <br/><br/>
/// For example, maybe one has a main text editor, but also a peek window open of the same underlying
/// <see cref="TextEditorModel"/>. The main text editor is one <see cref="TextEditorViewModel"/> and the peek window is a separate <see cref="TextEditorViewModel"/>.
/// Both of those <see cref="TextEditorViewModel"/>(s) are referencing the same <see cref="TextEditorModel"/>. Therefore typing into the peek window will also result in the main
/// text editor re-rendering with the updated text and vice versa.</summary>
public record TextEditorViewModel : IDisposable
{
    public TextEditorViewModel(
        Key<TextEditorViewModel> viewModelKey,
        ResourceUri resourceUri,
        ITextEditorService textEditorService,
        VirtualizationResult<List<RichCharacter>> virtualizationResult,
        bool displayCommandBar,
        TextEditorCategory category)
    {
        ViewModelKey = viewModelKey;
        ResourceUri = resourceUri;
        TextEditorService = textEditorService;
        VirtualizationResult = virtualizationResult;
        ShowCommandBar = displayCommandBar;
        Category = category;

        var primaryCursor = new TextEditorCursor(true);

        CursorList = new TextEditorCursor[]
        {
            primaryCursor
        }.ToImmutableArray();

        DisplayTracker = new(
            textEditorService,
            () => textEditorService.ViewModelApi.GetOrDefault(viewModelKey),
            () => textEditorService.ViewModelApi.GetModelOrDefault(viewModelKey));

        UnsafeState = new();
    }


    /// <summary>
    /// The first entry of <see cref="CursorList"/> should be the PrimaryCursor
    /// </summary>
    public TextEditorCursor PrimaryCursor => CursorList.First();
    /// <summary>
    /// The first entry of <see cref="CursorList"/> should be the PrimaryCursor
    /// </summary>
    public ImmutableArray<TextEditorCursor> CursorList { get; init; }

    /// <summary>
    /// This tracks which view models are actively rendered from Blazor's perspective.
    /// Thus, using this allows lazy recalculation of view model state when an underlying model changes.
    /// That is to say, if a view model isn't actively rendered, then do not re-calculate its state
    /// until it becomes rendered.
    /// </summary>
    public DisplayTracker DisplayTracker { get; }

    /// <summary>
    /// The main unique identifier for a <see cref="TextEditorViewModel"/>, used in many API.
    /// </summary>
    public Key<TextEditorViewModel> ViewModelKey { get; init; }
    /// <summary>
    /// The unique identifier for a <see cref="TextEditorModel"/>. The model is to say
    /// a representation of the file on a filesystem. The contents and such. Whereas
    /// the viewmodel is to track state regarding a rendered editor for that file,
    /// for example the cursor position.
    /// </summary>
    public ResourceUri ResourceUri { get; init; }
    /// <summary>
    /// Most API invocation (if not all) occurs through the <see cref="ITextEditorService"/>
    /// </summary>
    public ITextEditorService TextEditorService { get; init; }
    /// <summary>
    /// Given the dimensions of the rendered text editor, this provides a subset of the
    /// file's content, such that "only what is visible when rendered" is in this.
    /// There is some padding added so that scrolling looks a bit more natural however,
    /// so some content offscreen does get included here for a smoother experience.
    /// </summary>
    public VirtualizationResult<List<RichCharacter>> VirtualizationResult { get; init; }
    /// <summary>
    /// The command bar is referring to the <see cref="Keymaps.Models.Vims.TextEditorKeymapVim"/>.
    /// In the command line program "Vim" one can hit 'colon' to bring up a command bar.
    /// </summary>
    public bool ShowCommandBar { get; init; }
    /// <summary>
    /// <inheritdoc cref="Models.TextEditorCategory"/>
    /// </summary>
    public TextEditorCategory Category { get; }
    /// <summary>
    /// The find overlay refers to hitting the keymap { Ctrl + f } when browser focus is within a text editor.
    /// </summary>
    public bool ShowFindOverlay { get; init; }
    /// <summary>
    /// If one hits the keymap { Ctrl + s } when browser focus is within a text editor.
    /// </summary>
    public Action<ITextEditorModel>? OnSaveRequested { get; init; }
    /// <summary>
    /// When a view model is rendered within a <see cref="Groups.Models.TextEditorGroup"/>,
    /// this func can be used to rener a more friendly tab display name.
    /// </summary>
    public Func<TextEditorModel, string>? GetTabDisplayNameFunc { get; init; }
    /// <summary>
    /// <see cref="FirstPresentationLayerKeysList"/> is painted prior to any internal workings of the text editor.
    /// <br/><br/>
    /// Therefore the selected text background is rendered after anything in the <see cref="FirstPresentationLayerKeysList"/>.
    /// <br/><br/>
    /// When using the <see cref="FirstPresentationLayerKeysList"/> one might find their css overriden by for example, text being selected.
    /// </summary>
    public ImmutableList<Key<TextEditorPresentationModel>> FirstPresentationLayerKeysList { get; init; } = ImmutableList<Key<TextEditorPresentationModel>>.Empty;
    /// <summary>
    /// <see cref="LastPresentationLayerKeysList"/> is painted after any internal workings of the text editor.
    /// <br/><br/>
    /// Therefore the selected text background is rendered before anything in the <see cref="LastPresentationLayerKeysList"/>.
    /// <br/><br/>
    /// When using the <see cref="LastPresentationLayerKeysList"/> one might find the selected text background
    /// not being rendered with the text selection css if it were overriden by something in the <see cref="LastPresentationLayerKeysList"/>.
    /// </summary>
    public ImmutableList<Key<TextEditorPresentationModel>> LastPresentationLayerKeysList { get; init; } = ImmutableList<Key<TextEditorPresentationModel>>.Empty;

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
    public TextEditorViewModelUnsafeState UnsafeState { get; }

    public string BodyElementId => $"luth_te_text-editor-content_{ViewModelKey.Guid}";
    public string PrimaryCursorContentId => $"luth_te_text-editor-content_{ViewModelKey.Guid}_primary-cursor";
    public string GutterElementId => $"luth_te_text-editor-gutter_{ViewModelKey.Guid}";
    public string FindOverlayId => $"luth_te_find-overlay_{ViewModelKey.Guid}";

    public TextEditorEdit MutateScrollHorizontalPositionByPixelsFactory(double pixels)
    {
        return TextEditorService.ViewModelApi.MutateScrollHorizontalPositionFactory(
            BodyElementId,
            GutterElementId,
            pixels);
    }

    public TextEditorEdit MutateScrollVerticalPositionByPixelsFactory(double pixels)
    {
        return TextEditorService.ViewModelApi.MutateScrollVerticalPositionFactory(
            BodyElementId,
            GutterElementId,
            pixels);
    }

    public TextEditorEdit MutateScrollVerticalPositionByPagesFactory(double pages)
    {
        return MutateScrollVerticalPositionByPixelsFactory(
            pages * VirtualizationResult.TextEditorMeasurements.Height);
    }

    public TextEditorEdit MutateScrollVerticalPositionByLinesFactory(double lines)
    {
        return MutateScrollVerticalPositionByPixelsFactory(
            lines * VirtualizationResult.CharAndRowMeasurements.RowHeight);
    }

    /// <summary>If a parameter is null the JavaScript will not modify that value</summary>
    public TextEditorEdit SetScrollPositionFactory(double? scrollLeft, double? scrollTop)
    {
        return TextEditorService.ViewModelApi.SetScrollPositionFactory(
	        BodyElementId,
	        GutterElementId,
	        scrollLeft,
	        scrollTop);
    }

    public TextEditorEdit FocusFactory()
    {
        return TextEditorService.ViewModelApi.FocusPrimaryCursorFactory(
        	PrimaryCursorContentId);
    }

    public void Dispose()
    {
        DisplayTracker.Dispose();
    }
}
