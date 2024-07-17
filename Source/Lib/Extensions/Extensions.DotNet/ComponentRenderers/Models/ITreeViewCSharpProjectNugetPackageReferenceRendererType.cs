using Luthetus.Extensions.DotNet.Nugets.Models;

namespace Luthetus.Extensions.DotNet.ComponentRenderers.Models;

public interface ITreeViewCSharpProjectNugetPackageReferenceRendererType
{
	public CSharpProjectNugetPackageReference CSharpProjectNugetPackageReference { get; set; }
}