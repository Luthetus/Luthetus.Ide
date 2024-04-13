using Luthetus.Common.RazorLib.Keymaps.Models;

namespace Luthetus.TextEditor.RazorLib.Keymaps.Models.Vims;

public record VimGrammarToken(
    VimGrammarKind VimGrammarKind,
    KeymapArgument KeymapArgument);