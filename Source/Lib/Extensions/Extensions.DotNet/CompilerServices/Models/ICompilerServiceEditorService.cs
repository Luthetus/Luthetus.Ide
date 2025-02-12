using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.Extensions.DotNet.CompilerServices.Models;

public interface ICompilerServiceEditorService
{
	public event Action? CompilerServiceEditorStateChanged;
	
	public CompilerServiceEditorState GetCompilerServiceEditorState();

    public void ReduceSetTextEditorViewModelKeyAction(Key<TextEditorViewModel> textEditorViewModelKey);
}
