using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;

public interface ITextEditorTask : IBackgroundTask
{
	public string? Redundancy { get; }
	public TextEditorEdit Edit { get; }

    public Task InvokeWithEditContext(IEditContext editContext);
}
