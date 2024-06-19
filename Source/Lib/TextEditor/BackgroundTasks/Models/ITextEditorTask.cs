using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;

public interface ITextEditorTask : IBackgroundTask
{
	public IEditContext EditContext { get; set; }
}
