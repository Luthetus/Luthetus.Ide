using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices;

/// <summary>
/// <see cref="CompilationUnit"/>
/// </summary>
public class CompilationUnitTests
{
    /// <summary>
    /// <see cref="CompilationUnit(CodeBlockNode?, ILexer?, IParser?, IBinder?)"/>
	/// <br/>----<br/>
    /// <see cref="CompilationUnit.RootCodeBlockNode"/>
    /// <see cref="CompilationUnit.Lexer"/>
    /// <see cref="CompilationUnit.Parser"/>
    /// <see cref="CompilationUnit.Binder"/>
	/// <see cref="CompilationUnit.SyntaxKind"/>
	/// <see cref="CompilationUnit.IsFabricated"/>
	/// <see cref="CompilationUnit.DiagnosticsBag"/>
	/// <see cref="CompilationUnit.ChildBag"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
		var codeBlockNode = new CodeBlockNode(ImmutableArray<ISyntax>.Empty);
		var lexer = new TextEditorDefaultLexer();
		var parser = new TextEditorDefaultParser();
		var binder = new TextEditorDefaultBinder();

		var compilationUnit = new CompilationUnit(
            codeBlockNode,
            lexer,
            parser,
            binder);

		Assert.Equal(codeBlockNode, compilationUnit.RootCodeBlockNode);
		Assert.Equal(lexer, compilationUnit.Lexer);
		Assert.Equal(parser, compilationUnit.Parser);
		Assert.Equal(binder, compilationUnit.Binder);

		Assert.Equal(SyntaxKind.CompilationUnit, compilationUnit.SyntaxKind);
		Assert.False(compilationUnit.IsFabricated);

		Assert.Empty(compilationUnit.DiagnosticsBag);

		Assert.Single(compilationUnit.ChildBag);
		Assert.Equal(codeBlockNode, compilationUnit.ChildBag.Single());
	}
}