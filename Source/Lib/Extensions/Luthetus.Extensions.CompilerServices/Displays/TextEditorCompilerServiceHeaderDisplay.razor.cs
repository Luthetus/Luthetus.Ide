using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Lines.Models;
using Luthetus.Extensions.CompilerServices.Syntax;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Displays;

// Project ---- CurrentCodeBlockOwner --- Definitions inside the code block

public partial class TextEditorCompilerServiceHeaderDisplay : ComponentBase, ITextEditorDependentComponent
{
	[Inject]
	public TextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;

	[Parameter, EditorRequired]
	public Key<TextEditorComponentData> ComponentDataKey { get; set; }
	
	private ResourceUri _resourceUriPrevious = ResourceUri.Empty;
	
	private int _lineIndexPrevious = -1;
	private int _columnIndexPrevious = -1;
	
	private ICodeBlockOwner? _codeBlockOwner;
	private bool _shouldRender = false;
	
	private bool _showDefaultToolbar = false;
	
	private CancellationTokenSource _cancellationTokenSource = new();
	
	private Key<TextEditorComponentData> _componentDataKeyPrevious = Key<TextEditorComponentData>.Empty;
    private TextEditorComponentData? _componentData;
	
	protected override void OnInitialized()
    {
        TextEditorService.ViewModelApi.CursorShouldBlinkChanged += OnCursorShouldBlinkChanged;
        OnCursorShouldBlinkChanged();
        
        base.OnInitialized();
    }
    
    private TextEditorRenderBatch GetRenderBatch()
    {
    	return GetComponentData()?.RenderBatch ?? default;
    }
    
    private TextEditorComponentData? GetComponentData()
    {
    	if (_componentDataKeyPrevious != ComponentDataKey)
    	{
    		if (!TextEditorService.TextEditorState._componentDataMap.TryGetValue(ComponentDataKey, out var componentData) ||
    		    componentData is null)
    		{
    			_componentData = null;
    		}
    		else
    		{
    			_componentData = componentData;
				_componentDataKeyPrevious = ComponentDataKey;
    		}
    	}
    	
		return _componentData;
    }

	private async void OnCursorShouldBlinkChanged()
    {
    	if (TextEditorService.ViewModelApi.CursorShouldBlink)
    		UpdateUi();
    }
    
    private void ToggleDefaultToolbar()
    {
    	_showDefaultToolbar = !_showDefaultToolbar;
    }
    
