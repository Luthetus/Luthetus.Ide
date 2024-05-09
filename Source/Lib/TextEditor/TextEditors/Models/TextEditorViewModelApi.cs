using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.JsRuntimes.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.JavaScriptObjects.Models;
using Luthetus.TextEditor.RazorLib.JsRuntimes.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using Luthetus.TextEditor.RazorLib.Virtualizations.Models;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

public class TextEditorViewModelApi : ITextEditorViewModelApi
{
    private readonly ITextEditorService _textEditorService;
    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly IState<TextEditorViewModelState> _viewModelStateWrap;
    private readonly IState<TextEditorModelState> _modelStateWrap;
    private readonly IDispatcher _dispatcher;
    private readonly IDialogService _dialogService;

    // TODO: Perhaps do not reference IJSRuntime but instead wrap it in a 'IUiProvider' or something like that. The 'IUiProvider' would then expose methods that allow the TextEditorViewModel to adjust the scrollbars. 
    private readonly IJSRuntime _jsRuntime;

    public TextEditorViewModelApi(
        ITextEditorService textEditorService,
        IBackgroundTaskService backgroundTaskService,
        IState<TextEditorViewModelState> viewModelStateWrap,
        IState<TextEditorModelState> modelStateWrap,
        IJSRuntime jsRuntime,
        IDispatcher dispatcher,
        IDialogService dialogService)
    {
        _textEditorService = textEditorService;
        _backgroundTaskService = backgroundTaskService;
        _viewModelStateWrap = viewModelStateWrap;
        _modelStateWrap = modelStateWrap;
        _jsRuntime = jsRuntime;
        _dispatcher = dispatcher;
        _dialogService = dialogService;
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
    public void Register(
        Key<TextEditorViewModel> viewModelKey,
        ResourceUri resourceUri,
        Category category)
    {
        _dispatcher.Dispatch(new TextEditorViewModelState.RegisterAction(
            viewModelKey,
            resourceUri,
            category,
            _textEditorService,
            _dispatcher,
            _dialogService,
            _jsRuntime));
    }
    #endregion

    #region READ_METHODS
    public TextEditorViewModel? GetOrDefault(Key<TextEditorViewModel> textEditorViewModelKey)
    {
        return _textEditorService.ViewModelStateWrap.Value.ViewModelList.FirstOrDefault(
            x => x.ViewModelKey == textEditorViewModelKey);
    }

    public ImmutableList<TextEditorViewModel> GetViewModels()
    {
        return _textEditorService.ViewModelStateWrap.Value.ViewModelList;
    }

    public TextEditorModel? GetModelOrDefault(Key<TextEditorViewModel> textEditorViewModelKey)
    {
        var viewModelState = _textEditorService.ViewModelStateWrap.Value;

        var viewModel = viewModelState.ViewModelList.FirstOrDefault(
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

    public async Task<TextEditorDimensions> GetTextEditorMeasurementsAsync(string elementId)
    {
        return await _jsRuntime.GetLuthetusTextEditorApi()
            .GetTextEditorMeasurementsInPixelsById(
                elementId)
            .ConfigureAwait(false);
    }

    public async Task<CharAndLineMeasurements> MeasureCharacterWidthAndLineHeightAsync(
        string measureCharacterWidthAndLineHeightElementId,
        int countOfTestCharacters)
    {
        return await _jsRuntime.GetLuthetusTextEditorApi()
            .GetCharAndLineMeasurementsInPixelsById(
                measureCharacterWidthAndLineHeightElementId,
                countOfTestCharacters)
            .ConfigureAwait(false);
    }
    #endregion

    #region UPDATE_METHODS
    public TextEditorEdit WithValueFactory(
        Key<TextEditorViewModel> viewModelKey,
        Func<TextEditorViewModel, TextEditorViewModel> withFunc)
    {
        return editContext =>
        {
            _dispatcher.Dispatch(new TextEditorViewModelState.SetViewModelWithAction(
                TextEditorService.AuthenticatedActionKey,
                editContext,
                viewModelKey,
                withFunc));

            return Task.CompletedTask;
        };
    }

    public TextEditorEdit WithTaskFactory(
        Key<TextEditorViewModel> viewModelKey,
        Func<TextEditorViewModel, Task<Func<TextEditorViewModel, TextEditorViewModel>>> withFuncWrap)
    {
        return async editContext =>
        {
            var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);

            if (viewModelModifier is null)
                return;

            _dispatcher.Dispatch(new TextEditorViewModelState.SetViewModelWithAction(
                TextEditorService.AuthenticatedActionKey,
                editContext,
                viewModelKey,
                await withFuncWrap.Invoke(viewModelModifier.ViewModel).ConfigureAwait(false)));
        };
    }

    /// <summary>
    /// If a parameter is null the JavaScript will not modify that value
    /// </summary>
    public TextEditorEdit SetScrollPositionFactory(
        Key<TextEditorViewModel> viewModelKey,
        double? scrollLeftInPixels,
        double? scrollTopInPixels)
    {
        return editContext =>
        {
            var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);
            if (viewModelModifier is null)
                return Task.CompletedTask;

            viewModelModifier.ScrollWasModified = true;

            viewModelModifier.ViewModel = viewModelModifier.ViewModel with
            {
                VirtualizationResult = viewModelModifier.ViewModel.VirtualizationResult with
                {
                    TextEditorMeasurements = viewModelModifier.ViewModel.VirtualizationResult.TextEditorMeasurements with
                    {
                        ScrollLeft = scrollLeftInPixels is not null
                            ? (int)Math.Floor(scrollLeftInPixels.Value)
                            : viewModelModifier.ViewModel.VirtualizationResult.TextEditorMeasurements.ScrollLeft,
                        ScrollTop = scrollTopInPixels is not null
                            ? (int)Math.Floor(scrollTopInPixels.Value)
                            : viewModelModifier.ViewModel.VirtualizationResult.TextEditorMeasurements.ScrollTop,
                    }
                }
            };

            return Task.CompletedTask;
        };
    }

    public TextEditorEdit MutateScrollVerticalPositionFactory(
        Key<TextEditorViewModel> viewModelKey,
        double pixels)
    {
        return editContext =>
        {
            var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);
            if (viewModelModifier is null)
                return Task.CompletedTask;

            viewModelModifier.ScrollWasModified = true;

            viewModelModifier.ViewModel = viewModelModifier.ViewModel with
            {
                VirtualizationResult = viewModelModifier.ViewModel.VirtualizationResult with
                {
                    TextEditorMeasurements = viewModelModifier.ViewModel.VirtualizationResult.TextEditorMeasurements with
                    {
                        ScrollTop = (int)Math.Ceiling(pixels) +
                            viewModelModifier.ViewModel.VirtualizationResult.TextEditorMeasurements.ScrollTop,
                    }
                }
            };

            return Task.CompletedTask;
        };
    }

