using Luthetus.Common.RazorLib.Keymaps.Models;

namespace Luthetus.TextEditor.Tests.Basis.Keymaps.Models.Vims;

public record VimGrammarTokenTests(
    VimGrammarKind VimGrammarKind,
    KeymapArgument KeymapArgument);