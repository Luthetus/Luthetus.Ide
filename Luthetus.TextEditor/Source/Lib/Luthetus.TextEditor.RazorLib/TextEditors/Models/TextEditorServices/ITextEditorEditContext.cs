using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

public interface ITextEditorEditContext
{
    public ITextEditorService TextEditorService { get; }

    public TextEditorModelModifier? GetModelModifier(ResourceUri? modelResourceUri);
    public TextEditorModelModifier? GetModelModifierByViewModelKey(Key<TextEditorViewModel> viewModelKey);
    public TextEditorViewModelModifier? GetViewModelModifier(Key<TextEditorViewModel> viewModelKey);
    public TextEditorCursorModifierBag? GetCursorModifierBag(TextEditorViewModel viewModel);

    /// <summary>
    /// TODO: Caching for this method?
    /// </summary>
    public TextEditorCursorModifier? GetPrimaryCursorModifier(TextEditorCursorModifierBag? cursorModifierBag);
}

