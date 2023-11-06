using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.TextEditor.RazorLib.Options.Models;
using Luthetus.TextEditor.RazorLib.Virtualizations.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.JavaScriptObjects.Models;

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

        DisplayTracker = new(
            () => textEditorService.ViewModel.FindOrDefault(viewModelKey),
            () => textEditorService.ViewModel.FindBackingModelOrDefault(viewModelKey));
    }

    private const int _clearTrackingOfUniqueIdentifiersWhenCountIs = 250;

    private readonly object _trackingOfUniqueIdentifiersLock = new();

    private TextEditorMeasurements _mostRecentTextEditorMeasurements = new(0, 0, 0, 0, 0, 0, 0, CancellationToken.None);
    private BatchScrollEvents _batchScrollEvents = new();

    public IThrottle ThrottleRemeasure { get; } = new Throttle(IThrottle.DefaultThrottleTimeSpan);
    public IThrottle ThrottleCalculateVirtualizationResult { get; } = new Throttle(IThrottle.DefaultThrottleTimeSpan);

    public TextEditorCursor PrimaryCursor { get; } = new(true);
    public DisplayTracker DisplayTracker { get; }

    public Key<TextEditorViewModel> ViewModelKey { get; init; }
    public ResourceUri ResourceUri { get; init; }
    public ITextEditorService TextEditorService { get; init; }
    public VirtualizationResult<List<RichCharacter>> VirtualizationResult { get; init; }
    public bool DisplayCommandBar { get; init; }
    public Action<TextEditorModel>? OnSaveRequested { get; init; }
    public Func<TextEditorModel, string>? GetTabDisplayNameFunc { get; init; }
    /// <summary><see cref="FirstPresentationLayerKeysBag"/> is painted prior to any internal workings of the text editor.<br/><br/>Therefore the selected text background is rendered after anything in the <see cref="FirstPresentationLayerKeysBag"/>.<br/><br/>When using the <see cref="FirstPresentationLayerKeysBag"/> one might find their css overriden by for example, text being selected.</summary>
    public ImmutableList<Key<TextEditorPresentationModel>> FirstPresentationLayerKeysBag { get; init; } = ImmutableList<Key<TextEditorPresentationModel>>.Empty;
    /// <summary><see cref="LastPresentationLayerKeysBag"/> is painted after any internal workings of the text editor.<br/><br/>Therefore the selected text background is rendered before anything in the <see cref="LastPresentationLayerKeysBag"/>.<br/><br/>When using the <see cref="LastPresentationLayerKeysBag"/> one might find the selected text background not being rendered with the text selection css if it were overriden by something in the <see cref="LastPresentationLayerKeysBag"/>.</summary>
    public ImmutableList<Key<TextEditorPresentationModel>> LastPresentationLayerKeysBag { get; init; } = ImmutableList<Key<TextEditorPresentationModel>>.Empty;

    /// <summary>In order to prevent infinite loops, track the unique identifiers. Note, this HashSet is cleared when the options change or the count >= <see cref="_clearTrackingOfUniqueIdentifiersWhenCountIs"/>.</summary>
    public HashSet<Key<RenderState>> SeenModelRenderStateKeysBag { get; init; } = new();
    /// <summary>In order to prevent infinite loops, track the unique identifiers. Note, this HashSet is cleared when the count is >= <see cref="_clearTrackingOfUniqueIdentifiersWhenCountIs"/>.</summary>
    public HashSet<Key<RenderState>> SeenOptionsRenderStateKeysBag { get; init; } = new();

    public string CommandBarValue { get; set; } = string.Empty;
    public bool ShouldSetFocusAfterNextRender { get; set; }

    public string BodyElementId => $"luth_te_text-editor-content_{ViewModelKey.Guid}";
    public string PrimaryCursorContentId => $"luth_te_text-editor-content_{ViewModelKey.Guid}_primary-cursor";
    public string GutterElementId => $"luth_te_text-editor-gutter_{ViewModelKey.Guid}";

    public void CursorMovePageTop()
    {
        var localMostRecentlyRenderedVirtualizationResult = VirtualizationResult;

        if (localMostRecentlyRenderedVirtualizationResult?.EntryBag.Any() ?? false)
        {
            var firstEntry = localMostRecentlyRenderedVirtualizationResult.EntryBag.First();

            PrimaryCursor.IndexCoordinates = (firstEntry.Index, 0);
        }
    }

    public void CursorMovePageBottom()
    {
        var localMostRecentlyRenderedVirtualizationResult = VirtualizationResult;

        var textEditor = TextEditorService.ViewModel.FindBackingModelOrDefault(
            ViewModelKey);

        if (textEditor is not null &&
            (localMostRecentlyRenderedVirtualizationResult?.EntryBag.Any() ?? false))
        {
            var lastEntry = localMostRecentlyRenderedVirtualizationResult.EntryBag.Last();

            var lastEntriesRowLength = textEditor.GetLengthOfRow(lastEntry.Index);

            PrimaryCursor.IndexCoordinates = (lastEntry.Index, lastEntriesRowLength);
        }
    }

    public async Task MutateScrollHorizontalPositionByPixelsAsync(double pixels)
    {
        _batchScrollEvents.MutateScrollHorizontalPositionByPixels += pixels;

        await _batchScrollEvents.ThrottleMutateScrollHorizontalPositionByPixels.FireAsync(async () =>
        {
            var batch = _batchScrollEvents.MutateScrollHorizontalPositionByPixels;
            _batchScrollEvents.MutateScrollHorizontalPositionByPixels -= batch;

            await TextEditorService.ViewModel.MutateScrollHorizontalPositionAsync(
                BodyElementId,
                GutterElementId,
                batch);
        });
    }

    public async Task MutateScrollVerticalPositionByPixelsAsync(double pixels)
    {
        _batchScrollEvents.MutateScrollVerticalPositionByPixels += pixels;

        await _batchScrollEvents.ThrottleMutateScrollVerticalPositionByPixels.FireAsync(async () =>
        {
            var batch = _batchScrollEvents.MutateScrollVerticalPositionByPixels;
            _batchScrollEvents.MutateScrollVerticalPositionByPixels -= batch;

            await TextEditorService.ViewModel.MutateScrollVerticalPositionAsync(
                BodyElementId,
                GutterElementId,
                batch);
        });
    }

    public async Task MutateScrollVerticalPositionByPagesAsync(double pages)
    {
        await MutateScrollVerticalPositionByPixelsAsync(
            pages * _mostRecentTextEditorMeasurements.Height);
    }

    public async Task MutateScrollVerticalPositionByLinesAsync(double lines)
    {
        await MutateScrollVerticalPositionByPixelsAsync(
            lines * VirtualizationResult.CharAndRowMeasurements.RowHeight);
    }

    /// <summary>If a parameter is null the JavaScript will not modify that value</summary>
    public async Task SetScrollPositionAsync(double? scrollLeft, double? scrollTop)
    {
        await _batchScrollEvents.ThrottleSetScrollPosition.FireAsync(async () =>
        {
            await TextEditorService.ViewModel.SetScrollPositionAsync(
                BodyElementId,
                GutterElementId,
                scrollLeft,
                scrollTop);
        });
    }

    public async Task FocusAsync()
    {
        await TextEditorService.ViewModel.FocusPrimaryCursorAsync(PrimaryCursorContentId);
    }

    public async Task RemeasureAsync(
        TextEditorOptions options,
        string measureCharacterWidthAndRowHeightElementId,
        int countOfTestCharacters,
        CancellationToken cancellationToken)
    {
        await ThrottleRemeasure.FireAsync(async () =>
        {
            lock (_trackingOfUniqueIdentifiersLock)
            {
                if (SeenOptionsRenderStateKeysBag.Contains(options.RenderStateKey))
                    return;
            }

            var characterWidthAndRowHeight = await TextEditorService.ViewModel.MeasureCharacterWidthAndRowHeightAsync(
                measureCharacterWidthAndRowHeightElementId,
                countOfTestCharacters);

            VirtualizationResult.CharAndRowMeasurements = characterWidthAndRowHeight;

            lock (_trackingOfUniqueIdentifiersLock)
            {
                if (SeenOptionsRenderStateKeysBag.Count > _clearTrackingOfUniqueIdentifiersWhenCountIs)
                    SeenOptionsRenderStateKeysBag.Clear();

                SeenOptionsRenderStateKeysBag.Add(options.RenderStateKey);
            }

            TextEditorService.ViewModel.With(
                ViewModelKey,
                previousViewModel => previousViewModel with
                {
                    // Clear the SeenModelRenderStateKeys because one needs to recalculate the virtualization result now that the options have changed.
                    SeenModelRenderStateKeysBag = new(),
                    VirtualizationResult = previousViewModel.VirtualizationResult with
                    {
                        CharAndRowMeasurements = characterWidthAndRowHeight
                    }
                });
        });
    }

    public async Task CalculateVirtualizationResultAsync(
        TextEditorModel? model,
        TextEditorMeasurements? textEditorMeasurements,
        CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return;

        // Return because the UI still needs to be measured.
        if (!SeenOptionsRenderStateKeysBag.Any())
            return;

        await ThrottleCalculateVirtualizationResult.FireAsync(async () =>
        {
            if (model is null)
                return;

            // TODO: Should this '_trackingOfUniqueIdentifiersLock' logic when in regards to the TextEditorModel be removed? The issue is that when scrolling the TextEditorModel would show up in the HashSet and therefore the calculation of the virtualization result would not occur.
            //
            //lock (_trackingOfUniqueIdentifiersLock)
            //{
            //    if (SeenModelRenderStateKeys.Contains(model.RenderStateKey))
            //        return;
            //}

            var localCharacterWidthAndRowHeight = VirtualizationResult.CharAndRowMeasurements;

            if (textEditorMeasurements is null)
                textEditorMeasurements = await TextEditorService.ViewModel.GetTextEditorMeasurementsAsync(BodyElementId);

            _mostRecentTextEditorMeasurements = textEditorMeasurements;

            textEditorMeasurements = textEditorMeasurements with
            {
                MeasurementsExpiredCancellationToken = cancellationToken
            };

            var verticalStartingIndex = (int)Math.Floor(
                textEditorMeasurements.ScrollTop /
                localCharacterWidthAndRowHeight.RowHeight);

            var verticalTake = (int)Math.Ceiling(
                textEditorMeasurements.Height /
                localCharacterWidthAndRowHeight.RowHeight);

            // Vertical Padding (render some offscreen data)
            {
                verticalTake += 1;
            }

            // Check index boundaries
            {
                verticalStartingIndex = Math.Max(0, verticalStartingIndex);

                if (verticalStartingIndex + verticalTake > model.RowEndingPositionsBag.Length)
                    verticalTake = model.RowEndingPositionsBag.Length - verticalStartingIndex;

                verticalTake = Math.Max(0, verticalTake);
            }

            var horizontalStartingIndex = (int)Math.Floor(
                textEditorMeasurements.ScrollLeft /
                localCharacterWidthAndRowHeight.CharacterWidth);

            var horizontalTake = (int)Math.Ceiling(
                textEditorMeasurements.Width /
                localCharacterWidthAndRowHeight.CharacterWidth);

            var virtualizedEntryBag = model
                .GetRows(verticalStartingIndex, verticalTake)
                .Select((row, index) =>
                {
                    index += verticalStartingIndex;

                    var localHorizontalStartingIndex = horizontalStartingIndex;
                    var localHorizontalTake = horizontalTake;

                    // Adjust for tab key width
                    {
                        var maxValidColumnIndex = row.Count - 1;

                        var parameterForGetTabsCountOnSameRowBeforeCursor =
                            localHorizontalStartingIndex > maxValidColumnIndex
                                ? maxValidColumnIndex
                                : localHorizontalStartingIndex;

                        var tabsOnSameRowBeforeCursor = model.GetTabsCountOnSameRowBeforeCursor(
                            index,
                            parameterForGetTabsCountOnSameRowBeforeCursor);

                        // 1 of the character width is already accounted for
                        var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;

                        localHorizontalStartingIndex -= extraWidthPerTabKey * tabsOnSameRowBeforeCursor;
                    }

                    if (localHorizontalStartingIndex + localHorizontalTake > row.Count)
                        localHorizontalTake = row.Count - localHorizontalStartingIndex;

                    localHorizontalTake = Math.Max(0, localHorizontalTake);

                    var horizontallyVirtualizedRow = row
                        .Skip(localHorizontalStartingIndex)
                        .Take(localHorizontalTake)
                        .ToList();

                    var widthInPixels = horizontallyVirtualizedRow.Count *
                        localCharacterWidthAndRowHeight.CharacterWidth;

                    var leftInPixels = horizontalStartingIndex * // do not change this to localHorizontalStartingIndex
                        localCharacterWidthAndRowHeight.CharacterWidth;

                    var topInPixels = index * localCharacterWidthAndRowHeight.RowHeight;

                    return new VirtualizationEntry<List<RichCharacter>>(
                        index,
                        horizontallyVirtualizedRow,
                        widthInPixels,
                        localCharacterWidthAndRowHeight.RowHeight,
                        leftInPixels,
                        topInPixels);
                }).ToImmutableArray();

            var totalWidth = model.MostCharactersOnASingleRowTuple.rowLength *
                localCharacterWidthAndRowHeight.CharacterWidth;

            var totalHeight = model.RowEndingPositionsBag.Length *
                localCharacterWidthAndRowHeight.RowHeight;

            // Add vertical margin so the user can scroll beyond the final row of content
            double marginScrollHeight;
            {
                var percentOfMarginScrollHeightByPageUnit = 0.4;

                marginScrollHeight = textEditorMeasurements.Height * percentOfMarginScrollHeightByPageUnit;
                totalHeight += marginScrollHeight;
            }

            var leftBoundaryWidthInPixels = horizontalStartingIndex *
                localCharacterWidthAndRowHeight.CharacterWidth;

            var leftBoundary = new VirtualizationBoundary(
                leftBoundaryWidthInPixels,
                totalHeight,
                0,
                0);

            var rightBoundaryLeftInPixels = leftBoundary.WidthInPixels +
                localCharacterWidthAndRowHeight.CharacterWidth *
                horizontalTake;

            var rightBoundaryWidthInPixels = totalWidth - rightBoundaryLeftInPixels;

            var rightBoundary = new VirtualizationBoundary(
                rightBoundaryWidthInPixels,
                totalHeight,
                rightBoundaryLeftInPixels,
                0);

            var topBoundaryHeightInPixels = verticalStartingIndex *
                localCharacterWidthAndRowHeight.RowHeight;

            var topBoundary = new VirtualizationBoundary(
                totalWidth,
                topBoundaryHeightInPixels,
                0,
                0);

            var bottomBoundaryTopInPixels = topBoundary.HeightInPixels +
                localCharacterWidthAndRowHeight.RowHeight *
                verticalTake;

            var bottomBoundaryHeightInPixels = totalHeight - bottomBoundaryTopInPixels;

            var bottomBoundary = new VirtualizationBoundary(
                totalWidth,
                bottomBoundaryHeightInPixels,
                0,
                bottomBoundaryTopInPixels);

            var virtualizationResult = new VirtualizationResult<List<RichCharacter>>(
                virtualizedEntryBag,
                leftBoundary,
                rightBoundary,
                topBoundary,
                bottomBoundary,
                textEditorMeasurements with
                {
                    ScrollWidth = totalWidth,
                    ScrollHeight = totalHeight,
                    MarginScrollHeight = marginScrollHeight
                },
                localCharacterWidthAndRowHeight);

            lock (_trackingOfUniqueIdentifiersLock)
            {
                if (SeenModelRenderStateKeysBag.Count > _clearTrackingOfUniqueIdentifiersWhenCountIs)
                    SeenModelRenderStateKeysBag.Clear();

                SeenModelRenderStateKeysBag.Add(model.RenderStateKey);
            }

            TextEditorService.ViewModel.With(
                ViewModelKey,
                previousViewModel => previousViewModel with
                {
                    VirtualizationResult = virtualizationResult,
                });
        });
    }

    public void Dispose()
    {
        DisplayTracker.Dispose();
    }
}