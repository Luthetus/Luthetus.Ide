using Luthetus.Extensions.DotNet.CompilerServices.Models;
using Luthetus.Extensions.DotNet.ComponentRenderers.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Extensions.DotNet.CompilerServices.Displays;

public partial class TreeViewCompilerServiceDisplay : ComponentBase, ITreeViewCompilerServiceRendererType
{
	[Parameter, EditorRequired]
	public TreeViewCompilerService TreeViewCompilerService { get; set; } = null!;
}