using Luthetus.CompilerServices.Lang.DotNetSolution.CSharp;

namespace Luthetus.Ide.RazorLib.ComponentRenderersCase.Models;

public interface ITreeViewCSharpProjectToProjectReferenceRendererType
{
    public CSharpProjectToProjectReference CSharpProjectToProjectReference { get; set; }
}