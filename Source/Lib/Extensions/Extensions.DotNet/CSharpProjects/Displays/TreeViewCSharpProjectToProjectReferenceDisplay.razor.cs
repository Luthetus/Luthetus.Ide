using Microsoft.AspNetCore.Components;
using Luthetus.CompilerServices.DotNetSolution.Models.Project;
using Luthetus.Extensions.DotNet.ComponentRenderers.Models;

namespace Luthetus.Extensions.DotNet.CSharpProjects.Displays;

public partial class TreeViewCSharpProjectToProjectReferenceDisplay : ComponentBase, ITreeViewCSharpProjectToProjectReferenceRendererType
{
	[Parameter, EditorRequired]
	public CSharpProjectToProjectReference CSharpProjectToProjectReference { get; set; } = null!;
}