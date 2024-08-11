using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;
using Luthetus.TextEditor.RazorLib.Keymaps.Models.Defaults;
using Luthetus.TextEditor.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

namespace Luthetus.TextEditor.RazorLib.Events.Models;

public static class EventUtils
{
    public static KeyboardEventArgsKind GetKeyboardEventArgsKind(
		TextEditorComponentData componentData,
        KeyboardEventArgs keyboardEventArgs,
        bool hasSelection,
        ITextEditorService textEditorService,
        out CommandNoType command)
    {
        var eventIsCommand = CheckIfKeyboardEventArgsMapsToCommand(
			componentData,
            keyboardEventArgs,
            hasSelection,
            textEditorService,
            out Key<KeymapLayer> layerKey,
            out KeymapArgument keymapArgument,
            out var success,
            out command);

        var eventIsMovement = CheckIfKeyboardEventArgsMapsToMovement(keyboardEventArgs, command);

        var eventIsContextMenu = KeyboardKeyFacts.CheckIsContextMenuEvent(keyboardEventArgs);

        if (eventIsMovement)
            return KeyboardEventArgsKind.Movement;

        if (eventIsContextMenu)
            return KeyboardEventArgsKind.ContextMenu;

        if (eventIsCommand)
            return KeyboardEventArgsKind.Command;

        if (keyboardEventArgs.Key.Length == 1)
            return KeyboardEventArgsKind.Text;

        return KeyboardEventArgsKind.Other;
    }

    public static bool IsKeyboardEventArgsNoise(KeyboardEventArgs keyboardEventArgs)
    {
        if (keyboardEventArgs.Key == "Shift" ||
            keyboardEventArgs.Key == "Control" ||
            keyboardEventArgs.Key == "Alt" ||
            keyboardEventArgs.Key == "Meta")
        {
            return true;
        }

		// TODO: See following code block (its commented out).
		//       The commented out hack was not a good idea,
		//       This comment is here as a reminder not to repeat
		//       this bad solution.
		//       |
		//       i.e.: this is not yet fixed, but don't fix it the way thats commented out below.
		//             Once this is fixed, then delete this comment.
		//       |
		//       The issue was { Ctrl + Alt + (ArrowRight || ArrowLeft) }
		//       to perform "camel case movement of the cursor".
		// {
	        //if (keyboardEventArgs.CtrlKey && keyboardEventArgs.AltKey)
	        //{
	        //    // TODO: This if is a hack to fix the keybind: { Ctrl + Alt + S } causing...
	        //    // ...an 's' to be written out when using Vim keymap.
	        //    return true;
	        //}
        // }

        return false;
    }

    public static bool CheckIfKeyboardEventArgsMapsToCommand(
		TextEditorComponentData componentData,
        KeyboardEventArgs keyboardEventArgs,
        bool hasSelection,
        ITextEditorService textEditorService,
        out Key<KeymapLayer> layerKey,
        out KeymapArgument keymapArgument,
        out bool success,
        out CommandNoType command)
    {
        layerKey = ((ITextEditorKeymap)componentData.Options.Keymap!).GetLayer(hasSelection);

        keymapArgument = keyboardEventArgs.ToKeymapArgument() with
        {
            LayerKey = layerKey
        };

        success = ((ITextEditorKeymap)componentData.Options.Keymap!).TryMap(
            keyboardEventArgs,
            keymapArgument,
            componentData,
            out command);

        if (!success && keymapArgument.LayerKey != TextEditorKeymapDefaultFacts.DefaultLayer.Key)
        {
            _ = ((ITextEditorKeymap)componentData.Options.Keymap!).TryMap(
                keyboardEventArgs,
                keymapArgument with
                {
                    LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key,
                },
                componentData,
                out command);
        }

        if (KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE == keyboardEventArgs.Code && keyboardEventArgs.ShiftKey)
            command = TextEditorCommandDefaultFacts.NewLineBelow;

        return command is not null;
    }

    public static bool CheckIfKeyboardEventArgsMapsToMovement(KeyboardEventArgs keyboardEventArgs, CommandNoType command)
    {
        return KeyboardKeyFacts.IsMovementKey(keyboardEventArgs.Key) && command is null;
    }

