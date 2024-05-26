using Luthetus.Common.RazorLib.FileSystems.Models;

namespace Luthetus.Ide.RazorLib.ComponentRenderers.Models;

public interface IRemoveCSharpProjectFromSolutionRendererType
{
    public IAbsolutePath AbsolutePath { get; set; }
    public Func<IAbsolutePath, Task> OnAfterSubmitFunc { get; set; }
}