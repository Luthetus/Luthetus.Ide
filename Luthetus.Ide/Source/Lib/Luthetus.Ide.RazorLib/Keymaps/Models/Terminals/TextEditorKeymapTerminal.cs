using Luthetus.TextEditor.RazorLib.Options.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.Keymaps.Models.Defaults;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals.UiEvent;
using Luthetus.Common.RazorLib.Commands.Models;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using static Luthetus.TextEditor.RazorLib.TextEditors.Displays.TextEditorViewModelDisplay;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models;
using Luthetus.Ide.RazorLib.Keymaps.Models.Defaults;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;
using Luthetus.Ide.RazorLib.CommandLines.Models;
using Fluxor;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;

namespace Luthetus.Ide.RazorLib.Keymaps.Models.Terminals;

public class TextEditorKeymapTerminal : Keymap, ITextEditorKeymap
{
    private readonly IState<TerminalSessionState> _terminalSessionStateWrap;
    private readonly Key<TerminalSession> _terminalSessionKey;

    public TextEditorKeymapTerminal(
			IState<TerminalSessionState> terminalSessionStateWrap,
            Key<TerminalSession> terminalSessionKey)
        : base(
            new Key<Keymap>(Guid.Parse("baf160e1-6b43-494b-99db-0e8c7500facb")),
            "Terminal")
    {
        _terminalSessionStateWrap = terminalSessionStateWrap;
        _terminalSessionKey = terminalSessionKey;
    }

    public Key<KeymapLayer> GetLayer(bool hasSelection)
    {
        return hasSelection
            ? TextEditorKeymapTerminalFacts.HasSelectionLayer.Key
            : TextEditorKeymapTerminalFacts.DefaultLayer.Key;
    }

    public string GetCursorCssClassString()
    {
        return TextCursorKindFacts.BlockCssClassString;
    }

    public string GetCursorCssStyleString(
        TextEditorModel textEditorModel,
        TextEditorViewModel textEditorViewModel,
        TextEditorOptions textEditorOptions)
    {
		var characterWidthInPixels = textEditorViewModel.VirtualizationResult.CharAndRowMeasurements.CharacterWidth;
		var characterWidthInPixelsInvariantCulture = characterWidthInPixels.ToCssValue();
		return $"width: {characterWidthInPixelsInvariantCulture}px;";
	}

