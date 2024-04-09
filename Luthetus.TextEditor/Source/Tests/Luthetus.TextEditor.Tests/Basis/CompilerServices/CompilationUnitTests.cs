using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices;

/// <summary>
/// <see cref="CompilationUnit"/>
/// </summary>
public class CompilationUnitTests
{
    /// <summary>
    /// <see cref="CompilationUnit(CodeBlockNode?, ILuthLexer?, ILuthParser?, ILuthBinder?)"/>
	/// <br/>----<br/>
    /// <see cref="CompilationUnit.RootCodeBlockNode"/>
    /// <see cref="CompilationUnit.Lexer"/>
    /// <see cref="CompilationUnit.Parser"/>
    /// <see cref="CompilationUnit.Binder"/>
	/// <see cref="CompilationUnit.SyntaxKind"/>
	/// <see cref="CompilationUnit.IsFabricated"/>
	/// <see cref="CompilationUnit.DiagnosticsList"/>
	/// <see cref="CompilationUnit.ChildList"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
        throw new NotImplementedException("Test was broken on (2024-04-08)");
        //var codeBlockNode = new CodeBlockNode(ImmutableArray<ISyntax>.Empty);
        //var lexer = new LuthLexer(null, null, null);
        //var parser = new LuthParser(lexer);
        //var binder = new LuthBinder();

        //var compilationUnit = new CompilationUnit(
        //          codeBlockNode,
        //          lexer,
        //          parser,
        //          binder);

        //Assert.Equal(codeBlockNode, compilationUnit.RootCodeBlockNode);
        //Assert.Equal(lexer, compilationUnit.Lexer);
        //Assert.Equal(parser, compilationUnit.Parser);
        //Assert.Equal(binder, compilationUnit.Binder);

        //Assert.Equal(SyntaxKind.CompilationUnit, compilationUnit.SyntaxKind);
        //Assert.False(compilationUnit.IsFabricated);

        //Assert.Empty(compilationUnit.DiagnosticsList);

        //Assert.Single(compilationUnit.ChildList);
        //Assert.Equal(codeBlockNode, compilationUnit.ChildList.Single());
    }
}