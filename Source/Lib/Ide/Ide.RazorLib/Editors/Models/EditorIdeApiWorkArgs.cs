using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.Ide.RazorLib.Editors.Models;

public struct EditorIdeApiWorkArgs
{
	public EditorIdeApiWorkKind WorkKind { get; set; }
    public string InputFileAbsolutePathString { get; set; }
    public TextEditorModel TextEditorModel { get; set; }
    public DateTime FileLastWriteTime { get; set; }
    public Key<IDynamicViewModel> NotificationInformativeKey { get; set; }
}
