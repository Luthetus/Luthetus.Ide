using Microsoft.JSInterop;
using Fluxor;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.JavaScriptObjects.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Keyboards.Models;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Virtualizations.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;

public partial interface ITextEditorService
{
    public interface ITextEditorViewModelApi
    {
        #region CREATE_METHODS
        public void Register(Key<TextEditorViewModel> textEditorViewModelKey, ResourceUri resourceUri);
        #endregion

        #region READ_METHODS
        /// <summary>
        /// One should store the result of invoking this method in a variable, then reference that variable.
        /// If one continually invokes this, there is no guarantee that the data had not changed
        /// since the previous invocation.
        /// </summary>
        public ImmutableList<TextEditorViewModel> GetViewModels();
        public TextEditorModel? GetModelOrDefault(Key<TextEditorViewModel> viewModelKey);
        public Task<TextEditorMeasurements> GetTextEditorMeasurementsAsync(string elementId);

        public Task<CharAndRowMeasurements> MeasureCharacterWidthAndRowHeightAsync(
            string measureCharacterWidthAndRowHeightElementId,
            int countOfTestCharacters);

        public TextEditorViewModel? GetOrDefault(Key<TextEditorViewModel> viewModelKey);
        public string? GetAllText(Key<TextEditorViewModel> viewModelKey);

        public void SetCursorShouldBlink(bool cursorShouldBlink);
        #endregion

        #region UPDATE_METHODS
        public TextEditorEdit GetWithValueTask(
            Key<TextEditorViewModel> viewModelKey,
            Func<TextEditorViewModel, TextEditorViewModel> withFunc);

        public TextEditorEdit GetWithTaskTask(
            TextEditorViewModel viewModel,
            Func<TextEditorViewModel, Task<Func<TextEditorViewModel, TextEditorViewModel>>> withFuncWrap);

        /// <summary>
        /// If a parameter is null the JavaScript will not modify that value
        /// </summary>
        public TextEditorEdit GetSetScrollPositionTask(
            string bodyElementId,
            string gutterElementId,
            double? scrollLeftInPixels,
            double? scrollTopInPixels);

        public TextEditorEdit GetSetGutterScrollTopTask(
            string gutterElementId,
            double scrollTopInPixels);

        public TextEditorEdit GetMutateScrollVerticalPositionTask(
            string bodyElementId,
            string gutterElementId,
            double pixels);

        public TextEditorEdit GetMutateScrollHorizontalPositionTask(
            string bodyElementId,
            string gutterElementId,
            double pixels);

        public TextEditorEdit GetFocusPrimaryCursorTask(string primaryCursorContentId);

        public TextEditorEdit GetMoveCursorTask(
            KeyboardEventArgs keyboardEventArgs,
            TextEditorModel model,
            Key<TextEditorViewModel> viewModelKey,
            TextEditorCursorModifier primaryCursor);

        public TextEditorEdit GetCursorMovePageTopTask(
            ResourceUri modelResourceUri,
            TextEditorViewModel viewModel,
            TextEditorCursorModifier primaryCursor);

        public TextEditorEdit GetCursorMovePageBottomTask(
            TextEditorModel model,
            TextEditorViewModel viewModel,
            TextEditorCursorModifier primaryCursor);

        public TextEditorEdit GetCalculateVirtualizationResultTask(
            TextEditorModel model,
            TextEditorViewModel viewModel,
            TextEditorMeasurements? textEditorMeasurements,
            TextEditorCursorModifier primaryCursor,
            CancellationToken cancellationToken);

        public TextEditorEdit GetRemeasureTask(
            ResourceUri modelResourceUri,
            TextEditorViewModel viewModel,
            string measureCharacterWidthAndRowHeightElementId,
            int countOfTestCharacters,
            CancellationToken cancellationToken);
        #endregion

        #region DELETE_METHODS
        public void Dispose(Key<TextEditorViewModel> viewModelKey);
        #endregion

        public bool CursorShouldBlink { get; }
        public event Action? CursorShouldBlinkChanged;
    }

