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
using Luthetus.TextEditor.RazorLib.Commands.Models;
using static Luthetus.TextEditor.RazorLib.Commands.Models.TextEditorCommand;

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
        public ModificationTask FocusPrimaryCursorAsync(string primaryCursorContentId);
        public void FocusPrimaryCursorEnqueue(string primaryCursorContentId);

        public ModificationTask MutateScrollHorizontalPositionAsync(string bodyElementId, string gutterElementId, double pixels);
        public void MutateScrollHorizontalPositionEnqueue(string bodyElementId, string gutterElementId, double pixels);
        
        public ModificationTask MutateScrollVerticalPositionAsync(string bodyElementId, string gutterElementId, double pixels);
        public void MutateScrollVerticalPositionEnqueue(string bodyElementId, string gutterElementId, double pixels);
        
        public ModificationTask SetGutterScrollTopAsync(string gutterElementId, double scrollTopInPixels);
        public void SetGutterScrollTopEnqueue(string gutterElementId, double scrollTopInPixels);
        
        public ModificationTask SetScrollPositionAsync(
            string bodyElementId,
            string gutterElementId,
            double? scrollLeftInPixels,
            double? scrollTopInPixels);
        public void SetScrollPositionEnqueue(
            string bodyElementId,
            string gutterElementId,
            double? scrollLeftInPixels,
            double? scrollTopInPixels);

        public ModificationTask WithValueAsync(
            Key<TextEditorViewModel> viewModelKey,
            Func<TextEditorViewModel, TextEditorViewModel> withFunc);
        public void WithValueEnqueue(
            Key<TextEditorViewModel> viewModelKey,
            Func<TextEditorViewModel, TextEditorViewModel> withFunc);

        public ModificationTask WithTaskAsync(
            Key<TextEditorViewModel> viewModelKey,
            Func<TextEditorViewModel, Task<Func<TextEditorViewModel, TextEditorViewModel>>> withFuncWrap);
        public void WithTaskEnqueue(
            Key<TextEditorViewModel> viewModelKey,
            Func<TextEditorViewModel, Task<Func<TextEditorViewModel, TextEditorViewModel>>> withFuncWrap);

        public ModificationTask MoveCursorAsync(
            KeyboardEventArgs keyboardEventArgs,
            ResourceUri modelResourceUri,
            Key<TextEditorViewModel> viewModelKey,
            Key<TextEditorCursor> cursorKey);
        public void MoveCursorEnqueue(
            KeyboardEventArgs keyboardEventArgs,
            ResourceUri modelResourceUri,
            Key<TextEditorViewModel> viewModelKey,
            Key<TextEditorCursor> cursorKey);

        public ModificationTask CursorMovePageTopAsync(
            ResourceUri modelResourceUri,
            Key<TextEditorViewModel> viewModelKey,
            Key<TextEditorCursor> cursorKey);
        public void CursorMovePageTopEnqueue(
            ResourceUri modelResourceUri,
            Key<TextEditorViewModel> viewModelKey,
            Key<TextEditorCursor> cursorKey);

        public ModificationTask CursorMovePageBottomAsync(
            ResourceUri modelResourceUri,
            Key<TextEditorViewModel> viewModelKey,
            Key<TextEditorCursor> cursorKey);
        public void CursorMovePageBottomEnqueue(
            ResourceUri modelResourceUri,
            Key<TextEditorViewModel> viewModelKey,
            Key<TextEditorCursor> cursorKey);
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

        /// <summary>(2023-06-03) Previously this logic was in the TextEditorCursorDisplay itself. The Task.Run() would get re-executed upon each cancellation. With this version, the Task.Run() session is re-used with the while loop. As well, all the text editor cursors are blinking in sync.</summary>
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
        public ModificationTask WithValueAsync(
            Key<TextEditorViewModel> viewModelKey,
            Func<TextEditorViewModel, TextEditorViewModel> withFunc) =>
                (_, _, _, _, _) =>
                {
                    _dispatcher.Dispatch(new TextEditorViewModelState.SetViewModelWithAction(
                        viewModelKey,
                        withFunc));

                    return Task.CompletedTask;
                };

        public void WithValueEnqueue(
            Key<TextEditorViewModel> viewModelKey,
            Func<TextEditorViewModel, TextEditorViewModel> withFunc)
        {
            var commandArgs = new TextEditorCommandArgs(
                null, viewModelKey, false, null,
                _textEditorService, null, null, null, null, null, null);

            _textEditorService.EnqueueModification(
                nameof(WithValueAsync),
                commandArgs,
                WithValueAsync(viewModelKey, withFunc));
        }

        public ModificationTask WithTaskAsync(
            Key<TextEditorViewModel> viewModelKey,
            Func<TextEditorViewModel, Task<Func<TextEditorViewModel, TextEditorViewModel>>> withFuncWrap) =>
                async (_, _, viewModel, _, _) =>
                {
                    _dispatcher.Dispatch(new TextEditorViewModelState.SetViewModelWithAction(
                        viewModelKey,
                        await withFuncWrap.Invoke(viewModel)));
                };

        public void WithTaskEnqueue(
            Key<TextEditorViewModel> viewModelKey,
            Func<TextEditorViewModel, Task<Func<TextEditorViewModel, TextEditorViewModel>>> withFuncWrap) 
        {
            var commandArgs = new TextEditorCommandArgs(
                null, viewModelKey, false, null,
                _textEditorService, null, null, null, null, null, null);

            _textEditorService.EnqueueModification(
                nameof(WithTaskAsync),
                commandArgs,
                WithTaskAsync(viewModelKey, withFuncWrap));
        }

        /// <summary>
        /// If a parameter is null the JavaScript will not modify that value
        /// </summary>
        public ModificationTask SetScrollPositionAsync(
            string bodyElementId,
            string gutterElementId,
            double? scrollLeftInPixels,
            double? scrollTopInPixels) =>
                async (_, _, _, _, _) =>
                {
                    await _jsRuntime.InvokeVoidAsync("luthetusTextEditor.setScrollPosition",
                        bodyElementId,
                        gutterElementId,
                        scrollLeftInPixels,
                        scrollTopInPixels);
                };

        /// <summary>
        /// If a parameter is null the JavaScript will not modify that value
        /// </summary>
        public void SetScrollPositionEnqueue(
            string bodyElementId,
            string gutterElementId,
            double? scrollLeftInPixels,
            double? scrollTopInPixels)
        {
            var commandArgs = new TextEditorCommandArgs(
                null, Key<TextEditorViewModel>.Empty, false, null,
                _textEditorService, null, null, null, null, null, null);

            _textEditorService.EnqueueModification(
                nameof(SetScrollPositionAsync),
                commandArgs,
                SetScrollPositionAsync(
                    bodyElementId,
                    gutterElementId,
                    scrollLeftInPixels,
                    scrollTopInPixels));
        }

        public ModificationTask SetGutterScrollTopAsync(
            string gutterElementId,
            double scrollTopInPixels) =>
                async (_, _, _, _, _) =>
                {
                    await _jsRuntime.InvokeVoidAsync("luthetusTextEditor.setGutterScrollTop",
                        gutterElementId,
                        scrollTopInPixels);
                };

        public void SetGutterScrollTopEnqueue(
            string gutterElementId,
            double scrollTopInPixels)
        {
            var commandArgs = new TextEditorCommandArgs(
                null, Key<TextEditorViewModel>.Empty, false, null,
                _textEditorService, null, null, null, null, null, null);

            _textEditorService.EnqueueModification(
                nameof(SetGutterScrollTopAsync),
                commandArgs,
                SetGutterScrollTopAsync(gutterElementId, scrollTopInPixels));
        }

        public ModificationTask MutateScrollVerticalPositionAsync(
            string bodyElementId,
            string gutterElementId,
            double pixels) =>
                async (_, _, _, _, _) =>
                {
                    await _jsRuntime.InvokeVoidAsync("luthetusTextEditor.mutateScrollVerticalPositionByPixels",
                        bodyElementId,
                        gutterElementId,
                        pixels);
                };

        public void MutateScrollVerticalPositionEnqueue(
            string bodyElementId,
            string gutterElementId,
            double pixels)
        {
            var commandArgs = new TextEditorCommandArgs(
                null, Key<TextEditorViewModel>.Empty, false, null,
                _textEditorService, null, null, null, null, null, null);

            _textEditorService.EnqueueModification(
                nameof(MutateScrollVerticalPositionAsync),
                commandArgs,
                MutateScrollVerticalPositionAsync(bodyElementId, gutterElementId, pixels));
        }

        public ModificationTask MutateScrollHorizontalPositionAsync(
            string bodyElementId,
            string gutterElementId,
            double pixels) => 
                async (_, _, _, _, _) =>
                {
                    await _jsRuntime.InvokeVoidAsync("luthetusTextEditor.mutateScrollHorizontalPositionByPixels",
                        bodyElementId,
                        gutterElementId,
                        pixels);
                };

        public void MutateScrollHorizontalPositionEnqueue(
            string bodyElementId,
            string gutterElementId,
            double pixels)
        {
            var commandArgs = new TextEditorCommandArgs(
                null, Key<TextEditorViewModel>.Empty, false, null,
                _textEditorService, null, null, null, null, null, null);

            _textEditorService.EnqueueModification(
                nameof(MutateScrollHorizontalPositionAsync),
                commandArgs,
                MutateScrollHorizontalPositionAsync(bodyElementId, gutterElementId, pixels));
        }

        public ModificationTask FocusPrimaryCursorAsync(string primaryCursorContentId) =>
            async (_, _, _, _, _) =>
            {
                await _jsRuntime.InvokeVoidAsync("luthetusTextEditor.focusHtmlElementById",
                    primaryCursorContentId);
            };

        public void FocusPrimaryCursorEnqueue(string primaryCursorContentId)
        {
            var commandArgs = new TextEditorCommandArgs(
                null, Key<TextEditorViewModel>.Empty, false, null,
                _textEditorService, null, null, null, null, null, null);

            _textEditorService.EnqueueModification(
                nameof(FocusPrimaryCursorAsync),
                commandArgs,
                FocusPrimaryCursorAsync(primaryCursorContentId));
        }

        public ModificationTask MoveCursorAsync(
            KeyboardEventArgs keyboardEventArgs,
            ResourceUri modelResourceUri,
            Key<TextEditorViewModel> viewModelKey,
            Key<TextEditorCursor> cursorKey) =>
                (_, model, _, refreshCursorsRequest, primaryCursor) =>
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

        public void MoveCursorEnqueue(
            KeyboardEventArgs keyboardEventArgs,
            ResourceUri modelResourceUri,
            Key<TextEditorViewModel> viewModelKey,
            Key<TextEditorCursor> cursorKey)
        {
            var commandArgs = new TextEditorCommandArgs(
                modelResourceUri, viewModelKey, false, null,
                _textEditorService, null, null, null, null, null, null);

            _textEditorService.EnqueueModification(
                nameof(MoveCursorAsync),
                commandArgs,
                MoveCursorAsync(keyboardEventArgs, modelResourceUri, viewModelKey, cursorKey));
        }

        public ModificationTask CursorMovePageTopAsync(
            ResourceUri modelResourceUri,
            Key<TextEditorViewModel> viewModelKey,
            Key<TextEditorCursor> cursorKey) =>
                (_, _, viewModel, refreshCursorsRequest, primaryCursor) =>
                {
                    if (viewModel.VirtualizationResult?.EntryBag.Any() ?? false)
                    {
                        var firstEntry = viewModel.VirtualizationResult.EntryBag.First();

                        primaryCursor.RowIndex = firstEntry.Index;
                        primaryCursor.ColumnIndex = 0;
                    }

                    return Task.CompletedTask;
                };

        public void CursorMovePageTopEnqueue(
            ResourceUri modelResourceUri,
            Key<TextEditorViewModel> viewModelKey,
            Key<TextEditorCursor> cursorKey)
        {
            var commandArgs = new TextEditorCommandArgs(
                modelResourceUri, viewModelKey, false, null,
                _textEditorService, null, null, null, null, null, null);

            _textEditorService.EnqueueModification(
                nameof(CursorMovePageTopAsync),
                commandArgs,
                CursorMovePageTopAsync(modelResourceUri, viewModelKey, cursorKey));
        }

        public ModificationTask CursorMovePageBottomAsync(
            ResourceUri modelResourceUri,
            Key<TextEditorViewModel> viewModelKey,
            Key<TextEditorCursor> cursorKey) =>
                (_, model, viewModel, refreshCursorsRequest, primaryCursor) =>
                {
                    if ((viewModel.VirtualizationResult?.EntryBag.Any() ?? false))
                    {
                        var lastEntry = viewModel.VirtualizationResult.EntryBag.Last();
                        var lastEntriesRowLength = model.GetLengthOfRow(lastEntry.Index);

                        var cursor = refreshCursorsRequest.CursorBag.FirstOrDefault(x => x.Key == cursorKey);

                        if (cursor is null)
                            return Task.CompletedTask;

                        cursor.RowIndex = lastEntry.Index;
                        cursor.ColumnIndex = lastEntriesRowLength;
                    }

                    return Task.CompletedTask;
                };

        public void CursorMovePageBottomEnqueue(
            ResourceUri modelResourceUri,
            Key<TextEditorViewModel> viewModelKey,
            Key<TextEditorCursor> cursorKey)
        {
            var commandArgs = new TextEditorCommandArgs(
                modelResourceUri, viewModelKey, false, null,
                _textEditorService, null, null, null, null, null, null);

            _textEditorService.EnqueueModification(
                nameof(CursorMovePageBottomAsync),
                commandArgs,
                CursorMovePageBottomAsync(modelResourceUri, viewModelKey, cursorKey));
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