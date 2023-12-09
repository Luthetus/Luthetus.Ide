using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.Tests.Basis.Diffs.Models;

public record TextEditorDiffModelTests(
    Key<TextEditorDiffModel> DiffKey,
    Key<TextEditorViewModel> InViewModelKey,
    Key<TextEditorViewModel> OutViewModelKey)
{
    public Key<RenderState> RenderStateKey { get; init; } = Key<RenderState>.NewKey();
}