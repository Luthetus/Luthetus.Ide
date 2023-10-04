using Luthetus.Common.RazorLib.Keys.Models;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.Common.RazorLib.Keymaps.Models;

public static class KeyboardEventArgsExtensions
{
    public static KeymapArgument ToKeymapArgument(this KeyboardEventArgs keyboardEventArgs)
    {
        return new KeymapArgument(
            keyboardEventArgs.Code,
            keyboardEventArgs.ShiftKey,
            keyboardEventArgs.CtrlKey,
            keyboardEventArgs.AltKey,
            Key<KeymapLayer>.Empty);
    }
}