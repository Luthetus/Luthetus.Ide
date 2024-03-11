using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;
using Luthetus.TextEditor.RazorLib.Keymaps.Models.Defaults;
using Luthetus.TextEditor.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

public static class TextEditorEventsUtils
{
    public static KeyboardEventArgsKind GetKeyboardEventArgsKind(
        KeyboardEventArgs keyboardEventArgs,
        bool hasSelection,
        ITextEditorService textEditorService,
        out CommandNoType command)
    {
        var eventIsCommand = CheckIfKeyboardEventArgsMapsToCommand(
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
        KeyboardEventArgs keyboardEventArgs,
        bool hasSelection,
        ITextEditorService textEditorService,
        out Key<KeymapLayer> layerKey,
        out KeymapArgument keymapArgument,
        out bool success,
        out CommandNoType command)
    {
        layerKey = ((ITextEditorKeymap)textEditorService.OptionsStateWrap.Value.Options.Keymap!).GetLayer(hasSelection);

        keymapArgument = keyboardEventArgs.ToKeymapArgument() with
        {
            LayerKey = layerKey
        };

        success = textEditorService.OptionsStateWrap.Value.Options.Keymap!.Map.TryGetValue(
            keymapArgument,
            out command);

        if (!success && keymapArgument.LayerKey != TextEditorKeymapDefaultFacts.DefaultLayer.Key)
        {
            _ = textEditorService.OptionsStateWrap.Value.Options.Keymap!.Map.TryGetValue(
                keymapArgument with
                {
                    LayerKey = TextEditorKeymapDefaultFacts.DefaultLayer.Key,
                },
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
