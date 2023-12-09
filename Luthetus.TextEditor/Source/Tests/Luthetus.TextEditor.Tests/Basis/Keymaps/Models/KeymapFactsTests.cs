using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.Keymaps.Models.Defaults;
using Luthetus.TextEditor.RazorLib.Keymaps.Models.Vims;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.Tests.Basis.Keymaps.Models;

public static class TextEditorKeymapFactsTests
{
    public static readonly Keymap DefaultKeymap = new TextEditorKeymapDefault();

    public static readonly Keymap VimKeymap = new TextEditorKeymapVim();

    public static ImmutableArray<Keymap> AllKeymapsBag =>
        new Keymap[]
        {
            DefaultKeymap,
            VimKeymap
        }.ToImmutableArray();
}