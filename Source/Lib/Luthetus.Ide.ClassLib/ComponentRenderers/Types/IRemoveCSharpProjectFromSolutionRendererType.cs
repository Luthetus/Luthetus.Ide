using Luthetus.Common.RazorLib.FileSystem.Interfaces;

namespace Luthetus.Ide.ClassLib.ComponentRenderers.Types;

public interface IRemoveCSharpProjectFromSolutionRendererType
{
    public IAbsolutePath AbsoluteFilePath { get; set; }
    public Action<IAbsolutePath> OnAfterSubmitAction { get; set; }
}