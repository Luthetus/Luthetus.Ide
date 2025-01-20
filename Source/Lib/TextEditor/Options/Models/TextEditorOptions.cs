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
    bool UseMonospaceOptimizations)
{
    public Key<RenderState> RenderStateKey { get; init; } = Key<RenderState>.NewKey();
    
    /// <summary>
    /// Hacky setter on this property in particular because it can be overridden.
    /// And when overridden it causes an object allocation, and this happens frequently enough to be cause for concern.
    /// </summary>
    public Keymap Keymap { get; set; }
}