	public bool TryMap(
		KeyboardEventArgs keyboardEventArgs,
		KeymapArgument keymapArgument,
		TextEditorEvents events,
		out CommandNoType? command)
	{
		var commandDisplayName = "Terminal::InterceptDefaultKeymap";

		command = new TextEditorCommand(
			commandDisplayName, "terminal_intercept-default-keymap", false, true, TextEditKind.None, null,
			interfaceCommandArgs =>
			{
				var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

				commandArgs.TextEditorService.Post(
					nameof(commandDisplayName),
					async editContext =>
					{
						var modelModifier = editContext.GetModelModifier(commandArgs.ModelResourceUri);
						var viewModelModifier = editContext.GetViewModelModifier(commandArgs.ViewModelKey);
						var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
						var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

						if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
							return;

						var throttleEventOnKeyDown = new ThrottleEventOnKeyDown(
							new TextEditorEvents(events, new TextEditorKeymapDefault()),
							keyboardEventArgs,
							commandArgs.ModelResourceUri,
							commandArgs.ViewModelKey);

						var selectionContainsCurrentRow = false;
						var selectionRowCount = 0;

						if (throttleEventOnKeyDown.TentativeHasSelection)
						{
							var selectionBoundPositionIndices = TextEditorSelectionHelper.GetSelectionBounds(primaryCursorModifier);

                            var selectionBoundRowIndices = TextEditorSelectionHelper.ConvertSelectionOfPositionIndexUnitsToRowIndexUnits(
								modelModifier,
                                selectionBoundPositionIndices);

							if (primaryCursorModifier.RowIndex >= selectionBoundRowIndices.lowerRowIndexInclusive &&
                                primaryCursorModifier.RowIndex < selectionBoundRowIndices.upperRowIndexExclusive)
							{
								selectionContainsCurrentRow = true;
                            }

							selectionRowCount = selectionBoundRowIndices.upperRowIndexExclusive - selectionBoundRowIndices.lowerRowIndexInclusive;
                        }

						if (throttleEventOnKeyDown.TentativeKeyboardEventArgsKind == KeyboardEventArgsKind.Text ||
							throttleEventOnKeyDown.TentativeKeyboardEventArgsKind == KeyboardEventArgsKind.Other)
						{
                            // Only the last line of the terminal is editable.
                            if (primaryCursorModifier.RowIndex == modelModifier.RowCount - 1)
							{
                                // Furthermore, if a selection contains more than 1 row,
                                // it would therefore edit a line other than the last.
								if (selectionRowCount == 0 ||
                                    selectionContainsCurrentRow && selectionRowCount == 1)
								{
                                    if (keyboardEventArgs.Code == KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE)
                                    {
                                        // Notify the underlying terminal (tty) that the user wrote to standard out.

                                        var generalTerminalSession = _terminalSessionStateWrap.Value.TerminalSessionMap[_terminalSessionKey];

                                        var terminalCompilerService = (TerminalCompilerService)modelModifier.CompilerService;

                                        if (terminalCompilerService.GetCompilerServiceResourceFor(modelModifier.ResourceUri) is not TerminalResource terminalResource)
                                            return;

                                        // The final entry of the decoration list is hackily being presumed to be a working directory.
                                        var mostRecentWorkingDirectoryText = terminalResource.ManualDecorationTextSpanList.Last();

										var input = new TextEditorTextSpan(
											mostRecentWorkingDirectoryText.EndingIndexExclusive,
											modelModifier.DocumentLength,
											0,
											modelModifier.ResourceUri,
											modelModifier.GetAllText());

										TextEditorTextSpan targetFilePathTextSpan;
										TextEditorTextSpan argumentsTextSpan;

										lock (terminalResource.UnsafeStateLock)
										{
											targetFilePathTextSpan = terminalResource.TargetFilePathTextSpan;
											argumentsTextSpan = terminalResource.ArgumentsTextSpan;
                                        }

                                        var formattedCommand = new FormattedCommand(
											targetFilePathTextSpan.GetText(),
                                            new string[] { argumentsTextSpan.GetText() });

										formattedCommand.HACK_ArgumentsString = argumentsTextSpan.GetText();

                                        var terminalCommand = new TerminalCommand(
                                            Key<TerminalCommand>.NewKey(),
                                        formattedCommand);

                                        await editContext.TextEditorService.ModelApi.InsertTextFactory(
                                                modelModifier.ResourceUri,
												viewModelModifier.ViewModel.ViewModelKey,
												"\n",
												CancellationToken.None)
											.Invoke(editContext)
											.ConfigureAwait(false);

                                        await generalTerminalSession.EnqueueCommandAsync(terminalCommand);
                                    }
                                    else if (keyboardEventArgs.Code == "Backspace" && primaryCursorModifier.ColumnIndex == 0)
                                    {
                                        // TODO: Console.Beep(); // ???
                                    }
                                    else
                                    {
										// TODO: This method is an "if, else" nightmare and needs to be cleaned up.
										//
										// The "working directory" is written on the last line of the terminal.
										// Ensure that the user is not about to type over the "working directory"

										var terminalCompilerService = (TerminalCompilerService)modelModifier.CompilerService;

                                        var terminalResource = terminalCompilerService.GetCompilerServiceResourceFor(modelModifier.ResourceUri) as TerminalResource;
                                        if (terminalResource is null)
                                            return;

                                        // The final entry of the decoration list is hackily being presumed to be a working directory.
                                        var mostRecentWorkingDirectoryText = terminalResource.ManualDecorationTextSpanList.Last();

										var primaryCursorModifierPositionIndex = modelModifier.GetPositionIndex(primaryCursorModifier);

                                        if (mostRecentWorkingDirectoryText.StartingIndexInclusive >= primaryCursorModifierPositionIndex ||
                                            mostRecentWorkingDirectoryText.EndingIndexExclusive > primaryCursorModifierPositionIndex)
										{
											// Don't let them type
                                        }
										else if (mostRecentWorkingDirectoryText.EndingIndexExclusive == primaryCursorModifierPositionIndex &&
                                                 keyboardEventArgs.Code == "Backspace")
										{
                                            // Don't let them type
                                        }
                                        else if (throttleEventOnKeyDown.TentativeHasSelection &&
                                                 mostRecentWorkingDirectoryText.StartingIndexInclusive >= TextEditorSelectionHelper.GetSelectionBounds(primaryCursorModifier).lowerPositionIndexInclusive)
                                        {
                                            // Don't let them type
                                        }
										else
										{
                                            await throttleEventOnKeyDown.InvokeWithEditContext(editContext);
											terminalCompilerService.ResourceWasModified(terminalResource.ResourceUri, ImmutableArray<TextEditorTextSpan>.Empty);
                                        }
                                    }
                                }
							}
						}
						else if (throttleEventOnKeyDown.Command is not null)
						{
							if (throttleEventOnKeyDown.Command.InternalIdentifier == TextEditorCommandDefaultFacts.Copy.InternalIdentifier ||
								throttleEventOnKeyDown.Command.InternalIdentifier == TextEditorCommandDefaultFacts.PasteCommand.InternalIdentifier ||
								throttleEventOnKeyDown.Command.InternalIdentifier == TextEditorCommandDefaultFacts.SelectAll.InternalIdentifier ||
								throttleEventOnKeyDown.Command.InternalIdentifier == TextEditorCommandDefaultFacts.ScrollLineDown.InternalIdentifier ||
								throttleEventOnKeyDown.Command.InternalIdentifier == TextEditorCommandDefaultFacts.ScrollLineUp.InternalIdentifier ||
								throttleEventOnKeyDown.Command.InternalIdentifier == TextEditorCommandDefaultFacts.ScrollPageDown.InternalIdentifier ||
								throttleEventOnKeyDown.Command.InternalIdentifier == TextEditorCommandDefaultFacts.ScrollPageUp.InternalIdentifier ||
								throttleEventOnKeyDown.Command.InternalIdentifier == TextEditorCommandDefaultFacts.CursorMovePageBottom.InternalIdentifier ||
								throttleEventOnKeyDown.Command.InternalIdentifier == TextEditorCommandDefaultFacts.CursorMovePageTop.InternalIdentifier ||
								throttleEventOnKeyDown.Command.InternalIdentifier == TextEditorCommandDefaultFacts.ShowFindOverlay.InternalIdentifier)
                            {
                                await throttleEventOnKeyDown.InvokeWithEditContext(editContext);
                            }
							else
							{
								// Don't let them do the command
							}
						}
						else
						{
							await throttleEventOnKeyDown.InvokeWithEditContext(editContext);

                            var terminalCompilerService = (TerminalCompilerService)modelModifier.CompilerService;

                            var terminalResource = terminalCompilerService.GetCompilerServiceResourceFor(modelModifier.ResourceUri) as TerminalResource;
                            if (terminalResource is null)
                                return;

                            terminalCompilerService.ResourceWasModified(terminalResource.ResourceUri, ImmutableArray<TextEditorTextSpan>.Empty);
                        }
					});

				return Task.CompletedTask;
			});

		return true;
	}
}
