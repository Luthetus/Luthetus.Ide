using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.TextEditors.States;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

/// <summary>
/// If one invokes, "_ = editContext.GetViewModelModifier(viewModelKey);".<br/>
/// Then a side effect occurs where the view model with the provided viewModelKey, will be marked as a having been modified.<br/>
/// After a Post is finished, the text editor service iterates over all 'modified' view models and re-renders them.<br/>
/// This behavior is a bit hacky since it requires some amount of 'hidden knowledge'.<br/>
/// To get a view model without marking it as modified, use the optional parameter, 'isReadonly' and set it to 'true'.<br/>
/// The main reason for making this comment, is that I sometimes assign the result of 'GetViewModelModifier' to discard.<br/>
/// When I do this it feels especially hacky, so I want to leave a paper trail of what I mean by that statement.<br/>
/// </summary>
public interface ITextEditorEditContext
{
	public Dictionary<ResourceUri, TextEditorModelModifier?> ModelCache { get; }
    public Dictionary<Key<TextEditorViewModel>, ResourceUri?> ViewModelToModelResourceUriCache { get; }
    public Dictionary<Key<TextEditorViewModel>, TextEditorViewModelModifier?> ViewModelCache { get; }
    public Dictionary<Key<TextEditorViewModel>, TextEditorCursorModifierBag?> CursorModifierBagCache { get; }

    public ITextEditorService TextEditorService { get; }
    public Key<TextEditorAuthenticatedAction> AuthenticatedActionKey { get; }

    /// <inheritdoc cref="ITextEditorEditContext"/>
    public TextEditorModelModifier? GetModelModifier(ResourceUri? modelResourceUri, bool isReadonly = false);

    /// <inheritdoc cref="ITextEditorEditContext"/>
    public TextEditorModelModifier? GetModelModifierByViewModelKey(Key<TextEditorViewModel> viewModelKey, bool isReadonly = false);

    /// <inheritdoc cref="ITextEditorEditContext"/>
    public TextEditorViewModelModifier? GetViewModelModifier(Key<TextEditorViewModel> viewModelKey, bool isReadonly = false);

    /// <inheritdoc cref="ITextEditorEditContext"/>
    public TextEditorCursorModifierBag? GetCursorModifierBag(TextEditorViewModel? viewModel);

    /// <summary>
    /// TODO: Caching for this method?<br/>
    /// <inheritdoc cref="ITextEditorEditContext"/>
    /// </summary>
    public TextEditorCursorModifier? GetPrimaryCursorModifier(TextEditorCursorModifierBag? cursorModifierBag);
}

