using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;
using Luthetus.TextEditor.RazorLib.Keymaps.Models.Defaults;
using Luthetus.TextEditor.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Microsoft.AspNetCore.Components.Web;
using static Luthetus.TextEditor.RazorLib.TextEditors.Displays.TextEditorViewModelDisplay;

namespace Luthetus.TextEditor.RazorLib.Events;

public static class EventUtils
{
    public static KeyboardEventArgsKind GetKeyboardEventArgsKind(
        TextEditorEvents events,
        KeyboardEventArgs keyboardEventArgs,
        bool hasSelection,
        ITextEditorService textEditorService,
        out CommandNoType command)
    {
        var eventIsCommand = CheckIfKeyboardEventArgsMapsToCommand(
            events,
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

    public static bool CheckIfKeyboardEventArgsIsNoise(KeyboardEventArgs keyboardEventArgs)
    {
        if (keyboardEventArgs.Key == "Shift" ||
            keyboardEventArgs.Key == "Control" ||
            keyboardEventArgs.Key == "Alt" ||
            keyboardEventArgs.Key == "Meta")
        {
            return true;
        }

        if (keyboardEventArgs.CtrlKey && keyboardEventArgs.AltKey)
        {
            // TODO: This if is a hack to fix the keybind: { Ctrl + Alt + S } causing...
            // ...an 's' to be written out when using Vim keymap.
            return true;
        }

        return false;
    }

    public static bool CheckIfKeyboardEventArgsMapsToCommand(
        TextEditorEvents events,
        KeyboardEventArgs keyboardEventArgs,
        bool hasSelection,
        ITextEditorService textEditorService,
        out Key<KeymapLayer> layerKey,
        out KeymapArgument keymapArgument,
        out bool success,
        out CommandNoType command)
    {
        layerKey = ((ITextEditorKeymap)events.Options.Keymap!).GetLayer(hasSelection);

        keymapArgument = keyboardEventArgs.ToKeymapArgument() with
        {
            LayerKey = layerKey
        };

        success = ((ITextEditorKeymap)events.Options.Keymap!).TryMap(
            keyboardEventArgs,
            keymapArgument,
            events,
            out command);

        if (!success && keymapArgument.LayerKey != TextEditorKeymapDefaultFacts.DefaultLayer.Key)
        {
            _ = ((ITextEditorKeymap)events.Options.Keymap!).TryMap(
                keyboardEventArgs,
                keymapArgument with
                {
                    LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key,
                },
                events,
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
}
