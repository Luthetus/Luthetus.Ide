using Luthetus.Ide.RazorLib.CommandCase.Models;

namespace Luthetus.Ide.RazorLib.KeymapCase.Models;

public class Keymap
{
    public static readonly Keymap Empty = new Keymap();

    public Dictionary<KeymapArgument, ICommand> Map { get; set; } = new();
}
