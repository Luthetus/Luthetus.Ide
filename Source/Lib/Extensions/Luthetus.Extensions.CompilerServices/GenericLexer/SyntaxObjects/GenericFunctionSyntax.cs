using Luthetus.Extensions.CompilerServices.GenericLexer;
using Luthetus.Extensions.CompilerServices.GenericLexer.SyntaxEnums;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.Extensions.CompilerServices.GenericLexer.SyntaxObjects;

public class GenericFunctionSyntax : IGenericSyntax
{
	public GenericFunctionSyntax(TextEditorTextSpan textSpan)
	{
		TextSpan = textSpan;
	}

	public TextEditorTextSpan TextSpan { get; }
	public IReadOnlyList<IGenericSyntax> ChildList => Array.Empty<IGenericSyntax>();
	public GenericSyntaxKind GenericSyntaxKind => GenericSyntaxKind.Function;
}