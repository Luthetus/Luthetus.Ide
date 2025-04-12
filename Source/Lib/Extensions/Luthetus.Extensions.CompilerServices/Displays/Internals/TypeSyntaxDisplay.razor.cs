using Microsoft.AspNetCore.Components;
using Luthetus.TextEditor.RazorLib;
using Luthetus.Extensions.CompilerServices.Syntax;

namespace Luthetus.Extensions.CompilerServices.Displays.Internals;

public partial class TypeSyntaxDisplay : ComponentBase
{
	[Inject]
	private ITextEditorService TextEditorService { get; set; } = null!;
	
	[Parameter, EditorRequired]
	public SyntaxViewModel SyntaxViewModel { get; set; } = default!;
	[Parameter, EditorRequired]
	public TypeReference TypeReference { get; set; } = default!;
}