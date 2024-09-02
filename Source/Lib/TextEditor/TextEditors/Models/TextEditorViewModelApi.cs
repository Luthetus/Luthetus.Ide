using System.Collections.Immutable;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.JsRuntimes.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.TextEditor.RazorLib.JavaScriptObjects.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using Luthetus.TextEditor.RazorLib.Virtualizations.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

public class TextEditorViewModelApi : ITextEditorViewModelApi
{
    private readonly ITextEditorService _textEditorService;
    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly IState<TextEditorState> _textEditorStateWrap;
    private readonly IDispatcher _dispatcher;
    private readonly IDialogService _dialogService;

    // TODO: Perhaps do not reference IJSRuntime but instead wrap it in a 'IUiProvider' or something like that. The 'IUiProvider' would then expose methods that allow the TextEditorViewModel to adjust the scrollbars. 
    private readonly IJSRuntime _jsRuntime;

    public TextEditorViewModelApi(
        ITextEditorService textEditorService,
        IBackgroundTaskService backgroundTaskService,
        IState<TextEditorState> textEditorStateWrap,
        IJSRuntime jsRuntime,
        IDispatcher dispatcher,
        IDialogService dialogService)
    {
        _textEditorService = textEditorService;
        _backgroundTaskService = backgroundTaskService;
        _textEditorStateWrap = textEditorStateWrap;
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
        _dispatcher.Dispatch(new TextEditorState.RegisterViewModelAction(
            viewModelKey,
            resourceUri,
            category,
            _textEditorService,
            _dispatcher,
            _dialogService,
            _jsRuntime));
    }
    
    public void Register(TextEditorViewModel viewModel)
    {
        _dispatcher.Dispatch(new TextEditorState.RegisterViewModelExistingAction(viewModel));
    }
    #endregion

    #region READ_METHODS
    public TextEditorViewModel? GetOrDefault(Key<TextEditorViewModel> viewModelKey)
    {
        return _textEditorService.TextEditorStateWrap.Value.ViewModelList.FirstOrDefault(
            x => x.ViewModelKey == viewModelKey);
    }

    public ImmutableList<TextEditorViewModel> GetViewModels()
    {
        return _textEditorService.TextEditorStateWrap.Value.ViewModelList;
    }

    public TextEditorModel? GetModelOrDefault(Key<TextEditorViewModel> viewModelKey)
    {
        var textEditorState = _textEditorService.TextEditorStateWrap.Value;

        var viewModel = textEditorState.ViewModelList.FirstOrDefault(
            x => x.ViewModelKey == viewModelKey);

        if (viewModel is null)
            return null;

        return _textEditorService.ModelApi.GetOrDefault(viewModel.ResourceUri);
    }

    public string? GetAllText(Key<TextEditorViewModel> viewModelKey)
    {
        var textEditorModel = GetModelOrDefault(viewModelKey);

        return textEditorModel is null
            ? null
            : _textEditorService.ModelApi.GetAllText(textEditorModel.ResourceUri);
    }

    public async Task<TextEditorDimensions> GetTextEditorMeasurementsAsync(string elementId)
    {
        return await _textEditorService.JsRuntimeTextEditorApi
            .GetTextEditorMeasurementsInPixelsById(elementId)
            .ConfigureAwait(false);
    }

    public async Task<CharAndLineMeasurements> MeasureCharacterWidthAndLineHeightAsync(
        string measureCharacterWidthAndLineHeightElementId,
        int countOfTestCharacters)
    {
        return await _textEditorService.JsRuntimeTextEditorApi
            .GetCharAndLineMeasurementsInPixelsById(
                measureCharacterWidthAndLineHeightElementId,
                countOfTestCharacters)
            .ConfigureAwait(false);
    }
    #endregion

