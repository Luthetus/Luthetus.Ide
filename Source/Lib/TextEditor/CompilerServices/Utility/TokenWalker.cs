using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.Exceptions;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Utility;

public class TokenWalker
{
    private readonly DiagnosticBag _diagnosticBag;

    private int _index;
    private int _indexTrivia;
	private (int openTokenIndex, int closeTokenIndex, int tokenIndexToRestore)? _deferredParsingTuple;

    public TokenWalker(List<ISyntaxToken> tokenList, DiagnosticBag diagnosticBag)
    {
    	if (tokenList.Count > 0 &&
    		tokenList[tokenList.Count - 1].SyntaxKind != SyntaxKind.EndOfFileToken)
    	{
    		throw new LuthetusTextEditorException($"The last token must be 'SyntaxKind.EndOfFileToken'.");
    	}
    
        TokenList = tokenList;
        _diagnosticBag = diagnosticBag;
    }
    
    public TokenWalker(List<ISyntaxToken> tokenList, List<TextEditorTextSpan> triviaList, DiagnosticBag diagnosticBag)
    	: base(tokenList, diagnosticBag)
    {
    	TriviaList = triviaList;
    }

	public int ConsumeCounter { get; private set; }
	
	#if DEBUG
	public bool SuppressProtectedSyntaxKindConsumption { get; set; } = true;
	public List<SyntaxKind> ProtectedTokenSyntaxKindList { get; set; }
	#endif
	
    public List<ISyntaxToken> TokenList { get; }
    public List<TextEditorTextSpan>? TriviaList { get; }
    public ISyntaxToken Current => Peek(0);
    public ISyntaxToken Next => Peek(1);
    public ISyntaxToken Previous => Peek(-1);
    public bool IsEof => Current.SyntaxKind == SyntaxKind.EndOfFileToken;
    public int Index => _index;
    public TextEditorTextSpan CurrentTrivia => GetCurrentTrivia();

    /// <summary>If there are any tokens, then assume the final token is the end of file token. Otherwise, fabricate an end of file token.</summary>
    private ISyntaxToken EOF => TokenList.Count > 0
        ? TokenList[TokenList.Count - 1]
        : new EndOfFileToken(new(0, 0, 0, ResourceUri.Empty, string.Empty));

    /// <summary>The input to this method can be positive OR negative.<br/><br/>Returns <see cref="BadToken"/> when an index out of bounds error would've occurred.</summary>
    public ISyntaxToken Peek(int offset)
    {
        var index = _index + offset;

        if (index < 0)
            return GetBadToken();
        else if (index >= TokenList.Count)
            return EOF; // Return the end of file token (the last token)

        return TokenList[index];
    }

    public ISyntaxToken Consume()
    {
        if (_index >= TokenList.Count)
            return EOF; // Return the end of file token (the last token)
            
		if (_deferredParsingTuple is not null)
		{
			if (_index == _deferredParsingTuple.Value.closeTokenIndex)
			{
				var closeChildScopeToken = TokenList[_index];
				_index = _deferredParsingTuple.Value.tokenIndexToRestore;
				ConsumeCounter++;
				_deferredParsingTuple = null;
				return closeChildScopeToken;
			}
		}

        var consumedToken = TokenList[_index++];
        ConsumeCounter++;
        
        #if DEBUG
		if (!SuppressProtectedSyntaxKindConsumption)
		{
			if (ProtectedTokenSyntaxKindList.Contains(consumedToken.SyntaxKind))
				Console.WriteLine($"The protected syntax kind: '{consumedToken.SyntaxKind}' was unexpectedly consumed.");
				// throw new Exception($"The protected syntax kind: '{consumedToken.SyntaxKind}' was unexpectedly consumed.");
		}
		#endif

        return consumedToken;
    }

    public ISyntaxToken Backtrack()
    {
        if (_index > 0)
        {
            _index--;
            ConsumeCounter--;
        }

        return Peek(_index);
    }

    /// <summary>If the syntaxKind passed in does not match the current token, then a syntax token with that syntax kind will be fabricated and then returned instead.</summary>
    public ISyntaxToken Match(SyntaxKind expectedSyntaxKind)
    {
        var currentToken = Peek(0);

        // TODO: Checking for the text 'args' is likely not a good solution. When parsing a main method, it might have the method arguments: 'string[] args'. The issue here is that 'args' comes up as a keyword while being the identifier for that method argument.
        if (currentToken.TextSpan.GetText() == "args" && expectedSyntaxKind == SyntaxKind.IdentifierToken)
        {
            _ = Consume();
            return new IdentifierToken(currentToken.TextSpan);
        }

        if (currentToken.SyntaxKind == expectedSyntaxKind)
            return Consume();

        var fabricatedToken = this.FabricateToken(expectedSyntaxKind);

        _diagnosticBag.ReportUnexpectedToken(
            fabricatedToken.TextSpan,
            currentToken.SyntaxKind.ToString(),
            expectedSyntaxKind.ToString());

        return fabricatedToken;
    }

