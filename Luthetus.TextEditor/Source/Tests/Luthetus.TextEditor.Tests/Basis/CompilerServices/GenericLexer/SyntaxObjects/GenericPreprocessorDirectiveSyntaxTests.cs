using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.GenericLexer.SyntaxObjects;

public class GenericPreprocessorDirectiveSyntaxTests
{
	[Fact]
	public void GenericPreprocessorDirectiveSyntax()
	{
		//public GenericPreprocessorDirectiveSyntax(
		//	TextEditorTextSpan textSpan,
		//	ImmutableArray<IGenericSyntax> childBag)
	}
    

	[Fact]
	public void TextSpan()
	{
		//public TextEditorTextSpan TextSpan { get; }
	}

	[Fact]
	public void ChildBag()
	{
		//public ImmutableArray<IGenericSyntax> ChildBag { get; }
	}

	[Fact]
	public void GenericSyntaxKind()
	{
		//public GenericSyntaxKind GenericSyntaxKind => GenericSyntaxKind.PreprocessorDirective;
	}
}