using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using System;
using System.Collections.Generic;

namespace Luthetus.Common.RazorLib.Keymaps.Models;

/// <summary>
/// TODO: Chords as part of keymaps (i.e: ({ Ctrl + '[' } + { Ctrl + 's' }) to focus solution explorer
/// </summary>
public class Keymap : IKeymap
{
    protected readonly object _syncRoot = new();
    protected readonly Dictionary<string, List<Keybind>> _mapKey = new();
    protected readonly Dictionary<string, List<Keybind>> _mapCode = new();

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
            var keybind = new Keybind(args, command);

            if (args.Key is not null)
            {
                if (!_mapKey.ContainsKey(args.Key))
                    _mapKey.Add(args.Key, new());

                _mapKey[args.Key].Add(keybind);
            }

            if (args.Code is not null)
            {
                if (!_mapCode.ContainsKey(args.Code))
                    _mapCode.Add(args.Code, new());

                _mapCode[args.Code].Add(keybind);
            }
        }

        return true;
    }

    public (List<Keybind>? keyMatchList, List<Keybind>? codeMatchList) MapAll(IKeymapArgs args)
    {
        List<Keybind>? keyMatchList = null;
        List<Keybind>? codeMatchList = null;

        lock (_syncRoot)
        {
            if (args.Key is not null)
                _ = _mapKey.TryGetValue(args.Key, out keyMatchList);

            if (args.Code is not null)
                _ = _mapKey.TryGetValue(args.Code, out codeMatchList);
        }

        return (keyMatchList, codeMatchList);
    }

    public bool MapFirstOrDefault(IKeymapArgs args, out CommandNoType? command)
    {
        List<Keybind>? keyMatchList = null;
        List<Keybind>? codeMatchList = null;

        lock (_syncRoot)
        {
            if (args.Key is not null)
                _ = _mapKey.TryGetValue(args.Key, out keyMatchList);

            if (args.Code is not null)
                _ = _mapKey.TryGetValue(args.Code, out codeMatchList);
        }

        if (keyMatchList is not null)
        {
            foreach (var keybind in keyMatchList)
            {
                if (keybind.KeymapArgs == args)
                {
                    command = keybind.CommandNoType;
                    return true;
                }
            }
        }

        if (codeMatchList is not null)
        {
            foreach (var keybind in codeMatchList)
            {
                if (keybind.KeymapArgs == args)
                {
                    command = keybind.CommandNoType;
                    return true;
                }
            }
        }

        command = null;
        return false;
    }

    public List<KeyValuePair<IKeymapArgs, CommandNoType>> GetKeyValuePairList()
    {
        var combinedList = new List<KeyValuePair<IKeymapArgs, CommandNoType>>();

        lock (_syncRoot)
        {
            var mapKeyList = _mapKey.Values
                .SelectMany(x => x.Select(y => new KeyValuePair<IKeymapArgs, CommandNoType>(y.KeymapArgs, y.CommandNoType)))
                .ToList();

            var mapCodeList = _mapCode.Values
                .SelectMany(x => x.Select(y => new KeyValuePair<IKeymapArgs, CommandNoType>(y.KeymapArgs, y.CommandNoType)))
                .ToList();
            
            combinedList.AddRange(mapKeyList);
            combinedList.AddRange(mapCodeList);
        }

        return combinedList;
    }

    public List<KeyValuePair<IKeymapArgs, CommandNoType>> GetKeyKeyValuePairList()
    {
        lock (_syncRoot)
        {
            return _mapKey.Values
                .SelectMany(x => x.Select(y => new KeyValuePair<IKeymapArgs, CommandNoType>(y.KeymapArgs, y.CommandNoType)))
                .ToList();
        }
    }

    public List<KeyValuePair<IKeymapArgs, CommandNoType>> GetCodeKeyValuePairList()
    {
        lock (_syncRoot)
        {
            return _mapCode.Values
                .SelectMany(x => x.Select(y => new KeyValuePair<IKeymapArgs, CommandNoType>(y.KeymapArgs, y.CommandNoType)))
                .ToList();
        }
    }
}
