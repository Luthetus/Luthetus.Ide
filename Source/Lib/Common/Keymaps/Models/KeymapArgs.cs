using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Microsoft.AspNetCore.Components.Web;
using System.Diagnostics.CodeAnalysis;

namespace Luthetus.Common.RazorLib.Keymaps.Models;

/// <summary>
/// Goal: Better Keymap code (2024-08-30)
/// =====================================
/// The immediate reason for wanting better keymap code,
/// is that on Linux the 'CapsLock' to 'Escape' setting is returning:
///     event.key == Escape
///     BUT 
///     event.code == CapsLock
public struct KeymapArgs : IKeymapArgs
{
	public KeymapArgs()
	{
	}

	public KeymapArgs(KeyboardEventArgs keyboardEventArgs)
    {
        Key = keyboardEventArgs.Key;
	    Code = keyboardEventArgs.Code;
	    Location = keyboardEventArgs.Location;
	    Repeat = keyboardEventArgs.Repeat;
	    CtrlKey = keyboardEventArgs.CtrlKey;
	    ShiftKey = keyboardEventArgs.ShiftKey;
	    AltKey = keyboardEventArgs.AltKey;
	    MetaKey = keyboardEventArgs.MetaKey;
	    Type = keyboardEventArgs.Type;
    }

    public Key<KeymapLayer> LayerKey { get; set; }
    
    public string? Key { get; set; }
    public string? Code { get; set; }
    public float Location { get; set; }
    public bool Repeat { get; set; }
    public bool CtrlKey { get; set; }
    public bool ShiftKey { get; set; }
    public bool AltKey { get; set; }
    public bool MetaKey { get; set; }
    public string? Type { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is not KeymapArgs keymapArgs)
            return false;

        return
            (LayerKey == keymapArgs.LayerKey) &&
            (Key is null || Key == keymapArgs.Key) &&
            (Code is null || Code == keymapArgs.Code) &&
            (CtrlKey == keymapArgs.CtrlKey) &&
            (ShiftKey == keymapArgs.ShiftKey) &&
            (AltKey == keymapArgs.AltKey) &&
            (MetaKey == keymapArgs.MetaKey);
    }

    public override int GetHashCode()
    {
        if (Key is not null)
            return Key.GetHashCode();

        if (Code is not null)
            return Code.GetHashCode();

        return string.Empty.GetHashCode();
    }

    public static bool operator ==(KeymapArgs left, KeymapArgs right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(KeymapArgs left, KeymapArgs right)
    {
        return !(left == right);
    }
}
