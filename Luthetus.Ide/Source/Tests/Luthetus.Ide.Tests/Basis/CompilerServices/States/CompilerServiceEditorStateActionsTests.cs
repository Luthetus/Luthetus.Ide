using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.Ide.Tests.Basis.CompilerServices.States;

public class CompilerServiceEditorStateActionsTests
{
    public record SetTextEditorViewModelKeyAction(Key<TextEditorViewModel> TextEditorViewModelKey);
}
