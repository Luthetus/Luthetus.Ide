using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.Ide.RazorLib.InputFiles.Models;
using Luthetus.Ide.RazorLib.Shareds.Displays;

namespace Luthetus.Ide.RazorLib.BackgroundTasks.Models;

public struct IdeBackgroundTaskApiWorkArgs
{
	public IdeBackgroundTaskApiWorkKind WorkKind { get; set; }
    public IdeHeader IdeHeader { get; set; }
    public string InputFileAbsolutePathString { get; set; }
    public TextEditorModel TextEditorModel { get; set; }
    public DateTime FileLastWriteTime { get; set; }
    public Key<IDynamicViewModel> NotificationInformativeKey { get; set; }
    public AbsolutePath AbsolutePath { get; set; }
    public string Content { get; set; }
    public Func<DateTime?, Task> OnAfterSaveCompletedWrittenDateTimeFunc { get; set; }
    public CancellationToken CancellationToken { get; set; }
    
    public string Message { get; set; }
    public Func<AbsolutePath, Task> OnAfterSubmitFunc { get; set; }
    public Func<AbsolutePath, Task<bool>> SelectionIsValidFunc { get; set; }
    public List<InputFilePattern> InputFilePatterns { get; set; }
}
