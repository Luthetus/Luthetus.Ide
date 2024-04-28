using Luthetus.Common.RazorLib.BackgroundTasks.Models;

namespace Luthetus.TextEditor.RazorLib.ComponentRenderers.Models;

public interface ICompilerServiceBackgroundTaskDisplayRendererType
{
    public IBackgroundTask BackgroundTask { get; set; }
}