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
    public static KeymapArgsKind GetKeymapArgsKind(
		TextEditorComponentData componentData,
        KeymapArgs keymapArgs,
        bool hasSelection,
        ITextEditorService textEditorService,
        out CommandNoType command)
    {
        var eventIsCommand = CheckIfKeymapArgsMapsToCommand(
			componentData,
            keymapArgs,
            hasSelection,
            textEditorService,
            out Key<KeymapLayer> layerKey,
            out var success,
            out command);

        var eventIsMovement = CheckIfKeyboardEventArgsMapsToMovement(keymapArgs, command);

        var eventIsContextMenu = KeyboardKeyFacts.CheckIsContextMenuEvent(keymapArgs);

        if (eventIsMovement)
            return KeymapArgsKind.Movement;

        if (eventIsContextMenu)
            return KeymapArgsKind.ContextMenu;

        if (eventIsCommand)
            return KeymapArgsKind.Command;

        if (keymapArgs.Key.Length == 1)
        {
        	// Only write text if no modifiers (other than shift) were held at the time of the event.
        	if (!keymapArgs.CtrlKey && !keymapArgs.AltKey && !keymapArgs.MetaKey)
        		return KeymapArgsKind.Text;
        	else
        		return KeymapArgsKind.None;
        }

        return KeymapArgsKind.Other;
    }

    public static bool IsKeyboardEventArgsNoise(KeymapArgs keymapArgs)
    {
        if (keymapArgs.Key == "Shift" ||
            keymapArgs.Key == "Control" ||
            keymapArgs.Key == "Alt" ||
            keymapArgs.Key == "Meta")
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

    public static bool CheckIfKeymapArgsMapsToCommand(
		TextEditorComponentData componentData,
        KeymapArgs keymapArgs,
        bool hasSelection,
        ITextEditorService textEditorService,
        out Key<KeymapLayer> layerKey,
        out bool success,
        out CommandNoType command)
    {
        layerKey = ((ITextEditorKeymap)componentData.Options.Keymap!).GetLayer(hasSelection);

        keymapArgs.LayerKey = layerKey;

        success = ((ITextEditorKeymap)componentData.Options.Keymap!).TryMap(
            keymapArgs,
            componentData,
            out command);

        if (!success && keymapArgs.LayerKey != TextEditorKeymapDefaultFacts.DefaultLayer.Key)
        {
            keymapArgs.LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key;

            _ = ((ITextEditorKeymap)componentData.Options.Keymap!).TryMap(
                keymapArgs,
                componentData,
                out command);
        }

        if (KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE == keymapArgs.Code && keymapArgs.ShiftKey)
            command = TextEditorCommandDefaultFacts.NewLineBelow;

        return command is not null;
    }

    public static bool CheckIfKeyboardEventArgsMapsToMovement(KeymapArgs keymapArgs, CommandNoType command)
    {
        return KeyboardKeyFacts.IsMovementKey(keymapArgs.Key) && command is null;
    }

	public static bool IsAutocompleteMenuInvoker(KeymapArgs keymapArgs)
    {
        // Is {Ctrl + Space} or LetterOrDigit was hit without Ctrl being held
        return keymapArgs.CtrlKey &&
                   keymapArgs.Code == KeyboardKeyFacts.WhitespaceCodes.SPACE_CODE ||
               !keymapArgs.CtrlKey &&
                   !KeyboardKeyFacts.IsWhitespaceCode(keymapArgs.Code) &&
                   !KeyboardKeyFacts.IsMetaKey(keymapArgs);
    }

	public static bool IsSyntaxHighlightingInvoker(KeymapArgs keymapArgs)
    {
        return keymapArgs.Key == ";" ||
               KeyboardKeyFacts.IsWhitespaceCode(keymapArgs.Code) ||
               keymapArgs.CtrlKey && keymapArgs.Key == "s" ||
               keymapArgs.CtrlKey && keymapArgs.Key == "v" ||
               keymapArgs.CtrlKey && keymapArgs.Key == "z" ||
               keymapArgs.CtrlKey && keymapArgs.Key == "y";
    }

    /// <summary>
    /// All keyboardEventArgs that return true from "IsAutocompleteIndexerInvoker"
    /// are to be 1 character long, as well either whitespace or punctuation.
    /// Therefore 1 character behind might be a word that can be indexed.
    /// </summary>
    public static bool IsAutocompleteIndexerInvoker(KeymapArgs keymapArgs)
    {
        return (KeyboardKeyFacts.IsWhitespaceCode(keymapArgs.Code) ||
                    KeyboardKeyFacts.IsPunctuationCharacter(keymapArgs.Key.First())) &&
                !keymapArgs.CtrlKey;
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
