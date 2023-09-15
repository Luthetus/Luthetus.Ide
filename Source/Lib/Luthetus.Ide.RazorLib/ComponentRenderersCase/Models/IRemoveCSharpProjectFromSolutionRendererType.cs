using Luthetus.Common.RazorLib.FileSystem.Interfaces;

namespace Luthetus.Ide.RazorLib.ComponentRenderersCase.Models;

public interface IRemoveCSharpProjectFromSolutionRendererType
{
    public IAbsolutePath AbsolutePath { get; set; }
    public Action<IAbsolutePath> OnAfterSubmitAction { get; set; }
}