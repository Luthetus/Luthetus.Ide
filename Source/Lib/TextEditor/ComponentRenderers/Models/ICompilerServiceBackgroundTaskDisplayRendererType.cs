using Luthetus.Common.RazorLib.BackgroundTasks.Models;

namespace Luthetus.TextEditor.RazorLib.ComponentRenderers.Models;

public interface ICompilerServiceBackgroundTaskDisplayRendererType
{
    public IBackgroundTaskGroup BackgroundTask { get; set; }
}