using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.TextEditors.States;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

public partial class TextEditorService
{
    private record TextEditorEditContext : ITextEditorEditContext
    {
        public Dictionary<ResourceUri, TextEditorModelModifier?> ModelCache { get; } = new();
        public Dictionary<Key<TextEditorViewModel>, ResourceUri?> ViewModelToModelResourceUriCache { get; } = new();
        public Dictionary<Key<TextEditorViewModel>, TextEditorViewModelModifier?> ViewModelCache { get; } = new();
        public Dictionary<Key<TextEditorViewModel>, TextEditorCursorModifierBag?> CursorModifierBagCache { get; } = new();

        public TextEditorEditContext(
            ITextEditorService textEditorService,
            Key<TextEditorAuthenticatedAction> authenticatedActionKey)
        {
            TextEditorService = textEditorService;
            AuthenticatedActionKey = authenticatedActionKey;
        }

        public ITextEditorService TextEditorService { get; }
        public Key<TextEditorAuthenticatedAction> AuthenticatedActionKey { get; }
        
        public TextEditorModelModifier? GetModelModifier(
            ResourceUri? modelResourceUri,
            bool isReadonly = false)
        {
            if (modelResourceUri is not null)
            {
                if (!ModelCache.TryGetValue(modelResourceUri, out var modelModifier))
                {
                    var model = TextEditorService.ModelApi.GetOrDefault(modelResourceUri);
                    modelModifier = model is null ? null : new(model);

                    ModelCache.Add(modelResourceUri, modelModifier);
                }

                if (!isReadonly && modelModifier is not null)
                    modelModifier.WasModified = true;

                return modelModifier;
            }
            
            return null;
        }
        
        public TextEditorModelModifier? GetModelModifierByViewModelKey(
            Key<TextEditorViewModel> viewModelKey,
            bool isReadonly = false)
        {
            if (viewModelKey != Key<TextEditorViewModel>.Empty)
            {
                if (!ViewModelToModelResourceUriCache.TryGetValue(viewModelKey, out var modelResourceUri))
                {
                    var model = TextEditorService.ViewModelApi.GetModelOrDefault(viewModelKey);
                    modelResourceUri = model?.ResourceUri;

                    ViewModelToModelResourceUriCache.Add(viewModelKey, modelResourceUri);
                }

                return GetModelModifier(modelResourceUri);
            }

            return null;
        }

        public TextEditorViewModelModifier? GetViewModelModifier(
            Key<TextEditorViewModel> viewModelKey,
            bool isReadonly = false)
        {
            if (viewModelKey != Key<TextEditorViewModel>.Empty)
            {
                if (!ViewModelCache.TryGetValue(viewModelKey, out var viewModelModifier))
                {
                    var viewModel = TextEditorService.ViewModelApi.GetOrDefault(viewModelKey);
                    viewModelModifier = viewModel is null ? null : new(viewModel);

                    ViewModelCache.Add(viewModelKey, viewModelModifier);
                }

                if (!isReadonly && viewModelModifier is not null)
                    viewModelModifier.WasModified = true;

                return viewModelModifier;
            }

            return null;
        }

        public TextEditorCursorModifierBag? GetCursorModifierBag(TextEditorViewModel? viewModel)
        {
            if (viewModel is not null)
            {
                if (!CursorModifierBagCache.TryGetValue(viewModel.ViewModelKey, out var cursorModifierBag))
                {
                    cursorModifierBag = new TextEditorCursorModifierBag(
                        viewModel.ViewModelKey,
                        viewModel.CursorList.Select(x => new TextEditorCursorModifier(x)).ToList());

                    CursorModifierBagCache.Add(viewModel.ViewModelKey, cursorModifierBag);
                }

                return cursorModifierBag;
            }

            return null;
        }

        public TextEditorCursorModifier? GetPrimaryCursorModifier(TextEditorCursorModifierBag? cursorModifierBag)
        {
            var primaryCursor = (TextEditorCursorModifier?)null;

            if (cursorModifierBag is not null)
                primaryCursor = cursorModifierBag.List.FirstOrDefault(x => x.IsPrimaryCursor);

            return primaryCursor;
        }
    }
}

