using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.CompilerServices.CSharp.LexerCase;
using Luthetus.CompilerServices.CSharp.ParserCase;

namespace Luthetus.CompilerServices.CSharp.Tests.Basis.ParserCase.Internals;

public class VerbatimStringTests
{
	[Fact]
	public void AtDollarSign()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText =
@"var aaa = $@"""";";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;
        
        var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.GetChildList()[0];
        var variableAssignmentExpressionNode = (VariableAssignmentExpressionNode)topCodeBlock.GetChildList()[1];
        
        foreach (var child in variableAssignmentExpressionNode.GetChildList())
        {
        	Console.WriteLine(child.SyntaxKind);
        }
	}
	
	[Fact]
	public void DollarSignAt()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText =
@"var aaa = @$"""";";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;
        
        var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.GetChildList()[0];
        var variableAssignmentExpressionNode = (VariableAssignmentExpressionNode)topCodeBlock.GetChildList()[1];
        
        foreach (var child in variableAssignmentExpressionNode.GetChildList())
        {
        	Console.WriteLine(child.SyntaxKind);
        }
	}
}
