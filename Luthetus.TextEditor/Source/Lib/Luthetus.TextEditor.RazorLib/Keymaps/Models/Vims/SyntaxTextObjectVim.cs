using Luthetus.TextEditor.RazorLib.Commands.Models.Vims;
using System.Collections.Immutable;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;

namespace Luthetus.TextEditor.RazorLib.Keymaps.Models.Vims;

public static class SyntaxTextObjectVim
{
    public static bool TryLex(KeymapArgument keymapArgument, bool hasTextSelection, out VimGrammarToken? vimGrammarToken)
    {
        if (!keymapArgument.CtrlKey)
        {
            var isGrammarToken = false;

            switch (keymapArgument.Code)
            {
                case "KeyW":
                case "KeyE":
                case "KeyB":
                case "KeyH":
                case "KeyJ":
                case "KeyK":
                case "KeyL":
                case "Digit0":
                    isGrammarToken = true;
                    break;
                case "Digit4":
                    if (keymapArgument.ShiftKey)
                        isGrammarToken = true;

                    break;
            }

            if (isGrammarToken)
            {
                vimGrammarToken = new VimGrammarToken(VimGrammarKind.TextObject, keymapArgument);
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
        out CommandTextEditor? textEditorCommand)
    {
        var currentToken = sentenceSnapshotBag[indexInSentence];

        var shiftKey = textEditorKeymapVim.ActiveVimMode == VimMode.Visual ||
            textEditorKeymapVim.ActiveVimMode == VimMode.VisualLine;

        if (!currentToken.KeymapArgument.CtrlKey)
        {
            switch (currentToken.KeymapArgument.Code)
            {
                case "KeyW":
                    textEditorCommand = TextEditorCommandVimFacts.Motions.Word;
                    return true;
                case "KeyE":
                    textEditorCommand = TextEditorCommandVimFacts.Motions.End;
                    return true;
                case "KeyB":
                    textEditorCommand = TextEditorCommandVimFacts.Motions.Back;
                    return true;
                case "KeyH":
                    {
                        // Move the cursor 1 column to the left
                        textEditorCommand = new CommandTextEditor(
                                interfaceCommandParameter =>
                                {
                                    var commandParameter = (TextEditorCommandParameter)interfaceCommandParameter;

                                    TextEditorCursor.MoveCursor(
                                        new KeyboardEventArgs
                                        {
                                            Key = KeyboardKeyFacts.MovementKeys.ARROW_LEFT,
                                            ShiftKey = shiftKey
                                        },
                                        commandParameter.PrimaryCursorSnapshot.UserCursor,
                                        commandParameter.Model);

                                    return Task.CompletedTask;
                                },
                                true,
                                "Vim::h",
                                "vim_h");

                        return true;
                    }
                case "KeyJ":
                    {
                        // Move the cursor 1 row down
                        textEditorCommand = new CommandTextEditor(
                            interfaceCommandParameter =>
                            {
                                var commandParameter = (TextEditorCommandParameter)interfaceCommandParameter;

                                TextEditorCursor.MoveCursor(
                                    new KeyboardEventArgs
                                    {
                                        Key = KeyboardKeyFacts.MovementKeys.ARROW_DOWN,
                                        ShiftKey = shiftKey
                                    },
                                    commandParameter.PrimaryCursorSnapshot.UserCursor,
                                    commandParameter.Model);

                                return Task.CompletedTask;
                            },
                            true,
                            "Vim::j",
                            "vim_j");

                        return true;
                    }
                case "KeyK":
                    {
                        // Move the cursor 1 row up
                        textEditorCommand = new CommandTextEditor(
                            interfaceCommandParameter =>
                            {
                                var commandParameter = (TextEditorCommandParameter)interfaceCommandParameter;

                                TextEditorCursor.MoveCursor(
                                    new KeyboardEventArgs
                                    {
                                        Key = KeyboardKeyFacts.MovementKeys.ARROW_UP,
                                        ShiftKey = shiftKey
                                    },
                                    commandParameter.PrimaryCursorSnapshot.UserCursor,
                                    commandParameter.Model);

                                return Task.CompletedTask;
                            },
                            true,
                            "Vim::k",
                            "vim_k");

                        return true;
                    }
                case "KeyL":
                    {
                        // Move the cursor 1 column to the right
                        textEditorCommand = new CommandTextEditor(
                            interfaceCommandParameter =>
                            {
                                var commandParameter = (TextEditorCommandParameter)interfaceCommandParameter;

                                TextEditorCursor.MoveCursor(
                                    new KeyboardEventArgs
                                    {
                                        Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                                        ShiftKey = shiftKey
                                    },
                                    commandParameter.PrimaryCursorSnapshot.UserCursor,
                                    commandParameter.Model);

                                return Task.CompletedTask;
                            },
                            true,
                            "Vim::l",
                            "vim_l");

                        return true;
                    }
                case "Digit4":
                    {

                        if (!keymapArgument.ShiftKey)
                        {
                            textEditorCommand = TextEditorCommandDefaultFacts.DoNothingDiscard;
                            return true;
                        }
                        // Move the cursor to the end of the current line.
                        textEditorCommand = new CommandTextEditor(
                            interfaceCommandParameter =>
                            {
                                var commandParameter = (TextEditorCommandParameter)interfaceCommandParameter;

                                TextEditorCursor.MoveCursor(
                                    new KeyboardEventArgs
                                    {
                                        Key = KeyboardKeyFacts.MovementKeys.END,
                                        ShiftKey = shiftKey
                                    },
                                    commandParameter.PrimaryCursorSnapshot.UserCursor,
                                    commandParameter.Model);

                                return Task.CompletedTask;
                            },
                            true,
                            "Vim::$",
                            "vim_$");

                        return true;
                    }
                case "Digit0":
                    {
                        // Move the cursor to the start of the current line.
                        textEditorCommand = new CommandTextEditor(
                            interfaceCommandParameter =>
                            {
                                var commandParameter = (TextEditorCommandParameter)interfaceCommandParameter;

                                TextEditorCursor.MoveCursor(
                                    new KeyboardEventArgs
                                    {
                                        Key = KeyboardKeyFacts.MovementKeys.HOME,
                                        ShiftKey = shiftKey
                                    },
                                    commandParameter.PrimaryCursorSnapshot.UserCursor,
                                    commandParameter.Model);

                                return Task.CompletedTask;
                            },
                            true,
                            "Vim::0",
                            "vim_0");

                        return true;
                    }
            }
        }

        textEditorCommand = TextEditorCommandDefaultFacts.DoNothingDiscard;
        return true;
    }
}