using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models.Defaults;
using Luthetus.TextEditor.RazorLib.Keymaps.Models.Vims;

namespace Luthetus.TextEditor.RazorLib.Keymaps.Models;

public static class TextEditorKeymapFacts
{
    public static readonly Keymap DefaultKeymap = new TextEditorKeymapDefault();
    public static readonly Keymap VimKeymap = new TextEditorKeymapVim();

    public static List<Keymap> AllKeymapsList { get; } =
        new()
        {
            DefaultKeymap,
            VimKeymap,
		};
}