using Microsoft.AspNetCore.Components;
using Fluxor;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.CompilerServices.DotNetSolution.Models.Project;
using Luthetus.Extensions.DotNet.ComponentRenderers.Models;

namespace Luthetus.Extensions.DotNet.CSharpProjects.Displays;

public partial class TreeViewCSharpProjectToProjectReferenceDisplay : ComponentBase, ITreeViewCSharpProjectToProjectReferenceRendererType
{
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;
    
	[Parameter, EditorRequired]
	public CSharpProjectToProjectReference CSharpProjectToProjectReference { get; set; } = null!;
}