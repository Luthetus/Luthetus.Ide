using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models.Vims;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Options.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models.Defaults;
using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.Exceptions;

namespace Luthetus.TextEditor.RazorLib.Keymaps.Models.Vims;

public class TextEditorKeymapVim : Keymap, ITextEditorKeymap
{
    public TextEditorKeymapVim()
        : base(new Key<Keymap>(Guid.Parse("d2122a7a-5a88-4d31-af20-5486a36e9c0c")),
               "Vim")
    {
        AddVimMotionToLayer(TextEditorKeymapVimFacts.NormalLayer.Key);
        AddVimMotionToLayer(TextEditorKeymapVimFacts.VisualLayer.Key);
        AddVimMotionToLayer(TextEditorKeymapVimFacts.VisualLineLayer.Key);

        AddInsertLayer();
        AddCommandLayer();
        AddMiscLayer();
    }

    private readonly TextEditorKeymapDefault _textEditorKeymapDefault = new();

    public VimMode ActiveVimMode { get; set; } = VimMode.Normal;
    public VimSentence VimSentence { get; } = new();

    public Key<KeymapLayer> GetLayer(bool hasSelection)
    {
        switch (ActiveVimMode)
        {
            case VimMode.Normal:
                return TextEditorKeymapVimFacts.NormalLayer.Key;
            case VimMode.Insert:
                return TextEditorKeymapVimFacts.InsertLayer.Key;
            case VimMode.Visual:
                return TextEditorKeymapVimFacts.VisualLayer.Key;
            case VimMode.VisualLine:
                return TextEditorKeymapVimFacts.VisualLineLayer.Key;
            case VimMode.Command:
                return TextEditorKeymapVimFacts.CommandLayer.Key;
            default:
                throw new LuthetusTextEditorException($"The {nameof(ActiveVimMode)}: '{ActiveVimMode}' was not recognized.");
        }
    }

    public string GetCursorCssClassString()
    {
        switch (ActiveVimMode)
        {
            case VimMode.Normal:
            case VimMode.Visual:
            case VimMode.VisualLine:
                return TextCursorKindFacts.BlockCssClassString;
            default:
                return string.Empty;
        }
    }

    public string GetCursorCssStyleString(
        TextEditorModel textEditorModel,
        TextEditorViewModel textEditorViewModel,
        TextEditorOptions textEditorOptions)
    {
        var characterWidthInPixels = textEditorViewModel.CharAndLineMeasurements.CharacterWidth;

        switch (ActiveVimMode)
        {
            case VimMode.Normal:
            case VimMode.Visual:
            case VimMode.VisualLine:
                {
                    var characterWidthInPixelsInvariantCulture = characterWidthInPixels.ToCssValue();
                    return $"width: {characterWidthInPixelsInvariantCulture}px;";
                }
        }

        return string.Empty;
    }

    private void AddVimMotionToLayer(Key<KeymapLayer> keymapLayerKey)
    {
        // "Escape"
        {
            Map.Add(new KeymapArgument("Escape", "Escape")
            {
                LayerKey = keymapLayerKey
            }, TextEditorCommandDefaultFacts.ClearTextSelection);
        }

        // "i"
        {
            var commandDisplayName = "Vim::i";

            var command = new TextEditorCommand(
                commandDisplayName, "vim-i", false, true, TextEditKind.None, null,
                interfaceCommandArgs =>
                {
                    var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
                    
                    var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
		            var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
		            var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
		            var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);

                    ActiveVimMode = VimMode.Insert;
	                _ = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
					return Task.CompletedTask;
                });

            Map.Add(new KeymapArgument("i", "KeyI")
            {
                LayerKey = keymapLayerKey
            }, command);
        }

