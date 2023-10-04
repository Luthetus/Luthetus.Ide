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
        var isGrammarToken = false;

        if (keymapArgument.CtrlKey)
        {
            switch (keymapArgument.Code)
            {

            }
        }

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
        ImmutableArray<VimGrammarToken> sentenceSnapshotBag,
        int indexInSentence,
        KeymapArgument keymapArgument,
        bool hasTextSelection,
        out CommandTextEditor? textEditorCommand)
    {
        bool verbWasTypedTwoTimesInARow = false;

        var currentToken = sentenceSnapshotBag[indexInSentence];

        if (indexInSentence + 1 < sentenceSnapshotBag.Length)
        {
            var nextToken = sentenceSnapshotBag[indexInSentence + 1];

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
                    textEditorCommand = TextEditorCommandVimFacts.Verbs.DeleteLine;
                    return true;
                case "KeyC":
                    textEditorCommand = TextEditorCommandVimFacts.Verbs.ChangeLine;
                    return true;
                case "KeyY":
                    textEditorCommand = TextEditorCommandDefaultFacts.Copy;
                    return true;
                case "KeyP":
                    textEditorCommand = TextEditorCommandDefaultFacts.Paste;
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
        else if (indexInSentence + 1 < sentenceSnapshotBag.Length)
        {
            // Track locally the displacement of the user's cursor after the
            // inner text editor command is invoked.

            if (VimSentence.TryParseNextToken(
                    textEditorKeymapVim,
                    sentenceSnapshotBag,
                    indexInSentence + 1,
                    keymapArgument,
                    hasTextSelection,
                    out var innerCommand) &&
                innerCommand is not null)
            {
                switch (currentToken.KeymapArgument.Code)
                {
                    case "KeyD":
                        textEditorCommand = TextEditorCommandVimFacts.Verbs.GetDeleteMotion(innerCommand);
                        return true;
                    case "KeyC":
                        textEditorCommand = TextEditorCommandVimFacts.Verbs.GetChangeMotion(innerCommand);
                        return true;
                    case "KeyY":
                        textEditorCommand = TextEditorCommandDefaultFacts.Copy;
                        return true;
                    case "KeyP":
                        textEditorCommand = TextEditorCommandDefaultFacts.Paste;
                        return true;
                }
            }
        }
        else if (hasTextSelection)
        {
            if (sentenceSnapshotBag.Any(x => x.VimGrammarKind == VimGrammarKind.Repeat))
            {
                switch (currentToken.KeymapArgument.Code)
                {
                    case "KeyP":
                        textEditorCommand = TextEditorCommandDefaultFacts.Paste;
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
                        textEditorCommand = TextEditorCommandVimFacts.Verbs.ChangeSelection;
                        return true;
                    case "KeyY":
                        textEditorCommand = TextEditorCommandVimFacts.Verbs.Yank;
                        return true;
                    case "KeyP":
                        textEditorCommand = TextEditorCommandDefaultFacts.Paste;
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