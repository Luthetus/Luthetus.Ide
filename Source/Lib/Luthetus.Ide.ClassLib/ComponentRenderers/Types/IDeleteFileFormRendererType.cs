using Luthetus.Ide.ClassLib.FileSystem.Interfaces;

namespace Luthetus.Ide.ClassLib.ComponentRenderers.Types;

public interface IDeleteFileFormRendererType
{
    public IAbsoluteFilePath AbsoluteFilePath { get; set; }
    public bool IsDirectory { get; set; }
    public Action<IAbsoluteFilePath> OnAfterSubmitAction { get; set; }
}