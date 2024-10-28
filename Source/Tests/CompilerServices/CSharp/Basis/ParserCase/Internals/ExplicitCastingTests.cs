using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.CompilerServices.CSharp.LexerCase;
using Luthetus.CompilerServices.CSharp.ParserCase;

namespace Luthetus.CompilerServices.CSharp.Tests.Basis.ParserCase.Internals;

public class ExplicitCastingTests
{
	[Fact]
	public void Aaa()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText =
@"var typeBodyCodeBlockNode = (CodeBlockNode)typeDefinitionNode.TypeBodyCodeBlockNode;";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;
        
        var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.GetChildList()[0];
        var variableAssignmentExpressionNode = (VariableAssignmentExpressionNode)topCodeBlock.GetChildList()[1];
        
        var identifierToken = (IdentifierToken)variableAssignmentExpressionNode.GetChildList()[0];
        var equalsToken = (EqualsToken)variableAssignmentExpressionNode.GetChildList()[1];
        
        // var parenthesizedExpressionNode = (ParenthesizedExpressionNode)variableAssignmentExpressionNode.GetChildList()[2];
        var explicitCastNode = (ExplicitCastNode)variableAssignmentExpressionNode.GetChildList()[2];
        
        foreach (var child in variableAssignmentExpressionNode.GetChildList())
        {
        	Console.WriteLine(child.SyntaxKind);
        }
        
        Console.WriteLine(variableAssignmentExpressionNode);
        
/*

                                   CastExplicitNode
                                  /                \
TypeClauseNode (ParenthesizedExpressionNode)        variableAssignmentExpressionNode.GetChildList()[2]
										             |
										             IndexOperator
										             |
										             
										             VariableReferenceNode
										             MemberAccessNode
										             IndexNode
										             
=================================================================================================

                    CastExplicitNode -------------------------
                   /      |         \                        |
OpenParenthesisToken  TypeClauseNode  CloseParenthesisToken  IExpressionNode

'(T)' is a unary operator that explicitly casts the expression that follows to type 'T'?

It is NOT a unary operator, because its operator isn't represented by a single token.
It would take 3 tokens to represent the operator.

*/
	}
}
