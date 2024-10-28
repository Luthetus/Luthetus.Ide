using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.CompilerServices.CSharp.LexerCase;
using Luthetus.CompilerServices.CSharp.ParserCase;
using Luthetus.CompilerServices.CSharp.ParserCase.Internals;
using Luthetus.CompilerServices.CSharp.Facts;

namespace Luthetus.CompilerServices.CSharp.ParserCase;

public class ExpressionSession
{
	private readonly List<(SyntaxKind DelimiterSyntaxKind, IExpressionNode ExpressionNode)> _shortCircuitList = new();

	public ExpressionSession(
		List<ISyntaxToken> tokenList,
		Stack<ISyntax> expressionStack)
	{
		TokenList = tokenList;
		ExpressionStack = expressionStack;
	}
	
	private string _shortCircuitListStringified = string.Empty;
	private bool _shortCircuitListStringifiedIsDirty;

	public IReadOnlyList<(SyntaxKind DelimiterSyntaxKind, IExpressionNode ExpressionNode)> ShortCircuitList => _shortCircuitList;

	public List<ISyntaxToken> TokenList { get; }
	public Stack<ISyntax> ExpressionStack { get; }
	public int Position { get; set; }
	
	public string ShortCircuitListStringified
	{
		get
		{
			if (_shortCircuitListStringifiedIsDirty)
			{
				_shortCircuitListStringifiedIsDirty = false;
				_shortCircuitListStringified = string.Join(',', ShortCircuitList.Select(x => x.DelimiterSyntaxKind));
			}
				
			return _shortCircuitListStringified;
		}
		private set => _shortCircuitListStringified = value;
	}
	
	public void AddShortCircuit((SyntaxKind DelimiterSyntaxKind, IExpressionNode ExpressionNode) shortCircuit)
	{
		_shortCircuitList.Add(shortCircuit);
		_shortCircuitListStringifiedIsDirty = true;
	}
	
	public void RemoveRangeShortCircuit(int index, int count)
	{
		_shortCircuitList.RemoveRange(index, count);
		_shortCircuitListStringifiedIsDirty = true;
	}
}

