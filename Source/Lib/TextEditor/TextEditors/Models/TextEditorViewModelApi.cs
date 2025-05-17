using System.Diagnostics;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.JsRuntimes.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.TextEditor.RazorLib.JavaScriptObjects.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.Virtualizations.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

public sealed class TextEditorViewModelApi
{
    private readonly TextEditorService _textEditorService;
    private readonly BackgroundTaskService _backgroundTaskService;
    private readonly IDialogService _dialogService;
    private readonly IPanelService _panelService;

    private readonly CommonBackgroundTaskApi _commonBackgroundTaskApi;

    public TextEditorViewModelApi(
        TextEditorService textEditorService,
        BackgroundTaskService backgroundTaskService,
        CommonBackgroundTaskApi commonBackgroundTaskApi,
        IDialogService dialogService,
        IPanelService panelService)
    {
        _textEditorService = textEditorService;
        _backgroundTaskService = backgroundTaskService;
        _commonBackgroundTaskApi = commonBackgroundTaskApi;
        _dialogService = dialogService;
        _panelService = panelService;
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
    	TextEditorEditContext editContext,
        Key<TextEditorViewModel> viewModelKey,
        ResourceUri resourceUri,
        Category category)
    {
	    var viewModel = new TextEditorViewModel(
			viewModelKey,
			resourceUri,
			_textEditorService,
			_panelService,
			_dialogService,
			_commonBackgroundTaskApi,
			VirtualizationGrid.Empty,
			new TextEditorDimensions(0, 0, 0, 0),
			scrollLeft: 0,
	    	scrollTop: 0,
		    scrollWidth: 0,
		    scrollHeight: 0,
		    marginScrollHeight: 0,
			category);
			
		_textEditorService.RegisterViewModel(editContext, viewModel);
    }
    
    public void Register(TextEditorEditContext editContext, TextEditorViewModel viewModel)
    {
        _textEditorService.RegisterViewModel(editContext, viewModel);
    }
    #endregion

    #region READ_METHODS
    public TextEditorViewModel? GetOrDefault(Key<TextEditorViewModel> viewModelKey)
    {
        return _textEditorService.TextEditorState.ViewModelGetOrDefault(
            viewModelKey);
    }

    public Dictionary<Key<TextEditorViewModel>, TextEditorViewModel> GetViewModels()
    {
        return _textEditorService.TextEditorState.ViewModelGetViewModels();
    }

    public TextEditorModel? GetModelOrDefault(Key<TextEditorViewModel> viewModelKey)
    {
        var viewModel = _textEditorService.TextEditorState.ViewModelGetOrDefault(
            viewModelKey);

        if (viewModel is null)
            return null;

        return _textEditorService.ModelApi.GetOrDefault(viewModel.PersistentState.ResourceUri);
    }

    public string? GetAllText(Key<TextEditorViewModel> viewModelKey)
    {
        var textEditorModel = GetModelOrDefault(viewModelKey);

        return textEditorModel is null
            ? null
            : _textEditorService.ModelApi.GetAllText(textEditorModel.PersistentState.ResourceUri);
    }

    public async ValueTask<TextEditorDimensions> GetTextEditorMeasurementsAsync(string elementId)
    {
        return await _textEditorService.JsRuntimeTextEditorApi
            .GetTextEditorMeasurementsInPixelsById(elementId)
            .ConfigureAwait(false);
    }
    #endregion

    #region UPDATE_METHODS
    public void SetScrollPositionBoth(
        TextEditorEditContext editContext,
        TextEditorViewModel viewModel,
        double scrollLeftInPixels,
        double scrollTopInPixels)
    {
    	viewModel.ScrollWasModified = true;

		viewModel.SetScrollLeft((int)Math.Floor(scrollLeftInPixels), viewModel.TextEditorDimensions);

		viewModel.SetScrollTop((int)Math.Floor(scrollTopInPixels), viewModel.TextEditorDimensions);
    }
        
    public void SetScrollPositionLeft(
        TextEditorEditContext editContext,
        TextEditorViewModel viewModel,
        double scrollLeftInPixels)
    {
    	viewModel.ScrollWasModified = true;

		viewModel.SetScrollLeft((int)Math.Floor(scrollLeftInPixels), viewModel.TextEditorDimensions);
    }
    
    public void SetScrollPositionTop(
        TextEditorEditContext editContext,
        TextEditorViewModel viewModel,
        double scrollTopInPixels)
    {
    	viewModel.ScrollWasModified = true;

		viewModel.SetScrollTop((int)Math.Floor(scrollTopInPixels), viewModel.TextEditorDimensions);
    }

    public void MutateScrollVerticalPosition(
        TextEditorEditContext editContext,
        TextEditorViewModel viewModel,
        double pixels)
    {
        viewModel.ScrollWasModified = true;

        viewModel.MutateScrollTop((int)Math.Ceiling(pixels), viewModel.TextEditorDimensions);
    }

    public void MutateScrollHorizontalPosition(
        TextEditorEditContext editContext,
        TextEditorViewModel viewModel,
        double pixels)
    {
        viewModel.ScrollWasModified = true;

		viewModel.MutateScrollLeft((int)Math.Ceiling(pixels), viewModel.TextEditorDimensions);
    }

    public void ScrollIntoView(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        TextEditorViewModel viewModel,
        TextEditorTextSpan textSpan)
    {
        var lineInformation = modelModifier.GetLineInformationFromPositionIndex(textSpan.StartInclusiveIndex);
        var lineIndex = lineInformation.Index;
        var columnIndex = textSpan.StartInclusiveIndex - lineInformation.Position_StartInclusiveIndex;

        // Unit of measurement is pixels (px)
        var scrollLeft = columnIndex *
            viewModel.CharAndLineMeasurements.CharacterWidth;
            
        var hiddenLineCount = 0;
        foreach (var index in viewModel.HiddenLineIndexHashSet)
        {
        	if (index < lineIndex)
        		hiddenLineCount++;
        }

        // Unit of measurement is pixels (px)
        var scrollTop = (lineIndex - hiddenLineCount) *
            viewModel.CharAndLineMeasurements.LineHeight;

		var currentScrollLeft = viewModel.ScrollLeft;
		var currentScrollTop = viewModel.ScrollTop;
		
		bool caseA;
		bool caseB;
		
        // If a given scroll direction is already within view of the text span, do not scroll on that direction
        
        // scrollLeft needs to be modified?
        var currentWidth = viewModel.TextEditorDimensions.Width;

        caseA = currentScrollLeft <= scrollLeft;
        caseB = scrollLeft < (currentWidth + currentScrollLeft);

        if (caseA && caseB)
            scrollLeft = currentScrollLeft;

        // scrollTop needs to be modified?
        var currentHeight = viewModel.TextEditorDimensions.Height;

        caseA = currentScrollTop <= scrollTop;
        caseB = scrollTop < (currentHeight + currentScrollTop);

        if (caseA && caseB)
            scrollTop = currentScrollTop;

        // Return early if both values are 'null'
        if (scrollLeft == currentScrollLeft && scrollTop == currentScrollTop)
            return;

        SetScrollPositionBoth(
            editContext,
	        viewModel,
	        scrollLeft,
            scrollTop);
    }

