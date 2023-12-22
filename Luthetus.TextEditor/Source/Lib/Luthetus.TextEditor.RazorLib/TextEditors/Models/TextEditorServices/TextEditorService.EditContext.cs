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
        public readonly Dictionary<ResourceUri, TextEditorModelModifier?> ModelCache = new();
        public readonly Dictionary<Key<TextEditorViewModel>, ResourceUri?> ViewModelToModelResourceUriCache = new();
        public readonly Dictionary<Key<TextEditorViewModel>, TextEditorViewModelModifier?> ViewModelCache = new();
        public readonly Dictionary<Key<TextEditorViewModel>, TextEditorCursorModifierBag?> CursorModifierBagCache = new();

        public TextEditorEditContext(
            ITextEditorService textEditorService,
            Key<AuthenticatedAction> authenticatedActionKey)
        {
            TextEditorService = textEditorService;
            AuthenticatedActionKey = authenticatedActionKey;
        }

        public ITextEditorService TextEditorService { get; }
        public Key<AuthenticatedAction> AuthenticatedActionKey { get; }
        
        public TextEditorModelModifier? GetModelModifier(ResourceUri? modelResourceUri)
        {
            if (modelResourceUri is not null)
            {
                if (!ModelCache.TryGetValue(modelResourceUri, out var modelModifier))
                {
                    var model = TextEditorService.ModelApi.GetOrDefault(modelResourceUri);
                    modelModifier = model is null ? null : new(model);

                    ModelCache.Add(modelResourceUri, modelModifier);
                }

                return modelModifier;
            }
            
            return null;
        }
        
        public TextEditorModelModifier? GetModelModifierByViewModelKey(Key<TextEditorViewModel> viewModelKey)
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

        public TextEditorViewModelModifier? GetViewModelModifier(Key<TextEditorViewModel> viewModelKey)
        {
            if (viewModelKey != Key<TextEditorViewModel>.Empty)
            {
                if (!ViewModelCache.TryGetValue(viewModelKey, out var viewModelModifier))
                {
                    var viewModel = TextEditorService.ViewModelApi.GetOrDefault(viewModelKey);
                    viewModelModifier = viewModel is null ? null : new(viewModel);

                    ViewModelCache.Add(viewModelKey, viewModelModifier);
                }

                return viewModelModifier;
            }

            return null;
        }

        public TextEditorCursorModifierBag? GetCursorModifierBag(TextEditorViewModel viewModel)
        {
            if (viewModel is not null)
            {
                if (!CursorModifierBagCache.TryGetValue(viewModel.ViewModelKey, out var cursorModifierBag))
                {
                    cursorModifierBag = new TextEditorCursorModifierBag(
                        viewModel.ViewModelKey,
                        viewModel.CursorBag.Select(x => new TextEditorCursorModifier(x)).ToList());

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
                primaryCursor = cursorModifierBag.CursorModifierBag.FirstOrDefault(x => x.IsPrimaryCursor);

            return primaryCursor;
        }
    }
}

