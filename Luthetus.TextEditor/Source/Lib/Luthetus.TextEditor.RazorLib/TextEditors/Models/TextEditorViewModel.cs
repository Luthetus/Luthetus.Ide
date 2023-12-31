using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.TextEditor.RazorLib.Virtualizations.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.JavaScriptObjects.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

/// <summary>Stores the state of the user interface.<br/><br/>For example, the user's <see cref="TextEditorCursor"/> instances are stored here.<br/><br/>Each <see cref="TextEditorViewModel"/> has a unique underlying <see cref="TextEditorModel"/>.<br/><br/>Therefore, if one has a <see cref="TextEditorModel"/> of a text file named "myHomework.txt", then arbitrary amount of <see cref="TextEditorViewModel"/>(s) can reference that <see cref="TextEditorModel"/>.<br/><br/>For example, maybe one has a main text editor, but also a peek window open of the same underlying <see cref="TextEditorModel"/>. The main text editor is one <see cref="TextEditorViewModel"/> and the peek window is a separate <see cref="TextEditorViewModel"/>. Both of those <see cref="TextEditorViewModel"/>(s) are referencing the same <see cref="TextEditorModel"/>. Therefore typing into the peek window will also result in the main text editor re-rendering with the updated text and vice versa.</summary>
public record TextEditorViewModel : IDisposable
{
    public TextEditorViewModel(
        Key<TextEditorViewModel> viewModelKey,
        ResourceUri resourceUri,
        ITextEditorService textEditorService,
        VirtualizationResult<List<RichCharacter>> virtualizationResult,
        bool displayCommandBar)
    {
        ViewModelKey = viewModelKey;
        ResourceUri = resourceUri;
        TextEditorService = textEditorService;
        VirtualizationResult = virtualizationResult;
        DisplayCommandBar = displayCommandBar;

        var primaryCursor = new TextEditorCursor(true);

        CursorBag = new TextEditorCursor[]
        {
            primaryCursor
        }.ToImmutableArray();

        DisplayTracker = new(
            textEditorService,
            () => textEditorService.ViewModelApi.GetOrDefault(viewModelKey),
            () => textEditorService.ViewModelApi.GetModelOrDefault(viewModelKey));
    }

    private BatchScrollEvents _batchScrollEvents = new();

    public readonly object TrackingOfUniqueIdentifiersLock = new();

    /// <summary>The first entry of CursorBag should be the PrimaryCursor</summary>
    public TextEditorCursor PrimaryCursor => CursorBag.First();
    /// <summary>The first entry of CursorBag should be the PrimaryCursor</summary>
    public ImmutableArray<TextEditorCursor> CursorBag { get; init; }

    public DisplayTracker DisplayTracker { get; }

    public Key<TextEditorViewModel> ViewModelKey { get; init; }
    public ResourceUri ResourceUri { get; init; }
    public ITextEditorService TextEditorService { get; init; }
    public VirtualizationResult<List<RichCharacter>> VirtualizationResult { get; init; }
    public bool DisplayCommandBar { get; init; }
    public Action<ITextEditorModel>? OnSaveRequested { get; init; }
    public Func<TextEditorModel, string>? GetTabDisplayNameFunc { get; init; }
    /// <summary><see cref="FirstPresentationLayerKeysBag"/> is painted prior to any internal workings of the text editor.<br/><br/>Therefore the selected text background is rendered after anything in the <see cref="FirstPresentationLayerKeysBag"/>.<br/><br/>When using the <see cref="FirstPresentationLayerKeysBag"/> one might find their css overriden by for example, text being selected.</summary>
    public ImmutableList<Key<TextEditorPresentationModel>> FirstPresentationLayerKeysBag { get; init; } = ImmutableList<Key<TextEditorPresentationModel>>.Empty;
    /// <summary><see cref="LastPresentationLayerKeysBag"/> is painted after any internal workings of the text editor.<br/><br/>Therefore the selected text background is rendered before anything in the <see cref="LastPresentationLayerKeysBag"/>.<br/><br/>When using the <see cref="LastPresentationLayerKeysBag"/> one might find the selected text background not being rendered with the text selection css if it were overriden by something in the <see cref="LastPresentationLayerKeysBag"/>.</summary>
    public ImmutableList<Key<TextEditorPresentationModel>> LastPresentationLayerKeysBag { get; init; } = ImmutableList<Key<TextEditorPresentationModel>>.Empty;

    public string CommandBarValue { get; set; } = string.Empty;
    public bool ShouldSetFocusAfterNextRender { get; set; }

    public string BodyElementId => $"luth_te_text-editor-content_{ViewModelKey.Guid}";
    public string PrimaryCursorContentId => $"luth_te_text-editor-content_{ViewModelKey.Guid}_primary-cursor";
    public string GutterElementId => $"luth_te_text-editor-gutter_{ViewModelKey.Guid}";

    public void MutateScrollHorizontalPositionByPixels(double pixels)
    {
        TextEditorService.Post(nameof(MutateScrollHorizontalPositionByPixels), TextEditorService.ViewModelApi.MutateScrollHorizontalPositionFactory(
            BodyElementId,
            GutterElementId,
            pixels));
    }

    public void MutateScrollVerticalPositionByPixels(double pixels)
    {
        TextEditorService.Post(nameof(MutateScrollVerticalPositionByPixels), TextEditorService.ViewModelApi.MutateScrollVerticalPositionFactory(
            BodyElementId,
            GutterElementId,
            pixels));
    }

    public void MutateScrollVerticalPositionByPages(double pages)
    {
        MutateScrollVerticalPositionByPixels(
            pages * VirtualizationResult.TextEditorMeasurements.Height);
    }

    public void MutateScrollVerticalPositionByLines(double lines)
    {
        MutateScrollVerticalPositionByPixels(
            lines * VirtualizationResult.CharAndRowMeasurements.RowHeight);
    }

    /// <summary>If a parameter is null the JavaScript will not modify that value</summary>
    public void SetScrollPosition(double? scrollLeft, double? scrollTop)
    {
        TextEditorService.Post(nameof(SetScrollPosition), TextEditorService.ViewModelApi.SetScrollPositionFactory(
            BodyElementId,
            GutterElementId,
            scrollLeft,
            scrollTop));
    }

    public void Focus()
    {
        TextEditorService.Post(nameof(Focus),
            TextEditorService.ViewModelApi.FocusPrimaryCursorFactory(
                PrimaryCursorContentId));
    }

    public void Dispose()
    {
        DisplayTracker.Dispose();
    }
}