    public ValueTask FocusPrimaryCursorAsync(string primaryCursorContentId)
    {
        return _commonBackgroundTaskApi.JsRuntimeCommonApi
            .FocusHtmlElementById(primaryCursorContentId, preventScroll: true);
    }

    public void MoveCursor(
        KeymapArgs keymapArgs,
		TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        TextEditorViewModel viewModel)
    {
        MoveCursorUnsafe(
            keymapArgs,
	        editContext,
	        modelModifier,
	        viewModel);

        viewModel.PersistentState.ShouldRevealCursor = true;
    }

    public void MoveCursorUnsafe(
        KeymapArgs keymapArgs,
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        TextEditorViewModel viewModel)
    {
        var shouldClearSelection = false;

        if (keymapArgs.ShiftKey)
        {
            if (viewModel.SelectionAnchorPositionIndex == -1 ||
                viewModel.SelectionEndingPositionIndex == viewModel.SelectionAnchorPositionIndex)
            {
                var positionIndex = modelModifier.GetPositionIndex(
                    viewModel.LineIndex,
                    viewModel.ColumnIndex);

                viewModel.SelectionAnchorPositionIndex = positionIndex;
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
                if (TextEditorSelectionHelper.HasSelectedText(viewModel) &&
                    !keymapArgs.ShiftKey)
                {
                    var selectionBounds = TextEditorSelectionHelper.GetSelectionBounds(viewModel);

                    var lowerLineInformation = modelModifier.GetLineInformationFromPositionIndex(
                        selectionBounds.Position_LowerInclusiveIndex);

                    viewModel.LineIndex = lowerLineInformation.Index;

                    viewModel.ColumnIndex = selectionBounds.Position_LowerInclusiveIndex -
                        lowerLineInformation.Position_StartInclusiveIndex;
                }
                else
                {
                    if (viewModel.ColumnIndex <= 0)
                    {
                        if (viewModel.LineIndex != 0)
                        {
                            viewModel.LineIndex--;

                            lengthOfLine = modelModifier.GetLineLength(viewModel.LineIndex);

                            viewModel.SetColumnIndexAndPreferred(lengthOfLine);
                        }
                        else
                        {
                            viewModel.SetColumnIndexAndPreferred(0);
                        }
                    }
                    else
                    {
                        if (keymapArgs.CtrlKey)
                        {
                        	var columnIndexOfCharacterWithDifferingKind = modelModifier.GetColumnIndexOfCharacterWithDifferingKind(
                                viewModel.LineIndex,
                                viewModel.ColumnIndex,
                                true);

                            if (columnIndexOfCharacterWithDifferingKind == -1) // Move to start of line
                            {
                                viewModel.SetColumnIndexAndPreferred(0);
                            }
                            else
                            {
                            	if (!keymapArgs.AltKey) // Move by character kind
                            	{
                            		viewModel.SetColumnIndexAndPreferred(columnIndexOfCharacterWithDifferingKind);
                            	}
                                else // Move by camel case
                                {
                                	var positionIndex = modelModifier.GetPositionIndex(viewModel);
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
										viewModel.SetColumnIndexAndPreferred(viewModel.ColumnIndex + columnDisplacement);
									}
									else
									{
										viewModel.SetColumnIndexAndPreferred(columnIndexOfCharacterWithDifferingKind);
									}
                                }
                            }
                        }
                        else
                        {
                            viewModel.SetColumnIndexAndPreferred(viewModel.ColumnIndex - 1);
                        }
                    }
                }

                break;
            case KeyboardKeyFacts.MovementKeys.ARROW_DOWN:
                if (viewModel.LineIndex < modelModifier.LineCount - 1)
                {
                    viewModel.LineIndex++;

                    lengthOfLine = modelModifier.GetLineLength(viewModel.LineIndex);

                    viewModel.ColumnIndex = lengthOfLine < viewModel.PreferredColumnIndex
                        ? lengthOfLine
                        : viewModel.PreferredColumnIndex;
                }

                break;
            case KeyboardKeyFacts.MovementKeys.ARROW_UP:
                if (viewModel.LineIndex > 0)
                {
                    viewModel.LineIndex--;

                    lengthOfLine = modelModifier.GetLineLength(viewModel.LineIndex);

                    viewModel.ColumnIndex = lengthOfLine < viewModel.PreferredColumnIndex
                        ? lengthOfLine
                        : viewModel.PreferredColumnIndex;
                }

                break;
            case KeyboardKeyFacts.MovementKeys.ARROW_RIGHT:
            	if (viewModel.PersistentState.VirtualAssociativityKind == VirtualAssociativityKind.Left)
            	{
            		viewModel.PersistentState.VirtualAssociativityKind = VirtualAssociativityKind.Right;
            	}
                else if (TextEditorSelectionHelper.HasSelectedText(viewModel) && !keymapArgs.ShiftKey)
                {
                    var selectionBounds = TextEditorSelectionHelper.GetSelectionBounds(viewModel);

                    var upperLineMetaData = modelModifier.GetLineInformationFromPositionIndex(
                        selectionBounds.Position_UpperExclusiveIndex);

                    viewModel.LineIndex = upperLineMetaData.Index;

                    if (viewModel.LineIndex >= modelModifier.LineCount)
                    {
                        viewModel.LineIndex = modelModifier.LineCount - 1;

                        var upperLineLength = modelModifier.GetLineLength(viewModel.LineIndex);

                        viewModel.ColumnIndex = upperLineLength;
                    }
                    else
                    {
                        viewModel.ColumnIndex =
                            selectionBounds.Position_UpperExclusiveIndex - upperLineMetaData.Position_StartInclusiveIndex;
                    }
                }
                else
                {
                    lengthOfLine = modelModifier.GetLineLength(viewModel.LineIndex);

                    if (viewModel.ColumnIndex >= lengthOfLine &&
                        viewModel.LineIndex < modelModifier.LineCount - 1)
                    {
                        viewModel.SetColumnIndexAndPreferred(0);
                        viewModel.LineIndex++;
                    }
                    else if (viewModel.ColumnIndex != lengthOfLine)
                    {
                        if (keymapArgs.CtrlKey)
                        {
                        	var columnIndexOfCharacterWithDifferingKind = modelModifier.GetColumnIndexOfCharacterWithDifferingKind(
                                viewModel.LineIndex,
                                viewModel.ColumnIndex,
                                false);

                            if (columnIndexOfCharacterWithDifferingKind == -1) // Move to end of line
                            {
                                viewModel.SetColumnIndexAndPreferred(lengthOfLine);
                            }
                            else
                            {
                            	if (!keymapArgs.AltKey) // Move by character kind
                            	{
                            		viewModel.SetColumnIndexAndPreferred(
                                    	columnIndexOfCharacterWithDifferingKind);
                            	}
                                else // Move by camel case
                                {
                                	var positionIndex = modelModifier.GetPositionIndex(viewModel);
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
										viewModel.SetColumnIndexAndPreferred(viewModel.ColumnIndex + columnDisplacement);
									}
									else
									{
										viewModel.SetColumnIndexAndPreferred(
                                    		columnIndexOfCharacterWithDifferingKind);
									}
                                }
                            }
                        }
                        else
                        {
                            viewModel.SetColumnIndexAndPreferred(viewModel.ColumnIndex + 1);
                        }
                    }
                }

                break;
            case KeyboardKeyFacts.MovementKeys.HOME:
                if (keymapArgs.CtrlKey)
                {
                    viewModel.LineIndex = 0;
                    viewModel.SetColumnIndexAndPreferred(0);
                }
				else
				{
					var originalPositionIndex = modelModifier.GetPositionIndex(viewModel);
					
					var lineInformation = modelModifier.GetLineInformation(viewModel.LineIndex);
					var lastValidPositionIndex = lineInformation.Position_StartInclusiveIndex + lineInformation.LastValidColumnIndex;
					
					viewModel.ColumnIndex = 0; // This column index = 0 is needed for the while loop below.
					var indentationPositionIndexExclusiveEnd = modelModifier.GetPositionIndex(viewModel);
					
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
						viewModel.SetColumnIndexAndPreferred(0);
					else
						viewModel.SetColumnIndexAndPreferred(
							indentationPositionIndexExclusiveEnd - lineInformation.Position_StartInclusiveIndex);
				}

                break;
            case KeyboardKeyFacts.MovementKeys.END:
                if (keymapArgs.CtrlKey)
                    viewModel.LineIndex = modelModifier.LineCount - 1;

                lengthOfLine = modelModifier.GetLineLength(viewModel.LineIndex);

                viewModel.SetColumnIndexAndPreferred(lengthOfLine);

                break;
        }
        
