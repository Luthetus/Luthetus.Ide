using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.Extensions.CompilerServices.Syntax;

namespace Luthetus.Extensions.CompilerServices.Utility;

public class TokenWalker
{
	private int _index;

	/// <summary>
	/// Use '-1' for each int value to indicate 'null' for the entirety of the _deferredParsingTuple;
	/// </summary>
	private (int openTokenIndex, int closeTokenIndex, int tokenIndexToRestore) _deferredParsingTuple = (-1, -1, -1);

	/// <summary>
	/// '-1' should not appear for any of the int values in the stack.
	/// _deferredParsingTuple is the cached Peek() result.
	///
	/// If this stack is empty, them the cached Peek() result should be '(-1, -1, -1)'.
	/// </summary>
	private Stack<(int openTokenIndex, int closeTokenIndex, int tokenIndexToRestore)>? _deferredParsingTupleStack;

	public TokenWalker(List<SyntaxToken> tokenList)
	{
		if (tokenList.Count > 0 &&
			tokenList[tokenList.Count - 1].SyntaxKind != SyntaxKind.EndOfFileToken)
		{
			throw new LuthetusTextEditorException($"The last token must be 'SyntaxKind.EndOfFileToken'.");
		}

		TokenList = tokenList;
	}

	public int ConsumeCounter { get; private set; }

#if DEBUG
	public bool SuppressProtectedSyntaxKindConsumption { get; set; } = true;
	public List<SyntaxKind> ProtectedTokenSyntaxKindList { get; set; }
#endif

	public List<SyntaxToken> TokenList { get; }
	public SyntaxToken Current => Peek(0);
	public SyntaxToken Next => Peek(1);
	public SyntaxToken Previous => Peek(-1);
	public bool IsEof => Current.SyntaxKind == SyntaxKind.EndOfFileToken;
	public int Index => _index;

	/// <summary>If there are any tokens, then assume the final token is the end of file token. Otherwise, fabricate an end of file token.</summary>
	private SyntaxToken EOF => TokenList.Count > 0
		? TokenList[TokenList.Count - 1]
		: new SyntaxToken(SyntaxKind.EndOfFileToken, new(0, 0, 0, ResourceUri.Empty, string.Empty));

	/// <summary>The input to this method can be positive OR negative.<br/><br/>Returns <see cref="BadToken"/> when an index out of bounds error would've occurred.</summary>
	public SyntaxToken Peek(int offset)
	{
		var index = _index + offset;

		if (index < 0)
			return GetBadToken();
		else if (index >= TokenList.Count)
			return EOF; // Return the end of file token (the last token)

		return TokenList[index];
	}

	public SyntaxToken Consume()
	{
		if (_index >= TokenList.Count)
			return EOF; // Return the end of file token (the last token)

		if (_deferredParsingTuple.closeTokenIndex != -1)
		{
			if (_index == _deferredParsingTuple.closeTokenIndex)
			{
				_deferredParsingTupleStack.Pop();

				var closeChildScopeToken = TokenList[_index];
				_index = _deferredParsingTuple.tokenIndexToRestore;
				ConsumeCounter++;

				if (_deferredParsingTupleStack.Count > 0)
					_deferredParsingTuple = _deferredParsingTupleStack.Peek();
				else
					_deferredParsingTuple = (-1, -1, -1);

				return closeChildScopeToken;
			}
		}

		var consumedToken = TokenList[_index++];
		ConsumeCounter++;

		/*#if DEBUG
		if (!SuppressProtectedSyntaxKindConsumption)
		{
			if (ProtectedTokenSyntaxKindList.Contains(consumedToken.SyntaxKind))
				Console.WriteLine($"The protected syntax kind: '{consumedToken.SyntaxKind}' was unexpectedly consumed.");
				// throw new Exception($"The protected syntax kind: '{consumedToken.SyntaxKind}' was unexpectedly consumed.");
		}
		#endif*/

		return consumedToken;
	}

	public SyntaxToken Backtrack()
	{
		if (_index > 0)
		{
			_index--;
			ConsumeCounter--;
		}

		return Peek(_index);
	}

	/// <summary>If the syntaxKind passed in does not match the current token, then a syntax token with that syntax kind will be fabricated and then returned instead.</summary>
	public SyntaxToken Match(SyntaxKind expectedSyntaxKind)
	{
		var currentToken = Peek(0);

		// TODO: Checking for the text 'args' is likely not a good solution. When parsing a main method, it might have the method arguments: 'string[] args'. The issue here is that 'args' comes up as a keyword while being the identifier for that method argument.
		if (currentToken.TextSpan.GetText() == "args" && expectedSyntaxKind == SyntaxKind.IdentifierToken)
		{
			_ = Consume();
			return new SyntaxToken(SyntaxKind.IdentifierToken, currentToken.TextSpan);
		}

		if (currentToken.SyntaxKind == expectedSyntaxKind)
			return Consume();

		var fabricatedToken = this.FabricateToken(expectedSyntaxKind);

		/*_diagnosticBag.ReportUnexpectedToken(
            fabricatedToken.TextSpan,
            currentToken.SyntaxKind.ToString(),
            expectedSyntaxKind.ToString());*/

		return fabricatedToken;
	}

	/// <summary>If the syntaxKind passed in does not match the current token, then a syntax token with that syntax kind will be fabricated and then returned instead.</summary>
	public SyntaxToken MatchRange(IEnumerable<SyntaxKind> validSyntaxKinds, SyntaxKind fabricationKind)
	{
		var currentToken = Peek(0);

		if (validSyntaxKinds.Contains(currentToken.SyntaxKind))
			return Consume();

		var fabricatedToken = this.FabricateToken(fabricationKind);

		/*_diagnosticBag.ReportUnexpectedToken(
            fabricatedToken.TextSpan,
            currentToken.SyntaxKind.ToString(),
            fabricationKind.ToString());*/

		return fabricatedToken;
	}

	/// <summary>
	/// TODO: This method is being added to support breadth first parsing...
	/// ...Having this be a public method is a bit hacky,
	/// preferably deferring the parsing of a child scope would
	/// be done entirely from this class, so the _index cannot be changed
	/// externally (2024-06-18).
	/// </summary>
	public void DeferredParsing(
		int openTokenIndex,
		int closeTokenIndex,
		int tokenIndexToRestore)
	{
		_deferredParsingTupleStack ??= new();

		_index = openTokenIndex;
		_deferredParsingTuple = (openTokenIndex, closeTokenIndex, tokenIndexToRestore);
		_deferredParsingTupleStack.Push((openTokenIndex, closeTokenIndex, tokenIndexToRestore));
		ConsumeCounter++;
	}

	public void SetNullDeferredParsingTuple()
	{
		_deferredParsingTuple = (-1, -1, -1);
	}

	public void ConsumeCounterReset()
	{
		ConsumeCounter = 0;
	}

	private SyntaxToken GetBadToken() => new SyntaxToken(SyntaxKind.BadToken, new(0, 0, 0, ResourceUri.Empty, string.Empty));
}
