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
            Key<TextEditorViewModel> viewModelKey,
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
            ResourceUri modelResourceUri,
            Key<TextEditorViewModel> viewModelKey,
            Key<TextEditorCursor> cursorKey);

        /// <summary>
        /// If one wants to guarantee that the state is up to date use <see cref="GetMoveCursorTask"/>
        /// instead of this method. This is because, the <see cref="ITextEditorService"/> will provide
        /// you the latest instance of the given <see cref="TextEditorCursor"/>. As opposed to whatever
        /// instance of the <see cref="TextEditorCursorModifier"/> you have at time of enqueueing.
        /// <br/><br/>
        /// This method is needed however, because if one wants to arbitrarily create a cursor that does not
        /// map to the view model's cursors, then one would use this method. Since an attempt to map
        /// the cursor key would come back as the cursor not existing.
        /// </summary>
        public TextEditorEdit GetMoveCursorUnsafeTask(
            KeyboardEventArgs keyboardEventArgs,
            ResourceUri modelResourceUri,
            Key<TextEditorViewModel> viewModelKey,
            TextEditorCursorModifier primaryCursor);

        public TextEditorEdit GetCursorMovePageTopTask(
            ResourceUri modelResourceUri,
            Key<TextEditorViewModel> viewModelKey,
            Key<TextEditorCursor> cursorKey);

        /// <summary>
        /// If one wants to guarantee that the state is up to date use <see cref="GetCursorMovePageTopTask"/>
        /// instead of this method. This is because, the <see cref="ITextEditorService"/> will provide
        /// you the latest instance of the given <see cref="TextEditorCursor"/>. As opposed to whatever
        /// instance of the <see cref="TextEditorCursorModifier"/> you have at time of enqueueing.
        /// <br/><br/>
        /// This method is needed however, because if one wants to arbitrarily create a cursor that does not
        /// map to the view model's cursors, then one would use this method. Since an attempt to map
        /// the cursor key would come back as the cursor not existing.
        /// </summary>
        public TextEditorEdit GetCursorMovePageTopUnsafeTask(
            ResourceUri modelResourceUri,
            Key<TextEditorViewModel> viewModelKey,
            TextEditorCursorModifier primaryCursor);

        public TextEditorEdit GetCursorMovePageBottomTask(
            ResourceUri modelResourceUri,
            Key<TextEditorViewModel> viewModelKey,
            Key<TextEditorCursor> cursorKey);

        public TextEditorEdit GetCursorMovePageBottomUnsafeTask(
            ResourceUri modelResourceUri,
            Key<TextEditorViewModel> viewModelKey,
            TextEditorCursorModifier cursorModifier);

        public TextEditorEdit GetCalculateVirtualizationResultTask(
            ResourceUri modelResourceUri,
            Key<TextEditorViewModel> viewModelKey,
            TextEditorMeasurements? textEditorMeasurements,
            Key<TextEditorCursor> cursorKey,
            CancellationToken cancellationToken);

        public TextEditorEdit GetRemeasureTask(
            ResourceUri modelResourceUri,
            Key<TextEditorViewModel> viewModelKey,
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
            Key<TextEditorViewModel> viewModelKey,
            Func<TextEditorViewModel, Task<Func<TextEditorViewModel, TextEditorViewModel>>> withFuncWrap)
        {
            return async editContext =>
            {
                var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);

                if (viewModelModifier is null)
                    return;

                _dispatcher.Dispatch(new TextEditorViewModelState.SetViewModelWithAction(
                    viewModelKey,
                    await withFuncWrap.Invoke(viewModelModifier.ViewModel),
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
            ResourceUri modelResourceUri,
            Key<TextEditorViewModel> viewModelKey,
            Key<TextEditorCursor> cursorKey)
        {
            return editContext =>
            {
                var modelModifier = editContext.GetModelModifier(modelResourceUri);
                var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);

                if (modelModifier is null || viewModelModifier is null)
                    return Task.CompletedTask;

                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier.ViewModel);
                var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (cursorModifierBag is null || primaryCursorModifier is null)
                    return Task.CompletedTask;

                var cursorModifier = cursorModifierBag.CursorModifierBag.FirstOrDefault(x => x.Key == cursorKey);

                if (cursorModifier is null)
                    return Task.CompletedTask;

                return GetMoveCursorUnsafeTask(keyboardEventArgs, modelResourceUri, viewModelKey, cursorModifier)
                    .Invoke(editContext);
            };
        }

        public TextEditorEdit GetMoveCursorUnsafeTask(
            KeyboardEventArgs keyboardEventArgs,
            ResourceUri modelResourceUri,
            Key<TextEditorViewModel> viewModelKey,
            TextEditorCursorModifier cursorModifier)
        {
            return editContext =>
            {
                var modelModifier = editContext.GetModelModifier(modelResourceUri);
                var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);

                if (modelModifier is null || viewModelModifier is null)
                    return Task.CompletedTask;

                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier.ViewModel);

                void MutateIndexCoordinatesAndPreferredColumnIndex(int columnIndex)
                {
                    cursorModifier.ColumnIndex = columnIndex;
                    cursorModifier.PreferredColumnIndex = columnIndex;
                }

                if (keyboardEventArgs.ShiftKey)
                {
                    if (cursorModifier.SelectionAnchorPositionIndex is null ||
                        cursorModifier.SelectionEndingPositionIndex == cursorModifier.SelectionAnchorPositionIndex)
                    {
                        var positionIndex = modelModifier.GetPositionIndex(
                            cursorModifier.RowIndex,
                            cursorModifier.ColumnIndex);

                        cursorModifier.SelectionAnchorPositionIndex = positionIndex;
                    }
                }
                else
                {
                    cursorModifier.SelectionAnchorPositionIndex = null;
                }

                int lengthOfRow = 0; // This variable is used in multiple switch cases.

                switch (keyboardEventArgs.Key)
                {
                    case KeyboardKeyFacts.MovementKeys.ARROW_LEFT:
                        if (TextEditorSelectionHelper.HasSelectedText(cursorModifier) &&
                            !keyboardEventArgs.ShiftKey)
                        {
                            var selectionBounds = TextEditorSelectionHelper.GetSelectionBounds(cursorModifier);

                            var lowerRowMetaData = modelModifier.FindRowInformation(
                                selectionBounds.lowerPositionIndexInclusive);

                            cursorModifier.RowIndex = lowerRowMetaData.rowIndex;

                            cursorModifier.ColumnIndex = selectionBounds.lowerPositionIndexInclusive -
                                lowerRowMetaData.rowStartPositionIndex;
                        }
                        else
                        {
                            if (cursorModifier.ColumnIndex == 0)
                            {
                                if (cursorModifier.RowIndex != 0)
                                {
                                    cursorModifier.RowIndex--;

                                    lengthOfRow = modelModifier.GetLengthOfRow(cursorModifier.RowIndex);

                                    MutateIndexCoordinatesAndPreferredColumnIndex(lengthOfRow);
                                }
                            }
                            else
                            {
                                if (keyboardEventArgs.CtrlKey)
                                {
                                    var columnIndexOfCharacterWithDifferingKind = modelModifier.GetColumnIndexOfCharacterWithDifferingKind(
                                        cursorModifier.RowIndex,
                                        cursorModifier.ColumnIndex,
                                        true);

                                    if (columnIndexOfCharacterWithDifferingKind == -1)
                                        MutateIndexCoordinatesAndPreferredColumnIndex(0);
                                    else
                                        MutateIndexCoordinatesAndPreferredColumnIndex(columnIndexOfCharacterWithDifferingKind);
                                }
                                else
                                {
                                    MutateIndexCoordinatesAndPreferredColumnIndex(cursorModifier.ColumnIndex - 1);
                                }
                            }
                        }

                        break;
                    case KeyboardKeyFacts.MovementKeys.ARROW_DOWN:
                        if (cursorModifier.RowIndex < modelModifier.RowCount - 1)
                        {
                            cursorModifier.RowIndex++;

                            lengthOfRow = modelModifier.GetLengthOfRow(cursorModifier.RowIndex);

                            cursorModifier.ColumnIndex = lengthOfRow < cursorModifier.PreferredColumnIndex
                                ? lengthOfRow
                                : cursorModifier.PreferredColumnIndex;
                        }

                        break;
                    case KeyboardKeyFacts.MovementKeys.ARROW_UP:
                        if (cursorModifier.RowIndex > 0)
                        {
                            cursorModifier.RowIndex--;

                            lengthOfRow = modelModifier.GetLengthOfRow(cursorModifier.RowIndex);

                            cursorModifier.ColumnIndex = lengthOfRow < cursorModifier.PreferredColumnIndex
                                ? lengthOfRow
                                : cursorModifier.PreferredColumnIndex;
                        }

                        break;
                    case KeyboardKeyFacts.MovementKeys.ARROW_RIGHT:
                        if (TextEditorSelectionHelper.HasSelectedText(cursorModifier) && !keyboardEventArgs.ShiftKey)
                        {
                            var selectionBounds = TextEditorSelectionHelper.GetSelectionBounds(cursorModifier);

                            var upperRowMetaData = modelModifier.FindRowInformation(selectionBounds.upperPositionIndexExclusive);

                            cursorModifier.RowIndex = upperRowMetaData.rowIndex;

                            if (cursorModifier.RowIndex >= modelModifier.RowCount)
                            {
                                cursorModifier.RowIndex = modelModifier.RowCount - 1;

                                var upperRowLength = modelModifier.GetLengthOfRow(cursorModifier.RowIndex);

                                cursorModifier.ColumnIndex = upperRowLength;
                            }
                            else
                            {
                                cursorModifier.ColumnIndex =
                                    selectionBounds.upperPositionIndexExclusive - upperRowMetaData.rowStartPositionIndex;
                            }
                        }
                        else
                        {
                            lengthOfRow = modelModifier.GetLengthOfRow(cursorModifier.RowIndex);

                            if (cursorModifier.ColumnIndex == lengthOfRow &&
                                cursorModifier.RowIndex < modelModifier.RowCount - 1)
                            {
                                MutateIndexCoordinatesAndPreferredColumnIndex(0);
                                cursorModifier.RowIndex++;
                            }
                            else if (cursorModifier.ColumnIndex != lengthOfRow)
                            {
                                if (keyboardEventArgs.CtrlKey)
                                {
                                    var columnIndexOfCharacterWithDifferingKind = modelModifier.GetColumnIndexOfCharacterWithDifferingKind(
                                        cursorModifier.RowIndex,
                                        cursorModifier.ColumnIndex,
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
                                    MutateIndexCoordinatesAndPreferredColumnIndex(cursorModifier.ColumnIndex + 1);
                                }
                            }
                        }

                        break;
                    case KeyboardKeyFacts.MovementKeys.HOME:
                        if (keyboardEventArgs.CtrlKey)
                            cursorModifier.RowIndex = 0;

                        MutateIndexCoordinatesAndPreferredColumnIndex(0);

                        break;
                    case KeyboardKeyFacts.MovementKeys.END:
                        if (keyboardEventArgs.CtrlKey)
                            cursorModifier.RowIndex = modelModifier.RowCount - 1;

                        lengthOfRow = modelModifier.GetLengthOfRow(cursorModifier.RowIndex);

                        MutateIndexCoordinatesAndPreferredColumnIndex(lengthOfRow);

                        break;
                }

                if (keyboardEventArgs.ShiftKey)
                {
                    cursorModifier.SelectionEndingPositionIndex = modelModifier.GetPositionIndex(
                        cursorModifier.RowIndex,
                        cursorModifier.ColumnIndex);
                }

                return Task.CompletedTask;
            };
        }

        public TextEditorEdit GetCursorMovePageTopTask(
            ResourceUri modelResourceUri,
            Key<TextEditorViewModel> viewModelKey,
            Key<TextEditorCursor> cursorKey)
        {
            return editContext =>
            {
                var modelModifier = editContext.GetModelModifier(modelResourceUri);
                var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);

                if (modelModifier is null || viewModelModifier is null)
                    return Task.CompletedTask;

                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier.ViewModel);
                var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (cursorModifierBag is null || primaryCursorModifier is null)
                    return Task.CompletedTask;

                var cursorModifier = cursorModifierBag.CursorModifierBag.FirstOrDefault(x => x.Key == cursorKey);

                if (cursorModifier is null)
                    return Task.CompletedTask;

                return GetCursorMovePageTopUnsafeTask(modelResourceUri, viewModelKey, cursorModifier)
                    .Invoke(editContext);
            };
        }
            
        public TextEditorEdit GetCursorMovePageTopUnsafeTask(
            ResourceUri modelResourceUri,
            Key<TextEditorViewModel> viewModelKey,
            TextEditorCursorModifier cursorModifier)
        {
            return editContext =>
            {
                var modelModifier = editContext.GetModelModifier(modelResourceUri);
                var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);

                if (modelModifier is null || viewModelModifier is null)
                    return Task.CompletedTask;

                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier.ViewModel);
                var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (viewModelModifier.ViewModel.VirtualizationResult?.EntryBag.Any() ?? false)
                {
                    var firstEntry = viewModelModifier.ViewModel.VirtualizationResult.EntryBag.First();

                    cursorModifier.RowIndex = firstEntry.Index;
                    cursorModifier.ColumnIndex = 0;
                }

                return Task.CompletedTask;
            };
        }

        public TextEditorEdit GetCursorMovePageBottomTask(
            ResourceUri modelResourceUri,
            Key<TextEditorViewModel> viewModelKey,
            Key<TextEditorCursor> cursorKey)
        {
            return editContext =>
            {
                var modelModifier = editContext.GetModelModifier(modelResourceUri);
                var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);

                if (modelModifier is null || viewModelModifier is null)
                    return Task.CompletedTask;

                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier.ViewModel);
                var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (cursorModifierBag is null || primaryCursorModifier is null)
                    return Task.CompletedTask;

                var cursorModifier = cursorModifierBag.CursorModifierBag.FirstOrDefault(x => x.Key == cursorKey);

                if (cursorModifier is null)
                    return Task.CompletedTask;

                return GetCursorMovePageBottomUnsafeTask(modelResourceUri, viewModelKey, cursorModifier)
                    .Invoke(editContext);
            };
        }

        public TextEditorEdit GetCursorMovePageBottomUnsafeTask(
            ResourceUri modelResourceUri,
            Key<TextEditorViewModel> viewModelKey,
            TextEditorCursorModifier cursorModifier)
        {
            return editContext =>
            {
                var modelModifier = editContext.GetModelModifier(modelResourceUri);
                var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);

                if (modelModifier is null || viewModelModifier is null)
                    return Task.CompletedTask;

                if ((viewModelModifier.ViewModel.VirtualizationResult?.EntryBag.Any() ?? false))
                {
                    var lastEntry = viewModelModifier.ViewModel.VirtualizationResult.EntryBag.Last();
                    var lastEntriesRowLength = modelModifier.GetLengthOfRow(lastEntry.Index);

                    cursorModifier.RowIndex = lastEntry.Index;
                    cursorModifier.ColumnIndex = lastEntriesRowLength;
                }

                return Task.CompletedTask;
            };
        }

        public TextEditorEdit GetCalculateVirtualizationResultTask(
            ResourceUri modelResourceUri,
            Key<TextEditorViewModel> viewModelKey,
            TextEditorMeasurements? textEditorMeasurements,
            Key<TextEditorCursor> cursorKey,
            CancellationToken cancellationToken)
        {
            return async editContext =>
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                var modelModifier = editContext.GetModelModifier(modelResourceUri);
                var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);

                if (modelModifier is null || viewModelModifier is null)
                    return;

                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier.ViewModel);
                var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (cursorModifierBag is null || primaryCursorModifier is null)
                    return;

                // Return because the UI still needs to be measured.
                if (!viewModelModifier.ViewModel.SeenOptionsRenderStateKeysBag.Any())
                    return;

                await viewModelModifier.ViewModel.ThrottleCalculateVirtualizationResult.FireAsync((Func<CancellationToken, Task>)(async _ =>
                {
                    if (modelModifier is null)
                        return;

                    // TODO: Should this '_trackingOfUniqueIdentifiersLock' logic when in regards to the TextEditorModel be removed? The issue is that when scrolling the TextEditorModel would show up in the HashSet and therefore the calculation of the virtualization result would not occur.
                    //
                    //lock (_trackingOfUniqueIdentifiersLock)
                    //{
                    //    if (SeenModelRenderStateKeys.Contains(model.RenderStateKey))
                    //        return;
                    //}

                    var localCharacterWidthAndRowHeight = viewModelModifier.ViewModel.VirtualizationResult.CharAndRowMeasurements;

                    textEditorMeasurements = await _textEditorService.ViewModelApi.GetTextEditorMeasurementsAsync(
                        viewModelModifier.ViewModel.BodyElementId);

                    viewModelModifier.ViewModel.MostRecentTextEditorMeasurements = textEditorMeasurements;

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

                        if (verticalStartingIndex + verticalTake > modelModifier.RowEndingPositionsBag.Count)
                            verticalTake = modelModifier.RowEndingPositionsBag.Count - verticalStartingIndex;

                        verticalTake = Math.Max(0, verticalTake);
                    }

                    var horizontalStartingIndex = (int)Math.Floor(
                        textEditorMeasurements.ScrollLeft /
                        localCharacterWidthAndRowHeight.CharacterWidth);

                    var horizontalTake = (int)Math.Ceiling(
                        textEditorMeasurements.Width /
                        localCharacterWidthAndRowHeight.CharacterWidth);

                    var virtualizedEntryBag = modelModifier
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

                                var tabsOnSameRowBeforeCursor = modelModifier.GetTabsCountOnSameRowBeforeCursor(
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

                                var tabsOnSameRowBeforeCursor = modelModifier.GetTabsCountOnSameRowBeforeCursor(
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

                    var totalWidth = modelModifier.MostCharactersOnASingleRowTuple.rowLength *
                        localCharacterWidthAndRowHeight.CharacterWidth;

                    var totalHeight = modelModifier.RowEndingPositionsBag.Count *
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

                    lock (viewModelModifier.ViewModel.TrackingOfUniqueIdentifiersLock)
                    {
                        if (viewModelModifier.ViewModel.SeenModelRenderStateKeysBag.Count > TextEditorViewModel.ClearTrackingOfUniqueIdentifiersWhenCountIs)
                            viewModelModifier.ViewModel.SeenModelRenderStateKeysBag.Clear();

                        viewModelModifier.ViewModel.SeenModelRenderStateKeysBag.Add(modelModifier.RenderStateKey);
                    }

                    await GetWithValueTask(
                            viewModelModifier.ViewModel.ViewModelKey,
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
            Key<TextEditorViewModel> viewModelKey,
            string measureCharacterWidthAndRowHeightElementId,
            int countOfTestCharacters,
            CancellationToken cancellationToken)
        {
            return async editContext =>
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                var modelModifier = editContext.GetModelModifier(modelResourceUri);
                var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);

                if (modelModifier is null || viewModelModifier is null)
                    return;

                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier.ViewModel);
                var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (cursorModifierBag is null || primaryCursorModifier is null)
                    return;

                var options = _textEditorService.OptionsApi.GetOptions();

                await viewModelModifier.ViewModel.ThrottleRemeasure.FireAsync(async _ =>
                {
                    lock (viewModelModifier.ViewModel.TrackingOfUniqueIdentifiersLock)
                    {
                        if (viewModelModifier.ViewModel.SeenOptionsRenderStateKeysBag.Contains(options.RenderStateKey))
                            return;
                    }

                    var characterWidthAndRowHeight = await _textEditorService.ViewModelApi.MeasureCharacterWidthAndRowHeightAsync(
                        measureCharacterWidthAndRowHeightElementId,
                        countOfTestCharacters);

                    viewModelModifier.ViewModel.VirtualizationResult.CharAndRowMeasurements = characterWidthAndRowHeight;

                    lock (viewModelModifier.ViewModel.TrackingOfUniqueIdentifiersLock)
                    {
                        if (viewModelModifier.ViewModel.SeenOptionsRenderStateKeysBag.Count > TextEditorViewModel.ClearTrackingOfUniqueIdentifiersWhenCountIs)
                            viewModelModifier.ViewModel.SeenOptionsRenderStateKeysBag.Clear();

                        viewModelModifier.ViewModel.SeenOptionsRenderStateKeysBag.Add(options.RenderStateKey);
                    }

                    await GetWithValueTask(
                            viewModelModifier.ViewModel.ViewModelKey,
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