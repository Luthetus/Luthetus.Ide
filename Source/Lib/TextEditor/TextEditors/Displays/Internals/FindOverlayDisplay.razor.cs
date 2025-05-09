using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.JsRuntimes.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.TextEditor.RazorLib.Edits.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class FindOverlayDisplay : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
	[Inject]
    private ICommonComponentRenderers CommonComponentRenderers { get; set; } = null!;
    [Inject]
    private CommonBackgroundTaskApi CommonBackgroundTaskApi { get; set; } = null!;
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;

    [Parameter, EditorRequired]
    public TextEditorRenderBatch RenderBatch { get; set; }

    private bool _lastSeenShowFindOverlayValue = false;
    private bool _lastFindOverlayValueExternallyChangedMarker = false;
    private string _inputValue = string.Empty;
    private string _inputReplace = string.Empty;
    private int? _activeIndexMatchedTextSpan = null;

    private Throttle _throttleInputValueChange = new Throttle(TimeSpan.FromMilliseconds(150));
    private TextEditorTextSpan? _decorationByteChangedTargetTextSpan;

    private string InputValue
    {
        get => _inputValue;
        set
        {
	    	var renderBatchLocal = RenderBatch;
	    	if (!renderBatchLocal.ConstructorWasInvoked)
	    		return;
        
            _inputValue = value;
            
            _throttleInputValueChange.Run(_ =>
            {
            	TextEditorService.WorkerArbitrary.PostUnique(
                    nameof(FindOverlayDisplay),
                    editContext =>
                    {
                        var viewModelModifier = editContext.GetViewModelModifier(renderBatchLocal.ViewModel.PersistentState.ViewModelKey);

                        if (viewModelModifier is null)
                            return ValueTask.CompletedTask;

                        var localInputValue = _inputValue;

                        viewModelModifier.PersistentState.FindOverlayValue = localInputValue;

                        var modelModifier = editContext.GetModelModifier(renderBatchLocal.Model.ResourceUri);

                        if (modelModifier is null)
                            return ValueTask.CompletedTask;

                        List<TextEditorTextSpan> textSpanMatches;

                        if (!string.IsNullOrWhiteSpace(localInputValue))
                            textSpanMatches = modelModifier.FindMatches(localInputValue);
                        else
                            textSpanMatches = new();

						TextEditorService.ModelApi.StartPendingCalculatePresentationModel(
                        	editContext,
                            modelModifier,
                            FindOverlayPresentationFacts.PresentationKey,
                            FindOverlayPresentationFacts.EmptyPresentationModel);

                        var presentationModel = modelModifier.PresentationModelList.First(
                            x => x.TextEditorPresentationKey == FindOverlayPresentationFacts.PresentationKey);

                        if (presentationModel.PendingCalculation is null)
                            throw new LuthetusTextEditorException($"{nameof(presentationModel)}.{nameof(presentationModel.PendingCalculation)} was not expected to be null here.");

                        modelModifier.CompletePendingCalculatePresentationModel(
                            FindOverlayPresentationFacts.PresentationKey,
                            FindOverlayPresentationFacts.EmptyPresentationModel,
                            textSpanMatches);

                        _activeIndexMatchedTextSpan = null;
                        _decorationByteChangedTargetTextSpan = null;
                        return ValueTask.CompletedTask;
                    });
				return Task.CompletedTask;
            });
        }
    }
    
    private string InputReplace
    {
        get => _inputReplace;
        set
        {
	    	var renderBatchLocal = RenderBatch;
	    	if (!renderBatchLocal.ConstructorWasInvoked)
	    		return;
        
            _inputReplace = value;
            
        	TextEditorService.WorkerArbitrary.PostUnique(
                nameof(FindOverlayDisplay),
                editContext =>
                {
                    var viewModelModifier = editContext.GetViewModelModifier(renderBatchLocal.ViewModel.PersistentState.ViewModelKey);

                    if (viewModelModifier is null)
                        return ValueTask.CompletedTask;

                    viewModelModifier.PersistentState.ReplaceValueInFindOverlay = value;
                    return ValueTask.CompletedTask;
                });
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
    	var renderBatchLocal = RenderBatch;
    	if (!renderBatchLocal.ConstructorWasInvoked)
    		return;
    		
    	var becameShown = false;
    		
        if (_lastSeenShowFindOverlayValue != renderBatchLocal.ViewModel.PersistentState.ShowFindOverlay)
        {
            _lastSeenShowFindOverlayValue = renderBatchLocal.ViewModel.PersistentState.ShowFindOverlay;

            // If it changes from 'false' to 'true', focus the input element
            if (_lastSeenShowFindOverlayValue)
            {
            	becameShown = true;
            	
                await CommonBackgroundTaskApi.JsRuntimeCommonApi
                    .FocusHtmlElementById(renderBatchLocal.ViewModel.PersistentState.FindOverlayId)
                    .ConfigureAwait(false);
            }
        }
        
        if (becameShown ||
        	_lastFindOverlayValueExternallyChangedMarker != renderBatchLocal.ViewModel.PersistentState.FindOverlayValueExternallyChangedMarker)
        {
        	_lastFindOverlayValueExternallyChangedMarker = renderBatchLocal.ViewModel.PersistentState.FindOverlayValueExternallyChangedMarker;
        	InputValue = renderBatchLocal.ViewModel.PersistentState.FindOverlayValue;
        	InputReplace = renderBatchLocal.ViewModel.PersistentState.ReplaceValueInFindOverlay;
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task HandleOnKeyDownAsync(KeyboardEventArgs keyboardEventArgs)
    {
    	var renderBatchLocal = RenderBatch;
    	if (!renderBatchLocal.ConstructorWasInvoked)
    		return;
    	
        if (keyboardEventArgs.Key == KeyboardKeyFacts.MetaKeys.ESCAPE)
        {
            await CommonBackgroundTaskApi.JsRuntimeCommonApi
                .FocusHtmlElementById(renderBatchLocal.ViewModel.PersistentState.PrimaryCursorContentId)
                .ConfigureAwait(false);

            TextEditorService.WorkerArbitrary.PostRedundant(
                nameof(FindOverlayDisplay),
				renderBatchLocal.ViewModel.PersistentState.ResourceUri,
                renderBatchLocal.ViewModel.PersistentState.ViewModelKey,
                editContext =>
                {
                    var viewModelModifier = editContext.GetViewModelModifier(renderBatchLocal.ViewModel.PersistentState.ViewModelKey);

                    if (viewModelModifier is null)
                        return ValueTask.CompletedTask;

                    viewModelModifier.PersistentState.ShowFindOverlay = false;

                    var modelModifier = editContext.GetModelModifier(renderBatchLocal.Model.ResourceUri);

                    if (modelModifier is null)
                        return ValueTask.CompletedTask;

                    TextEditorService.ModelApi.StartPendingCalculatePresentationModel(
                		editContext,
                        modelModifier,
                        FindOverlayPresentationFacts.PresentationKey,
                        FindOverlayPresentationFacts.EmptyPresentationModel);

                    var presentationModel = modelModifier.PresentationModelList.First(
                        x => x.TextEditorPresentationKey == FindOverlayPresentationFacts.PresentationKey);

                    if (presentationModel.PendingCalculation is null)
                        throw new LuthetusTextEditorException($"{nameof(presentationModel)}.{nameof(presentationModel.PendingCalculation)} was not expected to be null here.");

                    modelModifier.CompletePendingCalculatePresentationModel(
                        FindOverlayPresentationFacts.PresentationKey,
                        FindOverlayPresentationFacts.EmptyPresentationModel,
                        new());
                    return ValueTask.CompletedTask;
                });
        }
    }

    private async Task MoveActiveIndexMatchedTextSpanUp()
    {
    	var renderBatchLocal = RenderBatch;
    	if (!renderBatchLocal.ConstructorWasInvoked)
    		return;
    	
        var findOverlayPresentationModel = renderBatchLocal.Model.PresentationModelList.FirstOrDefault(
            x => x.TextEditorPresentationKey == FindOverlayPresentationFacts.PresentationKey);

        if (findOverlayPresentationModel is null)
            return;

        var completedCalculation = findOverlayPresentationModel.CompletedCalculation;

        if (completedCalculation is null)
            return;

        if (_activeIndexMatchedTextSpan is null)
        {
            _activeIndexMatchedTextSpan = completedCalculation.TextSpanList.Count - 1;
        }
        else
        {
			if (completedCalculation.TextSpanList.Count == 0)
			{
				_activeIndexMatchedTextSpan = null;
			}
			else
			{
				_activeIndexMatchedTextSpan--;
	            if (_activeIndexMatchedTextSpan <= -1)
					_activeIndexMatchedTextSpan = completedCalculation.TextSpanList.Count - 1;
			}
        }

        await HandleActiveIndexMatchedTextSpanChanged();
    }

    private async Task MoveActiveIndexMatchedTextSpanDown()
    {
    	var renderBatchLocal = RenderBatch;
    	if (!renderBatchLocal.ConstructorWasInvoked)
    		return;
    	
        var findOverlayPresentationModel = renderBatchLocal.Model.PresentationModelList.FirstOrDefault(
            x => x.TextEditorPresentationKey == FindOverlayPresentationFacts.PresentationKey);

        if (findOverlayPresentationModel is null)
            return;

        var completedCalculation = findOverlayPresentationModel.CompletedCalculation;

        if (completedCalculation is null)
            return;

        if (_activeIndexMatchedTextSpan is null)
        {
            _activeIndexMatchedTextSpan = 0;
        }
        else
        {
			if (completedCalculation.TextSpanList.Count == 0)
			{
				_activeIndexMatchedTextSpan = null;
			}
			else
			{
            	_activeIndexMatchedTextSpan++;
				if (_activeIndexMatchedTextSpan >= completedCalculation.TextSpanList.Count)
					_activeIndexMatchedTextSpan = 0;
			}
        }

        await HandleActiveIndexMatchedTextSpanChanged();
    }

    private Task HandleActiveIndexMatchedTextSpanChanged()
    {
    	var renderBatchLocal = RenderBatch;
    	if (!renderBatchLocal.ConstructorWasInvoked)
    		return Task.CompletedTask;
    	
        TextEditorService.WorkerArbitrary.PostUnique(
            nameof(HandleActiveIndexMatchedTextSpanChanged),
            editContext =>
            {
                var localActiveIndexMatchedTextSpan = _activeIndexMatchedTextSpan;

                if (localActiveIndexMatchedTextSpan is null)
                    return ValueTask.CompletedTask;

                var viewModelModifier = editContext.GetViewModelModifier(renderBatchLocal.ViewModel.PersistentState.ViewModelKey);

                if (viewModelModifier is null)
                    return ValueTask.CompletedTask;
                
                var modelModifier = editContext.GetModelModifier(renderBatchLocal.Model.ResourceUri);

                if (modelModifier is null)
                    return ValueTask.CompletedTask;

                var presentationModel = modelModifier.PresentationModelList.FirstOrDefault(x =>
                    x.TextEditorPresentationKey == FindOverlayPresentationFacts.PresentationKey);

                if (presentationModel?.CompletedCalculation is not null)
                {
                	var outTextSpanList = new List<TextEditorTextSpan>(presentationModel.CompletedCalculation.TextSpanList);
                
                	var decorationByteChangedTargetTextSpanLocal = _decorationByteChangedTargetTextSpan;
                    if (decorationByteChangedTargetTextSpanLocal is not null)
                    {
                    	TextEditorTextSpan needsColorResetSinceNoLongerActive = default;
                    	int indexNeedsColorResetSinceNoLongerActive = -1;
                    	
                    	for (int i = 0; i < presentationModel.CompletedCalculation.TextSpanList.Count; i++)
                    	{
                    		var x = presentationModel.CompletedCalculation.TextSpanList[i];
                    		
                    		if (x.StartInclusiveIndex == decorationByteChangedTargetTextSpanLocal.Value.StartInclusiveIndex &&
	                            x.EndExclusiveIndex == decorationByteChangedTargetTextSpanLocal.Value.EndExclusiveIndex &&
	                            x.ResourceUri == decorationByteChangedTargetTextSpanLocal.Value.ResourceUri &&
	                            x.GetText() == decorationByteChangedTargetTextSpanLocal.Value.GetText())
                    		{
                    			needsColorResetSinceNoLongerActive = x;
                    			indexNeedsColorResetSinceNoLongerActive = i;
                    		}
                    	}
                    
                        if (needsColorResetSinceNoLongerActive != default && indexNeedsColorResetSinceNoLongerActive != -1)
                        {
                        	outTextSpanList[indexNeedsColorResetSinceNoLongerActive] = needsColorResetSinceNoLongerActive with
                            {
                                DecorationByte = decorationByteChangedTargetTextSpanLocal.Value.DecorationByte
                            };
                        }
                    }

                    var targetTextSpan = presentationModel.CompletedCalculation.TextSpanList[localActiveIndexMatchedTextSpan.Value];
                    _decorationByteChangedTargetTextSpan = targetTextSpan;

                    outTextSpanList[localActiveIndexMatchedTextSpan.Value] = targetTextSpan with
                    {
                        DecorationByte = (byte)FindOverlayDecorationKind.Insertion,
                    };
                        
                    presentationModel.CompletedCalculation.TextSpanList = outTextSpanList;
                }

				{
					var decorationByteChangedTargetTextSpanLocal = _decorationByteChangedTargetTextSpan;
					
					if (decorationByteChangedTargetTextSpanLocal is not null)
					{
						TextEditorService.ViewModelApi.ScrollIntoView(
							editContext,
							modelModifier,						
							viewModelModifier,
							decorationByteChangedTargetTextSpanLocal.Value);
					}
				}
				
                return ValueTask.CompletedTask;
            });
		return Task.CompletedTask;
    }
    
    private void ToggleShowReplace()
    {
    	var renderBatchLocal = RenderBatch;
    	if (!renderBatchLocal.ConstructorWasInvoked)
    		return;
    	
    	TextEditorService.WorkerArbitrary.PostRedundant(
            nameof(FindOverlayDisplay),
			renderBatchLocal.ViewModel.PersistentState.ResourceUri,
            renderBatchLocal.ViewModel.PersistentState.ViewModelKey,
            (TextEditorEditContext editContext) =>
            {
                var viewModelModifier = editContext.GetViewModelModifier(renderBatchLocal.ViewModel.PersistentState.ViewModelKey);

                if (viewModelModifier is null)
                    return ValueTask.CompletedTask;

                viewModelModifier.PersistentState.ShowReplaceButtonInFindOverlay = !viewModelModifier.PersistentState.ShowReplaceButtonInFindOverlay;

                return ValueTask.CompletedTask;
            });
    }
    
    private void ReplaceCurrent()
    {
    	var renderBatchLocal = RenderBatch;
    	if (!renderBatchLocal.ConstructorWasInvoked)
    		return;
    
    	TextEditorService.WorkerArbitrary.PostRedundant(
            nameof(FindOverlayDisplay),
			renderBatchLocal.ViewModel.PersistentState.ResourceUri,
            renderBatchLocal.ViewModel.PersistentState.ViewModelKey,
            (TextEditorEditContext editContext) =>
            {
            	var modelModifier = editContext.GetModelModifier(renderBatchLocal.Model.ResourceUri);
                var viewModelModifier = editContext.GetViewModelModifier(renderBatchLocal.ViewModel.PersistentState.ViewModelKey);
                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier);
                var localActiveIndexMatchedTextSpan = _activeIndexMatchedTextSpan;

                if (modelModifier is null || viewModelModifier is null || localActiveIndexMatchedTextSpan is null)
                    return ValueTask.CompletedTask;

                var presentationModel = modelModifier.PresentationModelList.FirstOrDefault(x =>
                    x.TextEditorPresentationKey == FindOverlayPresentationFacts.PresentationKey);

                if (presentationModel?.CompletedCalculation is null)
                	return ValueTask.CompletedTask;

                var targetTextSpan = presentationModel.CompletedCalculation.TextSpanList[localActiveIndexMatchedTextSpan.Value];
                
                var (lineIndex, columnIndex) = modelModifier.GetLineAndColumnIndicesFromPositionIndex(
                	targetTextSpan.StartInclusiveIndex);
                	
                cursorModifierBag.CursorModifier.LineIndex = lineIndex;
                cursorModifierBag.CursorModifier.SetColumnIndexAndPreferred(columnIndex);
                cursorModifierBag.CursorModifier.SelectionAnchorPositionIndex = -1;
                
                modelModifier.Delete(
			        cursorModifierBag,
			        columnCount: targetTextSpan.Length,
			        expandWord: false,
			        TextEditorModel.DeleteKind.Delete);
			        
			    modelModifier.Insert(
			        viewModelModifier.PersistentState.ReplaceValueInFindOverlay,
			        cursorModifierBag);

                return ValueTask.CompletedTask;
            });
    }
    
    private void ReplaceAll()
    {
    	var renderBatchLocal = RenderBatch;
    	if (!renderBatchLocal.ConstructorWasInvoked)
    		return;
    
    	TextEditorService.WorkerArbitrary.PostRedundant(
            nameof(FindOverlayDisplay),
			renderBatchLocal.ViewModel.PersistentState.ResourceUri,
            renderBatchLocal.ViewModel.PersistentState.ViewModelKey,
            (TextEditorEditContext editContext) =>
            {
            	var modelModifier = editContext.GetModelModifier(renderBatchLocal.Model.ResourceUri);
                var viewModelModifier = editContext.GetViewModelModifier(renderBatchLocal.ViewModel.PersistentState.ViewModelKey);
                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier);
                var localActiveIndexMatchedTextSpan = _activeIndexMatchedTextSpan;

                if (modelModifier is null || viewModelModifier is null || localActiveIndexMatchedTextSpan is null)
                    return ValueTask.CompletedTask;

                var presentationModel = modelModifier.PresentationModelList.FirstOrDefault(x =>
                    x.TextEditorPresentationKey == FindOverlayPresentationFacts.PresentationKey);

                if (presentationModel?.CompletedCalculation is null)
                	return ValueTask.CompletedTask;
                
                modelModifier.EnsureUndoPoint(new TextEditorEdit(
                	TextEditorEditKind.OtherOpen,
                	"ReplaceAll",
                	modelModifier.GetPositionIndex(cursorModifierBag.CursorModifier),
                	cursorModifierBag.CursorModifier.ToCursor(),
                	cursorModifierBag.CursorModifier.ToCursor(),
                	editedTextBuilder: null));
                
                for (int i = presentationModel.CompletedCalculation.TextSpanList.Count - 1; i >= 0; i--)
                {
                	var targetTextSpan = presentationModel.CompletedCalculation.TextSpanList[i];
                	
	                var (lineIndex, columnIndex) = modelModifier.GetLineAndColumnIndicesFromPositionIndex(
	                	targetTextSpan.StartInclusiveIndex);
	                	
	                cursorModifierBag.CursorModifier.LineIndex = lineIndex;
	                cursorModifierBag.CursorModifier.SetColumnIndexAndPreferred(columnIndex);
	                cursorModifierBag.CursorModifier.SelectionAnchorPositionIndex = -1;
	                
	                modelModifier.Delete(
				        cursorModifierBag,
				        columnCount: targetTextSpan.Length,
				        expandWord: false,
				        TextEditorModel.DeleteKind.Delete);
				        
				    modelModifier.Insert(
				        viewModelModifier.PersistentState.ReplaceValueInFindOverlay,
				        cursorModifierBag);
                }
                
                modelModifier.EnsureUndoPoint(new TextEditorEdit(
                	TextEditorEditKind.OtherClose,
                	"ReplaceAll",
                	modelModifier.GetPositionIndex(cursorModifierBag.CursorModifier),
                	cursorModifierBag.CursorModifier.ToCursor(),
                	cursorModifierBag.CursorModifier.ToCursor(),
                	editedTextBuilder: null));

                return ValueTask.CompletedTask;
            });
    }
}