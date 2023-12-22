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
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.TextEditors.States;

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
        var characterWidthInPixels = textEditorViewModel.VirtualizationResult.CharAndRowMeasurements.CharacterWidth;

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
            var command = new TextEditorCommand(
                "Vim::i", "vim-i", false, true, TextEditKind.None, null,
                interfaceCommandArgs =>
                {
                    var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                    commandArgs.TextEditorService.Post(editContext =>
                    {
                        ActiveVimMode = VimMode.Insert;
                        return Task.CompletedTask;
                    });
                    return Task.CompletedTask;
                });

            Map.Add(new KeymapArgument("KeyI")
            {
                LayerKey = keymapLayerKey
            }, command);
        }

        // "v"
        {
            var command = new TextEditorCommand(
                "Vim::v", "vim-v", false, true, TextEditKind.None, null,
                interfaceCommandArgs =>
                {
                    var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                    commandArgs.TextEditorService.Post(editContext =>
                    {
                        var modelModifier = editContext.GetModelModifier(commandArgs.ModelResourceUri);
                        var viewModelModifier = editContext.GetViewModelModifier(commandArgs.ViewModelKey);

                        if (modelModifier is null || viewModelModifier is null)
                            return Task.CompletedTask;

                        var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier.ViewModel);
                        var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                        if (cursorModifierBag is null || primaryCursorModifier is null)
                            return Task.CompletedTask;

                        if (ActiveVimMode == VimMode.Visual)
                        {
                            ActiveVimMode = VimMode.Normal;

                            TextEditorCommandDefaultFacts.ClearTextSelection.DoAsyncFunc.Invoke(interfaceCommandArgs);
                            return Task.CompletedTask;
                        }

                        ActiveVimMode = VimMode.Visual;

                        var positionIndex = modelModifier.GetPositionIndex(
                            primaryCursorModifier.RowIndex,
                            primaryCursorModifier.ColumnIndex);

                        primaryCursorModifier.SelectionAnchorPositionIndex = positionIndex;
                        primaryCursorModifier.SelectionEndingPositionIndex = positionIndex + 1;

                        return Task.CompletedTask;
                    });
                    return Task.CompletedTask;
                });

            Map.Add(new KeymapArgument("KeyV")
            {
                LayerKey = keymapLayerKey
            }, command);
        }

        // "V"
        {
            var command = new TextEditorCommand(
                "Vim::V", "vim-V", false, true, TextEditKind.None, null,
                interfaceCommandArgs =>
                {
                    var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                    commandArgs.TextEditorService.Post(editContext =>
                    {
                        var modelModifier = editContext.GetModelModifier(commandArgs.ModelResourceUri);
                        var viewModelModifier = editContext.GetViewModelModifier(commandArgs.ViewModelKey);

                        if (modelModifier is null || viewModelModifier is null)
                            return Task.CompletedTask;

                        var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier.ViewModel);
                        var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                        if (cursorModifierBag is null || primaryCursorModifier is null)
                            return Task.CompletedTask;

                        if (ActiveVimMode == VimMode.VisualLine)
                        {
                            ActiveVimMode = VimMode.Normal;

                            TextEditorCommandDefaultFacts.ClearTextSelection.DoAsyncFunc.Invoke(interfaceCommandArgs);
                            return Task.CompletedTask;
                        }

                        ActiveVimMode = VimMode.VisualLine;

                        var startOfRowPositionIndexInclusive = modelModifier.GetPositionIndex(
                            primaryCursorModifier.RowIndex,
                            0);

                        primaryCursorModifier.SelectionAnchorPositionIndex =
                            startOfRowPositionIndexInclusive;

                        var endOfRowPositionIndexExclusive = modelModifier.RowEndingPositionsBag[
                                primaryCursorModifier.RowIndex]
                            .positionIndex;

                        primaryCursorModifier.SelectionEndingPositionIndex =
                            endOfRowPositionIndexExclusive;

                        return Task.CompletedTask;
                    });

                    return Task.CompletedTask;
                });

            Map.Add(new KeymapArgument("KeyV")
            {
                ShiftKey = true,
                LayerKey = keymapLayerKey
            }, command);
        }

        // ":"
        {
            var command = new TextEditorCommand(
                "Vim:::", "vim-:", false, true, TextEditKind.None, null,
                interfaceCommandArgs =>
                {
                    var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                    commandArgs.TextEditorService.Post(editContext =>
                    {
                        var modelModifier = editContext.GetModelModifier(commandArgs.ModelResourceUri);
                        var viewModelModifier = editContext.GetViewModelModifier(commandArgs.ViewModelKey);

                        if (modelModifier is null || viewModelModifier is null)
                            return Task.CompletedTask;

                        var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier.ViewModel);
                        var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                        if (cursorModifierBag is null || primaryCursorModifier is null)
                            return Task.CompletedTask;

                        commandArgs.Dispatcher.Dispatch(new TextEditorViewModelState.SetViewModelWithAction(
                            editContext,
                            viewModelModifier.ViewModel.ViewModelKey,
                            previousViewModel => previousViewModel with
                            {
                                DisplayCommandBar = true
                            }));

                        return Task.CompletedTask;
                    });
                    return Task.CompletedTask;
                });

            Map.Add(new KeymapArgument("Semicolon")
            {
                ShiftKey = true,
                LayerKey = keymapLayerKey
            }, command);
        }

        // "u"
        {
            var command = new TextEditorCommand(
                "Vim::u", "vim-u", false, true, TextEditKind.None, null,
                interfaceCommandArgs =>
                {
                    var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                    commandArgs.TextEditorService.Post(async editContext =>
                    {
                        var modelModifier = editContext.GetModelModifier(commandArgs.ModelResourceUri);
                        var viewModelModifier = editContext.GetViewModelModifier(commandArgs.ViewModelKey);

                        if (modelModifier is null || viewModelModifier is null)
                            return;

                        var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier.ViewModel);
                        var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                        if (cursorModifierBag is null || primaryCursorModifier is null)
                            return;

                        await commandArgs.TextEditorService.ModelApi
                            .UndoEditFactory(commandArgs.ModelResourceUri)
                            .Invoke(editContext);

                        await modelModifier.ApplySyntaxHighlightingAsync();
                    });
                    return Task.CompletedTask;
                });

            Map.Add(new KeymapArgument("KeyU")
            {
                LayerKey = keymapLayerKey
            }, command);
        }

        // "r"
        {
            var command = new TextEditorCommand(
                "Vim::r", "vim-r", false, true, TextEditKind.None, null,
                interfaceCommandArgs =>
                {
                    var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                    commandArgs.TextEditorService.Post(async editContext =>
                    {
                        var modelModifier = editContext.GetModelModifier(commandArgs.ModelResourceUri);
                        var viewModelModifier = editContext.GetViewModelModifier(commandArgs.ViewModelKey);

                        if (modelModifier is null || viewModelModifier is null)
                            return;

                        var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier.ViewModel);
                        var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                        if (cursorModifierBag is null || primaryCursorModifier is null)
                            return;

                        await commandArgs.TextEditorService.ModelApi
                            .RedoEditFactory(commandArgs.ModelResourceUri)
                            .Invoke(editContext);

                        await modelModifier.ApplySyntaxHighlightingAsync();
                    });
                    return Task.CompletedTask;
                });

            Map.Add(new KeymapArgument("KeyR")
            {
                CtrlKey = true,
                LayerKey = keymapLayerKey
            }, command);
        }

        // "o"
        {
            var command = new TextEditorCommand(
                "Vim::o", "vim-o", false, true, TextEditKind.None, null,
                interfaceCommandArgs =>
                {
                    var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                    commandArgs.TextEditorService.Post(editContext =>
                    {
                        return TextEditorCommandVimFacts.Verbs.NewLineBelowCommand.DoAsyncFunc
                            .Invoke(interfaceCommandArgs);
                    });
                    return Task.CompletedTask;
                });

            Map.Add(new KeymapArgument("KeyO")
            {
                LayerKey = keymapLayerKey
            }, command);
        }

        // "O"
        {
            var command = new TextEditorCommand(
                "Vim::O", "vim-O", false, true, TextEditKind.None, null,
                interfaceCommandArgs =>
                {
                    var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                    commandArgs.TextEditorService.Post(editContext =>
                    {
                        return TextEditorCommandVimFacts.Verbs.NewLineAboveCommand.DoAsyncFunc
                            .Invoke(interfaceCommandArgs);
                    });
                    return Task.CompletedTask;
                });

            Map.Add(new KeymapArgument("KeyO")
            {
                ShiftKey = true,
                LayerKey = keymapLayerKey
            }, command);
        }

        // "e"
        {
            var command = new TextEditorCommand(
                "Vim::e", "vim-e", false, false, TextEditKind.None, null,
                interfaceCommandArgs =>
                {
                    var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                    commandArgs.TextEditorService.Post(editContext =>
                    {
                        return TextEditorCommandDefaultFacts.ScrollLineDown.DoAsyncFunc
                        .Invoke(interfaceCommandArgs);
                    });
                    return Task.CompletedTask;
                });

            Map.Add(new KeymapArgument("KeyE")
            {
                CtrlKey = true,
                LayerKey = keymapLayerKey
            }, command);
        }

        // "y"
        {
            var command = new TextEditorCommand(
                "Vim::y", "vim-y", false, false, TextEditKind.None, null,
                interfaceCommandArgs =>
                {
                    var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                    commandArgs.TextEditorService.Post(editContext =>
                    {
                        return TextEditorCommandDefaultFacts.ScrollLineUp.DoAsyncFunc
                            .Invoke(interfaceCommandArgs);
                    });
                    return Task.CompletedTask;
                });

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
        var escapeCommand = new TextEditorCommand(
            "Vim::Escape", "vim-Escape", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                commandArgs.TextEditorService.Post(editContext =>
                {
                    ActiveVimMode = VimMode.Normal;
                    return Task.CompletedTask;
                });
                return Task.CompletedTask;
            });

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
            var escapeCommand = new TextEditorCommand(
                "Vim::PageDown", "vim-PageDown", false, true, TextEditKind.None, null,
                interfaceCommandArgs =>
                {
                    var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                    commandArgs.TextEditorService.Post(editContext =>
                    {
                        return TextEditorCommandDefaultFacts.ScrollPageDown.DoAsyncFunc
                            .Invoke(commandArgs);
                    });
                    return Task.CompletedTask;
                });

            Map.Add(new KeymapArgument("PageDown")
            {
                LayerKey = TextEditorKeymapVimFacts.InsertLayer.Key
            }, escapeCommand);
        }

        // PageUp
        {
            var escapeCommand = new TextEditorCommand(
                "Vim::PageUp", "vim-PageUp", false, true, TextEditKind.None, null,
                interfaceCommandArgs =>
                {
                    var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                    commandArgs.TextEditorService.Post(editContext =>
                    {
                        return TextEditorCommandDefaultFacts.ScrollPageUp.DoAsyncFunc
                            .Invoke(commandArgs);
                    });
                    return Task.CompletedTask;
                });

            Map.Add(new KeymapArgument("PageUp")
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

                commandArgs.TextEditorService.Post(editContext =>
                {
                    var success = VimSentence.TryLex(this, keymapArgument, commandArgs.HasTextSelection, out var lexCommand);

                    if (success && lexCommand is not null)
                        return lexCommand.DoAsyncFunc.Invoke(commandArgs);

                    return Task.CompletedTask;
                });
                return Task.CompletedTask;
            });
    }

    private TextEditorCommand BuildMovementCommand(KeymapArgument keymapArgument, string key)
    {
        return new TextEditorCommand(
            $"Vim::{key}", $"vim-{key}", false, true, TextEditKind.None, null,
            interfaceCommandArgs =>
            {
                var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                commandArgs.TextEditorService.Post(editContext =>
                {
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

                                commandArgs.TextEditorService.Post(editContext =>
                                {
                                    ActiveVimMode = VimMode.Insert;
                                    return Task.CompletedTask;
                                });

                                commandArgs.TextEditorService.ViewModelApi.MoveCursorFactory(
                                        keyboardEventArgs,
                                        commandArgs.ModelResourceUri,
                                        commandArgs.ViewModelKey)
                                    .Invoke(editContext);

                                return Task.CompletedTask;
                            });

                        TextEditorCommand finalCommand = modifiedCommand;

                        if (ActiveVimMode == VimMode.Visual)
                            finalCommand = TextEditorCommandVimFacts.Motions.GetVisualFactory(modifiedCommand, $"{nameof(TextEditorKeymapVim)}");

                        if (ActiveVimMode == VimMode.VisualLine)
                            finalCommand = TextEditorCommandVimFacts.Motions.GetVisualLineFactory(modifiedCommand, $"{nameof(TextEditorKeymapVim)}");

                        return finalCommand.DoAsyncFunc.Invoke(commandArgs);
                    }

                    return Task.CompletedTask;
                });
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
}
