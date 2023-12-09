using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Diffs.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.RazorLib.Diffs.States;

public partial class TextEditorDiffStateTests
{
    public record RegisterAction(
        Key<TextEditorDiffModel> DiffKey,
        Key<TextEditorViewModel> BeforeViewModelKey,
        Key<TextEditorViewModel> AfterViewModelKey);

    public record DisposeAction(Key<TextEditorDiffModel> DiffKey);
}