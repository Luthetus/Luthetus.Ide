namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

public interface ITextEditorEditContext
{
    public Task ExecuteAsync(ITextEditorEdit edit);
}

