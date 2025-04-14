using Microsoft.AspNetCore.Components;
using Luthetus.Extensions.CompilerServices.Syntax;

namespace Luthetus.Extensions.CompilerServices.Displays.Internals;

public partial class GenericSyntaxDisplay : ComponentBase
{
	[Parameter, EditorRequired]
	public SyntaxViewModel SyntaxViewModel { get; set; } = default!;
	
	[Parameter]
	public TypeReference TypeReference { get; set; } = default;
}