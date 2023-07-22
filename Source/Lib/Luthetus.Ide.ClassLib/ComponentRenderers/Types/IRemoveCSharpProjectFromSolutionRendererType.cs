using Luthetus.Common.RazorLib.FileSystem.Interfaces;

namespace Luthetus.Ide.ClassLib.ComponentRenderers.Types;

public interface IRemoveCSharpProjectFromSolutionRendererType
{
    public IAbsoluteFilePath AbsoluteFilePath { get; set; }
    public Action<IAbsoluteFilePath> OnAfterSubmitAction { get; set; }
}