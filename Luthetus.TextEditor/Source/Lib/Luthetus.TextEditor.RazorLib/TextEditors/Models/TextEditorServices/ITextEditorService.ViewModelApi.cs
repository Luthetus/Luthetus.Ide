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

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;

public partial interface ITextEditorService
{
    public interface ITextEditorViewModelApi
    {
        public void Dispose(Key<TextEditorViewModel> viewModelKey);
        public Task<TextEditorMeasurements> GetTextEditorMeasurementsAsync(string elementId);
        
        public Task<CharAndRowMeasurements> MeasureCharacterWidthAndRowHeightAsync(
            string measureCharacterWidthAndRowHeightElementId,
            int countOfTestCharacters);

        public TextEditorViewModel? GetOrDefault(Key<TextEditorViewModel> viewModelKey);
        public Task FocusPrimaryCursorAsync(string primaryCursorContentId);
        public string? GetAllText(Key<TextEditorViewModel> viewModelKey);
        public TextEditorModel? GetModelOrDefault(Key<TextEditorViewModel> viewModelKey);
        public Task MutateScrollHorizontalPositionAsync(string bodyElementId, string gutterElementId, double pixels);
        public Task MutateScrollVerticalPositionAsync(string bodyElementId, string gutterElementId, double pixels);
        public void Register(Key<TextEditorViewModel> textEditorViewModelKey, ResourceUri resourceUri);
        public Task SetGutterScrollTopAsync(string gutterElementId, double scrollTopInPixels);
        
        public Task SetScrollPositionAsync(
            string bodyElementId,
            string gutterElementId,
            double? scrollLeftInPixels,
            double? scrollTopInPixels);
        
        public void With(
            Key<TextEditorViewModel> viewModelKey,
            Func<TextEditorViewModel, TextEditorViewModel> withFunc);
        
        public void SetCursorShouldBlink(bool cursorShouldBlink);

        public void SetViewModelWith(
               Key<TextEditorViewModel> viewModelKey,
               Func<TextEditorViewModel, Task<Func<TextEditorViewModel, TextEditorViewModel>>> withFuncWrap);

        public void MoveCursor(
            KeyboardEventArgs keyboardEventArgs,
            ResourceUri modelResourceUri,
            Key<TextEditorViewModel> viewModelKey,
            Key<TextEditorCursor> cursorKey);

        public void CursorMovePageTop(
            ResourceUri modelResourceUri,
            Key<TextEditorViewModel> viewModelKey,
            Key<TextEditorCursor> cursorKey);

        public void CursorMovePageBottom(
            ResourceUri modelResourceUri,
            Key<TextEditorViewModel> viewModelKey,
            Key<TextEditorCursor> cursorKey);

        /// <summary>
        /// One should store the result of invoking this method in a variable, then reference that variable.
        /// If one continually invokes this, there is no guarantee that the data had not changed
        /// since the previous invocation.
        /// </summary>
        public ImmutableList<TextEditorViewModel> GetViewModels();

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

        public void With(
            Key<TextEditorViewModel> viewModelKey,
            Func<TextEditorViewModel, TextEditorViewModel> withFunc)
        {
            SetViewModelWith(viewModelKey, _ =>
            {
                return Task.FromResult(withFunc);
            });
        }

