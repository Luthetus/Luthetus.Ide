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
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Luthetus.Extensions.CompilerServices.Syntax;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Displays;

// Project ---- CurrentCodeBlockOwner --- Definitions inside the code block

public partial class TextEditorCompilerServiceHeaderDisplay : ComponentBase, ITextEditorDependentComponent
{
	[Inject]
	public ITextEditorService TextEditorService { get; set; } = null!;
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
    	return GetComponentData()?._activeRenderBatch ?? default;
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
    	if (!GetRenderBatch().ConstructorWasInvoked)
    		return;
    	
    	TextEditorService.WorkerArbitrary.PostUnique(nameof(TextEditorCompilerServiceHeaderDisplay), async editContext =>
    	{
    		var renderBatch = GetRenderBatch();
    	
    		var modelModifier = editContext.GetModelModifier(renderBatch.Model.ResourceUri);
            var viewModelModifier = editContext.GetViewModelModifier(renderBatch.ViewModel.ViewModelKey);
            var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier);
            var primaryCursorModifier = cursorModifierBag.CursorModifier;

            if (modelModifier is null || viewModelModifier is null || !cursorModifierBag.ConstructorWasInvoked || primaryCursorModifier is null)
                return;
            
            _lineIndexPrevious = primaryCursorModifier.LineIndex;
            _columnIndexPrevious = primaryCursorModifier.ColumnIndex;
            
            if (!viewModelModifier.FirstPresentationLayerKeysList.Contains(
            		TextEditorDevToolsPresentationFacts.PresentationKey))
            {
				var copy = new List<Key<TextEditorPresentationModel>>(viewModelModifier.FirstPresentationLayerKeysList);
				copy.Add(TextEditorDevToolsPresentationFacts.PresentationKey);

				viewModelModifier.FirstPresentationLayerKeysList = copy;
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
	
			if (modelModifier.CompilerService is not IExtendedCompilerService extendedCompilerService)
				return;
	
			var targetScope = extendedCompilerService.GetScopeByPositionIndex(
				resourceUri,
				modelModifier.GetPositionIndex(primaryCursorModifier));
			
			if (!targetScope.ConstructorWasInvoked)
				return;
    
			TextEditorTextSpan textSpanStart;
    		
    		if (!targetScope.CodeBlockOwner.OpenCodeBlockTextSpan.ConstructorWasInvoked)
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
		            targetScope.CodeBlockOwner.OpenCodeBlockTextSpan.StartingIndexInclusive,
		            targetScope.CodeBlockOwner.OpenCodeBlockTextSpan.StartingIndexInclusive + 1,
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
				
			var resource = extendedCompilerService.GetResource(modelModifier.ResourceUri);
			
			var virtualizedGutterChevronList = new List<GutterChevron>();
			
			if (resource.CompilationUnit is IExtendedCompilationUnit extendedCompilationUnit &&
				viewModelModifier.VirtualizationResult.EntryList.Any())
			{
	            var lowerLineIndexInclusive = viewModelModifier.VirtualizationResult.EntryList.First().LineIndex;
	            var upperLineIndexInclusive = viewModelModifier.VirtualizationResult.EntryList.Last().LineIndex;
	            
	            var lowerLine = modelModifier.GetLineInformation(lowerLineIndexInclusive);
	            var upperLine = modelModifier.GetLineInformation(upperLineIndexInclusive);
			
				if (extendedCompilationUnit.ScopeTypeDefinitionMap is not null)
				{
					foreach (var entry in extendedCompilationUnit.ScopeTypeDefinitionMap.Values)
					{
						Aaa(
					    	viewModelModifier,
					    	modelModifier,
					    	extendedCompilationUnit,
					    	virtualizedGutterChevronList,
					    	entry.TypeIdentifierToken,
					    	lowerLine,
					    	upperLine,
					    	entry.CloseCodeBlockTextSpan);
					}
				}
				
				if (extendedCompilationUnit.ScopeFunctionDefinitionMap is not null)
				{
					foreach (var entry in extendedCompilationUnit.ScopeFunctionDefinitionMap.Values)
					{
						Aaa(
					    	viewModelModifier,
					    	modelModifier,
					    	extendedCompilationUnit,
					    	virtualizedGutterChevronList,
					    	entry.FunctionIdentifierToken,
					    	lowerLine,
					    	upperLine,
					    	entry.CloseCodeBlockTextSpan);
					}
				}
			}
			
			viewModelModifier.VirtualizedGutterChevronList = virtualizedGutterChevronList;
				
			if (_codeBlockOwner != targetScope.CodeBlockOwner)
			{
				_codeBlockOwner = targetScope.CodeBlockOwner;
				_shouldRender = true;
			}
			
			await InvokeAsync(StateHasChanged);
    	});
    }
    
    private void Aaa(
    	TextEditorViewModel viewModelModifier,
    	TextEditorModel modelModifier,
    	IExtendedCompilationUnit extendedCompilationUnit,
    	List<GutterChevron> virtualizedGutterChevronList,
    	SyntaxToken token,
    	LineInformation lowerLine,
    	LineInformation upperLine,
    	TextEditorTextSpan closeCodeBlockTextSpan)
    {
    	if (token.TextSpan.ResourceUri != modelModifier.ResourceUri)
    		return;
    
    	if (lowerLine.StartPositionIndexInclusive <= token.TextSpan.StartingIndexInclusive &&
    	    upperLine.EndPositionIndexExclusive >= token.TextSpan.EndingIndexExclusive)
    	{
    		var lineAndColumnIndices = modelModifier.GetLineAndColumnIndicesFromPositionIndex(
    			token.TextSpan.StartingIndexInclusive);
    		
    		var indexPreviousChevron = viewModelModifier.AllGutterChevronList.FindIndex(
    			x => x.LineIndex == lineAndColumnIndices.lineIndex);
    			
    		bool isExpanded;
    		bool shouldAddToAll = false;
    			
			if (indexPreviousChevron != -1)
			{
				var previousChevron = viewModelModifier.AllGutterChevronList[indexPreviousChevron];
				isExpanded = viewModelModifier.AllGutterChevronList[indexPreviousChevron].IsExpanded;
				
				if (previousChevron.Identifier != token.TextSpan.GetText())
				{
					viewModelModifier.AllGutterChevronList.RemoveAt(indexPreviousChevron);
					shouldAddToAll = true;
				}
			}
			else
			{
				isExpanded = true;
				shouldAddToAll = true;
			}
    		
    		var closeCodeBlockLineAndColumnIndices = modelModifier.GetLineAndColumnIndicesFromPositionIndex(
    			closeCodeBlockTextSpan.StartingIndexInclusive);
    		
    		var newGutterChevron = new GutterChevron(
    			lineAndColumnIndices.lineIndex,
    			isExpanded,
    			token.TextSpan.GetText(),
    			token.TextSpan.StartingIndexInclusive,
    			token.TextSpan.EndingIndexExclusive,
    			closeCodeBlockLineAndColumnIndices.lineIndex + 1);
    		
    		virtualizedGutterChevronList.Add(newGutterChevron);
			if (shouldAddToAll)
				viewModelModifier.AllGutterChevronList.Add(newGutterChevron);
    	}
    }

	public void Dispose()
    {
    	TextEditorService.ViewModelApi.CursorShouldBlinkChanged -= OnCursorShouldBlinkChanged;
    	
    	_cancellationTokenSource.Cancel();
    	_cancellationTokenSource.Dispose();
    }
}