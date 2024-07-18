using Luthetus.CompilerServices.DotNetSolution.Models.Project;

namespace Luthetus.Extensions.DotNet.ComponentRenderers.Models;

public interface ITreeViewCSharpProjectToProjectReferenceRendererType
{
	public CSharpProjectToProjectReference CSharpProjectToProjectReference { get; set; }
}