        if (viewModel.HiddenLineIndexHashSet.Contains(viewModel.LineIndex))
        {
        	switch (keymapArgs.Key)
        	{
        		case KeyboardKeyFacts.MovementKeys.ARROW_LEFT:
        		{
        			CollapsePoint encompassingCollapsePoint = new CollapsePoint(-1, false, string.Empty, -1);;

        			foreach (var collapsePoint in viewModel.AllCollapsePointList)
        			{
        				var firstToHideLineIndex = collapsePoint.AppendToLineIndex + 1;
						for (var lineOffset = 0; lineOffset < collapsePoint.EndExclusiveLineIndex - collapsePoint.AppendToLineIndex - 1; lineOffset++)
						{
							if (viewModel.LineIndex == firstToHideLineIndex + lineOffset)
								encompassingCollapsePoint = collapsePoint;
						}
        			}
        			
        			if (encompassingCollapsePoint.AppendToLineIndex != -1)
        			{
        				var lineIndex = encompassingCollapsePoint.EndExclusiveLineIndex - 1;
        				var lineInformation = modelModifier.GetLineInformation(lineIndex);
        				
        				if (viewModel.ColumnIndex != lineInformation.LastValidColumnIndex)
        				{
        					for (int i = viewModel.LineIndex; i >= 0; i--)
		        			{
		        				if (!viewModel.HiddenLineIndexHashSet.Contains(i))
		        				{
		        					viewModel.LineIndex = i;
		        					lineInformation = modelModifier.GetLineInformation(i);
		        					viewModel.ColumnIndex = lineInformation.LastValidColumnIndex;
		        					break;
		        				}
		        			}
        				}
        			}
        		
        			break;
        		}
        		case KeyboardKeyFacts.MovementKeys.ARROW_DOWN:
        		{
        			var success = false;
        		
        			for (int i = viewModel.LineIndex + 1; i < modelModifier.LineCount; i++)
        			{
        				if (!viewModel.HiddenLineIndexHashSet.Contains(i))
        				{
        					success = true;
        					viewModel.LineIndex = i;
        					
        					var lineInformation = modelModifier.GetLineInformation(i);
        					
        					if (viewModel.ColumnIndex > lineInformation.LastValidColumnIndex)
        						viewModel.ColumnIndex = lineInformation.LastValidColumnIndex;
        					
        					break;
        				}
        			}
        			
        			if (!success)
        			{
        				for (int i = viewModel.LineIndex; i >= 0; i--)
	        			{
	        				if (!viewModel.HiddenLineIndexHashSet.Contains(i))
	        				{
	        					viewModel.LineIndex = i;
	        					
	        					var lineInformation = modelModifier.GetLineInformation(i);
	        					
	        					if (viewModel.ColumnIndex > lineInformation.LastValidColumnIndex)
	        						viewModel.ColumnIndex = lineInformation.LastValidColumnIndex;
	        					
	        					break;
	        				}
	        			}
        			}
        			
        			break;
        		}
        		case KeyboardKeyFacts.MovementKeys.ARROW_UP:
        		{
        			var success = false;
        			
        			for (int i = viewModel.LineIndex; i >= 0; i--)
        			{
        				if (!viewModel.HiddenLineIndexHashSet.Contains(i))
        				{
        					success = true;
        					viewModel.LineIndex = i;
        					
        					var lineInformation = modelModifier.GetLineInformation(i);
        					
        					if (viewModel.PreferredColumnIndex <= lineInformation.LastValidColumnIndex)
        						viewModel.ColumnIndex = viewModel.PreferredColumnIndex;
        					else if (viewModel.ColumnIndex > lineInformation.LastValidColumnIndex)
        						viewModel.ColumnIndex = lineInformation.LastValidColumnIndex;
        					
        					break;
        				}
        			}
        		
        			if (!success)
        			{
        				for (int i = viewModel.LineIndex + 1; i < modelModifier.LineCount; i++)
	        			{
	        				if (!viewModel.HiddenLineIndexHashSet.Contains(i))
	        				{
	        					viewModel.LineIndex = i;
	        					
	        					var lineInformation = modelModifier.GetLineInformation(i);
	        					
	        					if (viewModel.ColumnIndex > lineInformation.LastValidColumnIndex)
	        						viewModel.ColumnIndex = lineInformation.LastValidColumnIndex;
	        					
	        					break;
	        				}
	        			}
        			}
        			
        			break;
        		}
        		case KeyboardKeyFacts.MovementKeys.ARROW_RIGHT:
    			{
        			CollapsePoint encompassingCollapsePoint = new CollapsePoint(-1, false, string.Empty, -1);;

        			foreach (var collapsePoint in viewModel.AllCollapsePointList)
        			{
        				var firstToHideLineIndex = collapsePoint.AppendToLineIndex + 1;
						for (var lineOffset = 0; lineOffset < collapsePoint.EndExclusiveLineIndex - collapsePoint.AppendToLineIndex - 1; lineOffset++)
						{
							if (viewModel.LineIndex == firstToHideLineIndex + lineOffset)
								encompassingCollapsePoint = collapsePoint;
						}
        			}
        			
        			if (encompassingCollapsePoint.AppendToLineIndex != -1)
        			{
        				var lineIndex = encompassingCollapsePoint.EndExclusiveLineIndex - 1;
        			
        				var lineInformation = modelModifier.GetLineInformation(lineIndex);
						viewModel.LineIndex = lineIndex;
						viewModel.SetColumnIndexAndPreferred(lineInformation.LastValidColumnIndex);
        			}
	        			
        			break;
        		}
        		case KeyboardKeyFacts.MovementKeys.HOME:
        		case KeyboardKeyFacts.MovementKeys.END:
        		{
        			break;
        		}
        	}
        }
        
