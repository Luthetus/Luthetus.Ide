using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Dimensions.Models;

namespace Luthetus.Ide.RazorLib.TestExplorers.Displays;

public partial class TestExplorerDetailsDisplay : ComponentBase
{
	[Parameter, EditorRequired]
    public ElementDimensions ElementDimensions { get; set; } = null!;
}