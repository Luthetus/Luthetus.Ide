using Luthetus.Ide.RazorLib.Shareds.Displays;

namespace Luthetus.Ide.RazorLib.BackgroundTasks.Models;

public struct IdeBackgroundTaskApiWorkArgs
{
	public IdeBackgroundTaskApiWorkKind WorkKind { get; set; }
    public IdeHeader IdeHeader { get; set; }
}
