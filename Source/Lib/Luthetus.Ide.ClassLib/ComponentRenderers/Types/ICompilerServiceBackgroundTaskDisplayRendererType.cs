using Luthetus.Common.RazorLib.BackgroundTaskCase.BaseTypes;

namespace Luthetus.Ide.ClassLib.ComponentRenderers.Types;

public interface ICompilerServiceBackgroundTaskDisplayRendererType
{
    public IBackgroundTask BackgroundTask { get; set; }
}
