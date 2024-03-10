using Luthetus.Common.RazorLib.PolymorphicUis.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Common.RazorLib.PolymorphicUis.Displays;

public partial class PolymorphicTabDisplay : ComponentBase
{
	[Parameter, EditorRequired]
	public IPolymorphicTab Tab { get; set; } = null!;
}