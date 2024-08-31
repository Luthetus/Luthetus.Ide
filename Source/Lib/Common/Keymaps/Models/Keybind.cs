using Luthetus.Common.RazorLib.Exceptions;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Keymaps.Models;

/// <summary>
/// The idea behind this class is entirely based on the 'event.key' vs 'event.code' for a given keyboard event.
/// It is desired to use the 'event.key' sometimes, whereas other times the 'event.code'.
/// So, these corresponding properties on this type were made nullable.
/// It still is incredibly clumsy because the goal is that 1 of them isn't null, yet both will forever be "nullable".
/// 
/// Most of this type is the copy and pasted source code of 'KeyboardEventArgs.cs'
/// https://github.com/dotnet/aspnetcore/blob/3f1acb59718cadf111a0a796681e3d3509bb3381/src/Components/Web/src/Web/KeyboardEventArgs.cs
/// 
/// The difference being that the copy and pasted code was changed to be nullable and immutable,
/// and some extra code was added.
/// </summary>
public struct Keybind
{
    public Keybind(
        Key<KeymapLayer> layerKey,
        string? key,
        string? code,
        float location,
        bool repeat,
        bool ctrlKey,
        bool shiftKey,
        bool altKey,
        bool metaKey,
        string type)
    {
        LayerKey = layerKey;
        Key = key;
        Code = code;
        Location = location;
        Repeat = repeat;
        CtrlKey = ctrlKey;
        ShiftKey = shiftKey;
        AltKey = altKey;
        MetaKey = metaKey;
        Type = type;

        if (Key is null && Code is null)
            throw new LuthetusCommonException("Key is null && Code is null; must pick only one to provide");
        
        if (Key is not null && Code is not null)
            throw new LuthetusCommonException("Key is null && Code is null; must pick only one to provide");
    }

    public Key<KeymapLayer> LayerKey { get; }

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
    public string Type { get; }
    
    public override bool Equals(object? obj)
    {
        // Presumably,
        // a comparison against KeymapArgs is far more common to occur as it happens during a TryMap,
        // whereas a comparison against Keybind happens during TryRegister.
        if (obj is KeymapArgs keymapArgs)
        {
            return
                (LayerKey == keymapArgs.LayerKey) &&
                (Key is null || Key == keymapArgs.Key) &&
                (Code is null || Code == keymapArgs.Code) &&
                (CtrlKey == keymapArgs.CtrlKey) &&
                (ShiftKey == keymapArgs.ShiftKey) &&
                (AltKey == keymapArgs.AltKey) &&
                (MetaKey == keymapArgs.MetaKey);
        }
        else if (obj is Keybind other)
        {
            return
                (LayerKey == other.LayerKey) &&
                (Key is null || Key == other.Key) &&
                (Code is null || Code == other.Code) &&
                (CtrlKey == other.CtrlKey) &&
                (ShiftKey == other.ShiftKey) &&
                (AltKey == other.AltKey) &&
                (MetaKey == other.MetaKey);
        }

        return false;
    }

    public override int GetHashCode()
    {
        if (Key is not null)
            return Key.GetHashCode();

        if (Code is not null)
            return Code.GetHashCode();
        
        return string.Empty.GetHashCode();
    }
}
