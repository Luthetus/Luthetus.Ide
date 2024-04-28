using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;

namespace Luthetus.TextEditor.RazorLib.Options.Models;

public record TextEditorOptions(
    CommonOptions CommonOptions,
    bool ShowWhitespace,
    bool ShowNewlines,
    int? TextEditorHeightInPixels,
    double CursorWidthInPixels,
    Keymap Keymap,
    bool UseMonospaceOptimizations)
{
    public Key<RenderState> RenderStateKey { get; init; } = Key<RenderState>.NewKey();
}