        (int lineIndex, int columnIndex) lineAndColumnIndices = (0, 0);
		var inlineUi = new InlineUi(0, InlineUiKind.None);
		
		foreach (var inlineUiTuple in viewModel.InlineUiList)
		{
			lineAndColumnIndices = modelModifier.GetLineAndColumnIndicesFromPositionIndex(inlineUiTuple.InlineUi.PositionIndex);
			
			if (lineAndColumnIndices.lineIndex == viewModel.LineIndex &&
				lineAndColumnIndices.columnIndex == viewModel.ColumnIndex)
			{
				inlineUi = inlineUiTuple.InlineUi;
			}
		}
		
		if (viewModel.PersistentState.VirtualAssociativityKind == VirtualAssociativityKind.None &&
			inlineUi.InlineUiKind != InlineUiKind.None)
		{
			viewModel.PersistentState.VirtualAssociativityKind = VirtualAssociativityKind.Left;
		}
		
		if (inlineUi.InlineUiKind == InlineUiKind.None)
			viewModel.PersistentState.VirtualAssociativityKind = VirtualAssociativityKind.None;

        if (keymapArgs.ShiftKey)
        {
            viewModel.SelectionEndingPositionIndex = modelModifier.GetPositionIndex(
                viewModel.LineIndex,
                viewModel.ColumnIndex);
        }
        else if (!keymapArgs.ShiftKey && shouldClearSelection)
        {
            // The active selection is needed, and cannot be touched until the end.
            viewModel.SelectionAnchorPositionIndex = -1;
            viewModel.SelectionEndingPositionIndex = 0;
        }
    }

    public void CursorMovePageTop(
        TextEditorEditContext editContext,
        TextEditorViewModel viewModel)
    {
        CursorMovePageTopUnsafe(
	        editContext,
        	viewModel);
    }

    public void CursorMovePageTopUnsafe(
        TextEditorEditContext editContext,
        TextEditorViewModel viewModel)
    {
        if (viewModel.VirtualizationResult.EntryList.Any())
        {
            var firstEntry = viewModel.VirtualizationResult.EntryList.First();

            viewModel.LineIndex = firstEntry.LineIndex;
            viewModel.ColumnIndex = 0;
        }
    }

    public void CursorMovePageBottom(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        TextEditorViewModel viewModel)
    {
        CursorMovePageBottomUnsafe(
        	editContext,
        	modelModifier,
        	viewModel);
    }

    public void CursorMovePageBottomUnsafe(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        TextEditorViewModel viewModel)
    {
        if ((viewModel.VirtualizationResult.EntryList.Any()))
        {
            var lastEntry = viewModel.VirtualizationResult.EntryList.Last();
            var lastEntriesLineLength = modelModifier.GetLineLength(lastEntry.LineIndex);

            viewModel.LineIndex = lastEntry.LineIndex;
            viewModel.ColumnIndex = lastEntriesLineLength;
        }
    }
    
    public void RevealCursor(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        TextEditorViewModel viewModel)
    {
    	try
    	{
    		if (!viewModel.PersistentState.ShouldRevealCursor)
    			return;
    			
    		viewModel.PersistentState.ShouldRevealCursor = false;
    	
    		var cursorIsVisible = false;
    		
    		if (cursorIsVisible)
    			return;
    			
    		// Console.WriteLine(nameof(RevealCursor));
		
            var cursorPositionIndex = modelModifier.GetPositionIndex(viewModel);

            var cursorTextSpan = new TextEditorTextSpan(
                cursorPositionIndex,
                cursorPositionIndex + 1,
                0,
                modelModifier.PersistentState.ResourceUri,
                sourceText: string.Empty,
                getTextPrecalculatedResult: string.Empty);

            ScrollIntoView(
        		editContext,
		        modelModifier,
		        viewModel,
		        cursorTextSpan);
    	}
    	catch (LuthetusTextEditorException exception)
    	{
    		Console.WriteLine(exception);
    	}
    }

    public void CalculateVirtualizationResult(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
		TextEditorViewModel viewModel,
		TextEditorComponentData componentData)
    {
    	#if DEBUG
    	var startTime = Stopwatch.GetTimestamp();
    	#endif
    	
    	if (!viewModel.ShouldCalculateVirtualizationResult)
    		return;
    	
        try
		{
			viewModel.ShouldCalculateVirtualizationResult = false;
		
			var verticalStartingIndex = (int)Math.Floor(
				viewModel.ScrollTop /
				viewModel.CharAndLineMeasurements.LineHeight);

			var verticalTake = (int)Math.Ceiling(
				viewModel.TextEditorDimensions.Height /
				viewModel.CharAndLineMeasurements.LineHeight);

			// Vertical Padding (render some offscreen data)
			verticalTake += 1;

			var horizontalStartingIndex = (int)Math.Floor(
				viewModel.ScrollLeft /
				viewModel.CharAndLineMeasurements.CharacterWidth);

			var horizontalTake = (int)Math.Ceiling(
				viewModel.TextEditorDimensions.Width /
				viewModel.CharAndLineMeasurements.CharacterWidth);
			
			var hiddenCount = 0;
			
			for (int i = 0; i < verticalStartingIndex; i++)
			{
				if (viewModel.HiddenLineIndexHashSet.Contains(i))
				{
					hiddenCount++;
					verticalStartingIndex++;
				}
			}
			
			verticalStartingIndex = Math.Max(0, verticalStartingIndex);
			
			if (verticalStartingIndex + verticalTake > modelModifier.LineEndList.Count)
			    verticalTake = modelModifier.LineEndList.Count - verticalStartingIndex;
			
			verticalTake = Math.Max(0, verticalTake);
			
			var lineCountAvailable = modelModifier.LineEndList.Count - verticalStartingIndex;
			
			var lineCountToReturn = verticalTake < lineCountAvailable
			    ? verticalTake
			    : lineCountAvailable;
			
			var endingLineIndexExclusive = verticalStartingIndex + lineCountToReturn;
			
			if (lineCountToReturn < 0 || verticalStartingIndex < 0 || endingLineIndexExclusive < 0)
			    return;
			    
			var totalWidth = (int)Math.Ceiling(modelModifier.MostCharactersOnASingleLineTuple.lineLength *
				viewModel.CharAndLineMeasurements.CharacterWidth);

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
					viewModel.CharAndLineMeasurements.CharacterWidth);
			}

			var totalHeight = (int)Math.Ceiling(
				(modelModifier.LineEndList.Count - viewModel.HiddenLineIndexHashSet.Count) *
				viewModel.CharAndLineMeasurements.LineHeight);

			// Add vertical margin so the user can scroll beyond the final line of content
			var percentOfMarginScrollHeightByPageUnit = 0.4;
			int marginScrollHeight = (int)Math.Ceiling(viewModel.TextEditorDimensions.Height * percentOfMarginScrollHeightByPageUnit);
			totalHeight += marginScrollHeight;
			
			var virtualizedLineList = new List<VirtualizationLine>(lineCountToReturn);
			
			var virtualizationResult = new VirtualizationGrid(
				virtualizedLineList,
        		new List<VirtualizationSpan>(),
				totalWidth: totalWidth,
		        totalHeight: totalHeight,
		        resultWidth: horizontalTake * viewModel.CharAndLineMeasurements.CharacterWidth,
		        resultHeight: verticalTake * viewModel.CharAndLineMeasurements.LineHeight,
		        left: horizontalStartingIndex * viewModel.CharAndLineMeasurements.CharacterWidth,
		        top: verticalStartingIndex * viewModel.CharAndLineMeasurements.LineHeight);
						
			viewModel.VirtualizationResult = virtualizationResult;
			
			viewModel.ScrollWidth = totalWidth;
			viewModel.ScrollHeight = totalHeight;
			viewModel.MarginScrollHeight = marginScrollHeight;
			
			viewModel.GutterWidthInPixels = GetGutterWidthInPixels(modelModifier, viewModel, componentData);
			
			if (componentData.VisualizationLineCacheIsInvalid)
				componentData.VirtualizationLineCacheClear();
			else
				componentData.VirtualizedLineCacheUsageHashSet.Clear();
			
			var absDiffScrollLeft = Math.Abs(componentData.VirtualizedLineCacheCreatedWithScrollLeft - viewModel.ScrollLeft);
			var useAll = absDiffScrollLeft < 0.01 && componentData.VirtualizedLineCacheViewModelKey == viewModel.PersistentState.ViewModelKey;
			
			/*var reUsedLines = 0;
			var emptyLines = 0;
			var calculatedLines = 0;*/
			
			var tabKeyOutput = "&nbsp;&nbsp;&nbsp;&nbsp;";
		    var spaceKeyOutput = "&nbsp;";
	
			if (_textEditorService.OptionsApi.GetTextEditorOptionsState().Options.ShowWhitespace)
		    {
		        tabKeyOutput = "--->";
		        spaceKeyOutput = "·";
		    }
			
			_textEditorService.__StringBuilder.Clear();
			
			{
				// 1 of the character width is already accounted for
				var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;
				
				const double LINE_WIDTH_TO_TEXT_EDITOR_WIDTH_TO_TRIGGER_HORIZONTAL_VIRTUALIZATION = 1.25;
				
				var minLineWidthToTriggerVirtualizationExclusive = LINE_WIDTH_TO_TEXT_EDITOR_WIDTH_TO_TRIGGER_HORIZONTAL_VIRTUALIZATION *
					viewModel.TextEditorDimensions.Width;
					
				int lineOffset = -1;
				int linesTaken = 0;
				
				while (true)
				{
					lineOffset++;
				
					if (linesTaken >= lineCountToReturn)
						break;
					// TODO: Is this '>' or '>='?
					if (verticalStartingIndex + lineOffset >= modelModifier.LineEndList.Count)
						break;
				
					var lineIndex = verticalStartingIndex + lineOffset;

					if (viewModel.HiddenLineIndexHashSet.Contains(lineIndex))
					{
						hiddenCount++;
						continue;
					}
					
					var lineInformation = modelModifier.GetLineInformation(lineIndex);
								    
					var line_PositionStartInclusiveIndex = lineInformation.Position_StartInclusiveIndex;
					var lineEnd = modelModifier.LineEndList[lineIndex];
					
					// TODO: Was this code using length including line ending or excluding? (2024-12-29)
					var lineLength = lineInformation.Position_EndExclusiveIndex - lineInformation.Position_StartInclusiveIndex;
					
					// Don't bother with the extra width due to tabs until the very end.
					// It is thought to be too costly on average to get the tab count for the line in order to take less text overall
					// than to just take the estimated amount of characters.
					
					var widthInPixels = lineLength * viewModel.CharAndLineMeasurements.CharacterWidth;

					if (widthInPixels > minLineWidthToTriggerVirtualizationExclusive)
					{
						var localHorizontalStartingIndex = horizontalStartingIndex;
						var localHorizontalTake = horizontalTake;
						
						// Tab key adjustments
						var line = modelModifier.GetLineInformation(lineIndex);
						var firstInlineUiOnLineIndex = -1;
						var foundLine = false;
						var tabCharPositionIndexListCount = modelModifier.TabCharPositionIndexList.Count;
				
						// Move the horizontal starting index based on the extra character width from 'tab' characters.
						for (int i = 0; i < tabCharPositionIndexListCount; i++)
						{
							var tabCharPositionIndex = modelModifier.TabCharPositionIndexList[i];
							var tabKeyColumnIndex = tabCharPositionIndex - line.Position_StartInclusiveIndex;
						
							if (!foundLine)
							{
								if (tabCharPositionIndex >= line.Position_StartInclusiveIndex)
								{
									firstInlineUiOnLineIndex = i;
									foundLine = true;
								}
							}
							
							if (foundLine)
							{
								if (tabKeyColumnIndex >= localHorizontalStartingIndex + localHorizontalTake)
									break;
							
								localHorizontalStartingIndex -= extraWidthPerTabKey;
							}
						}
	
						if (localHorizontalStartingIndex + localHorizontalTake > lineLength)
							localHorizontalTake = lineLength - localHorizontalStartingIndex;
	
						localHorizontalStartingIndex = Math.Max(0, localHorizontalStartingIndex);
						localHorizontalTake = Math.Max(0, localHorizontalTake);
						
						var foundSplit = false;
						var unrenderedTabCount = 0;
						var resultTabCount = 0;
						
						// Count the 'tab' characters that preceed the text to display so that the 'left' can be modified by the extra width.
						// Count the 'tab' characters that are among the text to display so that the 'width' can be modified by the extra width.
						if (firstInlineUiOnLineIndex != -1)
						{
							for (int i = firstInlineUiOnLineIndex; i < tabCharPositionIndexListCount; i++)
							{
								var tabCharPositionIndex = modelModifier.TabCharPositionIndexList[i];
								var tabKeyColumnIndex = tabCharPositionIndex - line.Position_StartInclusiveIndex;
								
								if (tabKeyColumnIndex >= localHorizontalStartingIndex + localHorizontalTake)
									break;
							
								if (!foundSplit)
								{
									if (tabKeyColumnIndex < localHorizontalStartingIndex)
										unrenderedTabCount++;
									else
										foundSplit = true;
								}
								
								if (foundSplit)
									resultTabCount++;
							}
						}
						
						widthInPixels = ((localHorizontalTake - localHorizontalStartingIndex) + (extraWidthPerTabKey * resultTabCount)) *
							viewModel.CharAndLineMeasurements.CharacterWidth;
	
						double leftInPixels = localHorizontalStartingIndex *
							viewModel.CharAndLineMeasurements.CharacterWidth;
	
						// Adjust the unrendered for tab key width
						leftInPixels += (extraWidthPerTabKey *
							unrenderedTabCount *
							viewModel.CharAndLineMeasurements.CharacterWidth);
	
						leftInPixels = Math.Max(0, leftInPixels);
	
						var topInPixels = lineIndex * viewModel.CharAndLineMeasurements.LineHeight;
						var positionStartInclusiveIndex = line_PositionStartInclusiveIndex + localHorizontalStartingIndex;
						
						var positionEndExclusiveIndex = positionStartInclusiveIndex + localHorizontalTake;
						if (positionEndExclusiveIndex > lineInformation.UpperLineEnd.Position_StartInclusiveIndex)
							positionEndExclusiveIndex = lineInformation.UpperLineEnd.Position_StartInclusiveIndex;
						
						linesTaken++;
						virtualizedLineList.Add(new VirtualizationLine(
							lineIndex,
							position_StartInclusiveIndex: positionStartInclusiveIndex,
							position_EndExclusiveIndex: positionEndExclusiveIndex,
							virtualizationSpan_StartInclusiveIndex: 0,
							virtualizationSpan_EndExclusiveIndex: 0,
							widthInPixels,
							viewModel.CharAndLineMeasurements.LineHeight,
							viewModel.GutterWidthInPixels + leftInPixels,
							topInPixels - (viewModel.CharAndLineMeasurements.LineHeight * hiddenCount)));
							
						CreateCacheEach(
							modelModifier,
							viewModel,
							componentData,
							virtualizedLineList.Count - 1,
							tabKeyOutput,
							spaceKeyOutput,
							useAll);
					}
					else
					{
						var line = modelModifier.GetLineInformation(lineIndex);
				
						var foundLine = false;
						var resultTabCount = 0;
				
						// Count the tabs that are among the rendered content.
						foreach (var tabCharPositionIndex in modelModifier.TabCharPositionIndexList)
						{
							if (!foundLine)
							{
								if (tabCharPositionIndex >= line.Position_StartInclusiveIndex)
									foundLine = true;
							}
							
							if (foundLine)
							{
								if (tabCharPositionIndex >= line.LastValidColumnIndex)
									break;
							
								resultTabCount++;
							}
						}
						
						widthInPixels += (extraWidthPerTabKey * resultTabCount) *
							viewModel.CharAndLineMeasurements.CharacterWidth;
					
						linesTaken++;
						virtualizedLineList.Add(new VirtualizationLine(
							lineIndex,
							position_StartInclusiveIndex: lineInformation.Position_StartInclusiveIndex,
							position_EndExclusiveIndex: lineInformation.UpperLineEnd.Position_StartInclusiveIndex,
							virtualizationSpan_StartInclusiveIndex: 0,
							virtualizationSpan_EndExclusiveIndex: 0,
							widthInPixels,
							viewModel.CharAndLineMeasurements.LineHeight,
							leftInPixels: viewModel.GutterWidthInPixels,
							topInPixels: (lineIndex * viewModel.CharAndLineMeasurements.LineHeight) - (viewModel.CharAndLineMeasurements.LineHeight * hiddenCount)));
						
						CreateCacheEach(
							modelModifier,
							viewModel,
							componentData,
							virtualizedLineList.Count - 1,
							tabKeyOutput,
							spaceKeyOutput,
							useAll);
					}
				}
			}

			componentData.VirtualizedLineLineIndexWithModificationList.Clear();
		
			componentData.VirtualizedLineCacheViewModelKey = viewModel.PersistentState.ViewModelKey;
			componentData.VirtualizedLineCacheSpanList = viewModel.VirtualizationResult.VirtualizationSpanList;
			componentData.VirtualizedLineCacheCreatedWithScrollLeft = viewModel.ScrollLeft;
			
			for (var i = componentData.VirtualizedLineIndexKeyList.Count - 1; i >= 0; i--)
			{
				if (!componentData.VirtualizedLineCacheUsageHashSet.Contains(componentData.VirtualizedLineIndexKeyList[i]))
				{
					componentData.VirtualizedLineCacheEntryMap.Remove(componentData.VirtualizedLineIndexKeyList[i]);
					componentData.VirtualizedLineIndexKeyList.RemoveAt(i);
				}
			}
			
			// CreateCache(editContext.TextEditorService, modelModifier, viewModel);
			
			#if DEBUG
			LuthetusDebugSomething.SetTextEditorViewModelApi(Stopwatch.GetElapsedTime(startTime));
			#endif
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
			
			if (viewModel is not null)
			{
				if (viewModel.LineIndex >= modelModifier.LineCount)
					viewModel.LineIndex = modelModifier.LineCount - 1;
	
				var lineInformation = modelModifier.GetLineInformation(viewModel.LineIndex);
	
				viewModel.ColumnIndex = lineInformation.LastValidColumnIndex;
			}

