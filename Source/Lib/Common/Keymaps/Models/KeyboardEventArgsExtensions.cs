using Luthetus.Common.RazorLib.Keys.Models;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.Common.RazorLib.Keymaps.Models;

public static class KeyboardEventArgsExtensions
{
    public static KeymapArgument ToKeymapArgument(this KeyboardEventArgs keyboardEventArgs)
    {
        return new KeymapArgument(
            keyboardEventArgs.Key,
            keyboardEventArgs.Code,
            keyboardEventArgs.ShiftKey,
            keyboardEventArgs.CtrlKey,
            keyboardEventArgs.AltKey,
            keyboardEventArgs.MetaKey,
            Key<KeymapLayer>.Empty);
    }
}