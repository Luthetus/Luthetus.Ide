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
        #endregion

        #region UPDATE_METHODS
        public Task FocusPrimaryCursorAsync(string primaryCursorContentId, bool shouldEnqueue = true);
        public Task MutateScrollHorizontalPositionAsync(string bodyElementId, string gutterElementId, double pixels, bool shouldEnqueue = true);
        public Task MutateScrollVerticalPositionAsync(string bodyElementId, string gutterElementId, double pixels, bool shouldEnqueue = true);
        public Task SetGutterScrollTopAsync(string gutterElementId, double scrollTopInPixels, bool shouldEnqueue = true);
        
        public Task SetScrollPositionAsync(
            string bodyElementId,
            string gutterElementId,
            double? scrollLeftInPixels,
            double? scrollTopInPixels,
            bool shouldEnqueue = true);

        public void With(
            Key<TextEditorViewModel> viewModelKey,
            Func<TextEditorViewModel, TextEditorViewModel> withFunc,
            bool shouldEnqueue = true);

        public void WithAsync(
               Key<TextEditorViewModel> viewModelKey,
               Func<TextEditorViewModel, Task<Func<TextEditorViewModel, TextEditorViewModel>>> withFuncWrap,
               bool shouldEnqueue = true);

        public void MoveCursor(
            KeyboardEventArgs keyboardEventArgs,
            ResourceUri modelResourceUri,
            Key<TextEditorViewModel> viewModelKey,
            Key<TextEditorCursor> cursorKey,
            bool shouldEnqueue = true);

        public void CursorMovePageTop(
            ResourceUri modelResourceUri,
            Key<TextEditorViewModel> viewModelKey,
            Key<TextEditorCursor> cursorKey,
            bool shouldEnqueue = true);

        public void CursorMovePageBottom(
            ResourceUri modelResourceUri,
            Key<TextEditorViewModel> viewModelKey,
            Key<TextEditorCursor> cursorKey,
            bool shouldEnqueue = true);

        public void SetCursorShouldBlink(bool cursorShouldBlink, bool shouldEnqueue = true);
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
        public void SetCursorShouldBlink(bool cursorShouldBlink, bool shouldEnqueue = true)
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
        public void With(
            Key<TextEditorViewModel> viewModelKey,
            Func<TextEditorViewModel, TextEditorViewModel> withFunc,
            bool shouldEnqueue = true)
        {
            var commandArgs = new TextEditorCommandArgs(
                null, viewModelKey, false, null,
                _textEditorService, null, null, null, null, null, null);

            TextEditorCommand.ModificationTask modificationTask = (_, _, _, _, _) =>
                Task.FromResult(withFunc);

            if (shouldEnqueue)
            {
                _textEditorService.EnqueueModification(
                    nameof(With),
                    commandArgs,
                    modificationTask);
            }
            else
            {
                // TODO: await this
                _textEditorService.ModifyAsync(
                    nameof(With),
                    commandArgs,
                    modificationTask).Wait();
            }
        }

        public void WithAsync(
            Key<TextEditorViewModel> viewModelKey,
            Func<TextEditorViewModel, Task<Func<TextEditorViewModel, TextEditorViewModel>>> withFuncWrap,
            bool shouldEnqueue = true)
        {
            var commandArgs = new TextEditorCommandArgs(
                null, viewModelKey, false, null,
                _textEditorService, null, null, null, null, null, null);

            TextEditorCommand.ModificationTask modificationTask = async (_, _, viewModel, _, _) =>
            {
                _dispatcher.Dispatch(new TextEditorViewModelState.SetViewModelWithAction(
                    viewModelKey,
                    await withFuncWrap.Invoke(viewModel)));
            };

            if (shouldEnqueue)
            {
                _textEditorService.EnqueueModification(
                    nameof(WithAsync),
                    commandArgs,
                    modificationTask);
            }
            else
            {
                // TODO: await this
                _textEditorService.ModifyAsync(
                    nameof(WithAsync),
                    commandArgs,
                    modificationTask).Wait();
            }
        }

        /// <summary>
        /// If a parameter is null the JavaScript will not modify that value
        /// </summary>
        public async Task SetScrollPositionAsync(
            string bodyElementId,
            string gutterElementId,
            double? scrollLeftInPixels,
            double? scrollTopInPixels,
            bool shouldEnqueue = true)
        {
            var commandArgs = new TextEditorCommandArgs(
                null, Key<TextEditorViewModel>.Empty, false, null,
                _textEditorService, null, null, null, null, null, null);

            TextEditorCommand.ModificationTask modificationTask = async(_, _, _, _, _) =>
            {
                await _jsRuntime.InvokeVoidAsync("luthetusTextEditor.setScrollPosition",
                    bodyElementId,
                    gutterElementId,
                    scrollLeftInPixels,
                    scrollTopInPixels);
            };

            if (shouldEnqueue)
            {
                _textEditorService.EnqueueModification(
                    nameof(SetScrollPositionAsync),
                    commandArgs,
                    modificationTask);
            }
            else
            {
                // TODO: await this
                _textEditorService.ModifyAsync(
                    nameof(SetScrollPositionAsync),
                    commandArgs,
                    modificationTask).Wait();
            }
        }

        public async Task SetGutterScrollTopAsync(
            string gutterElementId,
            double scrollTopInPixels,
            bool shouldEnqueue = true)
        {
            var commandArgs = new TextEditorCommandArgs(
                null, Key<TextEditorViewModel>.Empty, false, null,
                _textEditorService, null, null, null, null, null, null);

            TextEditorCommand.ModificationTask modificationTask = async (_, _, _, _, _) =>
            {
                await _jsRuntime.InvokeVoidAsync("luthetusTextEditor.setGutterScrollTop",
                    gutterElementId,
                    scrollTopInPixels);
            };

            if (shouldEnqueue)
            {
                _textEditorService.EnqueueModification(
                    nameof(SetGutterScrollTopAsync),
                    commandArgs,
                    modificationTask);
            }
            else
            {
                // TODO: await this
                _textEditorService.ModifyAsync(
                    nameof(SetGutterScrollTopAsync),
                    commandArgs,
                    modificationTask).Wait();
            }
        }

        public async Task MutateScrollVerticalPositionAsync(
            string bodyElementId,
            string gutterElementId,
            double pixels,
            bool shouldEnqueue = true)
        {
            var commandArgs = new TextEditorCommandArgs(
                null, Key<TextEditorViewModel>.Empty, false, null,
                _textEditorService, null, null, null, null, null, null);

            TextEditorCommand.ModificationTask modificationTask = async (_, _, _, _, _) =>
            {
                await _jsRuntime.InvokeVoidAsync("luthetusTextEditor.mutateScrollVerticalPositionByPixels",
                    bodyElementId,
                    gutterElementId,
                    pixels);
            };

            if (shouldEnqueue)
            {
                _textEditorService.EnqueueModification(
                    nameof(MutateScrollVerticalPositionAsync),
                    commandArgs,
                    modificationTask);
            }
            else
            {
                // TODO: await this
                _textEditorService.ModifyAsync(
                    nameof(MutateScrollVerticalPositionAsync),
                    commandArgs,
                    modificationTask).Wait();
            }
        }

        public async Task MutateScrollHorizontalPositionAsync(
            string bodyElementId,
            string gutterElementId,
            double pixels,
            bool shouldEnqueue = true)
        {
            var commandArgs = new TextEditorCommandArgs(
                null, Key<TextEditorViewModel>.Empty, false, null,
                _textEditorService, null, null, null, null, null, null);

            TextEditorCommand.ModificationTask modificationTask = async (_, _, _, _, _) =>
            {
                await _jsRuntime.InvokeVoidAsync("luthetusTextEditor.mutateScrollHorizontalPositionByPixels",
                    bodyElementId,
                    gutterElementId,
                    pixels);
            };

            if (shouldEnqueue)
            {
                _textEditorService.EnqueueModification(
                    nameof(MutateScrollHorizontalPositionAsync),
                    commandArgs,
                    modificationTask);
            }
            else
            {
                // TODO: await this
                _textEditorService.ModifyAsync(
                    nameof(MutateScrollHorizontalPositionAsync),
                    commandArgs,
                    modificationTask).Wait();
            }
        }

        public async Task FocusPrimaryCursorAsync(
            string primaryCursorContentId,
            bool shouldEnqueue = true)
        {
            var commandArgs = new TextEditorCommandArgs(
                null, Key<TextEditorViewModel>.Empty, false, null,
                _textEditorService, null, null, null, null, null, null);

            TextEditorCommand.ModificationTask modificationTask = async (_, _, _, _, _) =>
            {
                await _jsRuntime.InvokeVoidAsync("luthetusTextEditor.focusHtmlElementById",
                    primaryCursorContentId);
            };

            if (shouldEnqueue)
            {
                _textEditorService.EnqueueModification(
                    nameof(FocusPrimaryCursorAsync),
                    commandArgs,
                    modificationTask);
            }
            else
            {
                // TODO: await this
                _textEditorService.ModifyAsync(
                    nameof(FocusPrimaryCursorAsync),
                    commandArgs,
                    modificationTask).Wait();
            }
        }

        public void MoveCursor(
            KeyboardEventArgs keyboardEventArgs,
            ResourceUri modelResourceUri,
            Key<TextEditorViewModel> viewModelKey,
            Key<TextEditorCursor> cursorKey,
            bool shouldEnqueue = true)
        {
            var commandArgs = new TextEditorCommandArgs(
                null, Key<TextEditorViewModel>.Empty, false, null,
                _textEditorService, null, null, null, null, null, null);

            TextEditorCommand.ModificationTask modificationTask = (_, model, _, refreshCursorsRequest, primaryCursor) =>
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

            if (shouldEnqueue)
            {
                _textEditorService.EnqueueModification(
                    nameof(MoveCursor),
                    commandArgs,
                    modificationTask);
            }
            else
            {
                // TODO: await this
                _textEditorService.ModifyAsync(
                    nameof(MoveCursor),
                    commandArgs,
                    modificationTask).Wait();
            }
        }

        public void CursorMovePageTop(
            ResourceUri modelResourceUri,
            Key<TextEditorViewModel> viewModelKey,
            Key<TextEditorCursor> cursorKey,
            bool shouldEnqueue = true)
        {
            var commandArgs = new TextEditorCommandArgs(
                null, Key<TextEditorViewModel>.Empty, false, null,
                _textEditorService, null, null, null, null, null, null);

            TextEditorCommand.ModificationTask modificationTask = (_, _, viewModel, refreshCursorsRequest, primaryCursor) =>
            {
                if (viewModel.VirtualizationResult?.EntryBag.Any() ?? false)
                {
                    var firstEntry = viewModel.VirtualizationResult.EntryBag.First();

                    primaryCursor.RowIndex = firstEntry.Index;
                    primaryCursor.ColumnIndex = 0;
                }

                return Task.CompletedTask;
            };

            if (shouldEnqueue)
            {
                _textEditorService.EnqueueModification(
                    nameof(CursorMovePageTop),
                    commandArgs,
                    modificationTask);
            }
            else
            {
                // TODO: await this
                _textEditorService.ModifyAsync(
                    nameof(CursorMovePageTop),
                    commandArgs,
                    modificationTask).Wait();
            }
        }

        public void CursorMovePageBottom(
            ResourceUri modelResourceUri,
            Key<TextEditorViewModel> viewModelKey,
            Key<TextEditorCursor> cursorKey,
            bool shouldEnqueue = true)
        {
            var commandArgs = new TextEditorCommandArgs(
                null, Key<TextEditorViewModel>.Empty, false, null,
                _textEditorService, null, null, null, null, null, null);

            TextEditorCommand.ModificationTask modificationTask = (_, model, viewModel, refreshCursorsRequest, primaryCursor) =>
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

            if (shouldEnqueue)
            {
                _textEditorService.EnqueueModification(
                    nameof(CursorMovePageBottom),
                    commandArgs,
                    modificationTask);
            }
            else
            {
                // TODO: await this
                _textEditorService.ModifyAsync(
                    nameof(CursorMovePageBottom),
                    commandArgs,
                    modificationTask).Wait();
            }
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