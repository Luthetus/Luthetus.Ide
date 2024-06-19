using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
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

            commandArgs.TextEditorService.PostTakeMostRecent(
                nameof(Copy),
				commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                TextEditorCommandDefaultFunctions.CopyFactory(
                    commandArgs.ModelResourceUri,
                    commandArgs.ViewModelKey,
                    commandArgs));
			return Task.CompletedTask;
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

            commandArgs.TextEditorService.PostSimpleBatch(
                nameof(Cut),
                TextEditorCommandDefaultFunctions.CutFactory(
                    commandArgs.ModelResourceUri,
                    commandArgs.ViewModelKey,
                    commandArgs));
			return Task.CompletedTask;
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

            commandArgs.TextEditorService.PostSimpleBatch(
                nameof(PasteCommand),
                TextEditorCommandDefaultFunctions.PasteFactory(
                    commandArgs.ModelResourceUri,
                    commandArgs.ViewModelKey,
                    commandArgs));
			return Task.CompletedTask;
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

            commandArgs.TextEditorService.PostTakeMostRecent(
                nameof(Save),
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                TextEditorCommandDefaultFunctions.SaveFactory(
                    commandArgs.ModelResourceUri,
                    commandArgs.ViewModelKey,
                    commandArgs));
			return Task.CompletedTask;
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

            commandArgs.TextEditorService.PostTakeMostRecent(
                nameof(SelectAll),
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                TextEditorCommandDefaultFunctions.SelectAllFactory(
                    commandArgs.ModelResourceUri,
                    commandArgs.ViewModelKey,
                    commandArgs));
			return Task.CompletedTask;
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

            commandArgs.TextEditorService.PostSimpleBatch(
                nameof(Undo),
                TextEditorCommandDefaultFunctions.UndoFactory(
                    commandArgs.ModelResourceUri,
                    commandArgs.ViewModelKey,
                    commandArgs));
			return Task.CompletedTask;
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

            commandArgs.TextEditorService.PostSimpleBatch(
                nameof(Redo),
                TextEditorCommandDefaultFunctions.RedoFactory(
                    commandArgs.ModelResourceUri,
                    commandArgs.ViewModelKey,
                    commandArgs));
			return Task.CompletedTask;
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

            commandArgs.TextEditorService.PostTakeMostRecent(
                nameof(Remeasure),
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                TextEditorCommandDefaultFunctions.RemeasureFactory(
                    commandArgs.ModelResourceUri,
                    commandArgs.ViewModelKey,
                    commandArgs));
			return Task.CompletedTask;
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

            commandArgs.TextEditorService.PostSimpleBatch(
                nameof(ScrollLineDown),
                TextEditorCommandDefaultFunctions.ScrollLineDownFactory(
                    commandArgs.ModelResourceUri,
                    commandArgs.ViewModelKey,
                    commandArgs));
			return Task.CompletedTask;
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

            commandArgs.TextEditorService.PostSimpleBatch(
                nameof(ScrollLineUp),
                TextEditorCommandDefaultFunctions.ScrollLineUpFactory(
                    commandArgs.ModelResourceUri,
                    commandArgs.ViewModelKey,
                    commandArgs));
			return Task.CompletedTask;
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

            commandArgs.TextEditorService.PostSimpleBatch(
                nameof(ScrollPageDown),
                TextEditorCommandDefaultFunctions.ScrollPageDownFactory(
                    commandArgs.ModelResourceUri,
                    commandArgs.ViewModelKey,
                    commandArgs));
			return Task.CompletedTask;
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

            commandArgs.TextEditorService.PostSimpleBatch(
                nameof(ScrollPageUp),
                TextEditorCommandDefaultFunctions.ScrollPageUpFactory(
                    commandArgs.ModelResourceUri,
                    commandArgs.ViewModelKey,
                    commandArgs));
			return Task.CompletedTask;
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

            commandArgs.TextEditorService.PostTakeMostRecent(
                nameof(CursorMovePageBottom),
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                TextEditorCommandDefaultFunctions.CursorMovePageBottomFactory(
                    commandArgs.ModelResourceUri,
                    commandArgs.ViewModelKey,
                    commandArgs));
			return Task.CompletedTask;
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

            commandArgs.TextEditorService.PostTakeMostRecent(
                nameof(CursorMovePageTop),
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                TextEditorCommandDefaultFunctions.CursorMovePageTopFactory(
                    commandArgs.ModelResourceUri,
                    commandArgs.ViewModelKey,
                    commandArgs));
			return Task.CompletedTask;
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

            commandArgs.TextEditorService.PostSimpleBatch(
                nameof(Duplicate),
                TextEditorCommandDefaultFunctions.DuplicateFactory(
                    commandArgs.ModelResourceUri,
                    commandArgs.ViewModelKey,
                    commandArgs));
			return Task.CompletedTask;
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

            commandArgs.TextEditorService.PostSimpleBatch(
                nameof(IndentMore),
                TextEditorCommandDefaultFunctions.IndentMoreFactory(
                    commandArgs.ModelResourceUri,
                    commandArgs.ViewModelKey,
                    commandArgs));
			return Task.CompletedTask;
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

            commandArgs.TextEditorService.PostSimpleBatch(
                nameof(IndentLess),
                TextEditorCommandDefaultFunctions.IndentLessFactory(
                    commandArgs.ModelResourceUri,
                    commandArgs.ViewModelKey,
                    commandArgs));
			return Task.CompletedTask;
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

            commandArgs.TextEditorService.PostTakeMostRecent(
                nameof(ClearTextSelection),
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                TextEditorCommandDefaultFunctions.ClearTextSelectionFactory(
                    commandArgs.ModelResourceUri,
                    commandArgs.ViewModelKey,
                    commandArgs));
			return Task.CompletedTask;
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

            commandArgs.TextEditorService.PostSimpleBatch(
                nameof(NewLineBelow),
                TextEditorCommandDefaultFunctions.NewLineBelowFactory(
                    commandArgs.ModelResourceUri,
                    commandArgs.ViewModelKey,
                    commandArgs));
			return Task.CompletedTask;
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

            commandArgs.TextEditorService.PostSimpleBatch(
                nameof(NewLineAbove),
                TextEditorCommandDefaultFunctions.NewLineAboveFactory(
                    commandArgs.ModelResourceUri,
                    commandArgs.ViewModelKey,
                    commandArgs));
			return Task.CompletedTask;
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

            commandArgs.TextEditorService.PostSimpleBatch(
                nameof(GoToMatchingCharacterFactory),
                TextEditorCommandDefaultFunctions.GoToMatchingCharacterFactory(
                    commandArgs.ModelResourceUri,
                    commandArgs.ViewModelKey,
                    commandArgs));
			return Task.CompletedTask;
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

            commandArgs.TextEditorService.PostTakeMostRecent(
                nameof(GoToDefinition),
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                TextEditorCommandDefaultFunctions.GoToDefinitionFactory(
                    commandArgs.ModelResourceUri,
                    commandArgs.ViewModelKey,
                    commandArgs));
			return Task.CompletedTask;
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

            commandArgs.TextEditorService.PostTakeMostRecent(
                nameof(ShowFindAllDialog),
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                TextEditorCommandDefaultFunctions.ShowFindAllDialogFactory(
                    commandArgs.ModelResourceUri,
                    commandArgs.ViewModelKey,
                    commandArgs));
			return Task.CompletedTask;
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

            commandArgs.TextEditorService.PostTakeMostRecent(
                nameof(ShowTooltipByCursorPosition),
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                TextEditorCommandDefaultFunctions.ShowTooltipByCursorPositionFactory(
                    commandArgs.ModelResourceUri,
                    commandArgs.ViewModelKey,
                    commandArgs));
			return Task.CompletedTask;
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

            commandArgs.TextEditorService.PostTakeMostRecent(
                nameof(ShowFindOverlay),
                commandArgs.ModelResourceUri,
                commandArgs.ViewModelKey,
                async editContext =>
                {
                    var viewModelModifier = editContext.GetViewModelModifier(commandArgs.ViewModelKey);

                    if (viewModelModifier is null)
                        return;

                    if (viewModelModifier.ViewModel.ShowFindOverlay)
                    {
                        await commandArgs.ServiceProvider.GetRequiredService<IJSRuntime>().GetLuthetusCommonApi()
                            .FocusHtmlElementById(viewModelModifier.ViewModel.FindOverlayId)
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
			return Task.CompletedTask;
        });
}