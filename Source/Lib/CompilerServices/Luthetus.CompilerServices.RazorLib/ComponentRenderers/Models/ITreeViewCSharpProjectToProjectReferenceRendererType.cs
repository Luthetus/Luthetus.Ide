using Luthetus.CompilerServices.DotNetSolution.Models.Project;

namespace Luthetus.CompilerServices.RazorLib.ComponentRenderers.Models;

public interface ITreeViewCSharpProjectToProjectReferenceRendererType
{
    public CSharpProjectToProjectReference CSharpProjectToProjectReference { get; set; }
}