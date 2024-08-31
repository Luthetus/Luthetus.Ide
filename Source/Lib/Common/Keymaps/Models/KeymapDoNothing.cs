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
    
    public bool TryMap(IKeymapArgs args, out CommandNoType command)
    {
        command = CommonCommand.Empty;
        return false;
    }

    public List<KeyValuePair<IKeymapArgs, CommandNoType>> GetKeyValuePairList()
    {
        return new();
    }
}
