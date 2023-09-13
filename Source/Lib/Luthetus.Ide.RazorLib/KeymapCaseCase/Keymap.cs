using Luthetus.Ide.ClassLib.CommandCase;

namespace Luthetus.Ide.ClassLib.KeymapCase;

public class Keymap
{
    public static readonly Keymap Empty = new Keymap();

    public Dictionary<KeymapArgument, ICommand> Map { get; set; } = new();
}
