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
    private readonly object _syncRoot = new();
    private readonly Dictionary<KeymapArgs, CommandNoType> _map = new();

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
            _map.Add(new KeymapArgs()
            {
                Key = "Escape",
                Code = "Escape",
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
					return ValueTask.CompletedTask;
                });

            _map.Add(new KeymapArgs()
            {
                Key = "i",
                Code = "KeyI",
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
                        return ValueTask.CompletedTask;

                    if (ActiveVimMode == VimMode.Visual)
                    {
                        ActiveVimMode = VimMode.Normal;

                        TextEditorCommandDefaultFacts.ClearTextSelection.CommandFunc.Invoke(interfaceCommandArgs);
                        return ValueTask.CompletedTask;
                    }

                    ActiveVimMode = VimMode.Visual;

                    var positionIndex = modelModifier.GetPositionIndex(
                        primaryCursorModifier.LineIndex,
                        primaryCursorModifier.ColumnIndex);

                    primaryCursorModifier.SelectionAnchorPositionIndex = positionIndex;
                    primaryCursorModifier.SelectionEndingPositionIndex = positionIndex + 1;

                    return ValueTask.CompletedTask;
                });

            _map.Add(new KeymapArgs()
            {
                Key = "v",
                Code = "KeyV",
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
                        return ValueTask.CompletedTask;

                    if (ActiveVimMode == VimMode.VisualLine)
                    {
                        ActiveVimMode = VimMode.Normal;

                        TextEditorCommandDefaultFacts.ClearTextSelection.CommandFunc.Invoke(interfaceCommandArgs);
                        return ValueTask.CompletedTask;
                    }

                    ActiveVimMode = VimMode.VisualLine;

                    var startOfRowPositionIndexInclusive = modelModifier.GetPositionIndex(
                        primaryCursorModifier.LineIndex,
                        0);

                    primaryCursorModifier.SelectionAnchorPositionIndex = startOfRowPositionIndexInclusive;

                    var endOfRowPositionIndexExclusive = modelModifier.LineEndList[primaryCursorModifier.LineIndex]
                        .EndPositionIndexExclusive;

                    primaryCursorModifier.SelectionEndingPositionIndex = endOfRowPositionIndexExclusive;
                    return ValueTask.CompletedTask;
                });

            _map.Add(new KeymapArgs()
            {
                Key = "V",
                Code = "KeyV",
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
                        return ValueTask.CompletedTask;

                    viewModelModifier.ViewModel = viewModelModifier.ViewModel with
                    {
                        ShowCommandBar = true
                    };

                    return ValueTask.CompletedTask;
                });

            _map.Add(new KeymapArgs()
            {
                Key = ":",
                Code = "Semicolon",
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
                        return ValueTask.CompletedTask;

                    commandArgs.TextEditorService.ModelApi.UndoEdit(
                    	commandArgs.EditContext,
                    	modelModifier);

                    commandArgs.EditContext.TextEditorService.ModelApi.ApplySyntaxHighlighting(
                        commandArgs.EditContext,
                    	modelModifier);
                    return ValueTask.CompletedTask;
                });

            _map.Add(new KeymapArgs()
            {
                Key = "u",
                Code = "KeyU",
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
                        return ValueTask.CompletedTask;

                    commandArgs.TextEditorService.ModelApi.RedoEdit(
                    	commandArgs.EditContext,
                    	modelModifier);

                    commandArgs.EditContext.TextEditorService.ModelApi.ApplySyntaxHighlighting(
                        commandArgs.EditContext,
                    	modelModifier);
                    return ValueTask.CompletedTask;
                });

            _map.Add(new KeymapArgs()
            {
                Key = "r",
                Code = "KeyR",
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

            _map.Add(new KeymapArgs()
            {
                Key = "o",
                Code = "KeyO",
                LayerKey = keymapLayerKey
            }, command);
        }

        // "O"
        {
            var commandDisplayName = "Vim::O";

            var command = new TextEditorCommand(
                commandDisplayName, "vim-O", false, true, TextEditKind.None, null,
                TextEditorCommandVimFacts.Verbs.NewLineAboveCommand.CommandFunc);

            _map.Add(new KeymapArgs()
            {
                Key = "O",
                Code = "KeyO",
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

            _map.Add(new KeymapArgs()
            {
                Key = "e",
                Code = "KeyE",
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

            _map.Add(new KeymapArgs()
            {
                Key = "y",
                Code = "KeyY",
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

                var keymapArgument = new KeymapArgs()
                {
                    Key = $"{character}",
                    Code = $"Digit{character}",
                    LayerKey = keymapLayerKey
                };

                var command = BuildCommandTextEditor(keymapArgument, $"Vim::{character}", $"vim-{character}");

                // We only want to fill the remaining empty keybinds.
                _ = _map.TryAdd(keymapArgument, command);
            }

            // 65 to 90 provides capital letters (both sides inclusive) (ASCII)
            for (int i = 65; i <= 90; i++)
            {
                var character = (char)i;

                var keymapArgument = new KeymapArgs()
                {
                    Key = $"{character}",
                    Code = $"Key{character}",
                    ShiftKey = true,
                    LayerKey = keymapLayerKey
                };

                var command = BuildCommandTextEditor(keymapArgument, $"Vim::{character}", $"vim-{character}");

                // We only want to fill the remaining empty keybinds.
                _ = _map.TryAdd(keymapArgument, command);
            }

            // 97 to 122 provides lowercase letters (both sides inclusive) (ASCII)
            for (int i = 97; i <= 122; i++)
            {
                var character = (char)i;

                var keymapArgument = new KeymapArgs()
                {
                    Key = $"{character}",
                    Code = $"Key{char.ToUpperInvariant(character)}",
                    LayerKey = keymapLayerKey
                };

                var command = BuildCommandTextEditor(keymapArgument, $"Vim::{character}", $"vim-{character}");

                _ = _map.TryAdd(keymapArgument, command);
            }

            // ` = Backquote 
            {
                var keymapArgument = new KeymapArgs()
                {
                    Key = "`",
                    Code = "Backquote",
                    LayerKey = keymapLayerKey
                };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::`", $"vim-`");
                _ = _map.TryAdd(keymapArgument, command);
            }
            // ~ = Shift + Backquote
            {
                var keymapArgument = new KeymapArgs()
                {
                    Key = "~",
                    Code = "Backquote",
                    ShiftKey = true,
                    LayerKey = keymapLayerKey
                };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::~", $"vim-~");
                _ = _map.TryAdd(keymapArgument, command);
            }
            // ! = Shift + Digit1
            {
                var keymapArgument = new KeymapArgs()
                {
                    Key = "!",
                    Code = "Digit1",
                    ShiftKey = true,
                    LayerKey = keymapLayerKey
                };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::!", $"vim-!");
                _ = _map.TryAdd(keymapArgument, command);
            }
            // @ = Shift + Digit2
            {
                var keymapArgument = new KeymapArgs()
                {
                    Key = "@",
                    Code = "Digit2",
                    ShiftKey = true,
                    LayerKey = keymapLayerKey
                };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::@", $"vim-@");
                _ = _map.TryAdd(keymapArgument, command);
            }
            // # = Shift + Digit3
            {
                var keymapArgument = new KeymapArgs()
                {
                    Key = "#",
                    Code = "Digit3",
                    ShiftKey = true,
                    LayerKey = keymapLayerKey
                };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::#", $"vim-#");
                _ = _map.TryAdd(keymapArgument, command);
            }
            // $ = Shift + Digit4
            {
                var keymapArgument = new KeymapArgs()
                {
                    Key = "$",
                    Code = "Digit4",
                    ShiftKey = true,
                    LayerKey = keymapLayerKey
                };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::$", $"vim-$");
                _ = _map.TryAdd(keymapArgument, command);
            }
            // % = Shift + Digit5
            {
                var keymapArgument = new KeymapArgs()
                {
                    Key = "%",
                    Code = "Digit5",
                    ShiftKey = true,
                    LayerKey = keymapLayerKey
                };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::%", $"vim-%");
                _ = _map.TryAdd(keymapArgument, command);
            }
            // ^ = Shift + Digit6
            {
                var keymapArgument = new KeymapArgs()
                {
                    Key = "^",
                    Code = "Digit6",
                    ShiftKey = true,
                    LayerKey = keymapLayerKey
                };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::^", $"vim-^");
                _ = _map.TryAdd(keymapArgument, command);
            }
            // & = Shift + Digit7
            {
                var keymapArgument = new KeymapArgs()
                {
                    Key = "&",
                    Code = "Digit7",
                    ShiftKey = true,
                    LayerKey = keymapLayerKey
                };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::&", $"vim-&");
                _ = _map.TryAdd(keymapArgument, command);
            }
            // * = Shift + Digit8
            {
                var keymapArgument = new KeymapArgs()
                {
                    Key = "*",
                    Code = "Digit8",
                    ShiftKey = true,
                    LayerKey = keymapLayerKey
                };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::*", $"vim-*");
                _ = _map.TryAdd(keymapArgument, command);
            }
            // ( = Shift + Digit9
            {
                var keymapArgument = new KeymapArgs()
                {
                    Key = "(",
                    Code = "Digit9",
                    ShiftKey = true,
                    LayerKey = keymapLayerKey
                };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::(", $"vim-(");
                _ = _map.TryAdd(keymapArgument, command);
            }
            // ) = Shift + Digit0
            {
                var keymapArgument = new KeymapArgs()
                {
                    Key = ")",
                    Code = "Digit0",
                    ShiftKey = true,
                    LayerKey = keymapLayerKey
                };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::)", $"vim-)");
                _ = _map.TryAdd(keymapArgument, command);
            }
            // - = Minus
            {
                var keymapArgument = new KeymapArgs()
                {
                    Key = "-",
                    Code = "Minus",
                    LayerKey = keymapLayerKey
                };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::-", $"vim--");
                _ = _map.TryAdd(keymapArgument, command);
            }
            // _ = Shift + Minus
            {
                var keymapArgument = new KeymapArgs()
                {
                    Key = "_",
                    Code = "Minus",
                    ShiftKey = true,
                    LayerKey = keymapLayerKey
                };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::_", $"vim-_");
                _ = _map.TryAdd(keymapArgument, command);
            }
            // = = Equal
            {
                var keymapArgument = new KeymapArgs()
                {
                    Key = "=",
                    Code = "Equal",
                    LayerKey = keymapLayerKey
                };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::=", $"vim-=");
                _ = _map.TryAdd(keymapArgument, command);
            }
            // + = Shift + Equal
            {
                var keymapArgument = new KeymapArgs()
                {
                    Key = "+",
                    Code = "Equal",
                    ShiftKey = true,
                    LayerKey = keymapLayerKey
                };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::+", $"vim-+");
                _ = _map.TryAdd(keymapArgument, command);
            }
            // [ = BracketLeft
            {
                var keymapArgument = new KeymapArgs()
                {
                    Key = "[",
                    Code = "BracketLeft",
                    LayerKey = keymapLayerKey
                };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::[", $"vim-[");
                _ = _map.TryAdd(keymapArgument, command);
            }
            // { = Shift + BracketLeft
            {
                var keymapArgument = new KeymapArgs()
                {
                    Key = "{",
                    Code = "BracketLeft",
                    ShiftKey = true,
                    LayerKey = keymapLayerKey
                };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::{{", $"vim-}}");
                _ = _map.TryAdd(keymapArgument, command);
            }
            // ] = BracketRight
            {
                var keymapArgument = new KeymapArgs()
                {
                    Key = "]",
                    Code = "BracketRight",
                    LayerKey = keymapLayerKey
                };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::]", $"vim-]");
                _ = _map.TryAdd(keymapArgument, command);
            }
            // } = Shift + BracketRight
            {
                var keymapArgument = new KeymapArgs()
                {
                    Key = "}",
                    Code = "BracketRight",
                    ShiftKey = true,
                    LayerKey = keymapLayerKey
                };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::}}", $"vim-}}");
                _ = _map.TryAdd(keymapArgument, command);
            }
            // \ = Backslash
            {
                var keymapArgument = new KeymapArgs()
                {
                    Key = "\\",
                    Code = "Backslash",
                    LayerKey = keymapLayerKey 
                };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::\\", $"vim-\\");
                _ = _map.TryAdd(keymapArgument, command);
            }
            // | = Shift + Backslash
            {
                var keymapArgument = new KeymapArgs()
                {
                    Key = "|",
                    Code = "Backslash",
                    ShiftKey = true,
                    LayerKey = keymapLayerKey
                };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::|", $"vim-|");
                _ = _map.TryAdd(keymapArgument, command);
            }
            // ; = Semicolon
            {
                var keymapArgument = new KeymapArgs()
                {
                    Key = ";",
                    Code = "Semicolon",
                    LayerKey = keymapLayerKey
                };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::;", $"vim-;");
                _ = _map.TryAdd(keymapArgument, command);
            }
            // : = Shift + Semicolon
            {
                var keymapArgument = new KeymapArgs()
                {
                    Key = ":",
                    Code = "Semicolon",
                    ShiftKey = true,
                    LayerKey = keymapLayerKey
                };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim:::", $"vim-:");
                _ = _map.TryAdd(keymapArgument, command);
            }
            // ' = Quote
            {
                var keymapArgument = new KeymapArgs()
                {
                    Key = "'",
                    Code = "Quote",
                    LayerKey = keymapLayerKey
                };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::'", $"vim-'");
                _ = _map.TryAdd(keymapArgument, command);
            }
            // " = Shift + Quote
            {
                var keymapArgument = new KeymapArgs()
                {
                    Key = "\"",
                    Code = "Quote",
                    ShiftKey = true,
                    LayerKey = keymapLayerKey
                };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::\"", $"vim-\"");
                _ = _map.TryAdd(keymapArgument, command);
            }
            // , = Comma
            {
                var keymapArgument = new KeymapArgs()
                {
                    Key = ",",
                    Code = "Comma",
                    LayerKey = keymapLayerKey
                };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::,", $"vim-,");
                _ = _map.TryAdd(keymapArgument, command);
            }
            // < = Shift + Comma
            {
                var keymapArgument = new KeymapArgs()
                {
                    Key = "<",
                    Code = "Comma",
                    ShiftKey = true,
                    LayerKey = keymapLayerKey
                };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::<", $"vim-<");
                _ = _map.TryAdd(keymapArgument, command);
            }
            // . = Period
            {
                var keymapArgument = new KeymapArgs()
                {
                    Key = ".",
                    Code = "Period",
                    LayerKey = keymapLayerKey
                };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::.", $"vim-.");
                _ = _map.TryAdd(keymapArgument, command);
            }
            // > = Shift + Period
            {
                var keymapArgument = new KeymapArgs()
                {
                    Key = ">",
                    Code = "Period",
                    ShiftKey = true,
                    LayerKey = keymapLayerKey
                };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::>", $"vim->");
                _ = _map.TryAdd(keymapArgument, command);
            }
            // / = Slash
            {
                var keymapArgument = new KeymapArgs()
                {
                    Key = "/",
                    Code = "Slash",
                    LayerKey = keymapLayerKey
                };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::/", $"vim-/");
                _ = _map.TryAdd(keymapArgument, command);
            }
            // ? = Shift + Slash
            {
                var keymapArgument = new KeymapArgs()
                {
                    Key = "?",
                    Code = "Slash",
                    ShiftKey = true,
                    LayerKey = keymapLayerKey
                };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::?", $"vim-?");
                _ = _map.TryAdd(keymapArgument, command);
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
                return ValueTask.CompletedTask;
            });

        _map.Add(new KeymapArgs()
        {
            Key = "Escape",
            Code = "Escape",
            LayerKey = TextEditorKeymapVimFacts.InsertLayer.Key
        }, escapeCommand);
    }

    private void AddCommandLayer()
    {
        // throw new NotImplementedException();
    }

    private void AddMiscLayer()
    {
        BuildModifiedMovementCommandsFromUnmodified(new KeymapArgs()
        {
            Key = "ArrowLeft",
            Code = "ArrowLeft",
            LayerKey = TextEditorKeymapVimFacts.InsertLayer.Key,
        }, "ArrowLeft");

        BuildModifiedMovementCommandsFromUnmodified(new KeymapArgs()
        {
            Key = "ArrowDown",
            Code = "ArrowDown",
            LayerKey = TextEditorKeymapVimFacts.InsertLayer.Key,
        }, "ArrowDown");
        BuildModifiedMovementCommandsFromUnmodified(new KeymapArgs()
        {
            Key = "ArrowUp",
            Code = "ArrowUp",
            LayerKey = TextEditorKeymapVimFacts.InsertLayer.Key,
        }, "ArrowUp");
        BuildModifiedMovementCommandsFromUnmodified(new KeymapArgs()
        {
            Key = "ArrowRight",
            Code = "ArrowRight",
            LayerKey = TextEditorKeymapVimFacts.InsertLayer.Key,
        }, "ArrowRight");
        BuildModifiedMovementCommandsFromUnmodified(new KeymapArgs()
        {
            Key = "Home",
            Code = "Home",
            LayerKey = TextEditorKeymapVimFacts.InsertLayer.Key,
        }, "Home");
        BuildModifiedMovementCommandsFromUnmodified(new KeymapArgs()
        {
            Key = "End",
            Code = "End",
            LayerKey = TextEditorKeymapVimFacts.InsertLayer.Key,
        }, "End");

        // PageDown
        {
            var commandDisplayName = "Vim::PageDown";

            var escapeCommand = new TextEditorCommand(
                commandDisplayName, "vim-PageDown", false, true, TextEditKind.None, null,
                TextEditorCommandDefaultFacts.ScrollPageDown.CommandFunc);

            _map.Add(new KeymapArgs()
            {
                Key = "PageDown",
                Code = "PageDown",
                LayerKey = TextEditorKeymapVimFacts.InsertLayer.Key
            }, escapeCommand);
        }

        // PageUp
        {
            var commandDisplayName = "Vim::PageUp";

            var escapeCommand = new TextEditorCommand(
                commandDisplayName, "vim-PageUp", false, true, TextEditKind.None, null,
                TextEditorCommandDefaultFacts.ScrollPageUp.CommandFunc);

            _map.Add(new KeymapArgs()
            {
                Key = "PageUp",
                Code = "PageUp",
                LayerKey = TextEditorKeymapVimFacts.InsertLayer.Key
            }, escapeCommand);
        }
    }

    private TextEditorCommand BuildCommandTextEditor(KeymapArgs keymapArgument, string displayName, string internalIdentifier)
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
	                return ValueTask.CompletedTask;

                var success = VimSentence.TryLex(this, keymapArgument, TextEditorSelectionHelper.HasSelectedText(primaryCursorModifier), out var lexCommand);

                if (success && lexCommand is not null)
                    return lexCommand.CommandFunc.Invoke(commandArgs);

                return ValueTask.CompletedTask;
            });
    }

    private TextEditorCommand BuildMovementCommand(KeymapArgs keymapArgs, string key)
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
                    keymapArgs = keymapArgs with
                    {
                        ShiftKey = true
                    };
                }

                TextEditorCommand? modifiedCommand = null;

                var success = _textEditorKeymapDefault.MapFirstOrDefault(keymapArgs, out var command);
                modifiedCommand = command as TextEditorCommand;

                if (modifiedCommand is null)
                {
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
				                return ValueTask.CompletedTask;

                            commandArgs.TextEditorService.ViewModelApi.MoveCursor(
                                new KeymapArgs
                                {
                                    Key = key,
                                    ShiftKey = keymapArgs.ShiftKey,
                                    CtrlKey = keymapArgs.CtrlKey
                                },
						        commandArgs.EditContext,
						        modelModifier,
						        viewModelModifier,
						        cursorModifierBag);
                            return ValueTask.CompletedTask;
                        });

                    TextEditorCommand finalCommand = modifiedCommand;

                    if (ActiveVimMode == VimMode.Visual)
                        finalCommand = TextEditorCommandVimFacts.Motions.GetVisualFactory(modifiedCommand, $"{nameof(TextEditorKeymapVim)}");

                    if (ActiveVimMode == VimMode.VisualLine)
                        finalCommand = TextEditorCommandVimFacts.Motions.GetVisualLineFactory(modifiedCommand, $"{nameof(TextEditorKeymapVim)}");

                    return finalCommand.CommandFunc.Invoke(commandArgs);
                }

                return ValueTask.CompletedTask;
            });
    }

    private void BuildModifiedMovementCommandsFromUnmodified(KeymapArgs keymapArgument, string key)
    {
        // Unmodified
        {
            var unmodifiedKeymapArgument = keymapArgument;

            var arrowLeftCommand = BuildMovementCommand(unmodifiedKeymapArgument, key);
            _map.Add(unmodifiedKeymapArgument, arrowLeftCommand);
        }

        // Shift
        {
            var shiftKeymapArgument = keymapArgument with
            {
                ShiftKey = true
            };

            var arrowLeftCommand = BuildMovementCommand(shiftKeymapArgument, key);
            _map.Add(shiftKeymapArgument, arrowLeftCommand);
        }

        // Ctrl
        {
            var ctrlKeymapArgument = keymapArgument with
            {
                CtrlKey = true
            };

            var arrowLeftCommand = BuildMovementCommand(ctrlKeymapArgument, key);
            _map.Add(ctrlKeymapArgument, arrowLeftCommand);
        }

        // Shift + Ctrl
        {
            var shiftCtrlKeymapArgument = keymapArgument with
            {
                ShiftKey = true,
                CtrlKey = true
            };

            var arrowLeftCommand = BuildMovementCommand(shiftCtrlKeymapArgument, key);
            _map.Add(shiftCtrlKeymapArgument, arrowLeftCommand);
        }
    }

	public bool TryMap(KeymapArgs keymapArgument, TextEditorComponentData componentData, out CommandNoType? command)
	{
		return _map.TryGetValue(keymapArgument, out command);
	}
}
