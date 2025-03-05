using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

// Project ---- CurrentCodeBlockOwner --- Definitions inside the code block

public partial class TextEditorCompilerServiceHeaderDisplay : ComponentBase, ITextEditorDependentComponent
{
	[Inject]
	public ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;

	[Parameter, EditorRequired]
	public TextEditorViewModelDisplay TextEditorViewModelDisplay { get; set; } = null!;
	
	private static readonly TimeSpan ThrottleTimeSpan = TimeSpan.FromMilliseconds(500);

	/// <summary>byte is used as TArgs just as a "throwaway" type. It isn't used.</summary>
	private Debounce<byte> _debounceRender;
	
	private ResourceUri _resourceUriPrevious = ResourceUri.Empty;
	
	private int _lineIndexPrevious = -1;
	private int _columnIndexPrevious = -1;
	
	private ICodeBlockOwner? _codeBlockOwner;
	private bool _shouldRender = false;
	
	private bool _showDefaultToolbar = false;
	
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
    	var renderBatch = TextEditorViewModelDisplay._activeRenderBatch;
    	
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
    	var renderBatch = TextEditorViewModelDisplay._activeRenderBatch;
    	if (renderBatch is null)
    		return;
    	
    	TextEditorService.TextEditorWorker.PostUnique(nameof(TextEditorCompilerServiceHeaderDisplay), editContext =>
    	{
    		try
    		{
	    		var modelModifier = editContext.GetModelModifier(renderBatch.Model.ResourceUri);
	            var viewModelModifier = editContext.GetViewModelModifier(renderBatch.ViewModel.ViewModelKey);
	            var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
	            var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);
	
	            if (modelModifier is null || viewModelModifier is null || !cursorModifierBag.ConstructorWasInvoked || primaryCursorModifier is null)
	                return ValueTask.CompletedTask;
	            
	            _lineIndexPrevious = primaryCursorModifier.LineIndex;
	            _columnIndexPrevious = primaryCursorModifier.ColumnIndex;
	            
	            if (!viewModelModifier.ViewModel.FirstPresentationLayerKeysList.Contains(
	            		TextEditorDevToolsPresentationFacts.PresentationKey))
	            {
					var copy = new List<Key<TextEditorPresentationModel>>(viewModelModifier.ViewModel.FirstPresentationLayerKeysList);
					copy.Add(TextEditorDevToolsPresentationFacts.PresentationKey);

					viewModelModifier.ViewModel = viewModelModifier.ViewModel with
		            {
		            	FirstPresentationLayerKeysList = copy
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
		
				var targetScope = modelModifier.CompilerService.GetScopeByPositionIndex(
					resourceUri,
					modelModifier.GetPositionIndex(primaryCursorModifier));
				
				if (!targetScope.ConstructorWasInvoked)
				{
					Console.WriteLine("aaa if (targetScope is null)");
					return ValueTask.CompletedTask;
				}
	    
				TextEditorTextSpan textSpanStart;
	    		
	    		if (targetScope.CodeBlockOwner.OpenCodeBlockTextSpan is null)
	    		{
	    			textSpanStart = new TextEditorTextSpan(
			            targetScope.StartingIndexInclusive,
			            targetScope.StartingIndexInclusive + 1,
					    (byte)TextEditorDevToolsDecorationKind.Scope,
					    resourceUri,
					    sourceText: string.Empty,
					    getTextPrecalculatedResult: string.Empty);
	    		}
	    		else
	    		{
	    			textSpanStart = new TextEditorTextSpan(
			            targetScope.CodeBlockOwner.OpenCodeBlockTextSpan.Value.StartingIndexInclusive,
			            targetScope.CodeBlockOwner.OpenCodeBlockTextSpan.Value.StartingIndexInclusive + 1,
					    (byte)TextEditorDevToolsDecorationKind.Scope,
					    resourceUri,
					    sourceText: string.Empty,
					    getTextPrecalculatedResult: string.Empty);
	    		}

				int useStartingIndexInclusive;
				if (targetScope.EndingIndexExclusive == -1)
					useStartingIndexInclusive = presentationModel.PendingCalculation.ContentAtRequest.Length - 1;
				else
					useStartingIndexInclusive = targetScope.EndingIndexExclusive -1;

				if (useStartingIndexInclusive < 0)
					useStartingIndexInclusive = 0;

				var useEndingIndexExclusive = targetScope.EndingIndexExclusive;
	    		if (useEndingIndexExclusive == -1)
	    			useEndingIndexExclusive = presentationModel.PendingCalculation.ContentAtRequest.Length;
	    			
				var textSpanEnd = new TextEditorTextSpan(
		            useStartingIndexInclusive,
				    useEndingIndexExclusive,
				    (byte)TextEditorDevToolsDecorationKind.Scope,
				    resourceUri,
				    sourceText: string.Empty,
				    getTextPrecalculatedResult: string.Empty);
		
				var diagnosticTextSpans = new List<TextEditorTextSpan> { textSpanStart, textSpanEnd };
	
				modelModifier.CompletePendingCalculatePresentationModel(
					TextEditorDevToolsPresentationFacts.PresentationKey,
					TextEditorDevToolsPresentationFacts.EmptyPresentationModel,
					diagnosticTextSpans);
					
				if (_codeBlockOwner != targetScope.CodeBlockOwner)
				{
					_codeBlockOwner = targetScope.CodeBlockOwner;
					_shouldRender = true;
				}
		
	    		return ValueTask.CompletedTask;
	    	}
	    	catch (Exception e)
	    	{
	    		Console.WriteLine(e);
	    		return ValueTask.CompletedTask;
	    	}
    	});
    }

	public void Dispose()
    {
    	TextEditorViewModelDisplay.RenderBatchChanged -= OnRenderBatchChanged;
    	
    	_cancellationTokenSource.Cancel();
    	_cancellationTokenSource.Dispose();
    }
}