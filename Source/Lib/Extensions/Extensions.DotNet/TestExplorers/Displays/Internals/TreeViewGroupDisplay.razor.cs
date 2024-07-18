using Microsoft.AspNetCore.Components;
using Luthetus.Extensions.DotNet.TestExplorers.Models;

namespace Luthetus.Extensions.DotNet.TestExplorers.Displays.Internals;

public partial class TreeViewGroupDisplay : ComponentBase
{
	[Parameter, EditorRequired]
	public TreeViewGroup TreeViewGroup { get; set; } = null!;
}