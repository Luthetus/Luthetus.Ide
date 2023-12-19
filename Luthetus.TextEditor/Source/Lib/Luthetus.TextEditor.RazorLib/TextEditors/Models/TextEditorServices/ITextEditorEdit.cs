namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

public interface ITextEditorEdit
{
    public Task ExecuteAsync(ITextEditorEditContext editContext);
}

