using Luthetus.Common.RazorLib.Namespaces.Models;

namespace Luthetus.Ide.RazorLib.ComponentRenderers.Models;

public interface ITreeViewNamespacePathRendererType
{
    public NamespacePath NamespacePath { get; set; }
}