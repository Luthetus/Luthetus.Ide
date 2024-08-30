using Luthetus.Common.RazorLib.Keys.Models;
using Microsoft.AspNetCore.Components.Web;

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
public struct KeymapArgs
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
    
#region Copy_Paste_KeyboardEventArgs_Source_Code
// https://github.com/dotnet/aspnetcore/blob/3f1acb59718cadf111a0a796681e3d3509bb3381/src/Components/Web/src/Web/KeyboardEventArgs.cs
//
// Copy and paste all of KeyboardEventArgs.cs instead of composing a property on KeymapArgs which is of type KeyboardEventArgs.
// This is because KeyboardEventArgs is a class, and I presume its easier on the garbage collector, for me to immediately
// move the data to a struct, and lose reference to the class. Rather than carrying the classes reference everywhere.
//
// As well, don't just use a KeyboardEventArgs type itself because we need more data attached to the KeymapArgs
// such as the LayerKey.
//
// A confusing situation: how to deal with a keybinds which don't "care" about various properties.
// If I want to use a Dictionary to map from a KeymapArgs to a CommandNoType,
// every one of the properties must match if I implement the comparison logic in a simple way (just compare 1 to 1 the properties).
//
// But, for some keybinds I might only want to specify that someone be pressing { Ctrl + c }, and that I do not care
// about the value of 'Location' or 'Repeat'.

    /// <summary>
    /// The key value of the key represented by the event.
    /// If the value has a printed representation, this attribute's value is the same as the char attribute.
    /// Otherwise, it's one of the key value strings specified in 'Key values'.
    /// If the key can't be identified, this is the string "Unidentified"
    /// </summary>
    public string Key { get; set; } = default!;

    /// <summary>
    /// Holds a string that identifies the physical key being pressed.
    /// The value is not affected by the current keyboard layout or modifier state, so a particular key will always return the same value.
    /// </summary>
    public string Code { get; set; } = default!;

    /// <summary>
    /// The location of the key on the device.
    /// </summary>
    public float Location { get; set; }

    /// <summary>
    /// true if a key has been depressed long enough to trigger key repetition, otherwise false.
    /// </summary>
    public bool Repeat { get; set; }

    /// <summary>
    /// true if the control key was down when the event was fired. false otherwise.
    /// </summary>
    public bool CtrlKey { get; set; }

    /// <summary>
    /// true if the shift key was down when the event was fired. false otherwise.
    /// </summary>
    public bool ShiftKey { get; set; }

    /// <summary>
    /// true if the alt key was down when the event was fired. false otherwise.
    /// </summary>
    public bool AltKey { get; set; }

    /// <summary>
    /// true if the meta key was down when the event was fired. false otherwise.
    /// </summary>
    public bool MetaKey { get; set; }

    /// <summary>
    /// Gets or sets the type of the event.
    /// </summary>
    public string Type { get; set; } = default!;
#endregion
}
