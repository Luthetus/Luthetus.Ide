using Luthetus.Extensions.Git.Models;

namespace Luthetus.Extensions.Git.ComponentRenderers.Models;

public interface ITreeViewGitFileRendererType
{
    public TreeViewGitFile TreeViewGitFile { get; set; }
}