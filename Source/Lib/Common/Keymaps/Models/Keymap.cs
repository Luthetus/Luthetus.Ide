using System.Text.Json.Serialization;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Keymaps.Models;

public class Keymap : IKeymap
{
    public Keymap(Key<Keymap> key, string displayName)
    {
        Key = key;
        DisplayName = displayName;
    }

    public Key<Keymap> Key { get; } = Key<Keymap>.Empty;
    public string DisplayName { get; } = string.Empty;
    
    public bool TryRegister(KeymapArgs args, Command command)
    {
    }
    
    public bool TryMap(KeymapArgs args, out Command command)
    {
    }
}
