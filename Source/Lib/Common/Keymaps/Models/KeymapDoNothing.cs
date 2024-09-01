using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Keymaps.Models;

public class KeymapDoNothing : IKeymap
{
    public Key<Keymap> Key => Key<Keymap>.Empty;
    public string DisplayName => nameof(KeymapDoNothing);

    public bool TryRegister(IKeymapArgs args, CommandNoType command)
    {
        return false;
    }

    public (bool KeyWasRegistered, bool CodeWasRegistered) TryRegisterRequireBothKeyAndCodeEquality(IKeymapArgs args, CommandNoType command)
    {
        return (false, false);
    }

    public (List<Keybind>? keyMatchList, List<Keybind>? codeMatchList) MapAll(IKeymapArgs args)
    {
        return (null, null);
    }

    public bool MapFirstOrDefault(IKeymapArgs args, out CommandNoType? command)
    {
        command = null;
        return false;
    }

    public List<KeyValuePair<IKeymapArgs, CommandNoType>> GetKeyValuePairList()
    {
        return new();
    }

    public List<KeyValuePair<IKeymapArgs, CommandNoType>> GetKeyKeyValuePairList()
    {
        return new();
    }

    public List<KeyValuePair<IKeymapArgs, CommandNoType>> GetCodeKeyValuePairList()
    {
        return new();
    }
}
