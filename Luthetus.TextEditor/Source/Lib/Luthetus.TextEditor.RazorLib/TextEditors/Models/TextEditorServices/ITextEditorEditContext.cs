﻿using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels.Internals;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

public interface ITextEditorEditContext
{
    public ITextEditorService TextEditorService { get; }

    public TextEditorModelModifier? GetModelModifier(ResourceUri? modelResourceUri, bool isReadonly = false);
    public TextEditorModelModifier? GetModelModifierByViewModelKey(Key<TextEditorViewModel> viewModelKey, bool isReadonly = false);
    public TextEditorViewModelModifier? GetViewModelModifier(Key<TextEditorViewModel> viewModelKey, bool isReadonly = false);
    public TextEditorCursorModifierBag? GetCursorModifierBag(TextEditorViewModel viewModel);

    /// <summary>
    /// TODO: Caching for this method?
    /// </summary>
    public TextEditorCursorModifier? GetPrimaryCursorModifier(TextEditorCursorModifierBag? cursorModifierBag);
}

