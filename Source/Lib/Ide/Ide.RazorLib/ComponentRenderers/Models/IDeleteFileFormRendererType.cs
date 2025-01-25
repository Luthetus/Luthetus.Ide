using Luthetus.Common.RazorLib.FileSystems.Models;

namespace Luthetus.Ide.RazorLib.ComponentRenderers.Models;

public interface IDeleteFileFormRendererType
{
    public AbsolutePath AbsolutePath { get; set; }
    public bool IsDirectory { get; set; }
    public Func<AbsolutePath, Task> OnAfterSubmitFunc { get; set; }
}