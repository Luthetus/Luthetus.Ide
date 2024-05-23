using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Diffs.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
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
public interface IEditContext
{
	public Dictionary<ResourceUri, TextEditorModelModifier?> ModelCache { get; }
    public Dictionary<Key<TextEditorViewModel>, ResourceUri?> ViewModelToModelResourceUriCache { get; }
    public Dictionary<Key<TextEditorViewModel>, TextEditorViewModelModifier?> ViewModelCache { get; }
    public Dictionary<Key<TextEditorViewModel>, CursorModifierBagTextEditor?> CursorModifierBagCache { get; }

    public ITextEditorService TextEditorService { get; }
    public Key<TextEditorAuthenticatedAction> AuthenticatedActionKey { get; }

    /// <inheritdoc cref="IEditContext"/>
    public TextEditorModelModifier? GetModelModifier(ResourceUri? modelResourceUri, bool isReadonly = false);

    /// <inheritdoc cref="IEditContext"/>
    public TextEditorModelModifier? GetModelModifierByViewModelKey(Key<TextEditorViewModel> viewModelKey, bool isReadonly = false);

    /// <inheritdoc cref="IEditContext"/>
    public TextEditorViewModelModifier? GetViewModelModifier(Key<TextEditorViewModel> viewModelKey, bool isReadonly = false);

    /// <inheritdoc cref="IEditContext"/>
    public CursorModifierBagTextEditor? GetCursorModifierBag(TextEditorViewModel? viewModel);

    /// <inheritdoc cref="IEditContext"/>
	public TextEditorCursorModifier? GetCursorModifier(
		Key<TextEditorCursor> cursorKey,
		Func<Key<TextEditorCursor>, TextEditorCursor> getCursorFunc);

    /// <summary>
    /// TODO: Caching for this method?<br/>
    /// <inheritdoc cref="IEditContext"/>
    /// </summary>
    public TextEditorCursorModifier? GetPrimaryCursorModifier(CursorModifierBagTextEditor? cursorModifierBag);

    /// <inheritdoc cref="IEditContext"/>
    public TextEditorDiffModelModifier? GetDiffModelModifier(Key<TextEditorDiffModel> diffModelKey, bool isReadonly = false);
}