	public static bool IsAutocompleteMenuInvoker(KeyboardEventArgs keyboardEventArgs)
    {
        // Is {Ctrl + Space} or LetterOrDigit was hit without Ctrl being held
        return keyboardEventArgs.CtrlKey &&
                   keyboardEventArgs.Code == KeyboardKeyFacts.WhitespaceCodes.SPACE_CODE ||
               !keyboardEventArgs.CtrlKey &&
                   !KeyboardKeyFacts.IsWhitespaceCode(keyboardEventArgs.Code) &&
                   !KeyboardKeyFacts.IsMetaKey(keyboardEventArgs);
    }

	public static bool IsSyntaxHighlightingInvoker(KeyboardEventArgs keyboardEventArgs)
    {
        return keyboardEventArgs.Key == ";" ||
               KeyboardKeyFacts.IsWhitespaceCode(keyboardEventArgs.Code) ||
               keyboardEventArgs.CtrlKey && keyboardEventArgs.Key == "s" ||
               keyboardEventArgs.CtrlKey && keyboardEventArgs.Key == "v" ||
               keyboardEventArgs.CtrlKey && keyboardEventArgs.Key == "z" ||
               keyboardEventArgs.CtrlKey && keyboardEventArgs.Key == "y";
    }

    /// <summary>
    /// All keyboardEventArgs that return true from "IsAutocompleteIndexerInvoker"
    /// are to be 1 character long, as well either whitespace or punctuation.
    /// Therefore 1 character behind might be a word that can be indexed.
    /// </summary>
    public static bool IsAutocompleteIndexerInvoker(KeyboardEventArgs keyboardEventArgs)
    {
        return (KeyboardKeyFacts.IsWhitespaceCode(keyboardEventArgs.Code) ||
                    KeyboardKeyFacts.IsPunctuationCharacter(keyboardEventArgs.Key.First())) &&
                !keyboardEventArgs.CtrlKey;
    }

	public static async Task<(int rowIndex, int columnIndex)> CalculateRowAndColumnIndex(
		ResourceUri resourceUri,
		Key<TextEditorViewModel> viewModelKey,
		MouseEventArgs mouseEventArgs,
		TextEditorComponentData componentData,
		ITextEditorEditContext editContext)
    {
        var modelModifier = editContext.GetModelModifier(resourceUri);
        var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);
        var globalTextEditorOptions = editContext.TextEditorService.OptionsStateWrap.Value.Options;

        if (modelModifier is null || viewModelModifier is null)
            return (0, 0);

        var charMeasurements = viewModelModifier.ViewModel.CharAndLineMeasurements;
		var textEditorDimensions = viewModelModifier.ViewModel.TextEditorDimensions;
		var scrollbarDimensions = viewModelModifier.ViewModel.ScrollbarDimensions;

		var relativeX = mouseEventArgs.ClientX - textEditorDimensions.BoundingClientRectLeft;
        var relativeY = mouseEventArgs.ClientY - textEditorDimensions.BoundingClientRectTop;

        var positionX = relativeX;
        var positionY = relativeY;

        // Scroll position offset
        {
            positionX += scrollbarDimensions.ScrollLeft;
            positionY += scrollbarDimensions.ScrollTop;
        }

        var rowIndex = (int)(positionY / charMeasurements.LineHeight);

        rowIndex = rowIndex > modelModifier.LineCount - 1
            ? modelModifier.LineCount - 1
            : rowIndex;

        int columnIndexInt;


        var columnIndexDouble = positionX / charMeasurements.CharacterWidth;
        columnIndexInt = (int)Math.Round(columnIndexDouble, MidpointRounding.AwayFromZero);

        var lengthOfRow = modelModifier.GetLineLength(rowIndex);

        // Tab key column offset
        {
            var parameterForGetTabsCountOnSameRowBeforeCursor = columnIndexInt > lengthOfRow
                ? lengthOfRow
                : columnIndexInt;

            int tabsOnSameRowBeforeCursor;

            try
            {
                tabsOnSameRowBeforeCursor = modelModifier.GetTabCountOnSameLineBeforeCursor(
                    rowIndex,
                    parameterForGetTabsCountOnSameRowBeforeCursor);
            }
            catch (LuthetusTextEditorException)
            {
                tabsOnSameRowBeforeCursor = 0;
            }

            // 1 of the character width is already accounted for
            var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;

            columnIndexInt -= extraWidthPerTabKey * tabsOnSameRowBeforeCursor;
        }

        columnIndexInt = columnIndexInt > lengthOfRow
            ? lengthOfRow
            : columnIndexInt;

        rowIndex = Math.Max(rowIndex, 0);
        columnIndexInt = Math.Max(columnIndexInt, 0);

        return (rowIndex, columnIndexInt);
    }
}
