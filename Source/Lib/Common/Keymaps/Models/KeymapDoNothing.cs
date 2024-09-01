using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Keymaps.Models;

public class KeymapDoNothing : IKeymap
{
    public Key<Keymap> Key => Key<Keymap>.Empty;
    public string DisplayName => nameof(KeymapDoNothing);

    public bool TryRegister(KeymapArgs args, CommandNoType command)
    {
        return false;
    }

    public (bool KeyWasRegistered, bool CodeWasRegistered) TryRegisterRequireBothKeyAndCodeEquality(KeymapArgs args, CommandNoType command)
    {
        return (false, false);
    }

    public (List<Keybind>? keyMatchList, List<Keybind>? codeMatchList) MapAll(KeymapArgs args)
    {
        return (null, null);
    }

    public bool MapFirstOrDefault(KeymapArgs args, out CommandNoType? command)
    {
        command = null;
        return false;
    }

    public List<KeyValuePair<KeymapArgs, CommandNoType>> GetKeyValuePairList()
    {
        return new();
    }

    public List<KeyValuePair<KeymapArgs, CommandNoType>> GetKeyKeyValuePairList()
    {
        return new();
    }

    public List<KeyValuePair<KeymapArgs, CommandNoType>> GetCodeKeyValuePairList()
    {
        return new();
    }
}