    #region UPDATE_METHODS
    /// <summary>
    /// If a parameter is null the JavaScript will not modify that value
    /// </summary>
    public void SetScrollPosition(
        ITextEditorEditContext editContext,
        TextEditorViewModelModifier viewModelModifier,
        double? scrollLeftInPixels,
        double? scrollTopInPixels)
    {
        viewModelModifier.ScrollWasModified = true;

		if (scrollLeftInPixels is not null)
		{
			viewModelModifier.ViewModel = viewModelModifier.ViewModel with
			{
				ScrollbarDimensions = viewModelModifier.ViewModel.ScrollbarDimensions
					.SetScrollLeft((int)Math.Floor(scrollLeftInPixels.Value), viewModelModifier.ViewModel.TextEditorDimensions)
			};
		}

		if (scrollTopInPixels is not null)
		{
			viewModelModifier.ViewModel = viewModelModifier.ViewModel with
			{
				ScrollbarDimensions = viewModelModifier.ViewModel.ScrollbarDimensions
					.SetScrollTop((int)Math.Floor(scrollTopInPixels.Value), viewModelModifier.ViewModel.TextEditorDimensions)
			};
		}
    }

    public void MutateScrollVerticalPosition(
        ITextEditorEditContext editContext,
        TextEditorViewModelModifier viewModelModifier,
        double pixels)
    {
        viewModelModifier.ScrollWasModified = true;

        viewModelModifier.ViewModel = viewModelModifier.ViewModel with
        {
			ScrollbarDimensions = viewModelModifier.ViewModel.ScrollbarDimensions
				.MutateScrollTop((int)Math.Ceiling(pixels), viewModelModifier.ViewModel.TextEditorDimensions)
        };
    }

    public void MutateScrollHorizontalPosition(
        ITextEditorEditContext editContext,
        TextEditorViewModelModifier viewModelModifier,
        double pixels)
    {
        viewModelModifier.ScrollWasModified = true;

		viewModelModifier.ViewModel = viewModelModifier.ViewModel with
        {
			ScrollbarDimensions = viewModelModifier.ViewModel.ScrollbarDimensions
				.MutateScrollLeft((int)Math.Ceiling(pixels), viewModelModifier.ViewModel.TextEditorDimensions)
        };
    }

    public void ScrollIntoView(
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        TextEditorViewModelModifier viewModelModifier,
        TextEditorTextSpan textSpan)
    {
        var lineInformation = modelModifier.GetLineInformationFromPositionIndex(textSpan.StartingIndexInclusive);
        var lineIndex = lineInformation.Index;
        var columnIndex = textSpan.StartingIndexInclusive - lineInformation.StartPositionIndexInclusive;

        // Unit of measurement is pixels (px)
        var scrollLeft = new Nullable<double>(columnIndex *
            viewModelModifier.ViewModel.CharAndLineMeasurements.CharacterWidth);

        // Unit of measurement is pixels (px)
        var scrollTop = new Nullable<double>(lineIndex *
            viewModelModifier.ViewModel.CharAndLineMeasurements.LineHeight);

        // If a given scroll direction is already within view of the text span, do not scroll on that direction
        {
            // scrollLeft needs to be modified?
            {
                var currentScrollLeft = viewModelModifier.ViewModel.ScrollbarDimensions.ScrollLeft;
                var currentWidth = viewModelModifier.ViewModel.TextEditorDimensions.Width;

                var caseA = currentScrollLeft <= scrollLeft;
                var caseB = (scrollLeft ?? 0) < (currentWidth + currentScrollLeft);

                if (caseA && caseB)
                    scrollLeft = null;
            }

            // scrollTop needs to be modified?
            {
                var currentScrollTop = viewModelModifier.ViewModel.ScrollbarDimensions.ScrollTop;
                var currentHeight = viewModelModifier.ViewModel.TextEditorDimensions.Height;

                var caseA = currentScrollTop <= scrollTop;
                var caseB = (scrollTop ?? 0) < (currentHeight + currentScrollTop);

                if (caseA && caseB)
                    scrollTop = null;
            }
        }

        // Return early if both values are 'null'
        if (scrollLeft is null && scrollTop is null)
            return;

        SetScrollPosition(
            editContext,
	        viewModelModifier,
	        scrollLeft,
            scrollTop);
    }

