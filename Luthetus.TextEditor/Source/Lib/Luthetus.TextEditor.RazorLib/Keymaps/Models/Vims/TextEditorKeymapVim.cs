using Luthetus.TextEditor.RazorLib.Commands.Models.Vims;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Options.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models.Defaults;
using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;

namespace Luthetus.TextEditor.RazorLib.Keymaps.Models.Vims;

public class TextEditorKeymapVim : Keymap, ITextEditorKeymap
{
    public TextEditorKeymapVim()
        : base(
            new Key<Keymap>(Guid.Parse("d2122a7a-5a88-4d31-af20-5486a36e9c0c")),
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
                throw new ApplicationException($"The {nameof(ActiveVimMode)}: '{ActiveVimMode}' was not recognized.");
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
        var characterWidthInPixels = textEditorViewModel.VirtualizationResult.CharacterWidthAndRowHeight.CharacterWidthInPixels;

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
            Map.Add(new KeymapArgument("Escape")
            {
                LayerKey = keymapLayerKey
            }, TextEditorCommandDefaultFacts.ClearTextSelection);
        }

        // "i"
        {
            var command = new CommandTextEditor(
                commandParameter =>
                {
                    ActiveVimMode = VimMode.Insert;
                    return Task.CompletedTask;
                },
                true,
                "Vim::i",
                "vim-i");

            Map.Add(new KeymapArgument("KeyI")
            {
                LayerKey = keymapLayerKey
            }, command);
        }

        // "v"
        {
            var command = new CommandTextEditor(
                interfaceCommandParameter =>
                {
                    if (ActiveVimMode == VimMode.Visual)
                    {
                        ActiveVimMode = VimMode.Normal;

                        TextEditorCommandDefaultFacts.ClearTextSelection.DoAsyncFunc.Invoke(interfaceCommandParameter);
                        return Task.CompletedTask;
                    }

                    ActiveVimMode = VimMode.Visual;

                    var commandParameter = (TextEditorCommandParameter)interfaceCommandParameter;

                    var positionIndex = commandParameter.Model.GetPositionIndex(
                        commandParameter.PrimaryCursorSnapshot.ImmutableCursor.RowIndex,
                        commandParameter.PrimaryCursorSnapshot.ImmutableCursor.ColumnIndex);

                    commandParameter.PrimaryCursorSnapshot.UserCursor.Selection.AnchorPositionIndex =
                        positionIndex;

                    commandParameter.PrimaryCursorSnapshot.UserCursor.Selection.EndingPositionIndex =
                        positionIndex + 1;

                    return Task.CompletedTask;
                },
                true,
                "Vim::v",
                "vim-v");

            Map.Add(new KeymapArgument("KeyV")
            {
                LayerKey = keymapLayerKey
            }, command);
        }

        // "V"
        {
            var command = new CommandTextEditor(
                interfaceCommandParameter =>
                {
                    if (ActiveVimMode == VimMode.VisualLine)
                    {
                        ActiveVimMode = VimMode.Normal;

                        TextEditorCommandDefaultFacts.ClearTextSelection.DoAsyncFunc.Invoke(interfaceCommandParameter);
                        return Task.CompletedTask;
                    }

                    ActiveVimMode = VimMode.VisualLine;

                    var commandParameter = (TextEditorCommandParameter)interfaceCommandParameter;

                    var startOfRowPositionIndexInclusive = commandParameter.Model.GetPositionIndex(
                        commandParameter.PrimaryCursorSnapshot.ImmutableCursor.RowIndex,
                        0);

                    commandParameter.PrimaryCursorSnapshot.UserCursor.Selection.AnchorPositionIndex =
                        startOfRowPositionIndexInclusive;

                    var endOfRowPositionIndexExclusive = commandParameter.Model.RowEndingPositionsBag[
                            commandParameter.PrimaryCursorSnapshot.ImmutableCursor.RowIndex]
                        .positionIndex;

                    commandParameter.PrimaryCursorSnapshot.UserCursor.Selection.EndingPositionIndex =
                        endOfRowPositionIndexExclusive;

                    return Task.CompletedTask;
                },
                true,
                "Vim::V",
                "vim-V");

            Map.Add(new KeymapArgument("KeyV")
            {
                ShiftKey = true,
                LayerKey = keymapLayerKey
            }, command);
        }