#if DEBUG
			// This exception happens a lot while using the editor.
			// It is believed that the published demo WASM app is being slowed down
			// in part from logging this a lot.
			Console.WriteLine(exception);
#endif
		}
    }
    
    private static int CountDigits(int argumentNumber)
    {
    	var digitCount = 1;
    	var runningNumber = argumentNumber;
    	
    	while ((runningNumber /= 10) > 0)
    	{
    		digitCount++;
    	}
    	
    	return digitCount;
    }

    private double GetGutterWidthInPixels(TextEditorModel model, TextEditorViewModel viewModel, TextEditorComponentData componentData)
    {
        if (!componentData.ViewModelDisplayOptions.IncludeGutterComponent)
            return 0;

        var mostDigitsInARowLineNumber = CountDigits(model!.LineCount);

        var gutterWidthInPixels = mostDigitsInARowLineNumber *
            viewModel!.CharAndLineMeasurements.CharacterWidth;

        gutterWidthInPixels += TextEditorModel.GUTTER_PADDING_LEFT_IN_PIXELS + TextEditorModel.GUTTER_PADDING_RIGHT_IN_PIXELS;

        return gutterWidthInPixels;
    }
    
    public void CreateCacheEach(
    	TextEditorModel model,
    	TextEditorViewModel viewModel,
    	TextEditorComponentData componentData,
    	int entryIndex,
    	string tabKeyOutput,
		string spaceKeyOutput,
		bool useAll)
    {
    	var virtualizationEntry = viewModel.VirtualizationResult.EntryList[entryIndex];
		
		if (virtualizationEntry.Position_EndExclusiveIndex - virtualizationEntry.Position_StartInclusiveIndex <= 0)
		{
			// Console.WriteLine("e");
			return;
		}
		
		(int lineIndex, int columnIndex) lineAndColumnIndices = (0, 0);
		var inlineUi = new InlineUi(0, InlineUiKind.None);
		
		for (int inlineUiIndex = 0; inlineUiIndex < viewModel.InlineUiList.Count; inlineUiIndex++)
		{
			var inlineUiTuple = viewModel.InlineUiList[inlineUiIndex];
			
			lineAndColumnIndices = model.GetLineAndColumnIndicesFromPositionIndex(inlineUiTuple.InlineUi.PositionIndex);
			
			if (lineAndColumnIndices.lineIndex == virtualizationEntry.LineIndex)
				inlineUi = inlineUiTuple.InlineUi;
		}
		
		virtualizationEntry.VirtualizationSpan_StartInclusiveIndex = viewModel.VirtualizationResult.VirtualizationSpanList.Count;
		
		componentData.VirtualizedLineCacheUsageHashSet.Add(virtualizationEntry.LineIndex);
		
		var wasCached = false;
		
		if (useAll && inlineUi.InlineUiKind == InlineUiKind.None)
		{
			var useThis = componentData.VirtualizedLineCacheEntryMap.ContainsKey(virtualizationEntry.LineIndex) &&
						  !componentData.VirtualizedLineLineIndexWithModificationList.Contains(virtualizationEntry.LineIndex);
			
			if (useThis)
			{
				var previous = componentData.VirtualizedLineCacheEntryMap[virtualizationEntry.LineIndex];
				
				for (int i = previous.VirtualizationSpan_StartInclusiveIndex; i < previous.VirtualizationSpan_EndExclusiveIndex; i++)
				{
					viewModel.VirtualizationResult.VirtualizationSpanList.Add(componentData.VirtualizedLineCacheSpanList[i]);
				}
				
				// WARNING CODE DUPLICATION (this also exists at the bottom of this for loop).
				virtualizationEntry.VirtualizationSpan_EndExclusiveIndex = viewModel.VirtualizationResult.VirtualizationSpanList.Count;
				viewModel.VirtualizationResult.EntryList[entryIndex] = virtualizationEntry;
				
				componentData.VirtualizedLineCacheEntryMap[virtualizationEntry.LineIndex] = virtualizationEntry;
				
				// Console.WriteLine("r");
				
				wasCached = true;
			}
		}
		
		if (!wasCached)
		{
			var currentDecorationByte = model.RichCharacterList[virtualizationEntry.Position_StartInclusiveIndex].DecorationByte;
		    
		    for (int i = virtualizationEntry.Position_StartInclusiveIndex; i < virtualizationEntry.Position_EndExclusiveIndex; i++)
		    {
		    	if (inlineUi.InlineUiKind != InlineUiKind.None && inlineUi.PositionIndex == i)
		    		_textEditorService.__StringBuilder.Append("&nbsp;&nbsp;&nbsp;");
		    	
		    	var richCharacter = model.RichCharacterList[i];
		    	 
				if (currentDecorationByte == richCharacter.DecorationByte)
			    {
			        // AppendTextEscaped(textEditorService.__StringBuilder, richCharacter, tabKeyOutput, spaceKeyOutput);
			        switch (richCharacter.Value)
			        {
			            case '\t':
			                _textEditorService.__StringBuilder.Append(tabKeyOutput);
			                break;
			            case ' ':
			                _textEditorService.__StringBuilder.Append(spaceKeyOutput);
			                break;
			            case '\r':
			                break;
			            case '\n':
			                break;
			            case '<':
			                _textEditorService.__StringBuilder.Append("&lt;");
			                break;
			            case '>':
			                _textEditorService.__StringBuilder.Append("&gt;");
			                break;
			            case '"':
			                _textEditorService.__StringBuilder.Append("&quot;");
			                break;
			            case '\'':
			                _textEditorService.__StringBuilder.Append("&#39;");
			                break;
			            case '&':
			                _textEditorService.__StringBuilder.Append("&amp;");
			                break;
			            default:
			                _textEditorService.__StringBuilder.Append(richCharacter.Value);
			                break;
			        }
			        // END OF INLINING AppendTextEscaped
			    }
			    else
			    {
			    	viewModel.VirtualizationResult.VirtualizationSpanList.Add(new VirtualizationSpan(
			    		cssClass: model.PersistentState.DecorationMapper.Map(currentDecorationByte),
			    		text: _textEditorService.__StringBuilder.ToString()));
			        _textEditorService.__StringBuilder.Clear();
			        
			        // AppendTextEscaped(textEditorService.__StringBuilder, richCharacter, tabKeyOutput, spaceKeyOutput);
			        switch (richCharacter.Value)
			        {
			            case '\t':
			                _textEditorService.__StringBuilder.Append(tabKeyOutput);
			                break;
			            case ' ':
			                _textEditorService.__StringBuilder.Append(spaceKeyOutput);
			                break;
			            case '\r':
			                break;
			            case '\n':
			                break;
			            case '<':
			                _textEditorService.__StringBuilder.Append("&lt;");
			                break;
			            case '>':
			                _textEditorService.__StringBuilder.Append("&gt;");
			                break;
			            case '"':
			                _textEditorService.__StringBuilder.Append("&quot;");
			                break;
			            case '\'':
			                _textEditorService.__StringBuilder.Append("&#39;");
			                break;
			            case '&':
			                _textEditorService.__StringBuilder.Append("&amp;");
			                break;
			            default:
			                _textEditorService.__StringBuilder.Append(richCharacter.Value);
			                break;
			        }
			        // END OF INLINING AppendTextEscaped
			        
					currentDecorationByte = richCharacter.DecorationByte;
			    }
		    }
		    
		    /* Final grouping of contiguous characters */
			viewModel.VirtualizationResult.VirtualizationSpanList.Add(new VirtualizationSpan(
	    		cssClass: model.PersistentState.DecorationMapper.Map(currentDecorationByte),
	    		text: _textEditorService.__StringBuilder.ToString()));
			_textEditorService.__StringBuilder.Clear();
			
			// WARNING CODE DUPLICATION (this also exists when copying a virtualizationEntry from cache).
			virtualizationEntry.VirtualizationSpan_EndExclusiveIndex = viewModel.VirtualizationResult.VirtualizationSpanList.Count;
			viewModel.VirtualizationResult.EntryList[entryIndex] = virtualizationEntry;
			
			if (componentData.VirtualizedLineCacheEntryMap.ContainsKey(virtualizationEntry.LineIndex))
			{
				componentData.VirtualizedLineCacheEntryMap[virtualizationEntry.LineIndex] = virtualizationEntry;
			}
			else
			{
				componentData.VirtualizedLineIndexKeyList.Add(virtualizationEntry.LineIndex);
				componentData.VirtualizedLineCacheEntryMap.Add(virtualizationEntry.LineIndex, virtualizationEntry);
			}
			
			// Console.WriteLine("c");
	    }
    }

    public async ValueTask RemeasureAsync(
        TextEditorEditContext editContext,
        TextEditorViewModel viewModel)
    {
        var options = _textEditorService.OptionsApi.GetOptions();
        
        var componentData = viewModel.PersistentState.DisplayTracker.ComponentData;
        if (componentData is null)
        	return;
		
		var textEditorMeasurements = await _textEditorService.ViewModelApi
			.GetTextEditorMeasurementsAsync(componentData.RowSectionElementId)
			.ConfigureAwait(false);

		viewModel.CharAndLineMeasurements = options.CharAndLineMeasurements;
		viewModel.TextEditorDimensions = textEditorMeasurements;
    }

    public void ForceRender(
        TextEditorEditContext editContext,
        TextEditorViewModel viewModel)
    {
        // Getting the ViewModel from the 'editContext' triggers a re-render
        //
        // A lot code is being changed and one result is this method now reads like non-sense,
        // (or more non-sense than it previously did)
        // Because we get a viewModel passed in to this method as an argument.
        // So this seems quite silly.
		_ = editContext.GetViewModelModifier(viewModel.PersistentState.ViewModelKey);
    }
    #endregion

    #region DELETE_METHODS
    public void Dispose(TextEditorEditContext editContext, Key<TextEditorViewModel> viewModelKey)
    {
        _textEditorService.DisposeViewModel(editContext, viewModelKey);
    }
    #endregion
}