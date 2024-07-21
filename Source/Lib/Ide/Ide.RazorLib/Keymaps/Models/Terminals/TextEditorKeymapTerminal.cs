using System.Collections.Immutable;
using Microsoft.AspNetCore.Components.Web;
using Fluxor;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.TextEditor.RazorLib.Options.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models.Defaults;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Luthetus.TextEditor.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.Ide.RazorLib.Keymaps.Models.Defaults;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.CommandLines.Models;
using Luthetus.Ide.RazorLib.Terminals.States;

using Luthetus.TextEditor.RazorLib.Events.Models;

namespace Luthetus.Ide.RazorLib.Keymaps.Models.Terminals;

public class TextEditorKeymapTerminal : Keymap, ITextEditorKeymap
{
    private readonly IState<TerminalState> _terminalStateWrap;
    private readonly Func<Key<Terminal>> _getTerminalKeyFunc;

    public TextEditorKeymapTerminal(
			IState<TerminalState> terminalStateWrap,
            Func<Key<Terminal>> getTerminalKeyFunc)
        : base(
            new Key<Keymap>(Guid.Parse("baf160e1-6b43-494b-99db-0e8c7500facb")),
            "Terminal")
    {
        _terminalStateWrap = terminalStateWrap;
        _getTerminalKeyFunc = getTerminalKeyFunc;
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
		var characterWidthInPixels = textEditorViewModel.CharAndLineMeasurements.CharacterWidth;
		var characterWidthInPixelsInvariantCulture = characterWidthInPixels.ToCssValue();
		return $"width: {characterWidthInPixelsInvariantCulture}px;";
	}