    public async Task FocusPrimaryCursorAsync(string primaryCursorContentId)
    {
        await _jsRuntime.GetLuthetusCommonApi()
            .FocusHtmlElementById(primaryCursorContentId, preventScroll: true);
    }

    public void MoveCursor(
        KeymapArgs keymapArgs,
		ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        TextEditorViewModelModifier viewModelModifier,
        CursorModifierBagTextEditor cursorModifierBag)
    {
        MoveCursorUnsafe(
            keymapArgs,
	        editContext,
	        modelModifier,
	        viewModelModifier,
	        cursorModifierBag,
	        editContext.GetPrimaryCursorModifier(cursorModifierBag));

        viewModelModifier.ViewModel.UnsafeState.ShouldRevealCursor = true;
    }

    public void MoveCursorUnsafe(
        KeymapArgs keymapArgs,
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        TextEditorViewModelModifier viewModelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        TextEditorCursorModifier cursorModifier)
    {
        var shouldClearSelection = false;

        if (keymapArgs.ShiftKey)
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

        switch (keymapArgs.Key)
        {
            case KeyboardKeyFacts.MovementKeys.ARROW_LEFT:
                if (TextEditorSelectionHelper.HasSelectedText(cursorModifier) &&
                    !keymapArgs.ShiftKey)
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
                        if (keymapArgs.CtrlKey)
                        {
                        	var columnIndexOfCharacterWithDifferingKind = modelModifier.GetColumnIndexOfCharacterWithDifferingKind(
                                cursorModifier.LineIndex,
                                cursorModifier.ColumnIndex,
                                true);

                            if (columnIndexOfCharacterWithDifferingKind == -1) // Move to start of line
                            {
                                cursorModifier.SetColumnIndexAndPreferred(0);
                            }
                            else
                            {
                            	if (!keymapArgs.AltKey) // Move by character kind
                            	{
                            		cursorModifier.SetColumnIndexAndPreferred(columnIndexOfCharacterWithDifferingKind);
                            	}
                                else // Move by camel case
                                {
                                	var positionIndex = modelModifier.GetPositionIndex(cursorModifier);
									var rememberStartPositionIndex = positionIndex;
									
									var minPositionIndex = columnIndexOfCharacterWithDifferingKind;
									var infiniteLoopPrediction = false;
									
									if (minPositionIndex > positionIndex)
										infiniteLoopPrediction = true;
									
									bool useCamelCaseResult = false;
									
									if (!infiniteLoopPrediction)
									{
										while (--positionIndex > minPositionIndex)
										{
											var currentRichCharacter = modelModifier.RichCharacterList[positionIndex];
											
											if (Char.IsUpper(currentRichCharacter.Value) || currentRichCharacter.Value == '_')
											{
												useCamelCaseResult = true;
												break;
											}
										}
									}
									
									if (useCamelCaseResult)
									{
										var columnDisplacement = positionIndex - rememberStartPositionIndex;
										cursorModifier.SetColumnIndexAndPreferred(cursorModifier.ColumnIndex + columnDisplacement);
									}
									else
									{
										cursorModifier.SetColumnIndexAndPreferred(columnIndexOfCharacterWithDifferingKind);
									}
                                }
                            }
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
                if (TextEditorSelectionHelper.HasSelectedText(cursorModifier) && !keymapArgs.ShiftKey)
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
                        if (keymapArgs.CtrlKey)
                        {
                        	var columnIndexOfCharacterWithDifferingKind = modelModifier.GetColumnIndexOfCharacterWithDifferingKind(
                                cursorModifier.LineIndex,
                                cursorModifier.ColumnIndex,
                                false);

                            if (columnIndexOfCharacterWithDifferingKind == -1) // Move to end of line
                            {
                                cursorModifier.SetColumnIndexAndPreferred(lengthOfLine);
                            }
                            else
                            {
                            	if (!keymapArgs.AltKey) // Move by character kind
                            	{
                            		cursorModifier.SetColumnIndexAndPreferred(
                                    	columnIndexOfCharacterWithDifferingKind);
                            	}
                                else // Move by camel case
                                {
                                	var positionIndex = modelModifier.GetPositionIndex(cursorModifier);
									var rememberStartPositionIndex = positionIndex;
									
									var maxPositionIndex = columnIndexOfCharacterWithDifferingKind;
									
									var infiniteLoopPrediction = false;
									
									if (maxPositionIndex < positionIndex)
										infiniteLoopPrediction = true;
									
									bool useCamelCaseResult = false;
									
									if (!infiniteLoopPrediction)
									{
										while (++positionIndex < maxPositionIndex)
										{
											var currentRichCharacter = modelModifier.RichCharacterList[positionIndex];
											
											if (Char.IsUpper(currentRichCharacter.Value) || currentRichCharacter.Value == '_')
											{
												useCamelCaseResult = true;
												break;
											}
										}
									}
									
									if (useCamelCaseResult)
									{
										var columnDisplacement = positionIndex - rememberStartPositionIndex;
										cursorModifier.SetColumnIndexAndPreferred(cursorModifier.ColumnIndex + columnDisplacement);
									}
									else
									{
										cursorModifier.SetColumnIndexAndPreferred(
                                    		columnIndexOfCharacterWithDifferingKind);
									}
                                }
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
                if (keymapArgs.CtrlKey)
                {
                    cursorModifier.LineIndex = 0;
                    cursorModifier.SetColumnIndexAndPreferred(0);
                }
				else
				{
					var originalPositionIndex = modelModifier.GetPositionIndex(cursorModifier);
					
					var lineInformation = modelModifier.GetLineInformation(cursorModifier.LineIndex);
					var lastValidPositionIndex = lineInformation.StartPositionIndexInclusive + lineInformation.LastValidColumnIndex;
					
					cursorModifier.ColumnIndex = 0; // This column index = 0 is needed for the while loop below.
					var indentationPositionIndexExclusiveEnd = modelModifier.GetPositionIndex(cursorModifier);
					
					var cursorWithinIndentation = false;
		
					while (indentationPositionIndexExclusiveEnd < lastValidPositionIndex)
					{
						var possibleIndentationChar = modelModifier.RichCharacterList[indentationPositionIndexExclusiveEnd].Value;
		
						if (possibleIndentationChar == '\t' || possibleIndentationChar == ' ')
						{
							if (indentationPositionIndexExclusiveEnd == originalPositionIndex)
								cursorWithinIndentation = true;
						}
						else
						{
							break;
						}
						
						indentationPositionIndexExclusiveEnd++;
					}
					
					if (originalPositionIndex == indentationPositionIndexExclusiveEnd)
						cursorModifier.SetColumnIndexAndPreferred(0);
					else
						cursorModifier.SetColumnIndexAndPreferred(
							indentationPositionIndexExclusiveEnd - lineInformation.StartPositionIndexInclusive);
				}

                break;
            case KeyboardKeyFacts.MovementKeys.END:
                if (keymapArgs.CtrlKey)
                    cursorModifier.LineIndex = modelModifier.LineCount - 1;

                lengthOfLine = modelModifier.GetLineLength(cursorModifier.LineIndex);

                cursorModifier.SetColumnIndexAndPreferred(lengthOfLine);

                break;
        }

        if (keymapArgs.ShiftKey)
        {
            cursorModifier.SelectionEndingPositionIndex = modelModifier.GetPositionIndex(
                cursorModifier.LineIndex,
                cursorModifier.ColumnIndex);
        }
        else if (!keymapArgs.ShiftKey && shouldClearSelection)
        {
            // The active selection is needed, and cannot be touched until the end.
            cursorModifier.SelectionAnchorPositionIndex = null;
            cursorModifier.SelectionEndingPositionIndex = 0;
        }
    }

    public void CursorMovePageTop(
        ITextEditorEditContext editContext,
        TextEditorViewModelModifier viewModelModifier,
        CursorModifierBagTextEditor cursorModifierBag)
    {
        CursorMovePageTopUnsafe(
	        editContext,
        	viewModelModifier,
        	cursorModifierBag,
        	editContext.GetPrimaryCursorModifier(cursorModifierBag));
    }

    public void CursorMovePageTopUnsafe(
        ITextEditorEditContext editContext,
        TextEditorViewModelModifier viewModelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        TextEditorCursorModifier cursorModifier)
    {
        if (viewModelModifier.ViewModel.VirtualizationResult?.EntryList.Any() ?? false)
        {
            var firstEntry = viewModelModifier.ViewModel.VirtualizationResult.EntryList.First();

            cursorModifier.LineIndex = firstEntry.Index;
            cursorModifier.ColumnIndex = 0;
        }
    }

    public void CursorMovePageBottom(
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        TextEditorViewModelModifier viewModelModifier,
        CursorModifierBagTextEditor cursorModifierBag)
    {
        CursorMovePageBottomUnsafe(
        	editContext,
        	modelModifier,
        	viewModelModifier,
        	cursorModifierBag,
        	editContext.GetPrimaryCursorModifier(cursorModifierBag));
    }

    public void CursorMovePageBottomUnsafe(
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        TextEditorViewModelModifier viewModelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        TextEditorCursorModifier cursorModifier)
    {
        if ((viewModelModifier.ViewModel.VirtualizationResult?.EntryList.Any() ?? false))
        {
            var lastEntry = viewModelModifier.ViewModel.VirtualizationResult.EntryList.Last();
            var lastEntriesLineLength = modelModifier.GetLineLength(lastEntry.Index);

            cursorModifier.LineIndex = lastEntry.Index;
            cursorModifier.ColumnIndex = lastEntriesLineLength;
        }
    }
    
    public void RevealCursor(
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        TextEditorViewModelModifier viewModelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        TextEditorCursorModifier cursorModifier)
    {
    	try
    	{
    		if (!viewModelModifier.ViewModel.UnsafeState.ShouldRevealCursor)
    			return;
    			
    		viewModelModifier.ViewModel.UnsafeState.ShouldRevealCursor = false;
    	
    		var cursorIsVisible = false;
    		
    		if (cursorIsVisible)
    			return;
    			
    		Console.WriteLine(nameof(RevealCursor));
		
            var cursorPositionIndex = modelModifier.GetPositionIndex(cursorModifier);

            var cursorTextSpan = new TextEditorTextSpan(
                cursorPositionIndex,
                cursorPositionIndex + 1,
                0,
                modelModifier.ResourceUri,
                sourceText: string.Empty,
                getTextPrecalculatedResult: string.Empty);

            ScrollIntoView(
        		editContext,
		        modelModifier,
		        viewModelModifier,
		        cursorTextSpan);
    	}
    	catch (LuthetusTextEditorException exception)
    	{
    		Console.WriteLine(exception);
    	}
    }

    public void CalculateVirtualizationResult(
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
		TextEditorViewModelModifier viewModelModifier,
        CancellationToken cancellationToken)
    {
        try
		{
			var virtualizationResult = viewModelModifier.ViewModel.VirtualizationResult;

			var verticalStartingIndex = (int)Math.Floor(
				viewModelModifier.ViewModel.ScrollbarDimensions.ScrollTop /
				viewModelModifier.ViewModel.CharAndLineMeasurements.LineHeight);

			var verticalTake = (int)Math.Ceiling(
				viewModelModifier.ViewModel.TextEditorDimensions.Height /
				viewModelModifier.ViewModel.CharAndLineMeasurements.LineHeight);

			// Vertical Padding (render some offscreen data)
			verticalTake += 1;

			// Check index boundaries
			{
				verticalStartingIndex = Math.Max(0, verticalStartingIndex);

				if (verticalStartingIndex + verticalTake > modelModifier.LineEndList.Count)
				{
					verticalTake = modelModifier.LineEndList.Count - verticalStartingIndex;
				}

				verticalTake = Math.Max(0, verticalTake);
			}

			var horizontalStartingIndex = (int)Math.Floor(
				viewModelModifier.ViewModel.ScrollbarDimensions.ScrollLeft /
				viewModelModifier.ViewModel.CharAndLineMeasurements.CharacterWidth);

			var horizontalTake = (int)Math.Ceiling(
				viewModelModifier.ViewModel.TextEditorDimensions.Width /
				viewModelModifier.ViewModel.CharAndLineMeasurements.CharacterWidth);

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
						viewModelModifier.ViewModel.CharAndLineMeasurements.CharacterWidth;

					var leftInPixels = localHorizontalStartingIndex *
						viewModelModifier.ViewModel.CharAndLineMeasurements.CharacterWidth;

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
							viewModelModifier.ViewModel.CharAndLineMeasurements.CharacterWidth);
					}

					leftInPixels = Math.Max(0, leftInPixels);

					var topInPixels = lineIndex * viewModelModifier.ViewModel.CharAndLineMeasurements.LineHeight;

					return new VirtualizationEntry<List<RichCharacter>>(
						lineIndex,
						horizontallyVirtualizedLine,
						widthInPixels,
						viewModelModifier.ViewModel.CharAndLineMeasurements.LineHeight,
						leftInPixels,
						topInPixels);
				}).ToArray();

			var totalWidth = (int)Math.Ceiling(modelModifier.MostCharactersOnASingleLineTuple.lineLength *
				viewModelModifier.ViewModel.CharAndLineMeasurements.CharacterWidth);

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
					viewModelModifier.ViewModel.CharAndLineMeasurements.CharacterWidth);
			}

			var totalHeight = (int)Math.Ceiling(modelModifier.LineEndList.Count *
				viewModelModifier.ViewModel.CharAndLineMeasurements.LineHeight);

			// Add vertical margin so the user can scroll beyond the final line of content
			int marginScrollHeight;
			{
				var percentOfMarginScrollHeightByPageUnit = 0.4;

				marginScrollHeight = (int)Math.Ceiling(viewModelModifier.ViewModel.TextEditorDimensions.Height * percentOfMarginScrollHeightByPageUnit);
				totalHeight += marginScrollHeight;
			}

			var leftBoundaryWidthInPixels = horizontalStartingIndex *
				viewModelModifier.ViewModel.CharAndLineMeasurements.CharacterWidth;

			var leftBoundary = new VirtualizationBoundary(
				leftBoundaryWidthInPixels,
				totalHeight,
				0,
				0);

			var rightBoundaryLeftInPixels = leftBoundary.WidthInPixels +
				viewModelModifier.ViewModel.CharAndLineMeasurements.CharacterWidth *
				horizontalTake;

			var rightBoundaryWidthInPixels = totalWidth - rightBoundaryLeftInPixels;

			var rightBoundary = new VirtualizationBoundary(
				rightBoundaryWidthInPixels,
				totalHeight,
				rightBoundaryLeftInPixels,
				0);

			var topBoundaryHeightInPixels = verticalStartingIndex *
				viewModelModifier.ViewModel.CharAndLineMeasurements.LineHeight;

			var topBoundary = new VirtualizationBoundary(
				totalWidth,
				topBoundaryHeightInPixels,
				0,
				0);

			var bottomBoundaryTopInPixels = topBoundary.HeightInPixels +
				viewModelModifier.ViewModel.CharAndLineMeasurements.LineHeight *
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
				bottomBoundary);

			viewModelModifier.ViewModel = viewModelModifier.ViewModel with
			{
				VirtualizationResult = virtualizationResult,
				ScrollbarDimensions = viewModelModifier.ViewModel.ScrollbarDimensions with
				{
					ScrollWidth = totalWidth,
					ScrollHeight = totalHeight,
					MarginScrollHeight = marginScrollHeight
				},
			};
		}
		catch (LuthetusTextEditorException exception)
		{
			/*
			 Got an exception from lineIndex being out of range (2024-05-12).
			 |
			 Move the cursor to the closest valid positionIndex in this situation.
			 =============================================================================
			 fail: Luthetus.Common.RazorLib.BackgroundTasks.Models.BackgroundTaskWorker[0]
			 Error occurred executing Cut_d6b3a112-449b-44c1-9466-837a98fbf941_sb.
			 Luthetus.TextEditor.RazorLib.Exceptions.LuthetusTextEditorException: 'lineIndex:8' >= model.LineCount:1
			 at Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModelExtensionMethods.AssertLineIndex(ITextEditorModel model, Int32 lineIndex) in C:\Users\hunte\Repos\Luthetus.Ide_Fork\Source\Lib\TextEditor\TextEditors\Models\TextEditorModelExtensionMethods.cs:line 564
			 at Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModelExtensionMethods.GetLineInformation(ITextEditorModel model, Int32 lineIndex) in C:\Users\hunte\Repos\Luthetus.Ide_Fork\Source\Lib\TextEditor\TextEditors\Models\TextEditorModelExtensionMethods.cs:line 306
			 at Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorViewModelApi.<>c__DisplayClass39_0.<<CalculateVirtualizationResultFactory>b__0>d.MoveNext() in C:\Users\hunte\Repos\Luthetus.Ide_Fork\Source\Lib\TextEditor\TextEditors\Models\TextEditorViewModelApi.cs:line 889
			 --- End of stack trace from previous location ---
			 at Luthetus.TextEditor.RazorLib.TextEditorServiceTask.HandleEvent(CancellationToken cancellationToken) in C:\Users\hunte\Repos\Luthetus.Ide_Fork\Source\Lib\TextEditor\TextEditorServiceTask.cs:line 113
			 at Luthetus.Common.RazorLib.BackgroundTasks.Models.BackgroundTaskWorker.ExecuteAsync(CancellationToken cancellationToken) in C:\Users\hunte\Repos\Luthetus.Ide_Fork\Source\Lib\Common\BackgroundTasks\Models\BackgroundTaskWorker.cs:line 49
			 */
			
			var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
			var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);
			
			if (primaryCursorModifier is not null)
			{
				if (primaryCursorModifier.LineIndex >= modelModifier.LineCount)
					primaryCursorModifier.LineIndex = modelModifier.LineCount - 1;
	
				var lineInformation = modelModifier.GetLineInformation(primaryCursorModifier.LineIndex);
	
				primaryCursorModifier.ColumnIndex = lineInformation.LastValidColumnIndex;
			}

