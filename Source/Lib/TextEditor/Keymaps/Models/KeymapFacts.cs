using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models.Defaults;

namespace Luthetus.TextEditor.RazorLib.Keymaps.Models;

public static class TextEditorKeymapFacts
{
    public static readonly Keymap DefaultKeymap = new TextEditorKeymapDefault();

    public static List<Keymap> AllKeymapsList { get; } =
        new()
        {
            DefaultKeymap,
		};
}