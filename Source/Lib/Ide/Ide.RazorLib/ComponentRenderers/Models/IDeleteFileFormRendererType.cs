using Luthetus.Common.RazorLib.FileSystems.Models;

namespace Luthetus.Ide.RazorLib.ComponentRenderers.Models;

public interface IDeleteFileFormRendererType
{
    public IAbsolutePath AbsolutePath { get; set; }
    public bool IsDirectory { get; set; }
    public Func<IAbsolutePath, Task> OnAfterSubmitFunc { get; set; }
}