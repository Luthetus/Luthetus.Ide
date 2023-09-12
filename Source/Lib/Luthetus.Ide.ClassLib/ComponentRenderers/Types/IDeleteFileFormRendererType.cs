using Luthetus.Common.RazorLib.FileSystem.Interfaces;

namespace Luthetus.Ide.ClassLib.ComponentRenderers.Types;

public interface IDeleteFileFormRendererType
{
    public IAbsolutePath AbsoluteFilePath { get; set; }
    public bool IsDirectory { get; set; }
    public Action<IAbsolutePath> OnAfterSubmitAction { get; set; }
}