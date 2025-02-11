using Microsoft.AspNetCore.Components;
using Fluxor;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Extensions.DotNet.Nugets.Models;
using Luthetus.Extensions.DotNet.ComponentRenderers.Models;

namespace Luthetus.Extensions.DotNet.CSharpProjects.Displays;

public partial class TreeViewCSharpProjectNugetPackageReferenceDisplay : ComponentBase, ITreeViewCSharpProjectNugetPackageReferenceRendererType
{
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;
    
	[Parameter, EditorRequired]
	public CSharpProjectNugetPackageReference CSharpProjectNugetPackageReference { get; set; } = null!;
}