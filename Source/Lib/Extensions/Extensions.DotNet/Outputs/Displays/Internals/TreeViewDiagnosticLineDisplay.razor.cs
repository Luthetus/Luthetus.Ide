using Microsoft.AspNetCore.Components;
using Luthetus.Extensions.DotNet.Outputs.Models;

namespace Luthetus.Extensions.DotNet.Outputs.Displays.Internals;

public partial class TreeViewDiagnosticLineDisplay : ComponentBase
{
	[Parameter, EditorRequired]
	public TreeViewDiagnosticLine TreeViewDiagnosticLine { get; set; } = null!;
}