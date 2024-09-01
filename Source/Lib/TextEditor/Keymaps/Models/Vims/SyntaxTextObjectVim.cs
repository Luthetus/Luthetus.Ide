using System.Collections.Immutable;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models.Vims;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.Keymaps.Models.Vims;

public static class SyntaxTextObjectVim
{
    public static bool TryLex(KeymapArgs keymapArgument, bool hasTextSelection, out VimGrammarToken? vimGrammarToken)
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

    public static Func<ITextEditorEditContext, Task> MoveCursorOneColumnLeftFactory(
        ResourceUri modelResourceUri,
        Key<TextEditorViewModel> viewModelKey,
        TextEditorCommandArgs commandArgs)
    {
        return (ITextEditorEditContext editContext) =>
        {
            var modelModifier = editContext.GetModelModifier(modelResourceUri);
            var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);
            var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;

            commandArgs.TextEditorService.ViewModelApi.MoveCursor(
        		new KeymapArgs
                {
                    Key = KeyboardKeyFacts.MovementKeys.ARROW_LEFT,
                    ShiftKey = commandArgs.ShiftKey
                },
		        editContext,
		        modelModifier,
		        viewModelModifier,
		        cursorModifierBag);
            return Task.CompletedTask;
        };
    }

    public static Func<ITextEditorEditContext, Task> MoveCursorOneRowDownFactory(
        ResourceUri modelResourceUri,
        Key<TextEditorViewModel> viewModelKey,
        TextEditorCommandArgs commandArgs)
    {
        return (ITextEditorEditContext editContext) =>
        {
            var modelModifier = editContext.GetModelModifier(modelResourceUri);
            var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);
            var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;

            commandArgs.TextEditorService.ViewModelApi.MoveCursor(
            	new KeymapArgs
                {
                    Key = KeyboardKeyFacts.MovementKeys.ARROW_DOWN,
                    ShiftKey = commandArgs.ShiftKey
                },
		        editContext,
		        modelModifier,
		        viewModelModifier,
		        cursorModifierBag);
		    return Task.CompletedTask;
        };
    }

    public static Func<ITextEditorEditContext, Task> MoveCursorOneRowUpFactory(
        ResourceUri modelResourceUri,
        Key<TextEditorViewModel> viewModelKey,
        TextEditorCommandArgs commandArgs)
    {
        return (ITextEditorEditContext editContext) =>
        {
            var modelModifier = editContext.GetModelModifier(modelResourceUri);
            var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);
            var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;

            commandArgs.TextEditorService.ViewModelApi.MoveCursor(
                new KeymapArgs
                {
                    Key = KeyboardKeyFacts.MovementKeys.ARROW_UP,
                    ShiftKey = commandArgs.ShiftKey
                },
		        editContext,
		        modelModifier,
		        viewModelModifier,
		        cursorModifierBag);
		    return Task.CompletedTask;
        };
    }

    public static Func<ITextEditorEditContext, Task> MoveCursorOneColumnRightFactory(
        ResourceUri modelResourceUri,
        Key<TextEditorViewModel> viewModelKey,
        TextEditorCommandArgs commandArgs)
    {
        return (ITextEditorEditContext editContext) =>
        {
            var modelModifier = editContext.GetModelModifier(modelResourceUri);
            var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);
            var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;

            commandArgs.TextEditorService.ViewModelApi.MoveCursor(
                new KeymapArgs
                {
                    Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                    ShiftKey = commandArgs.ShiftKey
                },
		        editContext,
		        modelModifier,
		        viewModelModifier,
		        cursorModifierBag);
		    return Task.CompletedTask;
        };
    }

    public static Func<ITextEditorEditContext, Task> MoveCursorEndCurrentLineFactory(
        ResourceUri modelResourceUri,
        Key<TextEditorViewModel> viewModelKey,
        TextEditorCommandArgs commandArgs)
    {
        return (ITextEditorEditContext editContext) =>
        {
            var modelModifier = editContext.GetModelModifier(modelResourceUri);
            var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);
            var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;

            commandArgs.TextEditorService.ViewModelApi.MoveCursor(
                new KeymapArgs
                {
                    Key = KeyboardKeyFacts.MovementKeys.END,
                    ShiftKey = commandArgs.ShiftKey
                },
		        editContext,
		        modelModifier,
		        viewModelModifier,
		        cursorModifierBag);
		    return Task.CompletedTask;
        };
    }

    public static Func<ITextEditorEditContext, Task> MoveCursorStartCurrentLineFactory(
        ResourceUri modelResourceUri,
        Key<TextEditorViewModel> viewModelKey,
        TextEditorCommandArgs commandArgs)
    {
        return (ITextEditorEditContext editContext) =>
        {
            var modelModifier = editContext.GetModelModifier(modelResourceUri);
            var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);
            var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return Task.CompletedTask;

            commandArgs.TextEditorService.ViewModelApi.MoveCursor(
                new KeymapArgs
                {
                    Key = KeyboardKeyFacts.MovementKeys.HOME,
                    ShiftKey = commandArgs.ShiftKey
                },
		        editContext,
		        modelModifier,
		        viewModelModifier,
		        cursorModifierBag);
		    return Task.CompletedTask;
        };
    }

    public static bool TryParse(TextEditorKeymapVim textEditorKeymapVim,
        ImmutableArray<VimGrammarToken> sentenceSnapshotList,
        int indexInSentence,
        KeymapArgs keymapArgument,
        bool hasTextSelection,
        out TextEditorCommand? textEditorCommand)
    {
        var currentToken = sentenceSnapshotList[indexInSentence];

        var shiftKey = textEditorKeymapVim.ActiveVimMode == VimMode.Visual ||
            textEditorKeymapVim.ActiveVimMode == VimMode.VisualLine;

        if (!currentToken.KeymapArgument.CtrlKey)
        {
            switch (currentToken.KeymapArgument.Code)
            {
                case "KeyW":
                    textEditorCommand = TextEditorCommandVimFacts.Motions.WordCommand;
                    return true;
                case "KeyE":
                    textEditorCommand = TextEditorCommandVimFacts.Motions.EndCommand;
                    return true;
                case "KeyB":
                    textEditorCommand = TextEditorCommandVimFacts.Motions.BackCommand;
                    return true;
                case "KeyH":
                    {
                        // Move the cursor 1 column to the left
                        textEditorCommand = new TextEditorCommand(
                            "Vim::h", "vim_h", false, true, TextEditKind.None, null,
                            interfaceCommandArgs =>
                            {
                                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
                                commandArgs.ShiftKey = shiftKey;

                                commandArgs.TextEditorService.PostUnique(
                                    nameof(MoveCursorOneColumnLeftFactory),
                                    MoveCursorOneColumnLeftFactory(
                                        commandArgs.ModelResourceUri,
                                        commandArgs.ViewModelKey,
                                        commandArgs));
								return Task.CompletedTask;
                            });

                        return true;
                    }
                case "KeyJ":
                    {
                        // Move the cursor 1 row down
                        textEditorCommand = new TextEditorCommand(
                            "Vim::j", "vim_j", false, true, TextEditKind.None, null,
                            interfaceCommandArgs =>
                            {
                                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
                                commandArgs.ShiftKey = shiftKey;

                                commandArgs.TextEditorService.PostUnique(
                                    nameof(MoveCursorOneRowDownFactory),
                                    MoveCursorOneRowDownFactory(
                                        commandArgs.ModelResourceUri,
                                        commandArgs.ViewModelKey,
                                        commandArgs));
								return Task.CompletedTask;
                            });

                        return true;
                    }
                case "KeyK":
                    {
                        // Move the cursor 1 row up 
                        textEditorCommand = new TextEditorCommand(
                            "Vim::k", "vim_k", false, true, TextEditKind.None, null,
                            interfaceCommandArgs =>
                            {
                                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
                                commandArgs.ShiftKey = shiftKey;

                                commandArgs.TextEditorService.PostUnique(
                                    nameof(MoveCursorOneRowUpFactory),
                                    MoveCursorOneRowUpFactory(
                                        commandArgs.ModelResourceUri,
                                        commandArgs.ViewModelKey,
                                        commandArgs));
								return Task.CompletedTask;
                            });

                        return true;
                    }
                case "KeyL":
                    {
                        // Move the cursor 1 column to the right
                        textEditorCommand = new TextEditorCommand(
                            "Vim::l", "vim_l", false, true, TextEditKind.None, null,
                            interfaceCommandArgs =>
                            {
                                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
                                commandArgs.ShiftKey = shiftKey;

                                commandArgs.TextEditorService.PostUnique(
                                    nameof(MoveCursorOneColumnRightFactory),
                                    MoveCursorOneColumnRightFactory(
                                        commandArgs.ModelResourceUri,
                                        commandArgs.ViewModelKey,
                                        commandArgs));
								return Task.CompletedTask;
                            });

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
                        textEditorCommand = new TextEditorCommand(
                            "Vim::$", "vim_$", false, true, TextEditKind.None, null,
                            interfaceCommandArgs =>
                            {
                                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
                                commandArgs.ShiftKey = shiftKey;

                                commandArgs.TextEditorService.PostUnique(
                                    nameof(MoveCursorEndCurrentLineFactory),
                                    MoveCursorEndCurrentLineFactory(
                                        commandArgs.ModelResourceUri,
                                        commandArgs.ViewModelKey,
                                        commandArgs));
								return Task.CompletedTask;
                            });

                        return true;
                    }
                case "Digit0":
                    {
                        // Move the cursor to the start of the current line.
                        textEditorCommand = new TextEditorCommand(
                            "Vim::0", "vim_0", false, true, TextEditKind.None, null,
                            interfaceCommandArgs =>
                            {
                                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
                                commandArgs.ShiftKey = shiftKey;

                                commandArgs.TextEditorService.PostUnique(
                                    nameof(MoveCursorStartCurrentLineFactory),
                                    MoveCursorStartCurrentLineFactory(
                                        commandArgs.ModelResourceUri,
                                        commandArgs.ViewModelKey,
                                        commandArgs));
								return Task.CompletedTask;
                            });

                        return true;
                    }
            }
        }

        textEditorCommand = TextEditorCommandDefaultFacts.DoNothingDiscard;
        return true;
    }
}