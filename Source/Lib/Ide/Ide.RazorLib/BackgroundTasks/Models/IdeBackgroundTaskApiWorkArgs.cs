using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
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
}