    public class TextEditorViewModelApi : ITextEditorViewModelApi
    {
        private readonly ITextEditorService _textEditorService;
        private readonly IBackgroundTaskService _backgroundTaskService;
        private readonly IState<TextEditorViewModelState> _viewModelStateWrap;
        private readonly IState<TextEditorModelState> _modelStateWrap;
        private readonly IDispatcher _dispatcher;

        // TODO: Perhaps do not reference IJSRuntime but instead wrap it in a 'IUiProvider' or something like that. The 'IUiProvider' would then expose methods that allow the TextEditorViewModel to adjust the scrollbars. 
        private readonly IJSRuntime _jsRuntime;

        public TextEditorViewModelApi(
            ITextEditorService textEditorService,
            IBackgroundTaskService backgroundTaskService,
            IState<TextEditorViewModelState> viewModelStateWrap,
            IState<TextEditorModelState> modelStateWrap,
            IJSRuntime jsRuntime,
            IDispatcher dispatcher)
        {
            _textEditorService = textEditorService;
            _backgroundTaskService = backgroundTaskService;
            _viewModelStateWrap = viewModelStateWrap;
            _modelStateWrap = modelStateWrap;
            _jsRuntime = jsRuntime;
            _dispatcher = dispatcher;
        }

        private Task _cursorShouldBlinkTask = Task.CompletedTask;
        private CancellationTokenSource _cursorShouldBlinkCancellationTokenSource = new();
        private TimeSpan _blinkingCursorTaskDelay = TimeSpan.FromMilliseconds(1000);

        public bool CursorShouldBlink { get; private set; } = true;
        public event Action? CursorShouldBlinkChanged;

        public void SetCursorShouldBlink(bool cursorShouldBlink)
        {
            if (!cursorShouldBlink)
            {
                if (CursorShouldBlink)
                {
                    // Change true -> false THEREFORE: notify subscribers
                    CursorShouldBlink = cursorShouldBlink;
                    CursorShouldBlinkChanged?.Invoke();
                }

                // Single Threaded Applications flicker every "_blinkingCursorTaskDelay" event while holding a key down if this line is not included
                _cursorShouldBlinkCancellationTokenSource.Cancel();

                if (_cursorShouldBlinkTask.IsCompleted)
                {
                    // Considering that just before entering this if block we cancel the cancellation token source. I want to ensure we get a new one if a new Task session beings.
                    _cursorShouldBlinkCancellationTokenSource = new();

                    _cursorShouldBlinkTask = Task.Run(async () =>
                    {
                        while (true)
                        {
                            try
                            {
                                var cancellationToken = _cursorShouldBlinkCancellationTokenSource.Token;

                                await Task
                                    .Delay(_blinkingCursorTaskDelay, cancellationToken)
                                    .ConfigureAwait(false);

                                // Change false -> true THEREFORE: notify subscribers
                                CursorShouldBlink = true;
                                CursorShouldBlinkChanged?.Invoke();
                                break;
                            }
                            catch (TaskCanceledException)
                            {
                                // Single Threaded Applications cannot exit the while loop unless they cancel the token themselves.
                                _cursorShouldBlinkCancellationTokenSource.Cancel();
                                _cursorShouldBlinkCancellationTokenSource = new();
                            }
                        }
                    });
                }
            }
        }

        #region CREATE_METHODS
        public void Register(Key<TextEditorViewModel> textEditorViewModelKey, ResourceUri resourceUri)
        {
            _dispatcher.Dispatch(new TextEditorViewModelState.RegisterAction(
                textEditorViewModelKey,
                resourceUri,
                _textEditorService));
        }
        #endregion

        #region READ_METHODS
        public TextEditorViewModel? GetOrDefault(Key<TextEditorViewModel> textEditorViewModelKey)
        {
            return _textEditorService.ViewModelStateWrap.Value.ViewModelBag.FirstOrDefault(
                x => x.ViewModelKey == textEditorViewModelKey);
        }

        public ImmutableList<TextEditorViewModel> GetViewModels()
        {
            return _textEditorService.ViewModelStateWrap.Value.ViewModelBag;
        }

