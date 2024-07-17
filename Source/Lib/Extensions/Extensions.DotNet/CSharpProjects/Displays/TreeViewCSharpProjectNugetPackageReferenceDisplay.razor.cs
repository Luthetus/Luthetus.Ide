using Microsoft.AspNetCore.Components;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Extensions.DotNet.Nugets.Models;
using Luthetus.Extensions.DotNet.ComponentRenderers.Models;

namespace Luthetus.Extensions.DotNet.CSharpProjects.Displays;

public partial class TreeViewCSharpProjectNugetPackageReferenceDisplay : ComponentBase, ITreeViewCSharpProjectNugetPackageReferenceRendererType
{
	[Parameter, EditorRequired]
	public CSharpProjectNugetPackageReference CSharpProjectNugetPackageReference { get; set; } = null!;
}