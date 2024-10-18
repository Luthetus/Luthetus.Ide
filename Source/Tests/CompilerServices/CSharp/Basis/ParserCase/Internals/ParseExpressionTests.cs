using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.CompilerServices.CSharp.LexerCase;
using Luthetus.CompilerServices.CSharp.ParserCase;

namespace Luthetus.CompilerServices.CSharp.Tests.Basis.ParserCase.Internals;

public class ParseExpressionTests
{
	/// <summary>
	/// Goal: Rewrite expression parsing
	/// ================================
	///
	/// Short description:
	/// ------------------
	/// - Use a 'while' loop rather than a recursive method.
	/// - A parent expression can "short circuit"(wording?) a child expression.
	/// - Consider the role of the main 'Parse(...)' method.
	///
	/// Long description:
	/// -----------------
	/// - Use a 'while' loop rather than a recursive method.
	/// 	asd
	/// - A parent expression can "short circuit"(wording?) a child expression.
	/// 	Given: '( 1 + );'
	/// 	Then:
	/// 		'(' opens an expression (likely a parenthesized expression),
	/// 		'1 +' is read as a binary expression node,
	/// 		But, when the right operand is attempted to be read, a ')' is read instead.
	/// 		Since ')' is the closing delimiter to the initial '(',
	/// 			then fill the remainder of the binary expression with fabricated nodes,
	/// 			and then set the inner expression for the parenthesized expression
	/// 			to be the binary expression.
	/// - Consider the role of the main 'Parse(...)' method.
	/// 	There is a complication where the 'Parse(...)' method needs to
	/// 		identify a syntax, but not duplicate the code.
	/// 	What I just wrote was my attempt at wording the issue.
	/// 		I need to continue thinking about this. I think it is
	/// 		vitally important that I find my words for this idea.
	/// 	Consider if I typed the statement: '2;'
	/// 		The C# compiler will give me an error, because
	/// 		Literal expressions are not allowed to be statements.
	/// 		The fact that the C# compiler gives me an error however,
	/// 		does not stop a user from typing this into the editor.
	/// 		So, the parser needs to handle this case, and recover when
	/// 		it reads the semicolon, then move on to the next statement as if all is fine.
	/// 	On a related note, consider I type the statement: 'var x = 2;'
	/// 		The parser will see 'var', so it invokes something named similarly to 'ParseVarContextualKeyword(...)'.
	/// 		Then 'ParseVarContextualKeyword(...)' sees an IdentifierToken follows the 'VarContextualKeywordToken'.
	/// 		The '{IdentifierToken} {IdentifierToken}' pattern in this situation is nonsensical,
	/// 			because there is no TypeDeclarationNode with an IdentifierToken of text: "var".
	/// 			So the 'var' is presumed to be the implicit TypeClauseNode that comes from the 'var' contextual keyword.
	/// 		To skip ahead to the point... the text 'var x =' results in a variable declaration node,
	/// 			and a variable assignment node.
	/// 		But, now we know we need to parse an expression. To make the point more clearly,
	/// 			we just as in the previous example need to parse the literal expression '2'.
	/// 		Except now we aren't coming directly from the 'Parse(...)' method,
	/// 			we are coming from the 'ParseVarContextualKeyword(...)' method.
	/// 	Okay, I think I know what to do.
	/// 		There is no 'ParseLiteralToken(...)', instead one immediately turns to 'ParseExpression(...)'.
	/// 	Otherwise, when given '2;', you go from
	/// 		'Parse(...)' to
	/// 		'ParseLiteralToken(...)' to
	/// 		'ParseExpression'.
	/// 	This contrasts to being given 'var x = 2;'
	/// 		'ParseVarContextualKeyword(...)' to
	/// 		'ParseIdentifierToken(...)' to
	/// 		'ParseVariableAssignment(...)' to
	/// 		'ParseExpression(...)' to
	/// 	The methods I wrote are not an exact replication of what happens in the parser at the moment.
	/// 		But, they illustrate that 'ParseLiteralToken(...)' is NOT being invoked for the 'var x = 2;' input.
	/// 		Therefore, any logic tied to 'ParseLiteralToken(...)' is likely irrelevant, or it duplicates code
	/// 		that also exists in 'ParseExpression(...)'.
	/// 	I feel like the example I gave comes across as quite underwhelming.
	/// 		But this situation is all throughout the current parser code.
	/// 		Every syntax that can appear as an expression, has its own standalone method as well.
	/// 		Eventually I started to just invoke 'ParseExpression(...)' from the standalone method.
	/// 		And the standalone method essentially did nothing of its own.
	/// 	If I add object initialization, and collection initialization,
	/// 		I need to make sure the code doesn't get duplicated in a standlone method
	/// 		and in 'ParseExpression(...)'.
	/// 	The other consideration is, does the main 'Parse(...)' loop only ever start and end a syntax?
	/// 		i.e.: either you are starting a statement, or at a statement delimiter?
	/// </summary>
	[Fact]
	public void Aaa()
	{
		var resourceUri = new ResourceUri("./unitTesting.txt");
        var sourceText =
@"
var aaa = 1;
";

		var lexer = new CSharpLexer(resourceUri, sourceText);
        lexer.Lex();
        var parser = new CSharpParser(lexer);
        var compilationUnit = parser.Parse();
        var topCodeBlock = compilationUnit.RootCodeBlockNode;
        
        var variableDeclarationNode = (VariableDeclarationNode)topCodeBlock.ChildList[0];
        var variableAssignmentExpressionNode = (VariableAssignmentExpressionNode)topCodeBlock.ChildList[1];
        
        var identifierToken = (IdentifierToken)variableAssignmentExpressionNode.ChildList[0];
        var equalsToken = (EqualsToken)variableAssignmentExpressionNode.ChildList[1];
        
        // var parenthesizedExpressionNode = (ParenthesizedExpressionNode)variableAssignmentExpressionNode.ChildList[2];
        var explicitCastNode = (ExplicitCastNode)variableAssignmentExpressionNode.ChildList[2];
        
        foreach (var child in variableAssignmentExpressionNode.ChildList)
        {
        	Console.WriteLine(child.SyntaxKind);
        }
        
        Console.WriteLine(variableAssignmentExpressionNode);
    }
}