    /// <summary>If the syntaxKind passed in does not match the current token, then a syntax token with that syntax kind will be fabricated and then returned instead.</summary>
    public ISyntaxToken MatchRange(IEnumerable<SyntaxKind> validSyntaxKinds, SyntaxKind fabricationKind)
    {
        var currentToken = Peek(0);

        if (validSyntaxKinds.Contains(currentToken.SyntaxKind))
            return Consume();

        var fabricatedToken = this.FabricateToken(fabricationKind);

        _diagnosticBag.ReportUnexpectedToken(
            fabricatedToken.TextSpan,
            currentToken.SyntaxKind.ToString(),
            fabricationKind.ToString());

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
		_index = openTokenIndex;
		_deferredParsingTuple = (openTokenIndex, closeTokenIndex, tokenIndexToRestore);
		ConsumeCounter++;
	}
	
	public void ConsumeCounterReset()
	{
		ConsumeCounter = 0;
	}

    private BadToken GetBadToken() => new BadToken(new(0, 0, 0, ResourceUri.Empty, string.Empty));
    
    /// <summary>
    /// Going to do an odd implementation of this to start.
    /// Then as the 'trivia' code gets more fleshed out then will improve this.
    ///
    /// -----------------------
    ///
    /// The '_indexTrivia' cannot go backwards, it can only go forwards.
    ///
    /// A token can provide its text span's position indices in order to create
    /// a "surveying area".
    ///
    /// The '_indexTrivia' is then set to whichever entry of 'TriviaList' is first to
    /// exist in that "surveying area" sorted by position index.
    ///
    /// After 'ConsumeTrivia(...)' then the _indexTrivia will point at the next "trivia" within
    /// the "surveying area".
    ///
    /// Then, 'ConsumeTrivia' will return non-null if there is an entry that has not yet been consumed within those indices,
    /// the "surveying area".
    /// 
    /// -----------------------
    ///
    /// So, this implementation feels quite odd / unnecessarily "complex/confusing".
    ///
    /// But, I think this is a good "stepping stone solution".
    ///
    /// In essence I view the "trivia" as providing additional details about an ISyntaxToken (or whitespace/etc...).
    ///
    /// This solution permits a token to tell the 'TokenWalker' what position indices it spans,
    /// then within that boundary, ask for the additional information that is stored.
    ///
    /// As well, this avoids every token instance instantiating their own list where
    /// there exists an arbitrary amount of syntax.
    ///
    /// They all just share the same list, then say the position indices that they're allowed to read from.
    /// Something along these lines.
    ///
    /// -----------------------
    ///
    /// This implementation as well is expected to "fast" since it does a
    /// linear search for the position indices, but will remember its last index and pick back up from it on the next invocation.
    ///
    /// With this code only being used by the parser, when going from the first ISyntaxToken in the file to the last,
    /// then this should be a a "fast" enough "stepping stone solution".
    ///
    /// -----------------------
    ///
    /// I think if an interpolated string has an expression that itself contains yet again another interpolated string,
    /// that the outer and inner interpolated strings will have their "surveying area" overlap,
    /// and that the outer interpolated string will consume the inner interpolated string's trivia.
    ///
    /// I'm not sure, but I don't think this implementation is perfect, I just need to wrap my head around
    /// what I'm trying to do here.
    ///
    /// -----------------------
    ///
    /// My first guess is that the interpolated expressions are 'trivia' because the alternative
    /// would be instantiating a list on the 'StringInterpolatedToken'.
    ///
    /// But, to instantiate a list on every 'StringInterpolatedToken' sounds
    /// like a very bad idea for performance / memory / garbage collection overhead.
    ///
    /// So by having all instances of 'StringInterpolatedToken' within the same
    /// file share the same List, it reduces the amount of allocations.
    ///
    /// -----------------------
    ///
    /// The trivia probably shouldn't be 'TextEditorTextSpan' but instead an 'ISyntaxToken'
    /// but I just want to focus on the interpolated expressions,
    /// then I'll better understand what I'm trying to even do here.
    /// </summary>
    public TextEditorTextSpan? ConsumeTrivia(int startInclusivePositionIndex, int endExclusivePositionIndex)
    {
    	if (TriviaList is null || _indexTrivia >= TriviaList.Count)
            return null;
        
        // Find surveying area start.
        while (_indexTrivia < TriviaList.Count)
    	{
    		if (TriviaList[_indexTrivia].StartingIndexInclusive >= startInclusivePositionIndex)
	        	break;
	        	
			_indexTrivia++;
    	}
        
        var trivia = TriviaList[_indexTrivia];
        
        if (trivia.EndingIndexExclusive >= endExclusivePositionIndex)
        	return null;
        
        _indexTrivia++;
        return trivia;
    }
    
    public TextEditorTextSpan? GetCurrentTrivia(int startInclusivePositionIndex, int endExclusivePositionIndex)
    {
    	var originalIndexTrivia = _indexTrivia;
    
    	var trivia = ConsumeTrivia(startInclusivePositionIndex, endExclusivePositionIndex);
    	_indexTrivia = originalIndexTrivia;
    	
    	return trivia;
    }
}