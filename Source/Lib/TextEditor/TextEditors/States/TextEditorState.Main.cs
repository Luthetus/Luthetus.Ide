using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.States;

/// <summary>
/// This type will replace what was 'TextEditorModelState' and 'TextEditorViewModelState'.
///
/// Edits to a text editor are done via the <see cref="ITextEditorEditContext"/>.
/// And prior to making this change, both 'TextEditorModelState' and 'TextEditorViewModelState'
/// needed to be dispatched to, once the <see cref="ITextEditorEditContext"/> was finalized.
///
/// The result of this double dispatching is that the text editor UI is told to re-render
/// twice.
///
/// One can fix this double re-render by tracking the last rendered model and viewModel,
/// but this approach requires extreme precision and is prone to errors: missed re-renders
/// when there should've been one.
///
/// So, the fix for double re-renders will be to combine the [FeatureState] for 'TextEditorModelState'
/// and 'TextEditorViewModelState' into a single [FeatureState].
///
/// A concern with this fix might be, "what if I only want to subscribe to 'TextEditorModelState' changes?"
///
/// (Okay, I'm writing this note after the fact. It seems like no matter what one would need to
///  introduce some 'HashCode' of sorts, that indicates that the model or viewModel has changed.
///  This use case is not in the app at the moment, but if it becomes of need then it can be added.)
/// The solution would be to use:
///
/// ```csharp
/// [Inject]
/// private IStateSelection<TextEditorState, TextEditorModelState?> TextEditorStateSelection { get; set; } = null!;
///
/// protected override void OnInitialized()
/// {
///     TextEditorStateSelection.Select(textEditorState =>
///     {
///         if (textEditorState.ModelList.TryGetValue(
///                 ResourceUri,
///                 out var model))
///		{
///				return model;
///		}
///
///         return null;
///     });
///
///     base.OnInitialized();
/// }
/// ```
///
/// TODO: This file has odd code where a try catch wraps a Dictionary 'TryGetValue(...)' method invocation and no other code is in the try clause.
/// </summary>
[FeatureState]
public partial record TextEditorState
{
	private readonly Dictionary<ResourceUri, TextEditorModel> _modelMap = new();
	private readonly Dictionary<Key<TextEditorViewModel>, TextEditorViewModel> _viewModelMap = new();
	
	public (TextEditorModel? TextEditorModel, TextEditorViewModel? TextEditorViewModel) GetModelAndViewModelOrDefault(
		ResourceUri resourceUri, Key<TextEditorViewModel> viewModelKey)
	{
		var inModel = (TextEditorModel?)null;
		var inViewModel = (TextEditorViewModel?)null;
		
		try
		{
			_ = _modelMap.TryGetValue(resourceUri, out inModel);
			_ = _viewModelMap.TryGetValue(viewModelKey, out inViewModel);
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
		}
		
		return (inModel, inViewModel);
	}
	
	/// <summary>
	/// This overload will lookup the model for the given view model, in the case that one only has access to the viewModelKey.
	/// </summary>
	public (TextEditorModel? Model, TextEditorViewModel? ViewModel) GetModelAndViewModelOrDefault(
		Key<TextEditorViewModel> viewModelKey)
	{
		var inModel = (TextEditorModel?)null;
		var inViewModel = (TextEditorViewModel?)null;
		
		try
		{
			_ = _viewModelMap.TryGetValue(viewModelKey, out inViewModel);
			
			if (inViewModel is not null)
				_ = _modelMap.TryGetValue(inViewModel.ResourceUri, out inModel);
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
		}
		
		return (inModel, inViewModel);
	}
	
	public TextEditorModel? ModelGetOrDefault(ResourceUri resourceUri)
    {
    	var inModel = (TextEditorModel?)null;
    	
    	try
    	{
    		var exists = _modelMap.TryGetValue(resourceUri, out inModel);
    	}
    	catch (Exception e)
		{
			Console.WriteLine(e);
		}
		
		return inModel;
    }
    
	/// <summary>
	/// Returns a shallow copy
	/// </summary>
    public Dictionary<ResourceUri, TextEditorModel> ModelGetModels()
    {
    	try
    	{
    		return new Dictionary<ResourceUri, TextEditorModel>(_modelMap);
    	}
    	catch (Exception e)
		{
			Console.WriteLine(e);
		}
		
		return new();
    }
    
    public int ModelGetModelsCount()
    {
    	try
    	{
    		return _modelMap.Count;
    	}
    	catch (Exception e)
		{
			Console.WriteLine(e);
		}
		
		return 0;
    }
    
    public ImmutableArray<TextEditorViewModel> ModelGetViewModelsOrEmpty(ResourceUri resourceUri)
    {
    	try
    	{
    		return _viewModelMap.Values
    			.Where(x => x.ResourceUri == resourceUri)
            	.ToImmutableArray();;
    	}
    	catch (Exception e)
		{
			Console.WriteLine(e);
		}
		
		return ImmutableArray<TextEditorViewModel>.Empty;
    }
    
    public TextEditorViewModel? ViewModelGetOrDefault(Key<TextEditorViewModel> viewModelKey)
    {
    	var inViewModel = (TextEditorViewModel?)null;
    
    	try
    	{
    		var exists = _viewModelMap.TryGetValue(viewModelKey, out inViewModel);
    	}
    	catch (Exception e)
		{
			Console.WriteLine(e);
		}
		
    	return inViewModel;
    }

	/// <summary>
	/// Returns a shallow copy
	/// </summary>
    public Dictionary<Key<TextEditorViewModel>, TextEditorViewModel> ViewModelGetViewModels()
    {
    	try
    	{
    		return new Dictionary<Key<TextEditorViewModel>, TextEditorViewModel>(_viewModelMap);
    	}
    	catch (Exception e)
		{
			Console.WriteLine(e);
		}
		
		return new();
    }
    
    public int ViewModelGetViewModelsCount()
    {
    	try
    	{
    		return _viewModelMap.Count;
    	}
    	catch (Exception e)
		{
			Console.WriteLine(e);
		}
		
		return 0;
    }
}