        // "v"
        {
            var commandDisplayName = "Vim::v";

            var command = new TextEditorCommand(
                commandDisplayName, "vim-v", false, true, TextEditKind.None, null,
                interfaceCommandArgs =>
                {
                    var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
                    
                    var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
		            var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
		            var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
		            var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);

                    if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                        return Task.CompletedTask;

                    if (ActiveVimMode == VimMode.Visual)
                    {
                        ActiveVimMode = VimMode.Normal;

                        TextEditorCommandDefaultFacts.ClearTextSelection.CommandFunc.Invoke(interfaceCommandArgs);
                        return Task.CompletedTask;
                    }

                    ActiveVimMode = VimMode.Visual;

                    var positionIndex = modelModifier.GetPositionIndex(
                        primaryCursorModifier.LineIndex,
                        primaryCursorModifier.ColumnIndex);

                    primaryCursorModifier.SelectionAnchorPositionIndex = positionIndex;
                    primaryCursorModifier.SelectionEndingPositionIndex = positionIndex + 1;

                    return Task.CompletedTask;
                });

            Map.Add(new KeymapArgument("v", "KeyV")
            {
                LayerKey = keymapLayerKey
            }, command);
        }

        // "V"
        {
            var commandDisplayName = "Vim::V";

            var command = new TextEditorCommand(
                commandDisplayName, "vim-V", false, true, TextEditKind.None, null,
                interfaceCommandArgs =>
                {
                    var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
                    
                    var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
		            var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
		            var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
		            var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);

                    if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                        return Task.CompletedTask;

                    if (ActiveVimMode == VimMode.VisualLine)
                    {
                        ActiveVimMode = VimMode.Normal;

                        TextEditorCommandDefaultFacts.ClearTextSelection.CommandFunc.Invoke(interfaceCommandArgs);
                        return Task.CompletedTask;
                    }

                    ActiveVimMode = VimMode.VisualLine;

                    var startOfRowPositionIndexInclusive = modelModifier.GetPositionIndex(
                        primaryCursorModifier.LineIndex,
                        0);

                    primaryCursorModifier.SelectionAnchorPositionIndex = startOfRowPositionIndexInclusive;

                    var endOfRowPositionIndexExclusive = modelModifier.LineEndList[primaryCursorModifier.LineIndex]
                        .EndPositionIndexExclusive;

                    primaryCursorModifier.SelectionEndingPositionIndex = endOfRowPositionIndexExclusive;
                    return Task.CompletedTask;
                });

            Map.Add(new KeymapArgument("V", "KeyV")
            {
                ShiftKey = true,
                LayerKey = keymapLayerKey
            }, command);
        }

        // ":"
        {
            var commandDisplayName = "Vim:::";

            var command = new TextEditorCommand(
                commandDisplayName, "vim-:", false, true, TextEditKind.None, null,
                interfaceCommandArgs =>
                {
                    var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
                    
                    var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
		            var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
		            var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
		            var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);

                    if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                        return Task.CompletedTask;

                    viewModelModifier.ViewModel = viewModelModifier.ViewModel with
                    {
                        ShowCommandBar = true
                    };

                    return Task.CompletedTask;
                });

            Map.Add(new KeymapArgument(":", "Semicolon")
            {
                ShiftKey = true,
                LayerKey = keymapLayerKey
            }, command);
        }

        // "u"
        {
            var commandDisplayName = "Vim::u";

            var command = new TextEditorCommand(
                commandDisplayName, "vim-u", false, true, TextEditKind.None, null,
                interfaceCommandArgs =>
                {
                    var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
                    
                    var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
		            var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
		            var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
		            var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);

                    if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                        return Task.CompletedTask;

                    commandArgs.TextEditorService.ModelApi.UndoEdit(
                    	commandArgs.EditContext,
                    	modelModifier);

                    commandArgs.EditContext.TextEditorService.ModelApi.ApplySyntaxHighlighting(
                        commandArgs.EditContext,
                    	modelModifier);
                    return Task.CompletedTask;
                });

            Map.Add(new KeymapArgument("u", "KeyU")
            {
                LayerKey = keymapLayerKey
            }, command);
        }

        // "r"
        {
            var commandDisplayName = "Vim::r";

            var command = new TextEditorCommand(
                commandDisplayName, "vim-r", false, true, TextEditKind.None, null,
                interfaceCommandArgs =>
                {
                    var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
                    
                    var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
		            var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
		            var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
		            var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);

                    if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                        return Task.CompletedTask;

                    commandArgs.TextEditorService.ModelApi.RedoEdit(
                    	commandArgs.EditContext,
                    	modelModifier);

                    commandArgs.EditContext.TextEditorService.ModelApi.ApplySyntaxHighlighting(
                        commandArgs.EditContext,
                    	modelModifier);
                    return Task.CompletedTask;
                });

            Map.Add(new KeymapArgument("r", "KeyR")
            {
                CtrlKey = true,
                LayerKey = keymapLayerKey
            }, command);
        }

        // "o"
        {
            var commandDisplayName = "Vim::o";

            var command = new TextEditorCommand(
                commandDisplayName, "vim-o", false, true, TextEditKind.None, null,
                TextEditorCommandVimFacts.Verbs.NewLineBelowCommand.CommandFunc);

            Map.Add(new KeymapArgument("o", "KeyO")
            {
                LayerKey = keymapLayerKey
            }, command);
        }

        // "O"
        {
            var commandDisplayName = "Vim::O";

            var command = new TextEditorCommand(
                commandDisplayName, "vim-O", false, true, TextEditKind.None, null,
                TextEditorCommandVimFacts.Verbs.NewLineAboveCommand.CommandFunc);

            Map.Add(new KeymapArgument("O", "KeyO")
            {
                ShiftKey = true,
                LayerKey = keymapLayerKey
            }, command);
        }

        // "e"
        {
            var commandDisplayName = "Vim::e";

            var command = new TextEditorCommand(
                commandDisplayName, "vim-e", false, false, TextEditKind.None, null,
                TextEditorCommandDefaultFacts.ScrollLineDown.CommandFunc);

            Map.Add(new KeymapArgument("e", "KeyE")
            {
                CtrlKey = true,
                LayerKey = keymapLayerKey
            }, command);
        }

        // "y"
        {
            var commandDisplayName = "Vim::y";

            var command = new TextEditorCommand(
                commandDisplayName, "vim-y", false, false, TextEditKind.None, null,
                TextEditorCommandDefaultFacts.ScrollLineUp.CommandFunc);

            Map.Add(new KeymapArgument("y", "KeyY")
            {
                CtrlKey = true,
                LayerKey = keymapLayerKey
            }, command);
        }

        // default case
        {
            // 48 to 57 provides digits (both sides inclusive) (ASCII)
            for (int i = 48; i <= 57; i++)
            {
                var character = (char)i;

                var keymapArgument = new KeymapArgument($"{character}", $"Digit{character}")
                {
                    LayerKey = keymapLayerKey
                };

                var command = BuildCommandTextEditor(keymapArgument, $"Vim::{character}", $"vim-{character}");

                // We only want to fill the remaining empty keybinds.
                _ = Map.TryAdd(keymapArgument, command);
            }

            // 65 to 90 provides capital letters (both sides inclusive) (ASCII)
            for (int i = 65; i <= 90; i++)
            {
                var character = (char)i;

                var keymapArgument = new KeymapArgument($"{character}", $"Key{character}")
                {
                    ShiftKey = true,
                    LayerKey = keymapLayerKey
                };

                var command = BuildCommandTextEditor(keymapArgument, $"Vim::{character}", $"vim-{character}");

                // We only want to fill the remaining empty keybinds.
                _ = Map.TryAdd(keymapArgument, command);
            }

            // 97 to 122 provides lowercase letters (both sides inclusive) (ASCII)
            for (int i = 97; i <= 122; i++)
            {
                var character = (char)i;

                var keymapArgument = new KeymapArgument($"{character}", $"Key{char.ToUpperInvariant(character)}")
                {
                    LayerKey = keymapLayerKey
                };

                var command = BuildCommandTextEditor(keymapArgument, $"Vim::{character}", $"vim-{character}");

                _ = Map.TryAdd(keymapArgument, command);
            }

            // ` = Backquote 
            {
                var keymapArgument = new KeymapArgument("`", "Backquote") { LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::`", $"vim-`");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // ~ = Shift + Backquote
            {
                var keymapArgument = new KeymapArgument("~", "Backquote") { ShiftKey = true, LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::~", $"vim-~");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // ! = Shift + Digit1
            {
                var keymapArgument = new KeymapArgument("!", "Digit1") { ShiftKey = true, LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::!", $"vim-!");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // @ = Shift + Digit2
            {
                var keymapArgument = new KeymapArgument("@", "Digit2") { ShiftKey = true, LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::@", $"vim-@");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // # = Shift + Digit3
            {
                var keymapArgument = new KeymapArgument("#", "Digit3") { ShiftKey = true, LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::#", $"vim-#");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // $ = Shift + Digit4
            {
                var keymapArgument = new KeymapArgument("$", "Digit4") { ShiftKey = true, LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::$", $"vim-$");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // % = Shift + Digit5
            {
                var keymapArgument = new KeymapArgument("%", "Digit5") { ShiftKey = true, LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::%", $"vim-%");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // ^ = Shift + Digit6
            {
                var keymapArgument = new KeymapArgument("^", "Digit6") { ShiftKey = true, LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::^", $"vim-^");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // & = Shift + Digit7
            {
                var keymapArgument = new KeymapArgument("&", "Digit7") { ShiftKey = true, LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::&", $"vim-&");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // * = Shift + Digit8
            {
                var keymapArgument = new KeymapArgument("*", "Digit8") { ShiftKey = true, LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::*", $"vim-*");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // ( = Shift + Digit9
            {
                var keymapArgument = new KeymapArgument("(", "Digit9") { ShiftKey = true, LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::(", $"vim-(");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // ) = Shift + Digit0
            {
                var keymapArgument = new KeymapArgument(")", "Digit0") { ShiftKey = true, LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::)", $"vim-)");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // - = Minus
            {
                var keymapArgument = new KeymapArgument("-", "Minus") { LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::-", $"vim--");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // _ = Shift + Minus
            {
                var keymapArgument = new KeymapArgument("_", "Minus") { ShiftKey = true, LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::_", $"vim-_");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // = = Equal
            {
                var keymapArgument = new KeymapArgument("=", "Equal") { LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::=", $"vim-=");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // + = Shift + Equal
            {
                var keymapArgument = new KeymapArgument("+", "Equal") { ShiftKey = true, LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::+", $"vim-+");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // [ = BracketLeft
            {
                var keymapArgument = new KeymapArgument("[", "BracketLeft") { LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::[", $"vim-[");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // { = Shift + BracketLeft
            {
                var keymapArgument = new KeymapArgument("{", "BracketLeft") { ShiftKey = true, LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::{{", $"vim-}}");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // ] = BracketRight
            {
                var keymapArgument = new KeymapArgument("]", "BracketRight") { LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::]", $"vim-]");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // } = Shift + BracketRight
            {
                var keymapArgument = new KeymapArgument("}", "BracketRight") { ShiftKey = true, LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::}}", $"vim-}}");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // \ = Backslash
            {
                var keymapArgument = new KeymapArgument("\\", "Backslash") { LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::\\", $"vim-\\");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // | = Shift + Backslash
            {
                var keymapArgument = new KeymapArgument("|", "Backslash") { ShiftKey = true, LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::|", $"vim-|");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // ; = Semicolon
            {
                var keymapArgument = new KeymapArgument(";", "Semicolon") { LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::;", $"vim-;");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // : = Shift + Semicolon
            {
                var keymapArgument = new KeymapArgument(":", "Semicolon") { ShiftKey = true, LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim:::", $"vim-:");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // ' = Quote
            {
                var keymapArgument = new KeymapArgument("'", "Quote") { LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::'", $"vim-'");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // " = Shift + Quote
            {
                var keymapArgument = new KeymapArgument("\"", "Quote") { ShiftKey = true, LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::\"", $"vim-\"");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // , = Comma
            {
                var keymapArgument = new KeymapArgument(",", "Comma") { LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::,", $"vim-,");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // < = Shift + Comma
            {
                var keymapArgument = new KeymapArgument("<", "Comma") { ShiftKey = true, LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::<", $"vim-<");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // . = Period
            {
                var keymapArgument = new KeymapArgument(".", "Period") { LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::.", $"vim-.");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // > = Shift + Period
            {
                var keymapArgument = new KeymapArgument(">", "Period") { ShiftKey = true, LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::>", $"vim->");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // / = Slash
            {
                var keymapArgument = new KeymapArgument("/", "Slash") { LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::/", $"vim-/");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // ? = Shift + Slash
            {
                var keymapArgument = new KeymapArgument("?", "Slash") { ShiftKey = true, LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::?", $"vim-?");
                _ = Map.TryAdd(keymapArgument, command);
            }
        }
    }

    private void AddInsertLayer()
    {
        var commandDisplayName = "Vim::Escape";

        var escapeCommand = new TextEditorCommand(
            commandDisplayName, "vim-Escape", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
                    
                var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
	            var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
	            var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
	            var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);

                ActiveVimMode = VimMode.Normal;
                _ = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
                return Task.CompletedTask;
            });

        Map.Add(new KeymapArgument("Escape", "Escape")
        {
            LayerKey = TextEditorKeymapVimFacts.InsertLayer.Key
        }, escapeCommand);
    }

    private void AddCommandLayer()
    {
        // throw new NotImplementedException();
    }

    private void AddMiscLayer()
    {
        BuildModifiedMovementCommandsFromUnmodified(new KeymapArgument("ArrowLeft", "ArrowLeft")
        {
            LayerKey = TextEditorKeymapVimFacts.InsertLayer.Key,
        }, "ArrowLeft");

        BuildModifiedMovementCommandsFromUnmodified(new KeymapArgument("ArrowDown", "ArrowDown")
        {
            LayerKey = TextEditorKeymapVimFacts.InsertLayer.Key,
        }, "ArrowDown");
        BuildModifiedMovementCommandsFromUnmodified(new KeymapArgument("ArrowUp", "ArrowUp")
        {
            LayerKey = TextEditorKeymapVimFacts.InsertLayer.Key,
        }, "ArrowUp");
        BuildModifiedMovementCommandsFromUnmodified(new KeymapArgument("ArrowRight", "ArrowRight")
        {
            LayerKey = TextEditorKeymapVimFacts.InsertLayer.Key,
        }, "ArrowRight");
        BuildModifiedMovementCommandsFromUnmodified(new KeymapArgument("Home", "Home")
        {
            LayerKey = TextEditorKeymapVimFacts.InsertLayer.Key,
        }, "Home");
        BuildModifiedMovementCommandsFromUnmodified(new KeymapArgument("End", "End")
        {
            LayerKey = TextEditorKeymapVimFacts.InsertLayer.Key,
        }, "End");

        // PageDown
        {
            var commandDisplayName = "Vim::PageDown";

            var escapeCommand = new TextEditorCommand(
                commandDisplayName, "vim-PageDown", false, true, TextEditKind.None, null,
                TextEditorCommandDefaultFacts.ScrollPageDown.CommandFunc);

            Map.Add(new KeymapArgument("PageDown", "PageDown")
            {
                LayerKey = TextEditorKeymapVimFacts.InsertLayer.Key
            }, escapeCommand);
        }

        // PageUp
        {
            var commandDisplayName = "Vim::PageUp";

            var escapeCommand = new TextEditorCommand(
                commandDisplayName, "vim-PageUp", false, true, TextEditKind.None, null,
                TextEditorCommandDefaultFacts.ScrollPageUp.CommandFunc);

            Map.Add(new KeymapArgument("PageUp", "PageUp")
            {
                LayerKey = TextEditorKeymapVimFacts.InsertLayer.Key
            }, escapeCommand);
        }
    }

    private TextEditorCommand BuildCommandTextEditor(KeymapArgument keymapArgument, string displayName, string internalIdentifier)
    {
        return new TextEditorCommand(
            displayName, internalIdentifier, false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
                
                var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
	            var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
	            var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
	            var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
	                return Task.CompletedTask;

                var success = VimSentence.TryLex(this, keymapArgument, TextEditorSelectionHelper.HasSelectedText(primaryCursorModifier), out var lexCommand);

                if (success && lexCommand is not null)
                    return lexCommand.CommandFunc.Invoke(commandArgs);

                return Task.CompletedTask;
            });
    }

    private TextEditorCommand BuildMovementCommand(KeymapArgument keymapArgument, string key)
    {
        var commandDisplayName = $"Vim::{key}";

        return new TextEditorCommand(
            commandDisplayName, $"vim-{key}", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
                    
                var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
	            var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
	            var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
	            var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (ActiveVimMode == VimMode.Visual || ActiveVimMode == VimMode.VisualLine)
                {
                    keymapArgument = keymapArgument with
                    {
                        ShiftKey = true
                    };
                }

                TextEditorCommand? modifiedCommand = null;

                modifiedCommand = (TextEditorCommand?)_textEditorKeymapDefault.Map[keymapArgument];

                if (modifiedCommand is null)
                {
                    var keyboardEventArgs = new KeyboardEventArgs
                    {
                        Key = key,
                        ShiftKey = keymapArgument.ShiftKey,
                        CtrlKey = keymapArgument.CtrlKey
                    };

                    modifiedCommand = new TextEditorCommand(
                        "MoveCursor", "MoveCursor", false, true, TextEditKind.None, null,
                        interfaceCommandArgs =>
                        {
                            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
                            
                            var modelModifier = commandArgs.EditContext.GetModelModifier(commandArgs.ModelResourceUri);
				            var viewModelModifier = commandArgs.EditContext.GetViewModelModifier(commandArgs.ViewModelKey);
				            var cursorModifierBag = commandArgs.EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
				            var primaryCursorModifier = commandArgs.EditContext.GetPrimaryCursorModifier(cursorModifierBag);
				
				            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
				                return Task.CompletedTask;

                            commandArgs.TextEditorService.ViewModelApi.MoveCursor(
                            	keyboardEventArgs,
						        commandArgs.EditContext,
						        modelModifier,
						        viewModelModifier,
						        cursorModifierBag);
                            return Task.CompletedTask;
                        });

                    TextEditorCommand finalCommand = modifiedCommand;

                    if (ActiveVimMode == VimMode.Visual)
                        finalCommand = TextEditorCommandVimFacts.Motions.GetVisualFactory(modifiedCommand, $"{nameof(TextEditorKeymapVim)}");

                    if (ActiveVimMode == VimMode.VisualLine)
                        finalCommand = TextEditorCommandVimFacts.Motions.GetVisualLineFactory(modifiedCommand, $"{nameof(TextEditorKeymapVim)}");

                    return finalCommand.CommandFunc.Invoke(commandArgs);
                }

                return Task.CompletedTask;
            });
    }

    private void BuildModifiedMovementCommandsFromUnmodified(KeymapArgument keymapArgument, string key)
    {
        // Unmodified
        {
            var unmodifiedKeymapArgument = keymapArgument;

            var arrowLeftCommand = BuildMovementCommand(unmodifiedKeymapArgument, key);
            Map.Add(unmodifiedKeymapArgument, arrowLeftCommand);
        }

        // Shift
        {
            var shiftKeymapArgument = keymapArgument with
            {
                ShiftKey = true
            };

            var arrowLeftCommand = BuildMovementCommand(shiftKeymapArgument, key);
            Map.Add(shiftKeymapArgument, arrowLeftCommand);
        }

        // Ctrl
        {
            var ctrlKeymapArgument = keymapArgument with
            {
                CtrlKey = true
            };

            var arrowLeftCommand = BuildMovementCommand(ctrlKeymapArgument, key);
            Map.Add(ctrlKeymapArgument, arrowLeftCommand);
        }

        // Shift + Ctrl
        {
            var shiftCtrlKeymapArgument = keymapArgument with
            {
                ShiftKey = true,
                CtrlKey = true
            };

            var arrowLeftCommand = BuildMovementCommand(shiftCtrlKeymapArgument, key);
            Map.Add(shiftCtrlKeymapArgument, arrowLeftCommand);
        }
    }

	public bool TryMap(KeyboardEventArgs keyboardEventArgs, KeymapArgument keymapArgument, TextEditorComponentData componentData, out CommandNoType? command)
	{
		return Map.TryGetValue(keymapArgument, out command);
	}
}
