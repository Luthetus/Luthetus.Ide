using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Keymaps.Models;

public interface IKeymap
{
	/// <summary>Unique identifier for the keymap</summary>
	public Key<Keymap> Key { get; } = Key<Keymap>.Empty;
	/// <summary>User facing name for the keymap</summary>
    public string DisplayName { get; } = string.Empty;

	public bool TryRegister(KeymapArgs args, Command command);
    public bool TryMap(KeymapArgs args, out Command command);
}
