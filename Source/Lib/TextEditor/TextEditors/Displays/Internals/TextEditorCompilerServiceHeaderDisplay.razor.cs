using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;
using Fluxor;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Options.States;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

// Project ---- CurrentCodeBlockOwner --- Definitions inside the code block

public partial class TextEditorCompilerServiceHeaderDisplay : ComponentBase, ITextEditorDependentComponent
{
	[Inject]
	public ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private IState<AppOptionsState> AppOptionsStateWrap { get; set; } = null!;

	[Parameter, EditorRequired]
	public TextEditorViewModelDisplay TextEditorViewModelDisplay { get; set; } = null!;
	
	private static readonly TimeSpan ThrottleTimeSpan = TimeSpan.FromMilliseconds(500);

	/// <summary>byte is used as TArgs just as a "throwaway" type. It isn't used.</summary>
	private Debounce<byte> _debounceRender;
	
	private ResourceUri _resourceUriPrevious = ResourceUri.Empty;
	
	private int _lineIndexPrevious = -1;
	private int _columnIndexPrevious = -1;
	
	private IBinderSession? _binderSessionPrevious = null;
	private ICodeBlockOwner? _codeBlockOwner;
	private bool _shouldRender = false;
	
	private bool _showDefaultToolbar;
	
	private CancellationTokenSource _cancellationTokenSource = new();
	
	protected override void OnInitialized()
    {
    	_debounceRender = new(ThrottleTimeSpan, _cancellationTokenSource.Token, async (_, _) =>
    	{
    		UpdateUi();
    	});
    
        TextEditorViewModelDisplay.RenderBatchChanged += OnRenderBatchChanged;
        OnRenderBatchChanged();
        
        base.OnInitialized();
    }

	private async void OnRenderBatchChanged()
    {
    	var renderBatch = TextEditorViewModelDisplay._storedRenderBatchTuple.Validated;
    	
    	if (renderBatch is null)
    		return;
    		
    	if (_shouldRender)
    	{
    		_shouldRender = false;
    		await InvokeAsync(StateHasChanged);
    	}
    
    	if (renderBatch.ViewModel.PrimaryCursor.LineIndex == _lineIndexPrevious &&
        	renderBatch.ViewModel.PrimaryCursor.ColumnIndex == _columnIndexPrevious)
        {
			return;
        }
        
    	_debounceRender.Run(0);
    }
    
    private void ToggleDefaultToolbar()
    {
    	_showDefaultToolbar = !_showDefaultToolbar;
    }
    
    private void UpdateUi()
    {
    	var renderBatch = TextEditorViewModelDisplay._storedRenderBatchTuple.Validated;
    	if (renderBatch is null)
    		return;
    	
    	TextEditorService.PostUnique(nameof(TextEditorCompilerServiceHeaderDisplay), editContext =>
    	{
    		var modelModifier = editContext.GetModelModifier(renderBatch.Model.ResourceUri);
            var viewModelModifier = editContext.GetViewModelModifier(renderBatch.ViewModel.ViewModelKey);
            var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;
            
            _lineIndexPrevious = primaryCursorModifier.LineIndex;
            _columnIndexPrevious = primaryCursorModifier.ColumnIndex;
            
            if (!viewModelModifier.ViewModel.FirstPresentationLayerKeysList.Contains(
            		TextEditorDevToolsPresentationFacts.PresentationKey))
            {
	            viewModelModifier.ViewModel = viewModelModifier.ViewModel with
	            {
	            	FirstPresentationLayerKeysList = viewModelModifier.ViewModel.FirstPresentationLayerKeysList
	            		.Add(TextEditorDevToolsPresentationFacts.PresentationKey)
	            };
	        }
    	
    		TextEditorService.ModelApi.StartPendingCalculatePresentationModel(
				editContext,
		        modelModifier,
		        TextEditorDevToolsPresentationFacts.PresentationKey,
				TextEditorDevToolsPresentationFacts.EmptyPresentationModel);
	
			var presentationModel = modelModifier.PresentationModelList.First(
				x => x.TextEditorPresentationKey == TextEditorDevToolsPresentationFacts.PresentationKey);
	
			if (presentationModel.PendingCalculation is null)
				throw new LuthetusTextEditorException($"{nameof(presentationModel)}.{nameof(presentationModel.PendingCalculation)} was not expected to be null here.");
	
	        var resourceUri = modelModifier.ResourceUri;
	
			var targetScope = modelModifier.CompilerService.Binder.
				GetScopeByPositionIndex(resourceUri, modelModifier.GetPositionIndex(primaryCursorModifier));
			
			if (targetScope is null)
				return Task.CompletedTask;
    
    		var textSpanStart = new TextEditorTextSpan(
	            targetScope.StartingIndexInclusive,
	            targetScope.StartingIndexInclusive + 1,
			    (byte)TextEditorDevToolsDecorationKind.Scope,
			    resourceUri,
			    sourceText: string.Empty,
			    getTextPrecalculatedResult: string.Empty);
    		
			var textSpanEnd = new TextEditorTextSpan(
	            (targetScope.EndingIndexExclusive ?? presentationModel.PendingCalculation.ContentAtRequest.Length) - 1,
			    targetScope.EndingIndexExclusive ?? presentationModel.PendingCalculation.ContentAtRequest.Length,
			    (byte)TextEditorDevToolsDecorationKind.Scope,
			    resourceUri,
			    sourceText: string.Empty,
			    getTextPrecalculatedResult: string.Empty);
	
			var diagnosticTextSpans = new [] { textSpanStart, textSpanEnd }
				.ToImmutableArray();

			modelModifier.CompletePendingCalculatePresentationModel(
				TextEditorDevToolsPresentationFacts.PresentationKey,
				TextEditorDevToolsPresentationFacts.EmptyPresentationModel,
				diagnosticTextSpans);
				
			if (_codeBlockOwner != targetScope.CodeBlockOwner)
			{
				_codeBlockOwner = targetScope.CodeBlockOwner;
				_shouldRender = true;
			}
	
    		return Task.CompletedTask;
    	});
    }

	public void Dispose()
    {
    	TextEditorViewModelDisplay.RenderBatchChanged -= OnRenderBatchChanged;
    	
    	_cancellationTokenSource.Cancel();
    	_cancellationTokenSource.Dispose();
    }
}