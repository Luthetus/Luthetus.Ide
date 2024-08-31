using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Keymaps.Models;

public interface IKeymapArgs : ICommandArgs
{
    public Key<KeymapLayer> LayerKey { get; }

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
    // There are minor changes that were made to the copy and pasted code.
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
    public string? Key { get; }

    /// <summary>
    /// Holds a string that identifies the physical key being pressed.
    /// The value is not affected by the current keyboard layout or modifier state, so a particular key will always return the same value.
    /// </summary>
    public string? Code { get; }

    /// <summary>
    /// The location of the key on the device.
    /// </summary>
    public float Location { get; }

    /// <summary>
    /// true if a key has been depressed long enough to trigger key repetition, otherwise false.
    /// </summary>
    public bool Repeat { get; }

    /// <summary>
    /// true if the control key was down when the event was fired. false otherwise.
    /// </summary>
    public bool CtrlKey { get; }

    /// <summary>
    /// true if the shift key was down when the event was fired. false otherwise.
    /// </summary>
    public bool ShiftKey { get; }

    /// <summary>
    /// true if the alt key was down when the event was fired. false otherwise.
    /// </summary>
    public bool AltKey { get; }

    /// <summary>
    /// true if the meta key was down when the event was fired. false otherwise.
    /// </summary>
    public bool MetaKey { get; }

    /// <summary>
    /// Gets or sets the type of the event.
    /// </summary>
    public string? Type { get; }
    #endregion
}
