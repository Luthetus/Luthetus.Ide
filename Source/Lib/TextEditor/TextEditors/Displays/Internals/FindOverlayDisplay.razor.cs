using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Fluxor;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.JsRuntimes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Exceptions;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class FindOverlayDisplay : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
	[Inject]
    private ICommonComponentRenderers CommonComponentRenderers { get; set; } = null!;
	[Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    [CascadingParameter]
    public TextEditorRenderBatchValidated? RenderBatch { get; set; }

    private bool _lastSeenShowFindOverlayValue = false;
    private string _inputValue = string.Empty;
    private int? _activeIndexMatchedTextSpan = null;

    private Throttle _throttleInputValueChange = new Throttle(TimeSpan.FromMilliseconds(150));
    private TextEditorTextSpan? _decorationByteChangedTargetTextSpan;

    private string InputValue
    {
        get => _inputValue;
        set
        {
	    	var renderBatchLocal = RenderBatch;
	    	if (renderBatchLocal is null)
	    		return;
        
            _inputValue = value;
            
            _throttleInputValueChange.Run(_ =>
            {
            	TextEditorService.PostUnique(
                    nameof(FindOverlayDisplay),
                    editContext =>
                    {
                        var viewModelModifier = editContext.GetViewModelModifier(renderBatchLocal.ViewModel.ViewModelKey);

                        if (viewModelModifier is null)
                            return Task.CompletedTask;

                        var localInputValue = _inputValue;

                        viewModelModifier.ViewModel = viewModelModifier.ViewModel with
                        {
                            FindOverlayValue = localInputValue,
                        };

                        var modelModifier = editContext.GetModelModifier(renderBatchLocal.Model.ResourceUri);

                        if (modelModifier is null)
                            return Task.CompletedTask;

                        ImmutableArray<TextEditorTextSpan> textSpanMatches = ImmutableArray<TextEditorTextSpan>.Empty;

                        if (!string.IsNullOrWhiteSpace(localInputValue))
                            textSpanMatches = modelModifier.FindMatches(localInputValue);

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
                        return Task.CompletedTask;
                    });
				return Task.CompletedTask;
            });
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
    	var renderBatchLocal = RenderBatch;
    	if (renderBatchLocal is null)
    		return;
    		
        if (_lastSeenShowFindOverlayValue != renderBatchLocal.ViewModel.ShowFindOverlay)
        {
            _lastSeenShowFindOverlayValue = renderBatchLocal.ViewModel.ShowFindOverlay;

            // If it changes from 'false' to 'true', focus the input element
            if (_lastSeenShowFindOverlayValue)
            {
                await JsRuntime.GetLuthetusCommonApi()
                    .FocusHtmlElementById(renderBatchLocal.ViewModel.FindOverlayId)
                    .ConfigureAwait(false);
            }
        }

        await base.OnAfterRenderAsync(firstRender);
    }

	private void HandleOnFocus()
	{
    	var renderBatchLocal = RenderBatch;
    	if (renderBatchLocal is null)
    		return;
    	
		// In the case where the find over value was changed, by an outside event,
		// just refresh the InputValue to be sure its up to date.
		//
		// Example: user has a selection when using the keybind to open the find overlay,
		// 		 then the find overlay would be populated with their text selection.
		InputValue = renderBatchLocal.ViewModel.FindOverlayValue;
	}

    private async Task HandleOnKeyDownAsync(KeyboardEventArgs keyboardEventArgs)
    {
    	var renderBatchLocal = RenderBatch;
    	if (renderBatchLocal is null)
    		return;
    	
        if (keyboardEventArgs.Key == KeyboardKeyFacts.MetaKeys.ESCAPE)
        {
            await JsRuntime.GetLuthetusCommonApi()
                .FocusHtmlElementById(renderBatchLocal.ViewModel.PrimaryCursorContentId)
                .ConfigureAwait(false);

            TextEditorService.PostRedundant(
                nameof(FindOverlayDisplay),
				renderBatchLocal.ViewModel.ResourceUri,
                renderBatchLocal.ViewModel.ViewModelKey,
                editContext =>
                {
                    var viewModelModifier = editContext.GetViewModelModifier(renderBatchLocal.ViewModel.ViewModelKey);

                    if (viewModelModifier is null)
                        return Task.CompletedTask;

                    viewModelModifier.ViewModel = viewModelModifier.ViewModel with
                    {
                        ShowFindOverlay = false,
                    };

                    var modelModifier = editContext.GetModelModifier(renderBatchLocal.Model.ResourceUri);

                    if (modelModifier is null)
                        return Task.CompletedTask;

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
                        ImmutableArray<TextEditorTextSpan>.Empty);
                    return Task.CompletedTask;
                });
        }
    }

    private async Task MoveActiveIndexMatchedTextSpanUp()
    {
    	var renderBatchLocal = RenderBatch;
    	if (renderBatchLocal is null)
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
            _activeIndexMatchedTextSpan = completedCalculation.TextSpanList.Length - 1;
        }
        else
        {
			if (completedCalculation.TextSpanList.Length == 0)
			{
				_activeIndexMatchedTextSpan = null;
			}
			else
			{
				_activeIndexMatchedTextSpan--;
	            if (_activeIndexMatchedTextSpan <= -1)
					_activeIndexMatchedTextSpan = completedCalculation.TextSpanList.Length - 1;
			}
        }

        await HandleActiveIndexMatchedTextSpanChanged();
    }

    private async Task MoveActiveIndexMatchedTextSpanDown()
    {
    	var renderBatchLocal = RenderBatch;
    	if (renderBatchLocal is null)
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
			if (completedCalculation.TextSpanList.Length == 0)
			{
				_activeIndexMatchedTextSpan = null;
			}
			else
			{
            	_activeIndexMatchedTextSpan++;
				if (_activeIndexMatchedTextSpan >= completedCalculation.TextSpanList.Length)
					_activeIndexMatchedTextSpan = 0;
			}
        }

        await HandleActiveIndexMatchedTextSpanChanged();
    }

    private Task HandleActiveIndexMatchedTextSpanChanged()
    {
    	var renderBatchLocal = RenderBatch;
    	if (renderBatchLocal is null)
    		return;
    	
        TextEditorService.PostUnique(
            nameof(HandleActiveIndexMatchedTextSpanChanged),
            editContext =>
            {
                var localActiveIndexMatchedTextSpan = _activeIndexMatchedTextSpan;

                if (localActiveIndexMatchedTextSpan is null)
                    return Task.CompletedTask;

                var viewModelModifier = editContext.GetViewModelModifier(renderBatchLocal.ViewModel.ViewModelKey);

                if (viewModelModifier is null)
                    return Task.CompletedTask;
                
                var modelModifier = editContext.GetModelModifier(renderBatchLocal.Model.ResourceUri);

                if (modelModifier is null)
                    return Task.CompletedTask;

                var presentationModel = modelModifier.PresentationModelList.FirstOrDefault(x =>
                    x.TextEditorPresentationKey == FindOverlayPresentationFacts.PresentationKey);

                if (presentationModel?.CompletedCalculation is not null)
                {
                    if (_decorationByteChangedTargetTextSpan is not null)
                    {
                        var needsColorResetSinceNoLongerActive = presentationModel.CompletedCalculation.TextSpanList.FirstOrDefault(x =>
                            x.StartingIndexInclusive == _decorationByteChangedTargetTextSpan.StartingIndexInclusive &&
                            x.EndingIndexExclusive == _decorationByteChangedTargetTextSpan.EndingIndexExclusive &&
                            x.ResourceUri == _decorationByteChangedTargetTextSpan.ResourceUri &&
                            x.GetText() == _decorationByteChangedTargetTextSpan.GetText());

                        if (needsColorResetSinceNoLongerActive is not null)
                        {
                            presentationModel.CompletedCalculation.TextSpanList = presentationModel.CompletedCalculation.TextSpanList.Replace(needsColorResetSinceNoLongerActive, needsColorResetSinceNoLongerActive with
                            {
                                DecorationByte = _decorationByteChangedTargetTextSpan.DecorationByte
                            });
                        }
                    }

                    var targetTextSpan = presentationModel.CompletedCalculation.TextSpanList[localActiveIndexMatchedTextSpan.Value];
                    _decorationByteChangedTargetTextSpan = targetTextSpan;

                    presentationModel.CompletedCalculation.TextSpanList =
                        presentationModel.CompletedCalculation.TextSpanList.Replace(targetTextSpan, targetTextSpan with
                        {
                            DecorationByte = (byte)FindOverlayDecorationKind.Insertion,
                        });
                }

				TextEditorService.ViewModelApi.ScrollIntoView(
					editContext,
					modelModifier,						
					viewModelModifier,
					_decorationByteChangedTargetTextSpan);
                return Task.CompletedTask;
            });
		return Task.CompletedTask;
    }
}