using Luthetus.TextEditor.RazorLib.Commands.Models.Vims;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;
using Luthetus.Common.RazorLib.Keymaps.Models;

namespace Luthetus.TextEditor.RazorLib.Keymaps.Models.Vims;

public static class SyntaxVerbVim
{
    public static bool TryLex(KeymapArgument keymapArgument, bool hasTextSelection, out VimGrammarToken? vimGrammarToken)
    {
        // Suppress unused parameter warnings because these will likely be used once further Vim emulation is implemented.
        _ = hasTextSelection;

        var isGrammarToken = false;

        switch (keymapArgument.Code)
        {
            case "KeyD":
            case "KeyC":
            case "KeyY":
            case "KeyP":
            case "KeyO":
                isGrammarToken = true;
                break;
            case "KeyE":
                if (keymapArgument.CtrlKey)
                    isGrammarToken = true;

                break;
            case "Comma":
            case "Period":
                if (keymapArgument.ShiftKey)
                    isGrammarToken = true;

                break;
        }


        if (isGrammarToken)
        {
            vimGrammarToken = new VimGrammarToken(VimGrammarKind.Verb, keymapArgument);
            return true;
        }

        vimGrammarToken = null;
        return false;
    }

    public static bool TryParse(TextEditorKeymapVim textEditorKeymapVim,
        ImmutableArray<VimGrammarToken> sentenceSnapshotList,
        int indexInSentence,
        KeymapArgument keymapArgument,
        bool hasTextSelection,
        out TextEditorCommand? textEditorCommand)
    {
        bool verbWasTypedTwoTimesInARow = false;

        var currentToken = sentenceSnapshotList[indexInSentence];

        if (indexInSentence + 1 < sentenceSnapshotList.Length)
        {
            var nextToken = sentenceSnapshotList[indexInSentence + 1];

            if (nextToken.VimGrammarKind == VimGrammarKind.Verb &&
                nextToken.KeymapArgument == currentToken.KeymapArgument)
            {
                verbWasTypedTwoTimesInARow = true;
            }
        }

        if (verbWasTypedTwoTimesInARow)
        {
            // TODO: When a verb is doubled is it always the case that the position indices to operate over are known without the need of a motion? Example, "dd" would delete the current line and copy it to the in memory clipboard. But no motion was needed to know what text to delete.
            switch (currentToken.KeymapArgument.Code)
            {
                case "KeyD":
                    textEditorCommand = TextEditorCommandVimFacts.Verbs.DeleteLineCommand;
                    return true;
                case "KeyC":
                    textEditorCommand = TextEditorCommandVimFacts.Verbs.ChangeLineCommand;
                    return true;
                case "KeyY":
                    textEditorCommand = TextEditorCommandDefaultFacts.Copy;
                    return true;
                case "KeyP":
                    textEditorCommand = TextEditorCommandDefaultFacts.PasteCommand;
                    return true;
            }
        }

        if (keymapArgument.CtrlKey)
        {
            switch (currentToken.KeymapArgument.Code)
            {
                case "KeyE":
                    textEditorCommand = TextEditorCommandDefaultFacts.ScrollLineDown;
                    return true;
                case "KeyY":
                    textEditorCommand = TextEditorCommandDefaultFacts.ScrollLineUp;
                    return true;
            }
        }
        else if (indexInSentence + 1 < sentenceSnapshotList.Length)
        {
            // Track locally the displacement of the user's cursor after the
            // inner text editor command is invoked.

            if (VimSentence.TryParseNextToken(
                    textEditorKeymapVim,
                    sentenceSnapshotList,
                    indexInSentence + 1,
                    keymapArgument,
                    hasTextSelection,
                    out var innerCommand) &&
                innerCommand is not null)
            {
                switch (currentToken.KeymapArgument.Code)
                {
                    case "KeyD":
                        textEditorCommand = TextEditorCommandVimFacts.Verbs.DeleteMotionCommandConstructor(innerCommand);
                        return true;
                    case "KeyC":
                        textEditorCommand = TextEditorCommandVimFacts.Verbs.ChangeMotionCommandConstructor(innerCommand);
                        return true;
                    case "KeyY":
                        textEditorCommand = TextEditorCommandDefaultFacts.Copy;
                        return true;
                    case "KeyP":
                        textEditorCommand = TextEditorCommandDefaultFacts.PasteCommand;
                        return true;
                }
            }
        }
        else if (hasTextSelection)
        {
            if (sentenceSnapshotList.Any(x => x.VimGrammarKind == VimGrammarKind.Repeat))
            {
                switch (currentToken.KeymapArgument.Code)
                {
                    case "KeyP":
                        textEditorCommand = TextEditorCommandDefaultFacts.PasteCommand;
                        return true;
                    case "Comma":
                        if (keymapArgument.ShiftKey)
                        {
                            textEditorCommand = TextEditorCommandDefaultFacts.IndentMore;
                            return true;
                        }

                        break;
                    case "Period":
                        if (keymapArgument.ShiftKey)
                        {
                            textEditorCommand = TextEditorCommandDefaultFacts.IndentMore;
                            return true;
                        }

                        break;
                }
            }
            else
            {
                switch (currentToken.KeymapArgument.Code)
                {
                    case "KeyD":
                        textEditorCommand = TextEditorCommandDefaultFacts.Cut;
                        return true;
                    case "KeyC":
                        textEditorCommand = TextEditorCommandVimFacts.Verbs.ChangeSelectionCommand;
                        return true;
                    case "KeyY":
                        textEditorCommand = TextEditorCommandVimFacts.Verbs.YankCommand;
                        return true;
                    case "KeyP":
                        textEditorCommand = TextEditorCommandDefaultFacts.PasteCommand;
                        return true;
                    case "Comma":
                        if (keymapArgument.ShiftKey)
                        {
                            textEditorCommand = TextEditorCommandDefaultFacts.IndentMore;
                            return true;
                        }

                        break;
                    case "Period":
                        if (keymapArgument.ShiftKey)
                        {
                            textEditorCommand = TextEditorCommandDefaultFacts.IndentLess;
                            return true;
                        }

                        break;
                }
            }
        }

        textEditorCommand = null;
        return false;
    }
}