    private void UpdateUi()
    {
    	if (!GetRenderBatch().IsValid)
    		return;
    	
    	TextEditorService.WorkerArbitrary.PostUnique(async editContext =>
    	{
    		var renderBatch = GetRenderBatch();
    	
    		var modelModifier = editContext.GetModelModifier(renderBatch.Model.PersistentState.ResourceUri);
            var viewModelModifier = editContext.GetViewModelModifier(renderBatch.ViewModel.PersistentState.ViewModelKey);

            if (modelModifier is null || viewModelModifier is null)
                return;
            
            _lineIndexPrevious = viewModelModifier.LineIndex;
            _columnIndexPrevious = viewModelModifier.ColumnIndex;
            
            if (!viewModelModifier.PersistentState.FirstPresentationLayerKeysList.Contains(
            		TextEditorDevToolsPresentationFacts.PresentationKey))
            {
				var copy = new List<Key<TextEditorPresentationModel>>(viewModelModifier.PersistentState.FirstPresentationLayerKeysList);
				copy.Add(TextEditorDevToolsPresentationFacts.PresentationKey);

				viewModelModifier.PersistentState.FirstPresentationLayerKeysList = copy;
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
	
	        var resourceUri = modelModifier.PersistentState.ResourceUri;
	
			if (modelModifier.PersistentState.CompilerService is not IExtendedCompilerService extendedCompilerService)
				return;
	
			var targetScope = extendedCompilerService.GetScopeByPositionIndex(
				resourceUri,
				modelModifier.GetPositionIndex(viewModelModifier));
			
			if (!targetScope.ConstructorWasInvoked)
				return;
    
			TextEditorTextSpan textSpanStart;
    		
    		if (!targetScope.CodeBlockOwner.OpenCodeBlockTextSpan.ConstructorWasInvoked)
    		{
    			textSpanStart = new TextEditorTextSpan(
		            targetScope.StartInclusiveIndex,
		            targetScope.StartInclusiveIndex + 1,
				    (byte)TextEditorDevToolsDecorationKind.Scope,
				    resourceUri,
				    sourceText: string.Empty,
				    getTextPrecalculatedResult: string.Empty);
    		}
    		else
    		{
    			textSpanStart = new TextEditorTextSpan(
		            targetScope.CodeBlockOwner.OpenCodeBlockTextSpan.StartInclusiveIndex,
		            targetScope.CodeBlockOwner.OpenCodeBlockTextSpan.StartInclusiveIndex + 1,
				    (byte)TextEditorDevToolsDecorationKind.Scope,
				    resourceUri,
				    sourceText: string.Empty,
				    getTextPrecalculatedResult: string.Empty);
    		}

			int useStartInclusiveIndex;
			if (targetScope.EndExclusiveIndex == -1)
				useStartInclusiveIndex = presentationModel.PendingCalculation.ContentAtRequest.Length - 1;
			else
				useStartInclusiveIndex = targetScope.EndExclusiveIndex - 1;

			if (useStartInclusiveIndex < 0)
				useStartInclusiveIndex = 0;

			var useEndExclusiveIndex = targetScope.EndExclusiveIndex;
    		if (useEndExclusiveIndex == -1)
    			useEndExclusiveIndex = presentationModel.PendingCalculation.ContentAtRequest.Length;
    			
			var textSpanEnd = new TextEditorTextSpan(
	            useStartInclusiveIndex,
			    useEndExclusiveIndex,
			    (byte)TextEditorDevToolsDecorationKind.Scope,
			    resourceUri,
			    sourceText: string.Empty,
			    getTextPrecalculatedResult: string.Empty);
	
			var diagnosticTextSpans = new List<TextEditorTextSpan> { textSpanStart, textSpanEnd };

			modelModifier.CompletePendingCalculatePresentationModel(
				TextEditorDevToolsPresentationFacts.PresentationKey,
				TextEditorDevToolsPresentationFacts.EmptyPresentationModel,
				diagnosticTextSpans);
			
			if (viewModelModifier.VirtualizationResult.EntryList.Any())
			{
				var lowerLineIndexInclusive = viewModelModifier.VirtualizationResult.EntryList.First().LineIndex;
	            var upperLineIndexInclusive = viewModelModifier.VirtualizationResult.EntryList.Last().LineIndex;
	            
	            var lowerLine = modelModifier.GetLineInformation(lowerLineIndexInclusive);
	            var upperLine = modelModifier.GetLineInformation(upperLineIndexInclusive);
				
				viewModelModifier.PersistentState.VirtualizedCollapsePointList = new();
				
				foreach (var collapsePoint in viewModelModifier.PersistentState.AllCollapsePointList)
				{
					if (lowerLine.Index <= collapsePoint.AppendToLineIndex &&
		    	    	upperLine.Index >= collapsePoint.AppendToLineIndex)
		    	    {
		    	    	viewModelModifier.PersistentState.VirtualizedCollapsePointList.Add(collapsePoint);
		    	    }
				}
				
				viewModelModifier.PersistentState.VirtualizedCollapsePointListVersion++;
			}
				
			if (_codeBlockOwner != targetScope.CodeBlockOwner)
			{
				_codeBlockOwner = targetScope.CodeBlockOwner;
				_shouldRender = true;
			}
			
			await InvokeAsync(StateHasChanged);
    	});
    }

	public void Dispose()
    {
    	TextEditorService.ViewModelApi.CursorShouldBlinkChanged -= OnCursorShouldBlinkChanged;
    	
    	_cancellationTokenSource.Cancel();
    	_cancellationTokenSource.Dispose();
    }
}