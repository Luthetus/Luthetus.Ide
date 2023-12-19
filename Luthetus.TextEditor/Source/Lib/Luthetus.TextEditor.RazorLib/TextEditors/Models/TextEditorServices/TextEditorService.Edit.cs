using Fluxor;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.JavaScriptObjects.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using Luthetus.TextEditor.RazorLib.Virtualizations.Models;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using static Luthetus.TextEditor.RazorLib.Commands.Models.TextEditorCommand;
using static Luthetus.TextEditor.RazorLib.TextEditors.States.TextEditorModelState;
using System.Reflection;
using static Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorService;
using Microsoft.JSInterop;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

public partial class TextEditorService
{
    private class TextEditorEditContext : ITextEditorEditContext
    {
        public async Task ExecuteAsync(ITextEditorEdit edit)
        {
            await ((TextEditorEdit)edit).Func.Invoke(this);
        }
    }

    private class TextEditorEdit : ITextEditorEdit
    {
        private readonly TextEditorService _textEditorService;
        private readonly IJSRuntime _jsRuntime;
        private readonly IDispatcher _dispatcher;
        
        public TextEditorEdit(TextEditorService textEditorService, IJSRuntime jsRuntime, Func<ITextEditorEditContext, Task> func, IDispatcher dispatcher)
        {
            _textEditorService = textEditorService;
            _jsRuntime = jsRuntime;
            Func = func;
            _dispatcher = dispatcher;
        }

        public readonly Func<ITextEditorEditContext, Task> Func;

        public TextEditorCommandArgs CommandArgs { get; set; }
        public TextEditorModel Model { get; set; }
        public TextEditorViewModel ViewModel { get; set; }
        public RefreshCursorsRequest RefreshCursorsRequest { get; set; }
        public TextEditorCursorModifier PrimaryCursor { get; set; }
        public bool? IsCompleted { get; private set; }

        public Task Model_UndoEdit(
            ResourceUri resourceUri)
        {
            _dispatcher.Dispatch(new UndoEditAction(resourceUri));
            return Task.CompletedTask;
        }

        public Task Model_SetUsingRowEndingKind(
            ResourceUri resourceUri,
            RowEndingKind rowEndingKind)
        {
            _dispatcher.Dispatch(new SetUsingRowEndingKindAction(
                resourceUri,
                rowEndingKind));

            return Task.CompletedTask;
        }

        public Task Model_SetResourceData(
            ResourceUri resourceUri,
            DateTime resourceLastWriteTime)
        {
            _dispatcher.Dispatch(new SetResourceDataAction(
                resourceUri,
                resourceLastWriteTime));

            return Task.CompletedTask;
        }

        public Task Model_Reload(
            ResourceUri resourceUri,
            string content,
            DateTime resourceLastWriteTime)
        {
            _dispatcher.Dispatch(new ReloadAction(
                resourceUri,
                content,
                resourceLastWriteTime));

            return Task.CompletedTask;
        }

        public Task Model_RedoEdit(ResourceUri resourceUri)
        {
            _dispatcher.Dispatch(new RedoEditAction(resourceUri));
            return Task.CompletedTask;
        }

        public Task Model_InsertText(
            InsertTextAction insertTextAction,
            RefreshCursorsRequest refreshCursorsRequest)
        {
            var cursorBag = refreshCursorsRequest?.CursorBag ?? insertTextAction.CursorModifierBag;

            insertTextAction = insertTextAction with
            {
                CursorModifierBag = cursorBag,
            };

            _dispatcher.Dispatch(insertTextAction);

            return Task.CompletedTask;
        }

        public Task Model_HandleKeyboardEvent(
            KeyboardEventAction keyboardEventAction,
            RefreshCursorsRequest refreshCursorsRequest)
        {
            var cursorBag = refreshCursorsRequest?.CursorBag ?? keyboardEventAction.CursorModifierBag;

            keyboardEventAction = keyboardEventAction with
            {
                CursorModifierBag = cursorBag,
            };

            _dispatcher.Dispatch(keyboardEventAction);

            return Task.CompletedTask;
        }

        public Task Model_DeleteTextByRange(
            DeleteTextByRangeAction deleteTextByRangeAction,
            RefreshCursorsRequest refreshCursorsRequest)
        {
            var cursorBag = refreshCursorsRequest?.CursorBag ?? deleteTextByRangeAction.CursorModifierBag;

            deleteTextByRangeAction = deleteTextByRangeAction with
            {
                CursorModifierBag = cursorBag,
            };

            _dispatcher.Dispatch(deleteTextByRangeAction);

            return Task.CompletedTask;
        }

        public Task Model_DeleteTextByMotion(
            DeleteTextByMotionAction deleteTextByMotionAction,
            RefreshCursorsRequest refreshCursorsRequest)
        {
            var cursorBag = refreshCursorsRequest?.CursorBag ?? deleteTextByMotionAction.CursorModifierBag;

            deleteTextByMotionAction = deleteTextByMotionAction with
            {
                CursorModifierBag = cursorBag,
            };

            _dispatcher.Dispatch(deleteTextByMotionAction);

            return Task.CompletedTask;
        }

