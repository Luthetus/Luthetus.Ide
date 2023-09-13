using Luthetus.Common.RazorLib.FileSystem.Interfaces;

namespace Luthetus.Ide.RazorLib.ComponentRenderersCase.Types;

public interface IRemoveCSharpProjectFromSolutionRendererType
{
    public IAbsolutePath AbsolutePath { get; set; }
    public Action<IAbsolutePath> OnAfterSubmitAction { get; set; }
}