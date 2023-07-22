using Luthetus.Common.RazorLib.BackgroundTaskCase.BaseTypes;

namespace Luthetus.Ide.ClassLib.ComponentRenderers.Types;

public interface IFileSystemBackgroundTaskDisplayRendererType
{
    public IBackgroundTask BackgroundTask { get; set; }
}