        public Task ViewModel_GetWithValueTask(
            Key<TextEditorViewModel> viewModelKey,
            Func<TextEditorViewModel, TextEditorViewModel> withFunc)
        {
            _dispatcher.Dispatch(new TextEditorViewModelState.SetViewModelWithAction(
                viewModelKey,
            withFunc));

            return Task.CompletedTask;
        }

        public async Task ViewModel_GetWithTaskTask(
            TextEditorViewModel viewModel,
            Func<TextEditorViewModel, Task<Func<TextEditorViewModel, TextEditorViewModel>>> withFuncWrap)
        {
            _dispatcher.Dispatch(new TextEditorViewModelState.SetViewModelWithAction(
                viewModel.ViewModelKey,
                await withFuncWrap.Invoke(viewModel)));
        }

        /// <summary>
        /// If a parameter is null the JavaScript will not modify that value
        /// </summary>
        public async Task ViewModel_GetSetScrollPositionTask(
            string bodyElementId,
            string gutterElementId,
            double? scrollLeftInPixels,
            double? scrollTopInPixels)
        {
            await _jsRuntime.InvokeVoidAsync("luthetusTextEditor.setScrollPosition",
                bodyElementId,
                gutterElementId,
                scrollLeftInPixels,
                scrollTopInPixels);
        }

        public async Task ViewModel_GetSetGutterScrollTopTask(
            string gutterElementId,
            double scrollTopInPixels)
        {
            await _jsRuntime.InvokeVoidAsync("luthetusTextEditor.setGutterScrollTop",
                gutterElementId,
                scrollTopInPixels);
        }

        public async Task ViewModel_GetMutateScrollVerticalPositionTask(
            string bodyElementId,
            string gutterElementId,
            double pixels)
        {
            await _jsRuntime.InvokeVoidAsync("luthetusTextEditor.mutateScrollVerticalPositionByPixels",
                bodyElementId,
                gutterElementId,
                pixels);
        }

        public async Task ViewModel_GetMutateScrollHorizontalPositionTask(
            string bodyElementId,
            string gutterElementId,
            double pixels)
        {
            await _jsRuntime.InvokeVoidAsync("luthetusTextEditor.mutateScrollHorizontalPositionByPixels",
                bodyElementId,
                gutterElementId,
                pixels);
        }

        public async Task ViewModel_GetFocusPrimaryCursorTask(string primaryCursorContentId)
        {
            await _jsRuntime.InvokeVoidAsync("luthetusTextEditor.focusHtmlElementById",
                primaryCursorContentId);
        }

        public Task ViewModel_GetMoveCursorTask(
            KeyboardEventArgs keyboardEventArgs,
            TextEditorModel model,
            Key<TextEditorViewModel> viewModelKey,
            TextEditorCursorModifier primaryCursor)
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
        }

        public Task ViewModel_GetCursorMovePageTopTask(
            ResourceUri modelResourceUri,
            TextEditorViewModel viewModel,
            TextEditorCursorModifier primaryCursor)
        {
            if (viewModel.VirtualizationResult?.EntryBag.Any() ?? false)
            {
                var firstEntry = viewModel.VirtualizationResult.EntryBag.First();

                primaryCursor.RowIndex = firstEntry.Index;
                primaryCursor.ColumnIndex = 0;
            }

            return Task.CompletedTask;
        }

        public Task ViewModel_GetCursorMovePageBottomTask(
            TextEditorModel model,
            TextEditorViewModel viewModel,
            TextEditorCursorModifier primaryCursor)
        {
            if ((viewModel.VirtualizationResult?.EntryBag.Any() ?? false))
            {
                var lastEntry = viewModel.VirtualizationResult.EntryBag.Last();
                var lastEntriesRowLength = model.GetLengthOfRow(lastEntry.Index);

                primaryCursor.RowIndex = lastEntry.Index;
                primaryCursor.ColumnIndex = lastEntriesRowLength;
            }

            return Task.CompletedTask;
        }

        public async Task ViewModel_GetCalculateVirtualizationResultTask(
            TextEditorModel model,
            TextEditorViewModel viewModel,
            TextEditorMeasurements? textEditorMeasurements,
            TextEditorCursorModifier primaryCursor,
            CancellationToken cancellationToken)
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

                await ViewModel_GetWithValueTask(
                    viewModel.ViewModelKey,
                    previousViewModel => previousViewModel with
                    {
                        VirtualizationResult = virtualizationResult,
                    });
            }));
        }

        public async Task ViewModel_GetRemeasureTask(
            ResourceUri modelResourceUri,
            TextEditorViewModel viewModel,
            string measureCharacterWidthAndRowHeightElementId,
            int countOfTestCharacters,
            CancellationToken cancellationToken)
        {
            var options = _textEditorService.OptionsApi.GetOptions();

            await viewModel.ThrottleRemeasure.FireAsync((async _ =>
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

                await ViewModel_GetWithValueTask(
                    viewModel.ViewModelKey,
                    previousViewModel => (previousViewModel with
                    {
                        // Clear the SeenModelRenderStateKeys because one needs to recalculate the virtualization result now that the options have changed.
                        SeenModelRenderStateKeysBag = new(),
                        VirtualizationResult = previousViewModel.VirtualizationResult with
                        {
                            CharAndRowMeasurements = characterWidthAndRowHeight
                        },
                    }));
            }));
        }
    }
}