    public TextEditorEdit MutateScrollHorizontalPositionFactory(
        Key<TextEditorViewModel> viewModelKey,
        double pixels)
    {
        return editContext =>
        {
            var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);
            if (viewModelModifier is null)
                return Task.CompletedTask;

            viewModelModifier.ScrollWasModified = true;

            viewModelModifier.ViewModel = viewModelModifier.ViewModel with
            {
                VirtualizationResult = viewModelModifier.ViewModel.VirtualizationResult with
                {
                    TextEditorMeasurements = viewModelModifier.ViewModel.VirtualizationResult.TextEditorMeasurements with
                    {
                        ScrollLeft = (int)Math.Ceiling(pixels) +
                            viewModelModifier.ViewModel.VirtualizationResult.TextEditorMeasurements.ScrollLeft,
                    }
                }
            };

            return Task.CompletedTask;
        };
    }

    public TextEditorEdit ScrollIntoViewFactory(
        ResourceUri modelResourceUri,
        Key<TextEditorViewModel> viewModelKey,
        TextEditorTextSpan textSpan)
    {
        return editContext =>
        {
            var modelModifier = editContext.GetModelModifier(modelResourceUri);
            if (modelModifier is null)
                return Task.CompletedTask;

            var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);
            if (viewModelModifier is null)
                return Task.CompletedTask;

            var lineInformation = modelModifier.GetLineInformationFromPositionIndex(textSpan.StartingIndexInclusive);
            var lineIndex = lineInformation.Index;
            var columnIndex = textSpan.StartingIndexInclusive - lineInformation.StartPositionIndexInclusive;

            // Unit of measurement is pixels (px)
            var scrollLeft = new Nullable<double>(columnIndex *
                viewModelModifier.ViewModel.VirtualizationResult.CharAndLineMeasurements.CharacterWidth);

            // Unit of measurement is pixels (px)
            var scrollTop = new Nullable<double>(lineIndex *
                viewModelModifier.ViewModel.VirtualizationResult.CharAndLineMeasurements.LineHeight);

            // If a given scroll direction is already within view of the text span, do not scroll on that direction
            {
                // scrollLeft needs to be modified?
                {
                    var currentScrollLeft = viewModelModifier.ViewModel.VirtualizationResult.TextEditorMeasurements.ScrollLeft;
                    var currentWidth = viewModelModifier.ViewModel.VirtualizationResult.TextEditorMeasurements.Width;

                    var caseA = currentScrollLeft <= scrollLeft;
                    var caseB = (scrollLeft ?? 0) < (currentWidth + currentScrollLeft);

                    if (caseA && caseB)
                        scrollLeft = null;
                }

                // scrollTop needs to be modified?
                {
                    var currentScrollTop = viewModelModifier.ViewModel.VirtualizationResult.TextEditorMeasurements.ScrollTop;
                    var currentHeight = viewModelModifier.ViewModel.VirtualizationResult.TextEditorMeasurements.Height;

                    var caseA = currentScrollTop <= scrollTop;
                    var caseB = (scrollTop ?? 0) < (currentHeight + currentScrollTop);

                    if (caseA && caseB)
                        scrollTop = null;
                }
            }

            // Return early if both values are 'null'
            if (scrollLeft is null && scrollTop is null)
                return Task.CompletedTask;

            return SetScrollPositionFactory(
                    viewModelKey,
                    scrollLeft,
                    scrollTop)
                .Invoke(editContext);
        };
    }

    public TextEditorEdit FocusPrimaryCursorFactory(string primaryCursorContentId)
    {
        return async editContext =>
        {
            await _jsRuntime.GetLuthetusCommonApi()
                .FocusHtmlElementById(
                    primaryCursorContentId)
                .ConfigureAwait(false);
        };
    }

    public TextEditorEdit MoveCursorFactory(
        KeyboardEventArgs keyboardEventArgs,
        ResourceUri modelResourceUri,
        Key<TextEditorViewModel> viewModelKey)
    {
        return async editContext =>
        {
            var modelModifier = editContext.GetModelModifier(modelResourceUri);
            var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);
            var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return;

            await MoveCursorUnsafeFactory(keyboardEventArgs, modelResourceUri, viewModelKey, primaryCursorModifier)
                .Invoke(editContext);

            viewModelModifier.ViewModel.UnsafeState.ShouldRevealCursor = true;
        };
    }

    public TextEditorEdit MoveCursorUnsafeFactory(
        KeyboardEventArgs keyboardEventArgs,
        ResourceUri modelResourceUri,
        Key<TextEditorViewModel> viewModelKey,
        TextEditorCursorModifier cursorModifier)
    {
        return editContext =>
        {
            var modelModifier = editContext.GetModelModifier(modelResourceUri, true);
            var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);

            if (modelModifier is null || viewModelModifier is null)
                return Task.CompletedTask;

            var shouldClearSelection = false;

            if (keyboardEventArgs.ShiftKey)
            {
                if (cursorModifier.SelectionAnchorPositionIndex is null ||
                    cursorModifier.SelectionEndingPositionIndex == cursorModifier.SelectionAnchorPositionIndex)
                {
                    var positionIndex = modelModifier.GetPositionIndex(
                        cursorModifier.LineIndex,
                        cursorModifier.ColumnIndex);

                    cursorModifier.SelectionAnchorPositionIndex = positionIndex;
                }
            }
            else
            {
                shouldClearSelection = true;
            }

            int lengthOfLine = 0; // This variable is used in multiple switch cases.

            switch (keyboardEventArgs.Key)
            {
                case KeyboardKeyFacts.MovementKeys.ARROW_LEFT:
                    if (TextEditorSelectionHelper.HasSelectedText(cursorModifier) &&
                        !keyboardEventArgs.ShiftKey)
                    {
                        var selectionBounds = TextEditorSelectionHelper.GetSelectionBounds(cursorModifier);

                        var lowerLineInformation = modelModifier.GetLineInformationFromPositionIndex(
                            selectionBounds.lowerPositionIndexInclusive);

                        cursorModifier.LineIndex = lowerLineInformation.Index;

                        cursorModifier.ColumnIndex = selectionBounds.lowerPositionIndexInclusive -
                            lowerLineInformation.StartPositionIndexInclusive;
                    }
                    else
                    {
                        if (cursorModifier.ColumnIndex <= 0)
                        {
                            if (cursorModifier.LineIndex != 0)
                            {
                                cursorModifier.LineIndex--;

                                lengthOfLine = modelModifier.GetLineLength(cursorModifier.LineIndex);

                                cursorModifier.SetColumnIndexAndPreferred(lengthOfLine);
                            }
                            else
                            {
                                cursorModifier.SetColumnIndexAndPreferred(0);
                            }
                        }
                        else
                        {
                            if (keyboardEventArgs.CtrlKey)
                            {
                                var columnIndexOfCharacterWithDifferingKind = modelModifier.GetColumnIndexOfCharacterWithDifferingKind(
                                    cursorModifier.LineIndex,
                                    cursorModifier.ColumnIndex,
                                    true);

                                if (columnIndexOfCharacterWithDifferingKind == -1)
                                    cursorModifier.SetColumnIndexAndPreferred(0);
                                else
                                    cursorModifier.SetColumnIndexAndPreferred(columnIndexOfCharacterWithDifferingKind);
                            }
                            else
                            {
                                cursorModifier.SetColumnIndexAndPreferred(cursorModifier.ColumnIndex - 1);
                            }
                        }
                    }

                    break;
                case KeyboardKeyFacts.MovementKeys.ARROW_DOWN:
                    if (cursorModifier.LineIndex < modelModifier.LineCount - 1)
                    {
                        cursorModifier.LineIndex++;

                        lengthOfLine = modelModifier.GetLineLength(cursorModifier.LineIndex);

                        cursorModifier.ColumnIndex = lengthOfLine < cursorModifier.PreferredColumnIndex
                            ? lengthOfLine
                            : cursorModifier.PreferredColumnIndex;
                    }

                    break;
                case KeyboardKeyFacts.MovementKeys.ARROW_UP:
                    if (cursorModifier.LineIndex > 0)
                    {
                        cursorModifier.LineIndex--;

                        lengthOfLine = modelModifier.GetLineLength(cursorModifier.LineIndex);

                        cursorModifier.ColumnIndex = lengthOfLine < cursorModifier.PreferredColumnIndex
                            ? lengthOfLine
                            : cursorModifier.PreferredColumnIndex;
                    }

                    break;
                case KeyboardKeyFacts.MovementKeys.ARROW_RIGHT:
                    if (TextEditorSelectionHelper.HasSelectedText(cursorModifier) && !keyboardEventArgs.ShiftKey)
                    {
                        var selectionBounds = TextEditorSelectionHelper.GetSelectionBounds(cursorModifier);

                        var upperLineMetaData = modelModifier.GetLineInformationFromPositionIndex(
                            selectionBounds.upperPositionIndexExclusive);

                        cursorModifier.LineIndex = upperLineMetaData.Index;

                        if (cursorModifier.LineIndex >= modelModifier.LineCount)
                        {
                            cursorModifier.LineIndex = modelModifier.LineCount - 1;

                            var upperLineLength = modelModifier.GetLineLength(cursorModifier.LineIndex);

                            cursorModifier.ColumnIndex = upperLineLength;
                        }
                        else
                        {
                            cursorModifier.ColumnIndex =
                                selectionBounds.upperPositionIndexExclusive - upperLineMetaData.StartPositionIndexInclusive;
                        }
                    }
                    else
                    {
                        lengthOfLine = modelModifier.GetLineLength(cursorModifier.LineIndex);

                        if (cursorModifier.ColumnIndex >= lengthOfLine &&
                            cursorModifier.LineIndex < modelModifier.LineCount - 1)
                        {
                            cursorModifier.SetColumnIndexAndPreferred(0);
                            cursorModifier.LineIndex++;
                        }
                        else if (cursorModifier.ColumnIndex != lengthOfLine)
                        {
                            if (keyboardEventArgs.CtrlKey)
                            {
                                var columnIndexOfCharacterWithDifferingKind = modelModifier.GetColumnIndexOfCharacterWithDifferingKind(
                                    cursorModifier.LineIndex,
                                    cursorModifier.ColumnIndex,
                                    false);

                                if (columnIndexOfCharacterWithDifferingKind == -1)
                                    cursorModifier.SetColumnIndexAndPreferred(lengthOfLine);
                                else
                                {
                                    cursorModifier.SetColumnIndexAndPreferred(
                                        columnIndexOfCharacterWithDifferingKind);
                                }
                            }
                            else
                            {
                                cursorModifier.SetColumnIndexAndPreferred(cursorModifier.ColumnIndex + 1);
                            }
                        }
                    }

                    break;
                case KeyboardKeyFacts.MovementKeys.HOME:
                    if (keyboardEventArgs.CtrlKey)
                        cursorModifier.LineIndex = 0;

                    cursorModifier.SetColumnIndexAndPreferred(0);

                    break;
                case KeyboardKeyFacts.MovementKeys.END:
                    if (keyboardEventArgs.CtrlKey)
                        cursorModifier.LineIndex = modelModifier.LineCount - 1;

                    lengthOfLine = modelModifier.GetLineLength(cursorModifier.LineIndex);

                    cursorModifier.SetColumnIndexAndPreferred(lengthOfLine);

                    break;
            }

            if (keyboardEventArgs.ShiftKey)
            {
                cursorModifier.SelectionEndingPositionIndex = modelModifier.GetPositionIndex(
                    cursorModifier.LineIndex,
                    cursorModifier.ColumnIndex);
            }
            else if (!keyboardEventArgs.ShiftKey && shouldClearSelection)
            {
                // The active selection is needed, and cannot be touched until the end.
                cursorModifier.SelectionAnchorPositionIndex = null;
                cursorModifier.SelectionEndingPositionIndex = 0;
            }

            return Task.CompletedTask;
        };
    }

    public TextEditorEdit CursorMovePageTopFactory(
        ResourceUri modelResourceUri,
        Key<TextEditorViewModel> viewModelKey)
    {
        return editContext =>
        {
            var modelModifier = editContext.GetModelModifier(modelResourceUri);
            var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);
            var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;

            return CursorMovePageTopUnsafeFactory(modelResourceUri, viewModelKey, primaryCursorModifier)
                .Invoke(editContext);
        };
    }

    public TextEditorEdit CursorMovePageTopUnsafeFactory(
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

            if (viewModelModifier.ViewModel.VirtualizationResult?.EntryList.Any() ?? false)
            {
                var firstEntry = viewModelModifier.ViewModel.VirtualizationResult.EntryList.First();

                cursorModifier.LineIndex = firstEntry.Index;
                cursorModifier.ColumnIndex = 0;
            }

            return Task.CompletedTask;
        };
    }

    public TextEditorEdit CursorMovePageBottomFactory(
        ResourceUri modelResourceUri,
        Key<TextEditorViewModel> viewModelKey)
    {
        return editContext =>
        {
            var modelModifier = editContext.GetModelModifier(modelResourceUri);
            var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);
            var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;

            return CursorMovePageBottomUnsafeFactory(modelResourceUri, viewModelKey, primaryCursorModifier)
                .Invoke(editContext);
        };
    }

    public TextEditorEdit CursorMovePageBottomUnsafeFactory(
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

            if ((viewModelModifier.ViewModel.VirtualizationResult?.EntryList.Any() ?? false))
            {
                var lastEntry = viewModelModifier.ViewModel.VirtualizationResult.EntryList.Last();
                var lastEntriesLineLength = modelModifier.GetLineLength(lastEntry.Index);

                cursorModifier.LineIndex = lastEntry.Index;
                cursorModifier.ColumnIndex = lastEntriesLineLength;
            }

            return Task.CompletedTask;
        };
    }

    public TextEditorEdit CalculateVirtualizationResultFactory(
        ResourceUri modelResourceUri,
        Key<TextEditorViewModel> viewModelKey,
        CancellationToken cancellationToken)
    {
        return async editContext =>
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var modelModifier = editContext.GetModelModifier(modelResourceUri, true);
            var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);
            var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return;

            // throw new NotImplementedException("Goal: Rewrite TextEditorMeasurements. (2024-05-09)");
            //
            // Preferably I'd throw "throw new NotImplementedException("Goal: Rewrite TextEditorMeasurements. (2024-05-09)");"
            // here, but this is the final piece of the puzzle to change, otherwise nothing will work.
            var textEditorMeasurements = await _textEditorService.ViewModelApi
                .GetTextEditorMeasurementsAsync(viewModelModifier.ViewModel.BodyElementId)
                .ConfigureAwait(false);

            if (textEditorMeasurements is null)
                return;

            var virtualizationResult = viewModelModifier.ViewModel.VirtualizationResult with
            {
                TextEditorMeasurements = textEditorMeasurements
            };

            var verticalStartingIndex = (int)Math.Floor(
                textEditorMeasurements.ScrollTop /
                virtualizationResult.CharAndLineMeasurements.LineHeight);

            var verticalTake = (int)Math.Ceiling(
                textEditorMeasurements.Height /
                virtualizationResult.CharAndLineMeasurements.LineHeight);

            // Vertical Padding (render some offscreen data)
            {
                verticalTake += 1;
            }

            // Check index boundaries
            {
                verticalStartingIndex = Math.Max(0, verticalStartingIndex);

                if (verticalStartingIndex + verticalTake > modelModifier.LineEndList.Count)
                    verticalTake = modelModifier.LineEndList.Count - verticalStartingIndex;

                verticalTake = Math.Max(0, verticalTake);
            }

            var horizontalStartingIndex = (int)Math.Floor(
                textEditorMeasurements.ScrollLeft /
                virtualizationResult.CharAndLineMeasurements.CharacterWidth);

            var horizontalTake = (int)Math.Ceiling(
                textEditorMeasurements.Width /
                virtualizationResult.CharAndLineMeasurements.CharacterWidth);

            var virtualizedEntryBag = modelModifier
                .GetLineRichCharacterRange(verticalStartingIndex, verticalTake)
                .Select((line, lineIndex) =>
                {
                    lineIndex += verticalStartingIndex;

                    var localHorizontalStartingIndex = horizontalStartingIndex;
                    var localHorizontalTake = horizontalTake;

                    // 1 of the character width is already accounted for
                    var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;

                    // Adjust for tab key width
                    {
                        var maxValidColumnIndex = line.Count > 0
                            ? line.Count - 1
                            : 0;

                        var parameterForGetTabsCountOnSameLineBeforeCursor =
                            localHorizontalStartingIndex > maxValidColumnIndex
                                ? maxValidColumnIndex
                                : localHorizontalStartingIndex;

                        var lineInformation = modelModifier.GetLineInformation(lineIndex);

                        if (parameterForGetTabsCountOnSameLineBeforeCursor > lineInformation.LastValidColumnIndex)
                            parameterForGetTabsCountOnSameLineBeforeCursor = lineInformation.LastValidColumnIndex;


                        var tabsOnSameLineBeforeCursor = modelModifier.GetTabCountOnSameLineBeforeCursor(
                            lineIndex,
                            parameterForGetTabsCountOnSameLineBeforeCursor);

                        localHorizontalStartingIndex -= extraWidthPerTabKey * tabsOnSameLineBeforeCursor;
                    }

                    if (localHorizontalStartingIndex + localHorizontalTake > line.Count)
                        localHorizontalTake = line.Count - localHorizontalStartingIndex;

                    localHorizontalStartingIndex = Math.Max(0, localHorizontalStartingIndex);
                    localHorizontalTake = Math.Max(0, localHorizontalTake);

                    var horizontallyVirtualizedLine = line
                        .Skip(localHorizontalStartingIndex)
                        .Take(localHorizontalTake)
                        .ToList();

                    var countTabKeysInVirtualizedLine = horizontallyVirtualizedLine
                        .Where(x => x.Value == KeyboardKeyFacts.WhitespaceCharacters.TAB)
                        .Count();

                    var widthInPixels = (horizontallyVirtualizedLine.Count + (extraWidthPerTabKey * countTabKeysInVirtualizedLine)) *
                        virtualizationResult.CharAndLineMeasurements.CharacterWidth;

                    var leftInPixels = localHorizontalStartingIndex *
                        virtualizationResult.CharAndLineMeasurements.CharacterWidth;

                    // Adjust for tab key width
                    {
                        var maxValidColumnIndex = line.Count > 0
                            ? line.Count - 1
                            : 0;

                        var parameterForGetTabsCountOnSameLineBeforeCursor =
                            localHorizontalStartingIndex > maxValidColumnIndex
                                ? maxValidColumnIndex
                                : localHorizontalStartingIndex;

                        var lineInformation = modelModifier.GetLineInformation(lineIndex);

                        if (parameterForGetTabsCountOnSameLineBeforeCursor > lineInformation.LastValidColumnIndex)
                            parameterForGetTabsCountOnSameLineBeforeCursor = lineInformation.LastValidColumnIndex;

                        var tabsOnSameLineBeforeCursor = modelModifier.GetTabCountOnSameLineBeforeCursor(
                            lineIndex,
                            parameterForGetTabsCountOnSameLineBeforeCursor);

                        leftInPixels += (extraWidthPerTabKey *
                            tabsOnSameLineBeforeCursor *
                            virtualizationResult.CharAndLineMeasurements.CharacterWidth);
                    }

                    leftInPixels = Math.Max(0, leftInPixels);

                    var topInPixels = lineIndex * virtualizationResult.CharAndLineMeasurements.LineHeight;

                    return new VirtualizationEntry<List<RichCharacter>>(
                        lineIndex,
                        horizontallyVirtualizedLine,
                        widthInPixels,
                        virtualizationResult.CharAndLineMeasurements.LineHeight,
                        leftInPixels,
                        topInPixels);
                }).ToImmutableArray();

            var totalWidth = (int)Math.Ceiling(modelModifier.MostCharactersOnASingleLineTuple.lineLength *
                virtualizationResult.CharAndLineMeasurements.CharacterWidth);

            // Account for any tab characters on the 'MostCharactersOnASingleLineTuple'
            //
            // TODO: This code is not fully correct...
            //       ...if the longest line is 50 non-tab characters,
            //       and the second longest line is 49 tab characters,
            //       this code will erroneously take the '50' non-tab characters
            //       to be the longest line.
            {
                var lineIndex = modelModifier.MostCharactersOnASingleLineTuple.lineIndex;
                var longestLineInformation = modelModifier.GetLineInformation(lineIndex);

                var tabCountOnLongestLine = modelModifier.GetTabCountOnSameLineBeforeCursor(
                    longestLineInformation.Index,
                    longestLineInformation.LastValidColumnIndex);

                // 1 of the character width is already accounted for
                var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;

                totalWidth += (int)Math.Ceiling(extraWidthPerTabKey *
                    tabCountOnLongestLine *
                    virtualizationResult.CharAndLineMeasurements.CharacterWidth);
            }

            var totalHeight = (int)Math.Ceiling(modelModifier.LineEndList.Count *
                virtualizationResult.CharAndLineMeasurements.LineHeight);

            // Add vertical margin so the user can scroll beyond the final line of content
            int marginScrollHeight;
            {
                var percentOfMarginScrollHeightByPageUnit = 0.4;

                marginScrollHeight = (int)Math.Ceiling(textEditorMeasurements.Height * percentOfMarginScrollHeightByPageUnit);
                totalHeight += marginScrollHeight;
            }

            var leftBoundaryWidthInPixels = horizontalStartingIndex *
                virtualizationResult.CharAndLineMeasurements.CharacterWidth;

            var leftBoundary = new VirtualizationBoundary(
                leftBoundaryWidthInPixels,
                totalHeight,
                0,
                0);

            var rightBoundaryLeftInPixels = leftBoundary.WidthInPixels +
                virtualizationResult.CharAndLineMeasurements.CharacterWidth *
                horizontalTake;

            var rightBoundaryWidthInPixels = totalWidth - rightBoundaryLeftInPixels;

            var rightBoundary = new VirtualizationBoundary(
                rightBoundaryWidthInPixels,
                totalHeight,
                rightBoundaryLeftInPixels,
                0);

            var topBoundaryHeightInPixels = verticalStartingIndex *
                virtualizationResult.CharAndLineMeasurements.LineHeight;

            var topBoundary = new VirtualizationBoundary(
                totalWidth,
                topBoundaryHeightInPixels,
                0,
                0);

            var bottomBoundaryTopInPixels = topBoundary.HeightInPixels +
                virtualizationResult.CharAndLineMeasurements.LineHeight *
                verticalTake;

            var bottomBoundaryHeightInPixels = totalHeight - bottomBoundaryTopInPixels;

            var bottomBoundary = new VirtualizationBoundary(
                totalWidth,
                bottomBoundaryHeightInPixels,
                0,
                bottomBoundaryTopInPixels);

            virtualizationResult = new VirtualizationResult<List<RichCharacter>>(
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
                virtualizationResult.CharAndLineMeasurements);

            viewModelModifier.ViewModel = viewModelModifier.ViewModel with
            {
                VirtualizationResult = virtualizationResult,
            };
        };
    }

    public TextEditorEdit RemeasureFactory(
        ResourceUri modelResourceUri,
        Key<TextEditorViewModel> viewModelKey,
        string measureCharacterWidthAndLineHeightElementId,
        int countOfTestCharacters,
        CancellationToken cancellationToken)
    {
        return async editContext =>
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var modelModifier = editContext.GetModelModifier(modelResourceUri);
            var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);
            var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return;

            var options = _textEditorService.OptionsApi.GetOptions();

            var characterWidthAndLineHeight = await _textEditorService.ViewModelApi.MeasureCharacterWidthAndLineHeightAsync(
                measureCharacterWidthAndLineHeightElementId,
                countOfTestCharacters);

            // throw new NotImplementedException("Goal: Rewrite TextEditorMeasurements. (2024-05-09)");
            //
            // This line of code was not removed from 'CalculateVirtualizationResultFactory(...)'
            // in order to keep the development website working and just make small incremental changes.
            // Therefore, do not forget to th remove this line from 'CalculateVirtualizationResultFactory(...)'.
            var textEditorMeasurements = await _textEditorService.ViewModelApi
                .GetTextEditorMeasurementsAsync(viewModelModifier.ViewModel.BodyElementId);

            viewModelModifier.ViewModel = viewModelModifier.ViewModel with
            {
                VirtualizationResult = viewModelModifier.ViewModel.VirtualizationResult with
                {
                    CharAndLineMeasurements = characterWidthAndLineHeight,
                    TextEditorMeasurements = textEditorMeasurements
                },
            };
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