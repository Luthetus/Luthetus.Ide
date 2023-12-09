using Luthetus.Common.RazorLib.Keymaps.Models;

namespace Luthetus.TextEditor.RazorLib.Keymaps.Models.Vims;

public record VimGrammarTokenTests(
    VimGrammarKind VimGrammarKind,
    KeymapArgument KeymapArgument);