	public bool TryMap(
		KeyboardEventArgs keyboardEventArgs,
		KeymapArgument keymapArgument,
		TextEditorComponentData componentData,
		out CommandNoType? command)
	{
		var terminalKey = _getTerminalKeyFunc.Invoke();
        var commandDisplayName = "Terminal::InterceptDefaultKeymap";

		command = new TextEditorCommand(
			commandDisplayName, "terminal_intercept-default-keymap", false, true, TextEditKind.None, null,
			interfaceCommandArgs =>
			{
				var commandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                commandArgs.TextEditorService.PostUnique(
					nameof(commandDisplayName),
                    async editContext =>
					{
						var modelModifier = editContext.GetModelModifier(commandArgs.ModelResourceUri);
						var viewModelModifier = editContext.GetViewModelModifier(commandArgs.ViewModelKey);
						var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
						var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

						if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
							return;

						// 'fakeEvents' allows for tricking 'OnKeyDownLateBatching' to use the 'TextEditorKeymapDefault'
						var fakeEvents = new TextEditorComponentData(componentData, new TextEditorKeymapDefault());

						var onKeyDown = new OnKeyDownLateBatching(
							fakeEvents,
							keyboardEventArgs,
							commandArgs.ModelResourceUri,
							commandArgs.ViewModelKey)
						{
							EditContext = editContext
						};

						var definiteHasSelection = TextEditorSelectionHelper.HasSelectedText(primaryCursorModifier);
	
			            var definiteKeyboardEventArgsKind = EventUtils.GetKeyboardEventArgsKind(
	                		fakeEvents, keyboardEventArgs, definiteHasSelection, editContext.TextEditorService, out var command);

						var selectionContainsCurrentRow = false;
						var selectionRowCount = 0;

						if (definiteHasSelection)
						{
							var selectionBoundPositionIndices = TextEditorSelectionHelper.GetSelectionBounds(primaryCursorModifier);

                            var selectionBoundRowIndices = TextEditorSelectionHelper.ConvertSelectionOfPositionIndexUnitsToRowIndexUnits(
								modelModifier,
                                selectionBoundPositionIndices);

							if (primaryCursorModifier.LineIndex >= selectionBoundRowIndices.lowerRowIndexInclusive &&
                                primaryCursorModifier.LineIndex < selectionBoundRowIndices.upperRowIndexExclusive)
							{
								selectionContainsCurrentRow = true;
                            }

							selectionRowCount = selectionBoundRowIndices.upperRowIndexExclusive - selectionBoundRowIndices.lowerRowIndexInclusive;
                        }

						if (definiteKeyboardEventArgsKind == KeyboardEventArgsKind.Text ||
							definiteKeyboardEventArgsKind == KeyboardEventArgsKind.Other)
						{
                            // Only the last line of the terminal is editable.
                            if (primaryCursorModifier.LineIndex == modelModifier.LineCount - 1)
							{
                                // Furthermore, if a selection contains more than 1 row,
                                // it would therefore edit a line other than the last.
								if (selectionRowCount == 0 ||
                                    selectionContainsCurrentRow && selectionRowCount == 1)
								{
                                    if (keyboardEventArgs.Code == KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE)
                                    {
                                        // Notify the underlying terminal (tty) that the user wrote to standard out.
                                        var generalTerminal = _terminalStateWrap.Value.TerminalMap[terminalKey];
                                        var terminalCompilerService = (TerminalCompilerService)modelModifier.CompilerService;

                                        if (terminalCompilerService.GetCompilerServiceResourceFor(modelModifier.ResourceUri) is not TerminalResource terminalResource)
                                            return;

                                        // The final entry of the decoration list is hackily being presumed to be a working directory.
                                        var mostRecentWorkingDirectoryText = terminalResource.ManualDecorationTextSpanList.Last();

										var input = new TextEditorTextSpan(
											mostRecentWorkingDirectoryText.EndingIndexExclusive,
											modelModifier.CharCount,
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

										terminalResource.ManualDecorationTextSpanList.Add(terminalResource.TargetFilePathTextSpan);

                                        await editContext.TextEditorService.ModelApi.InsertTextFactory(
                                                modelModifier.ResourceUri,
												viewModelModifier.ViewModel.ViewModelKey,
												"\n",
												CancellationToken.None)
											.Invoke(editContext)
											.ConfigureAwait(false);

                                        generalTerminal.EnqueueCommand(terminalCommand);
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
                                        else if (definiteHasSelection &&
                                                 mostRecentWorkingDirectoryText.StartingIndexInclusive >= TextEditorSelectionHelper.GetSelectionBounds(primaryCursorModifier).lowerPositionIndexInclusive)
                                        {
                                            // Don't let them type
                                        }
										else
										{
                                            await onKeyDown.HandleEvent(CancellationToken.None).ConfigureAwait(false);
                                            terminalCompilerService.ResourceWasModified(terminalResource.ResourceUri, ImmutableArray<TextEditorTextSpan>.Empty);
                                        }
                                    }
                                }
							}
						}
						else if (command is not null)
						{
							if (command.InternalIdentifier == TextEditorCommandDefaultFacts.Copy.InternalIdentifier ||
								command.InternalIdentifier == TextEditorCommandDefaultFacts.PasteCommand.InternalIdentifier ||
								command.InternalIdentifier == TextEditorCommandDefaultFacts.SelectAll.InternalIdentifier ||
								command.InternalIdentifier == TextEditorCommandDefaultFacts.ScrollLineDown.InternalIdentifier ||
								command.InternalIdentifier == TextEditorCommandDefaultFacts.ScrollLineUp.InternalIdentifier ||
								command.InternalIdentifier == TextEditorCommandDefaultFacts.ScrollPageDown.InternalIdentifier ||
								command.InternalIdentifier == TextEditorCommandDefaultFacts.ScrollPageUp.InternalIdentifier ||
								command.InternalIdentifier == TextEditorCommandDefaultFacts.CursorMovePageBottom.InternalIdentifier ||
								command.InternalIdentifier == TextEditorCommandDefaultFacts.CursorMovePageTop.InternalIdentifier ||
								command.InternalIdentifier == TextEditorCommandDefaultFacts.ShowFindOverlay.InternalIdentifier)
                            {
                                await onKeyDown.HandleEvent(CancellationToken.None).ConfigureAwait(false);
                            }
							else
							{
								// Don't let them do the command
							}
						}
						else
						{
							await onKeyDown.HandleEvent(CancellationToken.None).ConfigureAwait(false);

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
