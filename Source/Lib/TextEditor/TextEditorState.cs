using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

namespace Luthetus.TextEditor.RazorLib;

/// <summary>
/// Do not modify the '_modelMap' or '_viewModelMap' directly.
/// Use the 'ITextEditorService'.
/// Optimizations are very "agressively" being added at the moment.
/// Once the optimizations "feel" good then these dictionaries need to have their accessability decided on.
/// </summary>
public record TextEditorState
{
	// Move TextEditorState.Main.cs here (2025-02-08)
	public readonly Dictionary<ResourceUri, TextEditorModel> _modelMap = new();
	public readonly Dictionary<Key<TextEditorViewModel>, TextEditorViewModel> _viewModelMap = new();
	public readonly Dictionary<Key<TextEditorComponentData>, TextEditorComponentData> _componentDataMap = new();
	
	public (TextEditorModel? TextEditorModel, TextEditorViewModel? TextEditorViewModel)
		GetModelAndViewModelOrDefault(ResourceUri resourceUri, Key<TextEditorViewModel> viewModelKey)
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
				_ = _modelMap.TryGetValue(inViewModel.PersistentState.ResourceUri, out inModel);
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
    
    public List<TextEditorViewModel> ModelGetViewModelsOrEmpty(ResourceUri resourceUri)
    {
    	try
    	{
    		return _viewModelMap.Values
    			.Where(x => x.PersistentState.ResourceUri == resourceUri)
            	.ToList();
    	}
    	catch (Exception e)
		{
			Console.WriteLine(e);
		}
		
		return new();
    }
    
    public TextEditorViewModel? ViewModelGetOrDefault(Key<TextEditorViewModel> viewModelKey)
    {
    	try
    	{
    		return _viewModelMap[viewModelKey];
    	}
    	catch (Exception e)
		{
			// Eat this exception for now.
			// This is being done due to "race condition" like scenarios
			// and a performant solution to this needs to be decided on.
			//
			// Console.WriteLine(e);
			
			return null;
		}
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
