using Luthetus.CompilerServices.DotNetSolution.Models.Project;

namespace Luthetus.Extensions.DotNet.ComponentRenderers.Models;

public interface ITreeViewSolutionFolderRendererType
{
	public SolutionFolder DotNetSolutionFolder { get; set; }
}