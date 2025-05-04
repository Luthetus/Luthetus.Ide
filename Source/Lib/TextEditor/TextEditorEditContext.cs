using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Diffs.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib;

public struct TextEditorEditContext
{
    public TextEditorEditContext(ITextEditorService textEditorService)
    {
        TextEditorService = textEditorService;
    }

    public ITextEditorService TextEditorService { get; }

	/// <summary>
	/// 'isReadOnly == true' will not allocate a new TextEditorModel as well,
	/// nothing will be added to the '__ModelList'.
	/// </summary>
    public TextEditorModel? GetModelModifier(
        ResourceUri modelResourceUri,
        bool isReadOnly = false)
    {
    	if (modelResourceUri == ResourceUri.Empty)
    		return null;
    		
    	TextEditorModel? modelModifier = null;
    		
    	for (int i = 0; i < TextEditorService.__ModelList.Count; i++)
    	{
    		if (TextEditorService.__ModelList[i].ResourceUri == modelResourceUri)
    			modelModifier = TextEditorService.__ModelList[i];
    	}
    	
    	if (modelModifier is null)
    	{
    		var exists = TextEditorService.TextEditorState._modelMap.TryGetValue(
				modelResourceUri,
				out var model);
    		
    		if (isReadOnly || model is null)
    			return model;
    		
			modelModifier = model is null ? null : new(model);
        	TextEditorService.__ModelList.Add(modelModifier);
    	}

        return modelModifier;
    }

    public TextEditorModel? GetModelModifierByViewModelKey(
        Key<TextEditorViewModel> viewModelKey,
        bool isReadOnly = false)
    {
        if (viewModelKey != Key<TextEditorViewModel>.Empty)
        {
            if (!TextEditorService.__ViewModelToModelResourceUriCache.TryGetValue(viewModelKey, out var modelResourceUri))
            {
                var model = TextEditorService.ViewModelApi.GetModelOrDefault(viewModelKey);
                modelResourceUri = model?.ResourceUri;

                TextEditorService.__ViewModelToModelResourceUriCache.Add(viewModelKey, modelResourceUri);
            }

            return GetModelModifier(modelResourceUri.Value);
        }

        return null;
    }

    public TextEditorViewModel? GetViewModelModifier(
        Key<TextEditorViewModel> viewModelKey,
        bool isReadOnly = false)
    {
    	if (viewModelKey == Key<TextEditorViewModel>.Empty)
    		return null;
    		
    	TextEditorViewModel? viewModelModifier = null;
    		
    	for (int i = 0; i < TextEditorService.__ViewModelList.Count; i++)
    	{
    		if (TextEditorService.__ViewModelList[i].ViewModelKey == viewModelKey)
    			viewModelModifier = TextEditorService.__ViewModelList[i];
    	}
    	
    	if (viewModelModifier is null)
    	{
    		var exists = TextEditorService.TextEditorState._viewModelMap.TryGetValue(
				viewModelKey,
				out var viewModel);
    		
    		if (isReadOnly || viewModel is null)
    			return viewModel;
    		
			viewModelModifier = viewModel is null ? null : new(viewModel);
        	TextEditorService.__ViewModelList.Add(viewModelModifier);
    	}

        return viewModelModifier;
    }
    
    public CursorModifierBagTextEditor GetCursorModifierBag(TextEditorViewModel? viewModel)
    {
        if (viewModel is not null)
        {
            if (!TextEditorService.__CursorModifierBagCache.TryGetValue(viewModel.ViewModelKey, out var cursorModifierBag))
            {
            	TextEditorCursorModifier cursorModifier;
            	
    			if (TextEditorService.__IsAvailableCursorModifier)
    			{
    				TextEditorService.__IsAvailableCursorModifier = false;
    				
    				TextEditorService.__CursorModifier.LineIndex = viewModel.PrimaryCursor.LineIndex;
			        TextEditorService.__CursorModifier.ColumnIndex = viewModel.PrimaryCursor.ColumnIndex;
			        TextEditorService.__CursorModifier.PreferredColumnIndex = viewModel.PrimaryCursor.PreferredColumnIndex;
			        TextEditorService.__CursorModifier.IsPrimaryCursor = viewModel.PrimaryCursor.IsPrimaryCursor;
			        TextEditorService.__CursorModifier.SelectionAnchorPositionIndex = viewModel.PrimaryCursor.Selection.AnchorPositionIndex;
			        TextEditorService.__CursorModifier.SelectionEndingPositionIndex = viewModel.PrimaryCursor.Selection.EndingPositionIndex;
			        TextEditorService.__CursorModifier.Key = viewModel.PrimaryCursor.Key;
			        
			        cursorModifier = TextEditorService.__CursorModifier;
    			}
    			else
    			{
    				cursorModifier = new(viewModel.PrimaryCursor);
    			}
            
                cursorModifierBag = new CursorModifierBagTextEditor(
                    viewModel.ViewModelKey,
                    cursorModifier);

                TextEditorService.__CursorModifierBagCache.Add(viewModel.ViewModelKey, cursorModifierBag);
            }

            return cursorModifierBag;
        }

        return default(CursorModifierBagTextEditor);
    }

    public TextEditorDiffModelModifier? GetDiffModelModifier(
        Key<TextEditorDiffModel> diffModelKey,
        bool isReadOnly = false)
    {
        if (diffModelKey != Key<TextEditorDiffModel>.Empty)
        {
            if (!TextEditorService.__DiffModelCache.TryGetValue(diffModelKey, out var diffModelModifier))
            {
                var diffModel = TextEditorService.DiffApi.GetOrDefault(diffModelKey);
                diffModelModifier = diffModel is null ? null : new(diffModel);

                TextEditorService.__DiffModelCache.Add(diffModelKey, diffModelModifier);
            }

            if (!isReadOnly && diffModelModifier is not null)
                diffModelModifier.WasModified = true;

            return diffModelModifier;
        }

        return null;
    }
}