        // ":"
        {
            var command = new CommandTextEditor(
                interfaceCommandParameter =>
                {
                    var commandParameter = (TextEditorCommandParameter)interfaceCommandParameter;

                    commandParameter.TextEditorService.ViewModel.With(
                        commandParameter.ViewModel.ViewModelKey,
                        previousViewModel => previousViewModel with
                        {
                            DisplayCommandBar = true
                        });
                    return Task.CompletedTask;
                },
                true,
                "Vim:::",
                "vim-:");

            Map.Add(new KeymapArgument("Semicolon")
            {
                ShiftKey = true,
                LayerKey = keymapLayerKey
            }, command);
        }

        // "u"
        {
            var command = new CommandTextEditor(
                async interfaceCommandParameter =>
                {
                    var commandParameter = (TextEditorCommandParameter)interfaceCommandParameter;

                    commandParameter.TextEditorService.Model.UndoEdit(commandParameter.Model.ResourceUri);
                    await commandParameter.Model.ApplySyntaxHighlightingAsync();
                },
                true,
                "Vim::u",
                "vim-u");

            Map.Add(new KeymapArgument("KeyU")
            {
                LayerKey = keymapLayerKey
            }, command);
        }

        // "r"
        {
            var command = new CommandTextEditor(
                async interfaceCommandParameter =>
                {
                    var commandParameter = (TextEditorCommandParameter)interfaceCommandParameter;

                    commandParameter.TextEditorService.Model.RedoEdit(commandParameter.Model.ResourceUri);
                    await commandParameter.Model.ApplySyntaxHighlightingAsync();
                },
                true,
                "Vim::r",
                "vim-r");

            Map.Add(new KeymapArgument("KeyR")
            {
                CtrlKey = true,
                LayerKey = keymapLayerKey
            }, command);
        }

        // "o"
        {
            var command = new CommandTextEditor(
                interfaceCommandParameter =>
                {
                    return TextEditorCommandVimFacts.Verbs.NewLineBelow.DoAsyncFunc.Invoke(interfaceCommandParameter);
                },
                true,
                "Vim::o",
                "vim-o");

            Map.Add(new KeymapArgument("KeyO")
            {
                LayerKey = keymapLayerKey
            }, command);
        }

        // "O"
        {
            var command = new CommandTextEditor(
                interfaceCommandParameter =>
                {
                    return TextEditorCommandVimFacts.Verbs.NewLineAbove.DoAsyncFunc.Invoke(interfaceCommandParameter);
                },
                true,
                "Vim::O",
                "vim-O");

            Map.Add(new KeymapArgument("KeyO")
            {
                ShiftKey = true,
                LayerKey = keymapLayerKey
            }, command);
        }

        // "e"
        {
            var command = new CommandTextEditor(
                interfaceCommandParameter =>
                {
                    return TextEditorCommandDefaultFacts.ScrollLineDown.DoAsyncFunc.Invoke(interfaceCommandParameter);
                },
                false,
                "Vim::e",
                "vim-e");

            Map.Add(new KeymapArgument("KeyE")
            {
                CtrlKey = true,
                LayerKey = keymapLayerKey
            }, command);
        }