        public TextEditorModel? GetModelOrDefault(Key<TextEditorViewModel> textEditorViewModelKey)
        {
            var viewModelState = _textEditorService.ViewModelStateWrap.Value;

            var viewModel = viewModelState.ViewModelBag.FirstOrDefault(
                x => x.ViewModelKey == textEditorViewModelKey);

            if (viewModel is null)
                return null;

            return _textEditorService.ModelApi.GetOrDefault(viewModel.ResourceUri);
        }

        public string? GetAllText(Key<TextEditorViewModel> textEditorViewModelKey)
        {
            var textEditorModel = GetModelOrDefault(textEditorViewModelKey);

            return textEditorModel is null
                ? null
                : _textEditorService.ModelApi.GetAllText(textEditorModel.ResourceUri);
        }

        public async Task<TextEditorMeasurements> GetTextEditorMeasurementsAsync(string elementId)
        {
            return await _jsRuntime.InvokeAsync<TextEditorMeasurements>("luthetusTextEditor.getTextEditorMeasurementsInPixelsById",
                elementId);
        }

        public async Task<CharAndRowMeasurements> MeasureCharacterWidthAndRowHeightAsync(
            string measureCharacterWidthAndRowHeightElementId,
            int countOfTestCharacters)
        {
            return await _jsRuntime.InvokeAsync<CharAndRowMeasurements>("luthetusTextEditor.getCharAndRowMeasurementsInPixelsById",
                measureCharacterWidthAndRowHeightElementId,
                countOfTestCharacters);
        }
        #endregion

        #region UPDATE_METHODS
        public TextEditorEdit GetWithValueTask(
            Key<TextEditorViewModel> viewModelKey,
            Func<TextEditorViewModel, TextEditorViewModel> withFunc)
        {
            return editContext =>
            {
                _dispatcher.Dispatch(new TextEditorViewModelState.SetViewModelWithAction(
                    viewModelKey,
                    withFunc,
                    editContext.AuthenticatedActionKey));

                return Task.CompletedTask;
            };
        }

        public TextEditorEdit GetWithTaskTask(
            TextEditorViewModel viewModel,
            Func<TextEditorViewModel, Task<Func<TextEditorViewModel, TextEditorViewModel>>> withFuncWrap)
        {
            return async editContext =>
            {
                _dispatcher.Dispatch(new TextEditorViewModelState.SetViewModelWithAction(
                    viewModel.ViewModelKey,
                    await withFuncWrap.Invoke(viewModel),
                    editContext.AuthenticatedActionKey));
            };
        }

        /// <summary>
        /// If a parameter is null the JavaScript will not modify that value
        /// </summary>
        public TextEditorEdit GetSetScrollPositionTask(
            string bodyElementId,
            string gutterElementId,
            double? scrollLeftInPixels,
            double? scrollTopInPixels)
        {
            return async editContext =>
            {
                await _jsRuntime.InvokeVoidAsync("luthetusTextEditor.setScrollPosition",
                    bodyElementId,
                    gutterElementId,
                    scrollLeftInPixels,
                    scrollTopInPixels);
            };
        }

        public TextEditorEdit GetSetGutterScrollTopTask(
            string gutterElementId,
            double scrollTopInPixels)
        {
            return async editContext =>
            {
                await _jsRuntime.InvokeVoidAsync("luthetusTextEditor.setGutterScrollTop",
                    gutterElementId,
                    scrollTopInPixels);
            };
        }

        public TextEditorEdit GetMutateScrollVerticalPositionTask(
            string bodyElementId,
            string gutterElementId,
            double pixels)
        {
            return async editContext =>
            {
                await _jsRuntime.InvokeVoidAsync("luthetusTextEditor.mutateScrollVerticalPositionByPixels",
                    bodyElementId,
                    gutterElementId,
                    pixels);
            };
        }

        public TextEditorEdit GetMutateScrollHorizontalPositionTask(
            string bodyElementId,
            string gutterElementId,
            double pixels)
        {
            return async editContext =>
            {
                await _jsRuntime.InvokeVoidAsync("luthetusTextEditor.mutateScrollHorizontalPositionByPixels",
                    bodyElementId,
                    gutterElementId,
                    pixels);
            };
        }

