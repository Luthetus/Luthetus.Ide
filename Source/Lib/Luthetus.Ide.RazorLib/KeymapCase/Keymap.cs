using Luthetus.Ide.RazorLib.CommandCase;

namespace Luthetus.Ide.RazorLib.KeymapCase;

public class Keymap
{
    public static readonly Keymap Empty = new Keymap();

    public Dictionary<KeymapArgument, ICommand> Map { get; set; } = new();
}