        // "y"
        {
            var command = new CommandTextEditor(
                interfaceCommandParameter =>
                {
                    return TextEditorCommandDefaultFacts.ScrollLineUp.DoAsyncFunc.Invoke(interfaceCommandParameter);
                },
                false,
                "Vim::y",
                "vim-y");

            Map.Add(new KeymapArgument("KeyY")
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

                var keymapArgument = new KeymapArgument($"Digit{character}")
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

                var keymapArgument = new KeymapArgument($"Key{character}")
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

                var keymapArgument = new KeymapArgument($"Key{char.ToUpperInvariant(character)}")
                {
                    LayerKey = keymapLayerKey
                };

                var command = BuildCommandTextEditor(keymapArgument, $"Vim::{character}", $"vim-{character}");

                _ = Map.TryAdd(keymapArgument, command);
            }

            // ` = Backquote 
            {
                var keymapArgument = new KeymapArgument("Backquote") { LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::`", $"vim-`");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // ~ = Shift + Backquote
            {
                var keymapArgument = new KeymapArgument("Backquote") { ShiftKey = true, LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::~", $"vim-~");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // ! = Shift + Digit1
            {
                var keymapArgument = new KeymapArgument("Digit1") { ShiftKey = true, LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::!", $"vim-!");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // @ = Shift + Digit2
            {
                var keymapArgument = new KeymapArgument("Digit2") { ShiftKey = true, LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::@", $"vim-@");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // # = Shift + Digit3
            {
                var keymapArgument = new KeymapArgument("Digit3") { ShiftKey = true, LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::#", $"vim-#");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // $ = Shift + Digit4
            {
                var keymapArgument = new KeymapArgument("Digit4") { ShiftKey = true, LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::$", $"vim-$");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // % = Shift + Digit5
            {
                var keymapArgument = new KeymapArgument("Digit5") { ShiftKey = true, LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::%", $"vim-%");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // ^ = Shift + Digit6
            {
                var keymapArgument = new KeymapArgument("Digit6") { ShiftKey = true, LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::^", $"vim-^");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // & = Shift + Digit7
            {
                var keymapArgument = new KeymapArgument("Digit7") { ShiftKey = true, LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::&", $"vim-&");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // * = Shift + Digit8
            {
                var keymapArgument = new KeymapArgument("Digit8") { ShiftKey = true, LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::*", $"vim-*");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // ( = Shift + Digit9
            {
                var keymapArgument = new KeymapArgument("Digit9") { ShiftKey = true, LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::(", $"vim-(");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // ) = Shift + Digit0
            {
                var keymapArgument = new KeymapArgument("Digit0") { ShiftKey = true, LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::)", $"vim-)");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // - = Minus
            {
                var keymapArgument = new KeymapArgument("Minus") { LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::-", $"vim--");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // _ = Shift + Minus
            {
                var keymapArgument = new KeymapArgument("Minus") { ShiftKey = true, LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::_", $"vim-_");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // = = Equal
            {
                var keymapArgument = new KeymapArgument("Equal") { LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::=", $"vim-=");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // + = Shift + Equal
            {
                var keymapArgument = new KeymapArgument("Equal") { ShiftKey = true, LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::+", $"vim-+");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // [ = BracketLeft
            {
                var keymapArgument = new KeymapArgument("BracketLeft") { LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::[", $"vim-[");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // { = Shift + BracketLeft
            {
                var keymapArgument = new KeymapArgument("BracketLeft") { ShiftKey = true, LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::{{", $"vim-}}");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // ] = BracketRight
            {
                var keymapArgument = new KeymapArgument("BracketRight") { LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::]", $"vim-]");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // } = Shift + BracketRight
            {
                var keymapArgument = new KeymapArgument("BracketRight") { ShiftKey = true, LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::}}", $"vim-}}");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // \ = Backslash
            {
                var keymapArgument = new KeymapArgument("Backslash") { LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::\\", $"vim-\\");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // | = Shift + Backslash
            {
                var keymapArgument = new KeymapArgument("Backslash") { ShiftKey = true, LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::|", $"vim-|");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // ; = Semicolon
            {
                var keymapArgument = new KeymapArgument("Semicolon") { LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::;", $"vim-;");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // : = Shift + Semicolon
            {
                var keymapArgument = new KeymapArgument("Semicolon") { ShiftKey = true, LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim:::", $"vim-:");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // ' = Quote
            {
                var keymapArgument = new KeymapArgument("Quote") { LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::'", $"vim-'");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // " = Shift + Quote
            {
                var keymapArgument = new KeymapArgument("Quote") { ShiftKey = true, LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::\"", $"vim-\"");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // , = Comma
            {
                var keymapArgument = new KeymapArgument("Comma") { LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::,", $"vim-,");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // < = Shift + Comma
            {
                var keymapArgument = new KeymapArgument("Comma") { ShiftKey = true, LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::<", $"vim-<");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // . = Period
            {
                var keymapArgument = new KeymapArgument("Period") { LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::.", $"vim-.");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // > = Shift + Period
            {
                var keymapArgument = new KeymapArgument("Period") { ShiftKey = true, LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::>", $"vim->");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // / = Slash
            {
                var keymapArgument = new KeymapArgument("Slash") { LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::/", $"vim-/");
                _ = Map.TryAdd(keymapArgument, command);
            }
            // ? = Shift + Slash
            {
                var keymapArgument = new KeymapArgument("Slash") { ShiftKey = true, LayerKey = keymapLayerKey };
                var command = BuildCommandTextEditor(keymapArgument, $"Vim::?", $"vim-?");
                _ = Map.TryAdd(keymapArgument, command);
            }
        }
    }

    private void AddInsertLayer()
    {
        var escapeCommand = new CommandTextEditor(
            commandParameter =>
            {
                ActiveVimMode = VimMode.Normal;
                return Task.CompletedTask;
            },
            true,
            "Vim::Escape",
            "vim-Escape");

        Map.Add(new KeymapArgument("Escape")
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
        BuildModifiedMovementCommandsFromUnmodified(new KeymapArgument("ArrowLeft")
        {
            LayerKey = TextEditorKeymapVimFacts.InsertLayer.Key,
        }, "ArrowLeft");

        BuildModifiedMovementCommandsFromUnmodified(new KeymapArgument("ArrowDown")
        {
            LayerKey = TextEditorKeymapVimFacts.InsertLayer.Key,
        }, "ArrowDown");
        BuildModifiedMovementCommandsFromUnmodified(new KeymapArgument("ArrowUp")
        {
            LayerKey = TextEditorKeymapVimFacts.InsertLayer.Key,
        }, "ArrowUp");
        BuildModifiedMovementCommandsFromUnmodified(new KeymapArgument("ArrowRight")
        {
            LayerKey = TextEditorKeymapVimFacts.InsertLayer.Key,
        }, "ArrowRight");
        BuildModifiedMovementCommandsFromUnmodified(new KeymapArgument("Home")
        {
            LayerKey = TextEditorKeymapVimFacts.InsertLayer.Key,
        }, "Home");
        BuildModifiedMovementCommandsFromUnmodified(new KeymapArgument("End")
        {
            LayerKey = TextEditorKeymapVimFacts.InsertLayer.Key,
        }, "End");

        // PageDown
        {
            var escapeCommand = new CommandTextEditor(
                commandParameter =>
                {
                    return TextEditorCommandDefaultFacts.ScrollPageDown.DoAsyncFunc.Invoke(commandParameter);
                },
                true,
                "Vim::PageDown",
                "vim-PageDown");

            Map.Add(new KeymapArgument("PageDown")
            {
                LayerKey = TextEditorKeymapVimFacts.InsertLayer.Key
            }, escapeCommand);
        }

        // PageUp
        {
            var escapeCommand = new CommandTextEditor(
                commandParameter =>
                {
                    return TextEditorCommandDefaultFacts.ScrollPageUp.DoAsyncFunc.Invoke(commandParameter);
                },
                true,
                "Vim::PageUp",
                "vim-PageUp");

            Map.Add(new KeymapArgument("PageUp")
            {
                LayerKey = TextEditorKeymapVimFacts.InsertLayer.Key
            }, escapeCommand);
        }
    }

    private CommandTextEditor BuildCommandTextEditor(KeymapArgument keymapArgument, string displayName, string internalIdentifier)
    {
        return new CommandTextEditor(
            interfaceCommandParameter =>
            {
                var commandParameter = (TextEditorCommandParameter)interfaceCommandParameter;

                var success = VimSentence.TryLex(this, keymapArgument, commandParameter.HasTextSelection, out var lexCommand);

                if (success && lexCommand is not null)
                    return lexCommand.DoAsyncFunc.Invoke(commandParameter);

                return Task.CompletedTask;
            },
            true,
            displayName,
            internalIdentifier);
    }

    private CommandTextEditor BuildMovementCommand(KeymapArgument keymapArgument, string key)
    {
        return new CommandTextEditor(
                interfaceCommandParameter =>
                {
                    var commandParameter = (TextEditorCommandParameter)interfaceCommandParameter;

                    if (ActiveVimMode == VimMode.Visual || ActiveVimMode == VimMode.VisualLine)
                    {
                        keymapArgument = keymapArgument with
                        {
                            ShiftKey = true
                        };
                    }

                    CommandTextEditor? modifiedCommand = null;

                    modifiedCommand = (CommandTextEditor?)_textEditorKeymapDefault.Map[keymapArgument];

                    if (modifiedCommand is null)
                    {
                        var keyboardEventArgs = new KeyboardEventArgs
                        {
                            Key = key,
                            ShiftKey = keymapArgument.ShiftKey,
                            CtrlKey = keymapArgument.CtrlKey
                        };

                        modifiedCommand = new CommandTextEditor(
                            textEditorCommandParameter =>
                            {
                                TextEditorCursor.MoveCursor(
                                    keyboardEventArgs,
                                    commandParameter.PrimaryCursorSnapshot.UserCursor,
                                    commandParameter.Model);

                                return Task.CompletedTask;
                            },
                            true,
                            "MoveCursor",
                            "MoveCursor");

                        CommandTextEditor finalCommand = modifiedCommand;

                        if (ActiveVimMode == VimMode.Visual)
                            finalCommand = TextEditorCommandVimFacts.Motions.GetVisual(modifiedCommand, $"{nameof(TextEditorKeymapVim)}");

                        if (ActiveVimMode == VimMode.VisualLine)
                            finalCommand = TextEditorCommandVimFacts.Motions.GetVisualLine(modifiedCommand, $"{nameof(TextEditorKeymapVim)}");

                        return finalCommand.DoAsyncFunc.Invoke(commandParameter);
                    }

                    return Task.CompletedTask;
                },
                true,
                $"Vim::{key}",
                $"vim-{key}");
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
}