        public TextEditorEdit GetFocusPrimaryCursorTask(string primaryCursorContentId)
        {
            return async editContext =>
            {
                await _jsRuntime.InvokeVoidAsync("luthetusTextEditor.focusHtmlElementById",
                    primaryCursorContentId);
            };
        }

        public TextEditorEdit GetMoveCursorTask(
            KeyboardEventArgs keyboardEventArgs,
            TextEditorModel model,
            Key<TextEditorViewModel> viewModelKey,
            TextEditorCursorModifier primaryCursor)
        {
            return editContext =>
            {
                void MutateIndexCoordinatesAndPreferredColumnIndex(int columnIndex)
                {
                    primaryCursor.ColumnIndex = columnIndex;
                    primaryCursor.PreferredColumnIndex = columnIndex;
                }

                if (keyboardEventArgs.ShiftKey)
                {
                    if (primaryCursor.SelectionAnchorPositionIndex is null ||
                        primaryCursor.SelectionEndingPositionIndex == primaryCursor.SelectionAnchorPositionIndex)
                    {
                        var positionIndex = model.GetPositionIndex(
                            primaryCursor.RowIndex,
                            primaryCursor.ColumnIndex);

                        primaryCursor.SelectionAnchorPositionIndex = positionIndex;
                    }
                }
                else
                {
                    primaryCursor.SelectionAnchorPositionIndex = null;
                }

                int lengthOfRow = 0; // This variable is used in multiple switch cases.

                switch (keyboardEventArgs.Key)
                {
                    case KeyboardKeyFacts.MovementKeys.ARROW_LEFT:
                        if (TextEditorSelectionHelper.HasSelectedText(primaryCursor) &&
                            !keyboardEventArgs.ShiftKey)
                        {
                            var selectionBounds = TextEditorSelectionHelper.GetSelectionBounds(primaryCursor);

                            var lowerRowMetaData = model.FindRowInformation(
                                selectionBounds.lowerPositionIndexInclusive);

                            primaryCursor.RowIndex = lowerRowMetaData.rowIndex;

                            primaryCursor.ColumnIndex = selectionBounds.lowerPositionIndexInclusive -
                                lowerRowMetaData.rowStartPositionIndex;
                        }
                        else
                        {
                            if (primaryCursor.ColumnIndex == 0)
                            {
                                if (primaryCursor.RowIndex != 0)
                                {
                                    primaryCursor.RowIndex--;

                                    lengthOfRow = model.GetLengthOfRow(primaryCursor.RowIndex);

                                    MutateIndexCoordinatesAndPreferredColumnIndex(lengthOfRow);
                                }
                            }
                            else
                            {
                                if (keyboardEventArgs.CtrlKey)
                                {
                                    var columnIndexOfCharacterWithDifferingKind = model.GetColumnIndexOfCharacterWithDifferingKind(
                                        primaryCursor.RowIndex,
                                        primaryCursor.ColumnIndex,
                                        true);

                                    if (columnIndexOfCharacterWithDifferingKind == -1)
                                        MutateIndexCoordinatesAndPreferredColumnIndex(0);
                                    else
                                        MutateIndexCoordinatesAndPreferredColumnIndex(columnIndexOfCharacterWithDifferingKind);
                                }
                                else
                                {
                                    MutateIndexCoordinatesAndPreferredColumnIndex(primaryCursor.ColumnIndex - 1);
                                }
                            }
                        }

                        break;
                    case KeyboardKeyFacts.MovementKeys.ARROW_DOWN:
                        if (primaryCursor.RowIndex < model.RowCount - 1)
                        {
                            primaryCursor.RowIndex++;

                            lengthOfRow = model.GetLengthOfRow(primaryCursor.RowIndex);

                            primaryCursor.ColumnIndex = lengthOfRow < primaryCursor.PreferredColumnIndex
                                ? lengthOfRow
                                : primaryCursor.PreferredColumnIndex;
                        }

                        break;
                    case KeyboardKeyFacts.MovementKeys.ARROW_UP:
                        if (primaryCursor.RowIndex > 0)
                        {
                            primaryCursor.RowIndex--;

                            lengthOfRow = model.GetLengthOfRow(primaryCursor.RowIndex);

                            primaryCursor.ColumnIndex = lengthOfRow < primaryCursor.PreferredColumnIndex
                                ? lengthOfRow
                                : primaryCursor.PreferredColumnIndex;
                        }

                        break;
                    case KeyboardKeyFacts.MovementKeys.ARROW_RIGHT:
                        if (TextEditorSelectionHelper.HasSelectedText(primaryCursor) && !keyboardEventArgs.ShiftKey)
                        {
                            var selectionBounds = TextEditorSelectionHelper.GetSelectionBounds(primaryCursor);

                            var upperRowMetaData = model.FindRowInformation(selectionBounds.upperPositionIndexExclusive);

                            primaryCursor.RowIndex = upperRowMetaData.rowIndex;

                            if (primaryCursor.RowIndex >= model.RowCount)
                            {
                                primaryCursor.RowIndex = model.RowCount - 1;

                                var upperRowLength = model.GetLengthOfRow(primaryCursor.RowIndex);

                                primaryCursor.ColumnIndex = upperRowLength;
                            }
                            else
                            {
                                primaryCursor.ColumnIndex =
                                    selectionBounds.upperPositionIndexExclusive - upperRowMetaData.rowStartPositionIndex;
                            }
                        }
                        else
                        {
                            lengthOfRow = model.GetLengthOfRow(primaryCursor.RowIndex);

                            if (primaryCursor.ColumnIndex == lengthOfRow &&
                                primaryCursor.RowIndex < model.RowCount - 1)
                            {
                                MutateIndexCoordinatesAndPreferredColumnIndex(0);
                                primaryCursor.RowIndex++;
                            }
                            else if (primaryCursor.ColumnIndex != lengthOfRow)
                            {
                                if (keyboardEventArgs.CtrlKey)
                                {
                                    var columnIndexOfCharacterWithDifferingKind = model.GetColumnIndexOfCharacterWithDifferingKind(
                                        primaryCursor.RowIndex,
                                        primaryCursor.ColumnIndex,
                                        false);

                                    if (columnIndexOfCharacterWithDifferingKind == -1)
                                        MutateIndexCoordinatesAndPreferredColumnIndex(lengthOfRow);
                                    else
                                    {
                                        MutateIndexCoordinatesAndPreferredColumnIndex(
                                            columnIndexOfCharacterWithDifferingKind);
                                    }
                                }
                                else
                                {
                                    MutateIndexCoordinatesAndPreferredColumnIndex(primaryCursor.ColumnIndex + 1);
                                }
                            }
                        }

                        break;
                    case KeyboardKeyFacts.MovementKeys.HOME:
                        if (keyboardEventArgs.CtrlKey)
                            primaryCursor.RowIndex = 0;

                        MutateIndexCoordinatesAndPreferredColumnIndex(0);

                        break;
                    case KeyboardKeyFacts.MovementKeys.END:
                        if (keyboardEventArgs.CtrlKey)
                            primaryCursor.RowIndex = model.RowCount - 1;

                        lengthOfRow = model.GetLengthOfRow(primaryCursor.RowIndex);

                        MutateIndexCoordinatesAndPreferredColumnIndex(lengthOfRow);

                        break;
                }

                if (keyboardEventArgs.ShiftKey)
                {
                    primaryCursor.SelectionEndingPositionIndex = model.GetPositionIndex(
                        primaryCursor.RowIndex,
                        primaryCursor.ColumnIndex);
                }

                return Task.CompletedTask;
            };
        }

