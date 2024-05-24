using Luthetus.Common.RazorLib.JsRuntimes.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;

namespace Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;

public static class TextEditorCommandDefaultFacts
{
    public static readonly TextEditorCommand DoNothingDiscard = new(
        "DoNothingDiscard", "defaults_do-nothing-discard", false, false, TextEditKind.None, null,
        interfaceCommandArgs => Task.CompletedTask);

    public static readonly TextEditorCommand Copy = new(
        "Copy", "defaults_copy", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

if (commandArgs is null)
	Console.WriteLine("commandArgs was null");
else if (commandArgs.TextEditorService is null)
	Console.WriteLine("commandArgs.TextEditorService was null");

            return commandArgs.TextEditorService.PostSimpleBatch(
                $"{nameof(Copy)}_{commandArgs.ViewModelKey.Guid}",
                $"{nameof(Copy)}_{commandArgs.ViewModelKey.Guid}",
				null,
                TextEditorCommandDefaultFunctions.CopyFactory(
                    commandArgs.ModelResourceUri,
                    commandArgs.ViewModelKey,
                    commandArgs));
        })
    {
        TextEditorEditFactory = interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            return TextEditorCommandDefaultFunctions.CopyFactory(
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                commandArgs);
        }
    };

    public static readonly TextEditorCommand Cut = new(
        "Cut", "defaults_cut", false, true, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            return commandArgs.TextEditorService.PostSimpleBatch(
                $"{nameof(Cut)}_{commandArgs.ViewModelKey.Guid}",
                string.Empty,
				null,
                TextEditorCommandDefaultFunctions.CutFactory(
                    commandArgs.ModelResourceUri,
                    commandArgs.ViewModelKey,
                    commandArgs));
        })
    {
        TextEditorEditFactory = interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            return TextEditorCommandDefaultFunctions.CutFactory(
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                commandArgs);
        }
    };

    public static readonly TextEditorCommand PasteCommand = new(
        "Paste", "defaults_paste", false, true, TextEditKind.Other, "defaults_paste",
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            return commandArgs.TextEditorService.PostSimpleBatch(
                $"{nameof(PasteCommand)}_{commandArgs.ViewModelKey.Guid}",
                string.Empty,
				null,
                TextEditorCommandDefaultFunctions.PasteFactory(
                    commandArgs.ModelResourceUri,
                    commandArgs.ViewModelKey,
                    commandArgs));
        })
    {
        TextEditorEditFactory = interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            return TextEditorCommandDefaultFunctions.PasteFactory(
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                commandArgs);
        }
    };

    public static readonly TextEditorCommand Save = new(
        "Save", "defaults_save", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            return commandArgs.TextEditorService.PostSimpleBatch(
                $"{nameof(Save)}_{commandArgs.ViewModelKey.Guid}",
                $"{nameof(Save)}_{commandArgs.ViewModelKey.Guid}",
				null,
                TextEditorCommandDefaultFunctions.SaveFactory(
                    commandArgs.ModelResourceUri,
                    commandArgs.ViewModelKey,
                    commandArgs));
        })
    {
        TextEditorEditFactory = interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            return TextEditorCommandDefaultFunctions.SaveFactory(
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                commandArgs);
        }
    };

    public static readonly TextEditorCommand SelectAll = new(
        "Select All", "defaults_select-all", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            return commandArgs.TextEditorService.PostSimpleBatch(
                $"{nameof(SelectAll)}_{commandArgs.ViewModelKey.Guid}",
                $"{nameof(SelectAll)}_{commandArgs.ViewModelKey.Guid}",
				null,
                TextEditorCommandDefaultFunctions.SelectAllFactory(
                    commandArgs.ModelResourceUri,
                    commandArgs.ViewModelKey,
                    commandArgs));
        })
    {
        TextEditorEditFactory = interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            return TextEditorCommandDefaultFunctions.SelectAllFactory(
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                commandArgs);
        }
    };

    public static readonly TextEditorCommand Undo = new(
        "Undo", "defaults_undo", false, true, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            return commandArgs.TextEditorService.PostSimpleBatch(
                $"{nameof(Undo)}_{commandArgs.ViewModelKey.Guid}",
                string.Empty,
				null,
                TextEditorCommandDefaultFunctions.UndoFactory(
                    commandArgs.ModelResourceUri,
                    commandArgs.ViewModelKey,
                    commandArgs));
        })
    {
        TextEditorEditFactory = interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            return TextEditorCommandDefaultFunctions.UndoFactory(
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                commandArgs);
        }
    };

    public static readonly TextEditorCommand Redo = new(
        "Redo", "defaults_redo", false, true, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            return commandArgs.TextEditorService.PostSimpleBatch(
                $"{nameof(Redo)}_{commandArgs.ViewModelKey}",
                string.Empty,
				null,
                TextEditorCommandDefaultFunctions.RedoFactory(
                    commandArgs.ModelResourceUri,
                    commandArgs.ViewModelKey,
                    commandArgs));
        })
    {
        TextEditorEditFactory = interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            return TextEditorCommandDefaultFunctions.RedoFactory(
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                commandArgs);
        }
    };

    public static readonly TextEditorCommand Remeasure = new(
        "Remeasure", "defaults_remeasure", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            return commandArgs.TextEditorService.PostSimpleBatch(
                $"{nameof(Remeasure)}_{commandArgs.ViewModelKey}",
                $"{nameof(Remeasure)}_{commandArgs.ViewModelKey}",
				null,
                TextEditorCommandDefaultFunctions.RemeasureFactory(
                    commandArgs.ModelResourceUri,
                    commandArgs.ViewModelKey,
                    commandArgs));
        })
    {
        TextEditorEditFactory = interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            return TextEditorCommandDefaultFunctions.RemeasureFactory(
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                commandArgs);
        }
    };

    public static readonly TextEditorCommand ScrollLineDown = new(
        "Scroll Line Down", "defaults_scroll-line-down", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            return commandArgs.TextEditorService.PostSimpleBatch(
                $"{nameof(ScrollLineDown)}_{commandArgs.ViewModelKey}",
                string.Empty,
				null,
                TextEditorCommandDefaultFunctions.ScrollLineDownFactory(
                    commandArgs.ModelResourceUri,
                    commandArgs.ViewModelKey,
                    commandArgs));
        })
    {
        TextEditorEditFactory = interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            return TextEditorCommandDefaultFunctions.ScrollLineDownFactory(
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                commandArgs);
        }
    };

    public static readonly TextEditorCommand ScrollLineUp = new(
        "Scroll Line Up", "defaults_scroll-line-up", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            return commandArgs.TextEditorService.PostSimpleBatch(
                $"{nameof(ScrollLineUp)}_{commandArgs.ViewModelKey}",
                string.Empty,
				null,
                TextEditorCommandDefaultFunctions.ScrollLineUpFactory(
                    commandArgs.ModelResourceUri,
                    commandArgs.ViewModelKey,
                    commandArgs));
        })
    {
        TextEditorEditFactory = interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            return TextEditorCommandDefaultFunctions.ScrollLineUpFactory(
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                commandArgs);
        }
    };

    public static readonly TextEditorCommand ScrollPageDown = new(
        "Scroll Page Down", "defaults_scroll-page-down", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            return commandArgs.TextEditorService.PostSimpleBatch(
                $"{nameof(ScrollPageDown)}_{commandArgs.ViewModelKey}",
                string.Empty,
				null,
                TextEditorCommandDefaultFunctions.ScrollPageDownFactory(
                    commandArgs.ModelResourceUri,
                    commandArgs.ViewModelKey,
                    commandArgs));
        })
    {
        TextEditorEditFactory = interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            return TextEditorCommandDefaultFunctions.ScrollPageDownFactory(
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                commandArgs);
        }
    };

    public static readonly TextEditorCommand ScrollPageUp = new(
        "Scroll Page Up", "defaults_scroll-page-up", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            return commandArgs.TextEditorService.PostSimpleBatch(
                $"{nameof(ScrollPageUp)}_{commandArgs.ViewModelKey}",
                string.Empty,
				null,
                TextEditorCommandDefaultFunctions.ScrollPageUpFactory(
                    commandArgs.ModelResourceUri,
                    commandArgs.ViewModelKey,
                    commandArgs));
        })
    {
        TextEditorEditFactory = interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            return TextEditorCommandDefaultFunctions.ScrollPageUpFactory(
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                commandArgs);
        }
    };

    public static readonly TextEditorCommand CursorMovePageBottom = new(
        "Move Cursor to Bottom of the Page", "defaults_cursor-move-page-bottom", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            return commandArgs.TextEditorService.PostSimpleBatch(
                $"{nameof(CursorMovePageBottom)}_{commandArgs.ViewModelKey}",
                $"{nameof(CursorMovePageBottom)}_{commandArgs.ViewModelKey}",
				null,
                TextEditorCommandDefaultFunctions.CursorMovePageBottomFactory(
                    commandArgs.ModelResourceUri,
                    commandArgs.ViewModelKey,
                    commandArgs));
        })
    {
        TextEditorEditFactory = interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            return TextEditorCommandDefaultFunctions.CursorMovePageBottomFactory(
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                commandArgs);
        }
    };

    public static readonly TextEditorCommand CursorMovePageTop = new(
        "Move Cursor to Top of the Page", "defaults_cursor-move-page-top", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            return commandArgs.TextEditorService.PostSimpleBatch(
                $"{nameof(CursorMovePageTop)}_{commandArgs.ViewModelKey}",
                $"{nameof(CursorMovePageTop)}_{commandArgs.ViewModelKey}",
				null,
                TextEditorCommandDefaultFunctions.CursorMovePageTopFactory(
                    commandArgs.ModelResourceUri,
                    commandArgs.ViewModelKey,
                    commandArgs));
        })
    {
        TextEditorEditFactory = interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            return TextEditorCommandDefaultFunctions.CursorMovePageTopFactory(
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                commandArgs);
        }
    };

    public static readonly TextEditorCommand Duplicate = new(
        "Duplicate", "defaults_duplicate", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            return commandArgs.TextEditorService.PostSimpleBatch(
                $"{nameof(Duplicate)}_{commandArgs.ViewModelKey}",
                string.Empty,
				null,
                TextEditorCommandDefaultFunctions.DuplicateFactory(
                    commandArgs.ModelResourceUri,
                    commandArgs.ViewModelKey,
                    commandArgs));
        })
    {
        TextEditorEditFactory = interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            return TextEditorCommandDefaultFunctions.DuplicateFactory(
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                commandArgs);
        }
    };

    public static readonly TextEditorCommand IndentMore = new(
        "Indent More", "defaults_indent-more", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            return commandArgs.TextEditorService.PostSimpleBatch(
                $"{nameof(IndentMore)}_{commandArgs.ViewModelKey}",
                string.Empty,
				null,
                TextEditorCommandDefaultFunctions.IndentMoreFactory(
                    commandArgs.ModelResourceUri,
                    commandArgs.ViewModelKey,
                    commandArgs));
        })
    {
        TextEditorEditFactory = interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            return TextEditorCommandDefaultFunctions.IndentMoreFactory(
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                commandArgs);
        }
    };

    public static readonly TextEditorCommand IndentLess = new(
        "Indent Less", "defaults_indent-less", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            return commandArgs.TextEditorService.PostSimpleBatch(
                $"{nameof(IndentLess)}_{commandArgs.ViewModelKey}",
                string.Empty,
				null,
                TextEditorCommandDefaultFunctions.IndentLessFactory(
                    commandArgs.ModelResourceUri,
                    commandArgs.ViewModelKey,
                    commandArgs));
        })
    {
        TextEditorEditFactory = interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            return TextEditorCommandDefaultFunctions.IndentLessFactory(
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                commandArgs);
        }
    };

    public static readonly TextEditorCommand ClearTextSelection = new(
        "ClearTextSelection", "defaults_clear-text-selection", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            return commandArgs.TextEditorService.PostSimpleBatch(
                $"{nameof(ClearTextSelection)}_{commandArgs.ViewModelKey}",
                $"{nameof(ClearTextSelection)}_{commandArgs.ViewModelKey}",
				null,
                TextEditorCommandDefaultFunctions.ClearTextSelectionFactory(
                    commandArgs.ModelResourceUri,
                    commandArgs.ViewModelKey,
                    commandArgs));
        })
    {
        TextEditorEditFactory = interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            return TextEditorCommandDefaultFunctions.ClearTextSelectionFactory(
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                commandArgs);
        }
    };

    public static readonly TextEditorCommand NewLineBelow = new(
        "NewLineBelow", "defaults_new-line-below", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            return commandArgs.TextEditorService.PostSimpleBatch(
                $"{nameof(NewLineBelow)}_{commandArgs.ViewModelKey}",
                string.Empty,
				null,
                TextEditorCommandDefaultFunctions.NewLineBelowFactory(
                    commandArgs.ModelResourceUri,
                    commandArgs.ViewModelKey,
                    commandArgs));
        })
    {
        TextEditorEditFactory = interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            return TextEditorCommandDefaultFunctions.NewLineBelowFactory(
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                commandArgs);
        }
    };

    public static readonly TextEditorCommand NewLineAbove = new(
        "NewLineBelow", "defaults_new-line-below", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            return commandArgs.TextEditorService.PostSimpleBatch(
                $"{nameof(NewLineAbove)}_{commandArgs.ViewModelKey}",
                string.Empty,
				null,
                TextEditorCommandDefaultFunctions.NewLineAboveFactory(
                    commandArgs.ModelResourceUri,
                    commandArgs.ViewModelKey,
                    commandArgs));
        })
    {
        TextEditorEditFactory = interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            return TextEditorCommandDefaultFunctions.NewLineAboveFactory(
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                commandArgs);
        }
    };

    public static TextEditorCommand GoToMatchingCharacterFactory(bool shouldSelectText) => new(
        "GoToMatchingCharacter", "defaults_go-to-matching-character", false, true, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;
            commandArgs.ShouldSelectText = shouldSelectText;

            return commandArgs.TextEditorService.PostSimpleBatch(
                $"{nameof(GoToMatchingCharacterFactory)}_{commandArgs.ViewModelKey}",
                string.Empty,
				null,
                TextEditorCommandDefaultFunctions.GoToMatchingCharacterFactory(
                    commandArgs.ModelResourceUri,
                    commandArgs.ViewModelKey,
                    commandArgs));
        })
    {
        TextEditorEditFactory = interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            return TextEditorCommandDefaultFunctions.GoToMatchingCharacterFactory(
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                commandArgs);
        }
    };

    public static readonly TextEditorCommand GoToDefinition = new(
        "GoToDefinition", "defaults_go-to-definition", false, true, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            return commandArgs.TextEditorService.PostSimpleBatch(
                $"{nameof(GoToDefinition)}_{commandArgs.ViewModelKey}",
                $"{nameof(GoToDefinition)}_{commandArgs.ViewModelKey}",
				null,
                TextEditorCommandDefaultFunctions.GoToDefinitionFactory(
                    commandArgs.ModelResourceUri,
                    commandArgs.ViewModelKey,
                    commandArgs));
        })
    {
        TextEditorEditFactory = interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            return TextEditorCommandDefaultFunctions.GoToDefinitionFactory(
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                commandArgs);
        }
    };

    public static readonly TextEditorCommand ShowFindAllDialog = new(
        "OpenFindDialog", "defaults_open-find-dialog", false, false, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            return commandArgs.TextEditorService.PostSimpleBatch(
                $"{nameof(ShowFindAllDialog)}_{commandArgs.ViewModelKey}",
                $"{nameof(ShowFindAllDialog)}_{commandArgs.ViewModelKey}",
				null,
                TextEditorCommandDefaultFunctions.ShowFindAllDialogFactory(
                    commandArgs.ModelResourceUri,
                    commandArgs.ViewModelKey,
                    commandArgs));
        })
    {
        TextEditorEditFactory = interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            return TextEditorCommandDefaultFunctions.ShowFindAllDialogFactory(
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                commandArgs);
        }
    };

    /// <summary>
    /// <see cref="ShowTooltipByCursorPosition"/> is to fire the @onmouseover event
    /// so to speak. Such that a tooltip appears if one were to have moused over a symbol or etc...
    /// </summary>
    public static readonly TextEditorCommand ShowTooltipByCursorPosition = new(
        "ShowTooltipByCursorPosition", "defaults_show-tooltip-by-cursor-position", false, true, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            return commandArgs.TextEditorService.PostSimpleBatch(
                $"{nameof(ShowTooltipByCursorPosition)}_{commandArgs.ViewModelKey}",
                $"{nameof(ShowTooltipByCursorPosition)}_{commandArgs.ViewModelKey}",
				null,
                TextEditorCommandDefaultFunctions.ShowTooltipByCursorPositionFactory(
                    commandArgs.ModelResourceUri,
                    commandArgs.ViewModelKey,
                    commandArgs));
        })
    {
        TextEditorEditFactory = interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            return TextEditorCommandDefaultFunctions.ShowTooltipByCursorPositionFactory(
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                commandArgs);
        }
    };

    public static readonly TextEditorCommand ShowFindOverlay = new(
        "ShowFindOverlay", "defaults_show-find-overlay", false, true, TextEditKind.None, null,
        interfaceCommandArgs =>
        {
            var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

            return commandArgs.TextEditorService.PostSimpleBatch(
                $"{nameof(ShowFindOverlay)}_{commandArgs.ViewModelKey}",
                $"{nameof(ShowFindOverlay)}_{commandArgs.ViewModelKey}",
				null,
                async editContext =>
                {
                    var viewModelModifier = editContext.GetViewModelModifier(commandArgs.ViewModelKey);

                    if (viewModelModifier is null)
                        return;

                    if (viewModelModifier.ViewModel.ShowFindOverlay &&
                        commandArgs.JsRuntime is not null)
                    {
                        await commandArgs.JsRuntime.GetLuthetusCommonApi()
                            .FocusHtmlElementById(
                                viewModelModifier.ViewModel.FindOverlayId)
                            .ConfigureAwait(false);
                    }
                    else
                    {
                        viewModelModifier.ViewModel = viewModelModifier.ViewModel with
                        {
                            ShowFindOverlay = true,
                        };
                    }
                    
                    return;
                });
        });
}