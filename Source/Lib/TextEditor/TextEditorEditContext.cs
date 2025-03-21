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

    public TextEditorModelModifier? GetModelModifier(
        ResourceUri modelResourceUri,
        bool isReadonly = false)
    {
    	if (modelResourceUri == ResourceUri.Empty)
    		return null;
    
        if (!TextEditorService.__ModelCache.TryGetValue(modelResourceUri, out var modelModifier))
        {
            var model = TextEditorService.ModelApi.GetOrDefault(modelResourceUri);
            modelModifier = model is null ? null : new(model);

            TextEditorService.__ModelCache.Add(modelResourceUri, modelModifier);
        }

        if (!isReadonly && modelModifier is not null)
            modelModifier.WasModified = true;

        return modelModifier;
    }

    public TextEditorModelModifier? GetModelModifierByViewModelKey(
        Key<TextEditorViewModel> viewModelKey,
        bool isReadonly = false)
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

    public TextEditorViewModelModifier? GetViewModelModifier(
        Key<TextEditorViewModel> viewModelKey,
        bool isReadonly = false)
    {
        if (viewModelKey != Key<TextEditorViewModel>.Empty)
        {
            if (!TextEditorService.__ViewModelCache.TryGetValue(viewModelKey, out var viewModelModifier))
            {
                var viewModel = TextEditorService.ViewModelApi.GetOrDefault(viewModelKey);
                viewModelModifier = viewModel is null ? null : new(viewModel);

                TextEditorService.__ViewModelCache.Add(viewModelKey, viewModelModifier);
            }

            if (!isReadonly && viewModelModifier is not null)
                viewModelModifier.WasModified = true;

            return viewModelModifier;
        }

        return null;
    }
    
    public CursorModifierBagTextEditor GetCursorModifierBag(TextEditorViewModel? viewModel)
    {
        if (viewModel is not null)
        {
            if (!TextEditorService.__CursorModifierBagCache.TryGetValue(viewModel.ViewModelKey, out var cursorModifierBag))
            {
                cursorModifierBag = new CursorModifierBagTextEditor(
                    viewModel.ViewModelKey,
                    viewModel.CursorList.Select(x => new TextEditorCursorModifier(x)).ToList());

                TextEditorService.__CursorModifierBagCache.Add(viewModel.ViewModelKey, cursorModifierBag);
            }

            return cursorModifierBag;
        }

        return default(CursorModifierBagTextEditor);
    }

    public TextEditorCursorModifier? GetPrimaryCursorModifier(CursorModifierBagTextEditor cursorModifierBag)
    {
        var primaryCursor = (TextEditorCursorModifier?)null;

        if (cursorModifierBag.ConstructorWasInvoked)
            primaryCursor = cursorModifierBag.List.FirstOrDefault(x => x.IsPrimaryCursor);

        return primaryCursor;
    }

    public TextEditorDiffModelModifier? GetDiffModelModifier(
        Key<TextEditorDiffModel> diffModelKey,
        bool isReadonly = false)
    {
        if (diffModelKey != Key<TextEditorDiffModel>.Empty)
        {
            if (!TextEditorService.__DiffModelCache.TryGetValue(diffModelKey, out var diffModelModifier))
            {
                var diffModel = TextEditorService.DiffApi.GetOrDefault(diffModelKey);
                diffModelModifier = diffModel is null ? null : new(diffModel);

                TextEditorService.__DiffModelCache.Add(diffModelKey, diffModelModifier);
            }

            if (!isReadonly && diffModelModifier is not null)
                diffModelModifier.WasModified = true;

            return diffModelModifier;
        }

        return null;
    }
}
