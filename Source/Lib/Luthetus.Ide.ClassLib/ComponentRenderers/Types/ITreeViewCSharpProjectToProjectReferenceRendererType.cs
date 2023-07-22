using Luthetus.CompilerServices.Lang.DotNet.CSharp;

namespace Luthetus.Ide.ClassLib.ComponentRenderers.Types;

public interface ITreeViewCSharpProjectToProjectReferenceRendererType
{
    public CSharpProjectToProjectReference CSharpProjectToProjectReference { get; set; }
}