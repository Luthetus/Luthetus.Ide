using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Keymaps.Models;

/// <summary>
/// TODO: Chords as part of keymaps (i.e: ({ Ctrl + '[' } + { Ctrl + 's' }) to focus solution explorer
/// </summary>
public class Keymap : IKeymap
{
    private readonly object _syncRoot = new();
    private readonly Dictionary<IKeymapArgs, CommandNoType> _map = new();

    public Keymap(Key<Keymap> key, string displayName)
    {
        Key = key;
        DisplayName = displayName;
    }

    public Key<Keymap> Key { get; } = Key<Keymap>.Empty;
    public string DisplayName { get; } = string.Empty;
    
    public bool TryRegister(IKeymapArgs args, CommandNoType command)
    {
        lock (_syncRoot)
        {
            return _map.TryAdd(args, command);
        }
    }
    
    public bool TryMap(IKeymapArgs args, out CommandNoType command)
    {
        lock (_syncRoot)
        { 
            var success = _map.TryGetValue(args, out var localCommand);
            command = localCommand ?? CommonCommand.Empty;

            return success;
        }
    }

    public List<KeyValuePair<IKeymapArgs, CommandNoType>> GetKeyValuePairList()
    {
        lock (_syncRoot)
        {
            return _map.ToList();
        }
    }
}
