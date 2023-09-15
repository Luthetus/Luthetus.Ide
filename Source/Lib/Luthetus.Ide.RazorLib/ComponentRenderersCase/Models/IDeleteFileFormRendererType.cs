using Luthetus.Common.RazorLib.FileSystem.Models;

namespace Luthetus.Ide.RazorLib.ComponentRenderersCase.Models;

public interface IDeleteFileFormRendererType
{
    public IAbsolutePath AbsolutePath { get; set; }
    public bool IsDirectory { get; set; }
    public Action<IAbsolutePath> OnAfterSubmitAction { get; set; }
}