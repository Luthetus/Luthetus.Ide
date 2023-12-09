using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.GenericLexer.SyntaxObjects;

public class GenericCommentMultiLineSyntaxTests
{
	[Fact]
	public void GenericCommentMultiLineSyntax()
	{
		//public GenericCommentMultiLineSyntax(TextEditorTextSpan textSpan)
	}
    
	[Fact]
	public void TextSpan()
	{
		//public TextEditorTextSpan TextSpan { get; }
	}
    
	[Fact]
	public void ChildBag()
	{
		//public ImmutableArray<IGenericSyntax> ChildBag => ImmutableArray<IGenericSyntax>.Empty;
	}
    
	[Fact]
	public void GenericSyntaxKind()
	{
		//public GenericSyntaxKind GenericSyntaxKind => GenericSyntaxKind.CommentMultiLine;
	}
}