#if DEBUG
			// This exception happens a lot while using the editor.
			// It is believed that the published demo WASM app is being slowed down
			// in part from logging this a lot.
			Console.WriteLine(exception);
#endif
		}
    }

    public async Task RemeasureAsync(
        ITextEditorEditContext editContext,
        TextEditorViewModelModifier viewModelModifier,
        string measureCharacterWidthAndLineHeightElementId,
        int countOfTestCharacters,
        CancellationToken cancellationToken)
    {
        var options = _textEditorService.OptionsApi.GetOptions();

		var characterWidthAndLineHeight = await _textEditorService.ViewModelApi.MeasureCharacterWidthAndLineHeightAsync(
				measureCharacterWidthAndLineHeightElementId,
				countOfTestCharacters)
			.ConfigureAwait(false);

		var textEditorMeasurements = await _textEditorService.ViewModelApi
			.GetTextEditorMeasurementsAsync(viewModelModifier.ViewModel.BodyElementId)
			.ConfigureAwait(false);

		viewModelModifier.ViewModel = viewModelModifier.ViewModel with
		{
			CharAndLineMeasurements = characterWidthAndLineHeight,
			TextEditorDimensions = textEditorMeasurements
		};
    }

    public void ForceRender(
        ITextEditorEditContext editContext,
        TextEditorViewModelModifier viewModelModifier,
        CancellationToken cancellationToken)
    {
        // Getting the ViewModel from the 'editContext' triggers a re-render
        //
        // A lot code is being changed and one result is this method now reads like non-sense,
        // (or more non-sense than it previously did)
        // Because we get a viewModelModifier passed in to this method as an argument.
        // So this seems quite silly.
		_ = editContext.GetViewModelModifier(viewModelModifier.ViewModel.ViewModelKey);
    }
    #endregion

    #region DELETE_METHODS
    public void Dispose(Key<TextEditorViewModel> viewModelKey)
    {
        _dispatcher.Dispatch(new TextEditorState.DisposeViewModelAction(viewModelKey));
    }
    #endregion
}