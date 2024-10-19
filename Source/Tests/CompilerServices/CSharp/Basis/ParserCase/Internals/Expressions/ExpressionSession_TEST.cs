using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.CompilerServices.CSharp.LexerCase;
using Luthetus.CompilerServices.CSharp.ParserCase;
using Luthetus.CompilerServices.CSharp.ParserCase.Internals;
using Luthetus.CompilerServices.CSharp.Facts;

namespace Luthetus.CompilerServices.CSharp.Tests.Basis.ParserCase.Internals.Expressions;

public class ExpressionSession
{
	public ExpressionSession(
		List<ISyntaxToken> tokenList,
		Stack<ISyntax> expressionStack,
		List<(SyntaxKind DelimiterSyntaxKind, IExpressionNode ExpressionNode)> shortCircuitList)
	{
		TokenList = tokenList;
		ExpressionStack = expressionStack;
		ShortCircuitList = shortCircuitList;
	}

	public List<ISyntaxToken> TokenList { get; }
	public Stack<ISyntax> ExpressionStack { get; }
	public List<(SyntaxKind DelimiterSyntaxKind, IExpressionNode ExpressionNode)> ShortCircuitList { get; }
}
