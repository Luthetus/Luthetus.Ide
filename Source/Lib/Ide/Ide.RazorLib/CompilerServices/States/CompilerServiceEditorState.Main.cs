using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.Ide.RazorLib.CompilerServices.States;

[FeatureState]
public partial record CompilerServiceEditorState(Key<TextEditorViewModel> TextEditorViewModelKey)
{
    public CompilerServiceEditorState() : this(Key<TextEditorViewModel>.Empty)
    {
        
    }
}
