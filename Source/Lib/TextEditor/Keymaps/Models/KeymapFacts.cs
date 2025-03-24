using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models.Defaults;

namespace Luthetus.TextEditor.RazorLib.Keymaps.Models;

public static class TextEditorKeymapFacts
{
    public static readonly ITextEditorKeymap DefaultKeymap = new TextEditorKeymapDefault();

    public static List<ITextEditorKeymap> AllKeymapsList { get; } =
        new()
        {
            DefaultKeymap,
		};
}