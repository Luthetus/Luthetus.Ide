using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Keymaps.Models;

public interface IKeymap
{
	/// <summary>Unique identifier for the keymap</summary>
	public Key<Keymap> Key { get; }
	/// <summary>User facing name for the keymap</summary>
    public string DisplayName { get; }

    public bool TryRegister(KeymapArgs args, CommandNoType command);
    public (List<Keybind>? keyMatchList, List<Keybind>? codeMatchList) MapAll(KeymapArgs args);
    public bool MapFirstOrDefault(KeymapArgs args, out CommandNoType? command);
    public List<KeyValuePair<KeymapArgs, CommandNoType>> GetKeyValuePairList();
    public List<KeyValuePair<KeymapArgs, CommandNoType>> GetKeyKeyValuePairList();
    public List<KeyValuePair<KeymapArgs, CommandNoType>> GetCodeKeyValuePairList();

    public static readonly IKeymap Empty = new KeymapDoNothing();
}
