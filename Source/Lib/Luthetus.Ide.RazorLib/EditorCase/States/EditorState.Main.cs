using Fluxor;
using Luthetus.Common.RazorLib.KeyCase;
using Luthetus.TextEditor.RazorLib.Group;
using Luthetus.TextEditor.RazorLib.Group.Models;

namespace Luthetus.Ide.RazorLib.EditorCase.States;

[FeatureState]
public partial class EditorState
{
    public static readonly Key<TextEditorGroup> EditorTextEditorGroupKey = Key<TextEditorGroup>.NewKey();
}