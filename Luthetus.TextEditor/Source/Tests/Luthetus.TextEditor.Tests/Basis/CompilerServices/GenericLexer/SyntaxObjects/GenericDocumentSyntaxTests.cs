using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.SyntaxObjects;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.GenericLexer.SyntaxObjects;

/// <summary>
/// <see cref="GenericDocumentSyntax"/>
/// </summary>
public class GenericDocumentSyntaxTests
{
    /// <summary>
    /// <see cref="GenericDocumentSyntax(TextEditorTextSpan, ImmutableArray{IGenericSyntax})"/>
    /// <br/>----<br/>
    /// <see cref="GenericDocumentSyntax.TextSpan"/>
    /// <see cref="GenericDocumentSyntax.ChildList"/>
    /// <see cref="GenericDocumentSyntax.GenericSyntaxKind"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
		throw new NotImplementedException();
	}
}