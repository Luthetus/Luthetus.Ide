using Luthetus.Ide.ClassLib.DotNet.CSharp;

namespace Luthetus.Ide.ClassLib.ComponentRenderers.Types;

public interface ITreeViewCSharpProjectToProjectReferenceRendererType
{
    public CSharpProjectToProjectReference CSharpProjectToProjectReference { get; set; }
}