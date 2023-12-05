using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Ide.RazorLib.TestExplorers.Models;

namespace Luthetus.Ide.RazorLib.TestExplorers.Displays;

public partial class TestExplorerDetailsDisplay : ComponentBase
{
	[CascadingParameter]
    public TestExplorerRenderBatchValidated RenderBatch { get; set; } = null!;

	[Parameter, EditorRequired]
    public ElementDimensions ElementDimensions { get; set; } = null!;
}