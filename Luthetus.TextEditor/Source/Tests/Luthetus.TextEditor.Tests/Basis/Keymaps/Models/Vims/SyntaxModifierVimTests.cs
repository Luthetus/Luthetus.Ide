using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.Tests.Basis.Keymaps.Models.Vims;

public static class SyntaxModifierVimTests
{
    public static bool TryLex(KeymapArgument keymapArgument, bool hasTextSelection, out VimGrammarToken? vimGrammarToken)
    {
        switch (keymapArgument.Code)
        {
            case "KeyI":
                {
                    vimGrammarToken = new VimGrammarToken(VimGrammarKind.Modifier, keymapArgument);
                    return true;
                }
        }

        vimGrammarToken = null;
        return false;
    }

    public static bool TryParse(TextEditorKeymapVim textEditorKeymapVim,
        ImmutableArray<VimGrammarToken> sentenceSnapshotBag,
        int indexInSentence,
        KeymapArgument keymapArgument,
        bool hasTextSelection,
        out TextEditorCommand? textEditorCommand)
    {
        textEditorCommand = TextEditorCommandDefaultFacts.DoNothingDiscard;
        return true;
    }
}