using Luthetus.CompilerServices.Lang.DotNetSolution.CSharp;

namespace Luthetus.Ide.ClassLib.ComponentRenderers.Types;

public interface ITreeViewCSharpProjectToProjectReferenceRendererType
{
    public CSharpProjectToProjectReference CSharpProjectToProjectReference { get; set; }
}