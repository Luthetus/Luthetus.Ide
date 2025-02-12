using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.Extensions.DotNet.CompilerServices.Models;

public record struct CompilerServiceEditorState(Key<TextEditorViewModel> TextEditorViewModelKey)
{
    public CompilerServiceEditorState() : this(Key<TextEditorViewModel>.Empty)
    {
        
    }
}
