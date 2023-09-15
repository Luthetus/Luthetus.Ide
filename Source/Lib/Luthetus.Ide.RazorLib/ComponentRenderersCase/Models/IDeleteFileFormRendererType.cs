using Luthetus.Common.RazorLib.FileSystem.Interfaces;

namespace Luthetus.Ide.RazorLib.ComponentRenderersCase.Models;

public interface IDeleteFileFormRendererType
{
    public IAbsolutePath AbsolutePath { get; set; }
    public bool IsDirectory { get; set; }
    public Action<IAbsolutePath> OnAfterSubmitAction { get; set; }
}