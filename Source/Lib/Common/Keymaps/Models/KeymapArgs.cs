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
///
/// So, anywhere in the code that I checked for event.code,
/// the 'CapsLock' to 'Escape' setting on Linux works.
///
/// But, anywhere I checked for event.key isn't working now.
///
/// There are various other oddities, for example
/// { Shift + Tab } to 'IndentLess' does not work for me on Linux.
/// It just inserts a '\t' character.
///
/// In Visual Studio Code, when one uses the Vim emulator plugin,
/// I remember that this same issue occurs.
///
/// The fix was something along the lines of changing a .json file
/// to tell the Vim emulator plugin to read the event.key instead of event.code.
///
/// Was this .json setting singling out the 'CapsLock' code in specific to
/// where it would fallback to checking the event.key for 'Escape'?
///
/// Because I need to decide if I am storing the event.key, event.code, or both.
/// I think the correct answer is to store both.
///
/// Because not only is there issues of remapping a keyboard's button to a different event.key,
/// there also are issues relating to typing in another language than English.
///
/// If one types the letter 'n' with an accent "thing" as is done in Spanish at times,
/// or one types a Chinese character, what does the JavaScript event look like?
///
/// As well, if one types in Chinese, and it were to change the event.key, how would
/// a 'copy' keybind like { Ctrl + c } work?
///
/// Presumably the text editor would be reading the event.key, so it can write
/// out the chinese character, but if I then tried { Ctrl + c } to copy a selection
/// of chinese characters I'd need to read the event.code.
///
/// Is it as simple as saying, "No modifier key means text"
/// and "if they hold down Ctrl then check with the code"?
///
/// How do different keyboard layouts work, i.e.: Dvorak or etc...
///
/// I am on Ubuntu at the moment, and the "operating system" appears to be intercepting
/// my keyboard events. Because I cannot move by camel case
/// via the keybind: { Ctrl + Alt + (ArrowLeft | ArrowRight) }.
///
/// Doing keybind activates the operating system's "Multitasking" feature.
/// I found it so infuriating that I had to disable the feature entirely.
/// And yet I still cannot use the keybind.
///
/// The point is though, would the IDE need the ability to intercept keyboard events
/// in a similar way? If so, how would this be done?
///
/// Maybe it could be done with a custom Blazor event that originates from an 'onkeydown' event.
/// </summary>
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
