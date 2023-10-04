using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.RazorLib.Diffs.Models;

public record TextEditorDiffModel(
    Key<TextEditorDiffModel> DiffKey,
    Key<TextEditorViewModel> BeforeViewModelKey,
    Key<TextEditorViewModel> AfterViewModelKey)
{
    public Key<RenderState> RenderStateKey { get; init; } = Key<RenderState>.NewKey();
}