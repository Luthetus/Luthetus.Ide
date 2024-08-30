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

	/// <summary>Unique identifier for the keymap</summary>
	public Key<Keymap> Key { get; }
	/// <summary>User facing name for the keymap</summary>
    public string DisplayName { get; }
}
