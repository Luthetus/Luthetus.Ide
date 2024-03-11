using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

public interface ITextEditorTask : IBackgroundTask
{
	public Task InvokeWithEditContext(ITextEditorEditContext editContext);
}
