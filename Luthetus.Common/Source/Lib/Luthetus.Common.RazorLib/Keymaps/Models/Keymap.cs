using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using System.Text.Json.Serialization;

namespace Luthetus.Common.RazorLib.Keymaps.Models;

public class Keymap
{
    public static readonly Keymap Empty = new Keymap(Key<Keymap>.Empty, string.Empty);

    public Keymap(Key<Keymap> key, string displayName)
    {
        Key = key;
        DisplayName = displayName;
    }

    /// <summary>This constructor is used for JSON deserialization</summary>
    [Obsolete("This constructor is used for JSON deserialization")]
    public Keymap()
    {
    }

    [property: JsonIgnore]
    public Dictionary<KeymapArgument, CommandNoType> Map { get; set; } = new();
    public Key<Keymap> Key { get; set; } = Key<Keymap>.Empty;
    public string DisplayName { get; set; } = string.Empty;
}
