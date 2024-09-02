using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Keymaps.Models;

public class KeymapJsonDto
{
	public KeymapJsonDto(IKeymap keymap)
		: this(keymap.Key, keymap.DisplayName)
	{
	}

	public KeymapJsonDto(Key<Keymap> key, string displayName)
	{
		Key = key;
		DisplayName = displayName;
	}

	/// <inheritdoc cref="Keymap.Key"/>
	public Key<Keymap> Key { get; }
    /// <inheritdoc cref="Keymap.DisplayName"/>
    public string DisplayName { get; }
}