        public TextEditorEdit GetCursorMovePageTopTask(
            ResourceUri modelResourceUri,
            TextEditorViewModel viewModel,
            TextEditorCursorModifier primaryCursor)
        {
            return editContext =>
            {
                if (viewModel.VirtualizationResult?.EntryBag.Any() ?? false)
                {
                    var firstEntry = viewModel.VirtualizationResult.EntryBag.First();

                    primaryCursor.RowIndex = firstEntry.Index;
                    primaryCursor.ColumnIndex = 0;
                }

                return Task.CompletedTask;
            };
        }

        public TextEditorEdit GetCursorMovePageBottomTask(
            TextEditorModel model,
            TextEditorViewModel viewModel,
            TextEditorCursorModifier primaryCursor)
        {
            return editContext =>
            {
                if ((viewModel.VirtualizationResult?.EntryBag.Any() ?? false))
                {
                    var lastEntry = viewModel.VirtualizationResult.EntryBag.Last();
                    var lastEntriesRowLength = model.GetLengthOfRow(lastEntry.Index);

                    primaryCursor.RowIndex = lastEntry.Index;
                    primaryCursor.ColumnIndex = lastEntriesRowLength;
                }

                return Task.CompletedTask;
            };
        }

        public TextEditorEdit GetCalculateVirtualizationResultTask(
            TextEditorModel model,
            TextEditorViewModel viewModel,
            TextEditorMeasurements? textEditorMeasurements,
            TextEditorCursorModifier primaryCursor,
            CancellationToken cancellationToken)
        {
            return async editContext =>
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                // Return because the UI still needs to be measured.
                if (!viewModel.SeenOptionsRenderStateKeysBag.Any())
                    return;

                await viewModel.ThrottleCalculateVirtualizationResult.FireAsync((Func<CancellationToken, Task>)(async _ =>
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

                    var localCharacterWidthAndRowHeight = viewModel.VirtualizationResult.CharAndRowMeasurements;

                    textEditorMeasurements = await _textEditorService.ViewModelApi.GetTextEditorMeasurementsAsync(viewModel.BodyElementId);

                    viewModel.MostRecentTextEditorMeasurements = textEditorMeasurements;

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

                        if (verticalStartingIndex + verticalTake > model.RowEndingPositionsBag.Count)
                            verticalTake = model.RowEndingPositionsBag.Count - verticalStartingIndex;

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
                        .Select((row, rowIndex) =>
                        {
                            rowIndex += verticalStartingIndex;

                            var localHorizontalStartingIndex = horizontalStartingIndex;
                            var localHorizontalTake = horizontalTake;

                            // 1 of the character width is already accounted for
                            var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;

                            // Adjust for tab key width
                            {
                                var maxValidColumnIndex = row.Count - 1;

                                var parameterForGetTabsCountOnSameRowBeforeCursor =
                                    localHorizontalStartingIndex > maxValidColumnIndex
                                        ? maxValidColumnIndex
                                        : localHorizontalStartingIndex;

                                var tabsOnSameRowBeforeCursor = model.GetTabsCountOnSameRowBeforeCursor(
                                    rowIndex,
                                    parameterForGetTabsCountOnSameRowBeforeCursor);

                                localHorizontalStartingIndex -= extraWidthPerTabKey * tabsOnSameRowBeforeCursor;
                            }

                            if (localHorizontalStartingIndex + localHorizontalTake > row.Count)
                                localHorizontalTake = row.Count - localHorizontalStartingIndex;

                            localHorizontalStartingIndex = Math.Max(0, localHorizontalStartingIndex);
                            localHorizontalTake = Math.Max(0, localHorizontalTake);

                            var horizontallyVirtualizedRow = row
                                .Skip(localHorizontalStartingIndex)
                                .Take(localHorizontalTake)
                                .ToList();

                            var countTabKeysInVirtualizedRow = horizontallyVirtualizedRow
                                .Where(x => x.Value == KeyboardKeyFacts.WhitespaceCharacters.TAB)
                                .Count();

                            var widthInPixels = (horizontallyVirtualizedRow.Count + (extraWidthPerTabKey * countTabKeysInVirtualizedRow)) *
                                localCharacterWidthAndRowHeight.CharacterWidth;

                            var leftInPixels = localHorizontalStartingIndex *
                                localCharacterWidthAndRowHeight.CharacterWidth;

                            // Adjust for tab key width
                            {
                                var maxValidColumnIndex = row.Count - 1;

                                var parameterForGetTabsCountOnSameRowBeforeCursor =
                                    localHorizontalStartingIndex > maxValidColumnIndex
                                        ? maxValidColumnIndex
                                        : localHorizontalStartingIndex;

                                var tabsOnSameRowBeforeCursor = model.GetTabsCountOnSameRowBeforeCursor(
                                    rowIndex,
                                    parameterForGetTabsCountOnSameRowBeforeCursor);

                                leftInPixels += (extraWidthPerTabKey *
                                    tabsOnSameRowBeforeCursor *
                                    localCharacterWidthAndRowHeight.CharacterWidth);
                            }

                            leftInPixels = Math.Max(0, leftInPixels);

                            var topInPixels = rowIndex * localCharacterWidthAndRowHeight.RowHeight;

                            return new VirtualizationEntry<List<RichCharacter>>(
                                rowIndex,
                                horizontallyVirtualizedRow,
                                widthInPixels,
                                localCharacterWidthAndRowHeight.RowHeight,
                                leftInPixels,
                                topInPixels);
                        }).ToImmutableArray();

                    var totalWidth = model.MostCharactersOnASingleRowTuple.rowLength *
                        localCharacterWidthAndRowHeight.CharacterWidth;

                    var totalHeight = model.RowEndingPositionsBag.Count *
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

                    lock (viewModel.TrackingOfUniqueIdentifiersLock)
                    {
                        if (viewModel.SeenModelRenderStateKeysBag.Count > TextEditorViewModel.ClearTrackingOfUniqueIdentifiersWhenCountIs)
                            viewModel.SeenModelRenderStateKeysBag.Clear();

                        viewModel.SeenModelRenderStateKeysBag.Add(model.RenderStateKey);
                    }

                    await GetWithValueTask(
                            viewModel.ViewModelKey,
                            previousViewModel => previousViewModel with
                            {
                                VirtualizationResult = virtualizationResult,
                            })
                        .Invoke(editContext);
                }));
            };
        }

        public TextEditorEdit GetRemeasureTask(
            ResourceUri modelResourceUri,
            TextEditorViewModel viewModel,
            string measureCharacterWidthAndRowHeightElementId,
            int countOfTestCharacters,
            CancellationToken cancellationToken)
        {
            return async editContext =>
            {
                var options = _textEditorService.OptionsApi.GetOptions();

                await viewModel.ThrottleRemeasure.FireAsync(async _ =>
                {
                    lock (viewModel.TrackingOfUniqueIdentifiersLock)
                    {
                        if (viewModel.SeenOptionsRenderStateKeysBag.Contains(options.RenderStateKey))
                            return;
                    }

                    var characterWidthAndRowHeight = await _textEditorService.ViewModelApi.MeasureCharacterWidthAndRowHeightAsync(
                        measureCharacterWidthAndRowHeightElementId,
                        countOfTestCharacters);

                    viewModel.VirtualizationResult.CharAndRowMeasurements = characterWidthAndRowHeight;

                    lock (viewModel.TrackingOfUniqueIdentifiersLock)
                    {
                        if (viewModel.SeenOptionsRenderStateKeysBag.Count > TextEditorViewModel.ClearTrackingOfUniqueIdentifiersWhenCountIs)
                            viewModel.SeenOptionsRenderStateKeysBag.Clear();

                        viewModel.SeenOptionsRenderStateKeysBag.Add(options.RenderStateKey);
                    }

                    await GetWithValueTask(
                            viewModel.ViewModelKey,
                            previousViewModel => (previousViewModel with
                            {
                                // Clear the SeenModelRenderStateKeys because one needs to recalculate the virtualization result now that the options have changed.
                                SeenModelRenderStateKeysBag = new(),
                                VirtualizationResult = previousViewModel.VirtualizationResult with
                                {
                                    CharAndRowMeasurements = characterWidthAndRowHeight
                                },
                            }))
                        .Invoke(editContext);
                });
            };
        }
        #endregion

        #region DELETE_METHODS
        public void Dispose(Key<TextEditorViewModel> textEditorViewModelKey)
        {
            _dispatcher.Dispatch(new TextEditorViewModelState.DisposeAction(textEditorViewModelKey));
        }
        #endregion
    }
}