        /// <summary>
        /// If a parameter is null the JavaScript will not modify that value
        /// </summary>
        public async Task SetScrollPositionAsync(
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

        public async Task SetGutterScrollTopAsync(string gutterElementId, double scrollTopInPixels)
        {
            await _jsRuntime.InvokeVoidAsync("luthetusTextEditor.setGutterScrollTop",
                gutterElementId,
                scrollTopInPixels);
        }

        public void Register(Key<TextEditorViewModel> textEditorViewModelKey, ResourceUri resourceUri)
        {
            _dispatcher.Dispatch(new TextEditorViewModelState.RegisterAction(
                textEditorViewModelKey,
                resourceUri,
                _textEditorService));
        }

        public async Task MutateScrollVerticalPositionAsync(string bodyElementId, string gutterElementId, double pixels)
        {
            await _jsRuntime.InvokeVoidAsync("luthetusTextEditor.mutateScrollVerticalPositionByPixels",
                bodyElementId,
                gutterElementId,
                pixels);
        }

        public async Task MutateScrollHorizontalPositionAsync(string bodyElementId, string gutterElementId, double pixels)
        {
            await _jsRuntime.InvokeVoidAsync("luthetusTextEditor.mutateScrollHorizontalPositionByPixels",
                bodyElementId,
                gutterElementId,
                pixels);
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

        public async Task FocusPrimaryCursorAsync(string primaryCursorContentId)
        {
            await _jsRuntime.InvokeVoidAsync("luthetusTextEditor.focusHtmlElementById",
                primaryCursorContentId);
        }

        public TextEditorViewModel? GetOrDefault(Key<TextEditorViewModel> textEditorViewModelKey)
        {
            return _textEditorService.ViewModelStateWrap.Value.ViewModelBag.FirstOrDefault(
                x => x.ViewModelKey == textEditorViewModelKey);
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

        public void SetViewModelWith(
               Key<TextEditorViewModel> viewModelKey,
               Func<TextEditorViewModel, Task<Func<TextEditorViewModel, TextEditorViewModel>>> withFuncWrap)
        {
            _backgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(),
                ContinuousBackgroundTaskWorker.GetQueueKey(),
                "Move Cursor",
                async () => await SetViewModelWithAsync(
                    viewModelKey,
                    withFuncWrap));
        }

        public void MoveCursor(
            KeyboardEventArgs keyboardEventArgs,
            ResourceUri modelResourceUri,
            Key<TextEditorViewModel> viewModelKey,
            Key<TextEditorCursor> cursorKey)
        {
            _backgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(),
                ContinuousBackgroundTaskWorker.GetQueueKey(),
                "Move Cursor",
                async () => await MoveCursorAsync(
                    keyboardEventArgs,
                    modelResourceUri,
                    viewModelKey,
                    cursorKey));
        }

        private async Task SetViewModelWithAsync(
            Key<TextEditorViewModel> viewModelKey,
            Func<TextEditorViewModel, Task<Func<TextEditorViewModel, TextEditorViewModel>>> withFuncWrap)
        {
            var viewModel = _viewModelStateWrap.Value.ViewModelBag.First(
                x => x.ViewModelKey == viewModelKey);

            _dispatcher.Dispatch(new TextEditorViewModelState.SetViewModelWithAction(
                viewModelKey,
                await withFuncWrap.Invoke(viewModel)));
        }

        private async Task MoveCursorAsync(
            KeyboardEventArgs keyboardEventArgs,
            ResourceUri modelResourceUri,
            Key<TextEditorViewModel> viewModelKey,
            Key<TextEditorCursor> cursorKey)
        {
            if (!TryGetState(modelResourceUri, viewModelKey, cursorKey,
                             out var inModel, out var inViewModel, out var inCursor))
                return;

            var refRowIndex = inCursor.RowIndex;
            var refColumnIndex = inCursor.ColumnIndex;
            var refPreferredColumnIndex = inCursor.PreferredColumnIndex;
            var refTextEditorSelection = inCursor.Selection;

            void MutateIndexCoordinatesAndPreferredColumnIndex(int columnIndex)
            {
                refColumnIndex = columnIndex;
                refPreferredColumnIndex = columnIndex;
            }

            if (keyboardEventArgs.ShiftKey)
            {
                if (inCursor.Selection.AnchorPositionIndex is null ||
                    inCursor.Selection.EndingPositionIndex == inCursor.Selection.AnchorPositionIndex)
                {
                    var positionIndex = inModel.GetPositionIndex(
                        refRowIndex,
                        refColumnIndex);

                    refTextEditorSelection = refTextEditorSelection with
                    {
                        AnchorPositionIndex = positionIndex
                    };
                }
            }
            else
            {
                refTextEditorSelection = refTextEditorSelection with
                {
                    AnchorPositionIndex = null
                };
            }

            int lengthOfRow = 0; // This variable is used in multiple switch cases.

            switch (keyboardEventArgs.Key)
            {
                case KeyboardKeyFacts.MovementKeys.ARROW_LEFT:
                    if (TextEditorSelectionHelper.HasSelectedText(refTextEditorSelection) &&
                        !keyboardEventArgs.ShiftKey)
                    {
                        var selectionBounds = TextEditorSelectionHelper.GetSelectionBounds(refTextEditorSelection);

                        var lowerRowMetaData = inModel.FindRowInformation(
                            selectionBounds.lowerPositionIndexInclusive);

                        refRowIndex = lowerRowMetaData.rowIndex;

                        refColumnIndex = selectionBounds.lowerPositionIndexInclusive -
                            lowerRowMetaData.rowStartPositionIndex;
                    }
                    else
                    {
                        if (refColumnIndex == 0)
                        {
                            if (refRowIndex != 0)
                            {
                                refRowIndex--;

                                lengthOfRow = inModel.GetLengthOfRow(refRowIndex);

                                MutateIndexCoordinatesAndPreferredColumnIndex(lengthOfRow);
                            }
                        }
                        else
                        {
                            if (keyboardEventArgs.CtrlKey)
                            {
                                var columnIndexOfCharacterWithDifferingKind = inModel.GetColumnIndexOfCharacterWithDifferingKind(
                                    refRowIndex,
                                    refColumnIndex,
                                    true);

                                if (columnIndexOfCharacterWithDifferingKind == -1)
                                    MutateIndexCoordinatesAndPreferredColumnIndex(0);
                                else
                                    MutateIndexCoordinatesAndPreferredColumnIndex(columnIndexOfCharacterWithDifferingKind);
                            }
                            else
                            {
                                MutateIndexCoordinatesAndPreferredColumnIndex(refColumnIndex - 1);
                            }
                        }
                    }

                    break;
                case KeyboardKeyFacts.MovementKeys.ARROW_DOWN:
                    if (refRowIndex < inModel.RowCount - 1)
                    {
                        refRowIndex++;

                        lengthOfRow = inModel.GetLengthOfRow(refRowIndex);

                        refColumnIndex = lengthOfRow < refPreferredColumnIndex
                            ? lengthOfRow
                            : refPreferredColumnIndex;
                    }

                    break;
                case KeyboardKeyFacts.MovementKeys.ARROW_UP:
                    if (refRowIndex > 0)
                    {
                        refRowIndex--;

                        lengthOfRow = inModel.GetLengthOfRow(refRowIndex);

                        refColumnIndex = lengthOfRow < refPreferredColumnIndex
                            ? lengthOfRow
                            : refPreferredColumnIndex;
                    }

                    break;
                case KeyboardKeyFacts.MovementKeys.ARROW_RIGHT:
                    if (TextEditorSelectionHelper.HasSelectedText(refTextEditorSelection) && !keyboardEventArgs.ShiftKey)
                    {
                        var selectionBounds = TextEditorSelectionHelper.GetSelectionBounds(refTextEditorSelection);

                        var upperRowMetaData = inModel.FindRowInformation(selectionBounds.upperPositionIndexExclusive);

                        refRowIndex = upperRowMetaData.rowIndex;

                        if (refRowIndex >= inModel.RowCount)
                        {
                            refRowIndex = inModel.RowCount - 1;

                            var upperRowLength = inModel.GetLengthOfRow(refRowIndex);

                            refColumnIndex = upperRowLength;
                        }
                        else
                        {
                            refColumnIndex =
                                selectionBounds.upperPositionIndexExclusive - upperRowMetaData.rowStartPositionIndex;
                        }
                    }
                    else
                    {
                        lengthOfRow = inModel.GetLengthOfRow(refRowIndex);

                        if (refColumnIndex == lengthOfRow &&
                            refRowIndex < inModel.RowCount - 1)
                        {
                            MutateIndexCoordinatesAndPreferredColumnIndex(0);
                            refRowIndex++;
                        }
                        else if (refColumnIndex != lengthOfRow)
                        {
                            if (keyboardEventArgs.CtrlKey)
                            {
                                var columnIndexOfCharacterWithDifferingKind = inModel.GetColumnIndexOfCharacterWithDifferingKind(
                                    refRowIndex,
                                    refColumnIndex,
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
                                MutateIndexCoordinatesAndPreferredColumnIndex(refColumnIndex + 1);
                            }
                        }
                    }

                    break;
                case KeyboardKeyFacts.MovementKeys.HOME:
                    if (keyboardEventArgs.CtrlKey)
                        refRowIndex = 0;

                    MutateIndexCoordinatesAndPreferredColumnIndex(0);

                    break;
                case KeyboardKeyFacts.MovementKeys.END:
                    if (keyboardEventArgs.CtrlKey)
                        refRowIndex = inModel.RowCount - 1;

                    lengthOfRow = inModel.GetLengthOfRow(refRowIndex);

                    MutateIndexCoordinatesAndPreferredColumnIndex(lengthOfRow);

                    break;
            }

            var outSelection = inCursor.Selection;

            if (keyboardEventArgs.ShiftKey)
            {
                outSelection = outSelection with
                {
                    EndingPositionIndex = inModel.GetPositionIndex(
                        refRowIndex,
                        refColumnIndex)
                };
            }

            var outCursorBag = inViewModel.CursorBag.Replace(inCursor, inCursor with
            {
                RowIndex = refRowIndex,
                ColumnIndex = refColumnIndex,
                PreferredColumnIndex = refPreferredColumnIndex,
                Selection = outSelection,
            });

            _dispatcher.Dispatch(new TextEditorViewModelState.SetViewModelWithAction(
                viewModelKey,
                inState => inState with
                {
                    CursorBag = outCursorBag
                }));
        }

        public void CursorMovePageTop(
            ResourceUri modelResourceUri,
            Key<TextEditorViewModel> viewModelKey,
            Key<TextEditorCursor> cursorKey)
        {
            if (!TryGetState(modelResourceUri, viewModelKey, cursorKey,
                             out var inModel, out var inViewModel, out var inCursor))
                return;

            if (inViewModel.VirtualizationResult?.EntryBag.Any() ?? false)
            {
                var firstEntry = inViewModel.VirtualizationResult.EntryBag.First();

                var outCursorBag = inViewModel.CursorBag.Replace(inCursor, inCursor with
                {
                    RowIndex = firstEntry.Index,
                    ColumnIndex = 0,
                });

                _dispatcher.Dispatch(new TextEditorViewModelState.SetViewModelWithAction(
                    viewModelKey,
                    inState => inState with
                    {
                        CursorBag = outCursorBag
                    }));
            }
        }

        public void CursorMovePageBottom(
            ResourceUri modelResourceUri,
            Key<TextEditorViewModel> viewModelKey,
            Key<TextEditorCursor> cursorKey)
        {
            if (!TryGetState(modelResourceUri, viewModelKey, cursorKey,
                             out var inModel, out var inViewModel, out var inCursor))
                return;

            if ((inViewModel.VirtualizationResult?.EntryBag.Any() ?? false))
            {
                var lastEntry = inViewModel.VirtualizationResult.EntryBag.Last();
                var lastEntriesRowLength = inModel.GetLengthOfRow(lastEntry.Index);

                var outCursorBag = inViewModel.CursorBag.Replace(inCursor, inCursor with
                {
                    RowIndex = lastEntry.Index,
                    ColumnIndex = lastEntriesRowLength,
                });

                _dispatcher.Dispatch(new TextEditorViewModelState.SetViewModelWithAction(
                    viewModelKey,
                    inState => inState with
                    {
                        CursorBag = outCursorBag
                    }));
            }
        }

        /// <summary>
        /// This method suppresses nullability checks because there is a presumption that
        /// one will check if 'false' is returned.<br/>
        /// If 'false' is returned, do not use the out variables.
        /// If 'true' is returned, the out variables are non-null.
        /// </summary>
        private bool TryGetState(
            ResourceUri modelResourceUri,
            Key<TextEditorViewModel> viewModelKey,
            Key<TextEditorCursor> cursorKey,
            out TextEditorModel inModel,
            out TextEditorViewModel inViewModel,
            out TextEditorCursor inCursor)
        {
            inModel = _modelStateWrap.Value.ModelBag.FirstOrDefault(x => x.ResourceUri == modelResourceUri)!;
            inViewModel = _viewModelStateWrap.Value.ViewModelBag.FirstOrDefault(x => x.ViewModelKey == viewModelKey)!;
            inCursor = inViewModel?.CursorBag.FirstOrDefault(x => x.Key == cursorKey)!;

            if (inModel is null || inViewModel is null || inCursor is null)
                return false;

            return true;
        }

        public ImmutableList<TextEditorViewModel> GetViewModels()
        {
            return _textEditorService.ViewModelStateWrap.Value.ViewModelBag;
        }

        public void Dispose(Key<TextEditorViewModel> textEditorViewModelKey)
        {
            _dispatcher.Dispatch(new TextEditorViewModelState.DisposeAction(textEditorViewModelKey));
        }
    }
}