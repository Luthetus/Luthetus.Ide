using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;
using Fluxor;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Options.States;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

/*
Goal: Have another header (separate from the default header) which is a switch statement,
      and shows content based on the file extension (2024-11-23).
=========================================================================================

Should this be:
	- implemented as a Blazor component, which in the markup evaluates a switch statement.
	- or as a 'string, Type' map.
		- Such that the 'string' input is the file extension no period, and the output is a type that
		  inherits 'ComponentBase'?

The 'Blazor component' implementation sounds very restrictive,
	when compared with the 'string, Type' map.

The reason being that a 'Blazor component' implementation would require editing the markup
of the 'Blazor component' in order to add a switch case.

The 'string, Type' map is far more flexible because it is just a matter of invoking
something along the lines of: '_map.Add("csproj", typeof(CSProjHeader));

As for how this is not equivalent to the current ViewModelOptions:
The ViewModelOptions is intended to have a custom 'ViewModelOptions.HeaderComponentType'
set only once, prior to the first render.

If a user wants to display a header based on the file extension no period,
they would have to create the custom component that has this functionality.

This is all well and good but, how would many different C# projects
work together to alter the switch cases.

For example:
- One is using .NET
- But as well is using Python

If the .NET extensions C# Project created a Blazor component to switch
over the file extension no periods: { "cs", "razor", "csproj"... }
It is presumed for this example that it won't include the file extension "py".

If the Python extensions C# Project made the Blazor component,
then the same issue would occur.

There needs to be a centralized way to map an arbitrary file extension no period
to a Type that inherits 'ComponentBase'.

With this change
- One could set 'ViewModelOptions.HeaderComponentType' to null and no header would render
- Or they could set 'ViewModelOptions.HeaderComponentType' to the default header and NO switch statement would be there
- They could set 'ViewModelOptions.HeaderComponentType' to this new component and the switch statement would be there
- Lastly they could set 'ViewModelOptions.HeaderComponentType' to whatever they wanted for any custom case they desire.
	Conveniently, if one wanted to use their custom case, but fallback to the switch statement over a file extension no period
	they could do that by including the switch statement component in their custom component.

In which project should this component reside?
----------------------------------------------
The TextEditor project

Where would the state exist for the mapping of file extension no period to Type that inherits ComponentBase?
------------------------------------------------------------------------------------------------------------
This feature sounds very similar to how the ICompilerServiceRegistry works.

So, the first thought is to make 'ITextEditorHeaderRegistry'

````using System.Collections.Immutable;
````
````namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;
````
````public interface ITextEditorHeaderRegistry
````{
````    public Type GetHeader(string extensionNoPeriod);
````    public void SetHeader(string extensionNoPeriod, Type type);
````}

A .NET extensions project could:
````textEditorHeaderRegistry.SetHeader("cs", CSharpTextEditorHeader);

And just the same a Python extensions project could:
````textEditorHeaderRegistry.SetHeader("py", PythonTextEditorHeader);

*/

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

	private void OnRenderBatchChanged()
    {
    	var renderBatch = TextEditorViewModelDisplay._storedRenderBatchTuple.Validated;
    	
    	if (renderBatch is null)
    		return;
    
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
				
			if (modelModifier.CompilerService.Binder.TryGetBinderSession(resourceUri, out var binderSession) &&
				!Object.ReferenceEquals(binderSession, _binderSessionPrevious))
			{
				_binderSessionPrevious = binderSession;
				
				
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