namespace Luthetus.Common.RazorLib.Keymaps.Models;

/// <summary>
/// The list provided should not be modified after passing it as a parameter..
/// Make a shallow copy, and pass the shallow copy, if further modification of your list will be necessary.
///
/// ---
///
/// Use this state to lookup a <see cref="KeymapLayer"> to determine the 'when' clause of the keybind.
/// If a <see cref="KeymapLayer"> is used, but isn't registered in this state, it will still function properly
/// but the 'when' clause cannot be shown when the user inspects the keybind in the keymap.
/// </summary>
public record struct KeymapState(List<KeymapLayer> KeymapLayerList)
{
    public KeymapState() : this(new List<KeymapLayer>())
    {
    }
}
