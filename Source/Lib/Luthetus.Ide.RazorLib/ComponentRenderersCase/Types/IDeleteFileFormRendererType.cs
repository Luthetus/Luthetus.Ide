using Luthetus.Common.RazorLib.FileSystem.Interfaces;

namespace Luthetus.Ide.RazorLib.ComponentRenderersCase.Types;

public interface IDeleteFileFormRendererType
{
    public IAbsolutePath AbsolutePath { get; set; }
    public bool IsDirectory { get; set; }
    public Action<IAbsolutePath> OnAfterSubmitAction { get; set; }
}