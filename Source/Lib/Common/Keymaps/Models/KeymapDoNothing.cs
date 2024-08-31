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
    
    public bool TryMap(KeymapArgs args, out CommandNoType command)
    {
        command = CommonCommand.Empty;
        return false;
    }

    public List<KeyValuePair<KeymapArgs, CommandNoType>> GetKeyValuePairList()
    {
        return new();
    }
}
