namespace Luthetus.CompilerServices.CSharp.LexerCase;

/// <summary>
/// The empty constructor invocation is necessary for this type to initialize properly.
/// As -1 is used in order to signify null.
/// </summary>
public struct BracePairMetadata
{
	public BracePairMetadata()
	{
		OpenBraceTokenIndex = -1;
		CloseBraceTokenIndex = -1;
		ParentBracePairIndex = -1;
		FirstChildBracePairIndex = -1;
		LastChildBracePairIndex = -1;
	}

	/// <summary>ISyntaxToken in the TokenWalker is what this points to.</summary>
	public int OpenBraceTokenIndex { get; set; }
	
	/// <summary>ISyntaxToken in the TokenWalker is what this points to.</summary>
	public int CloseBraceTokenIndex { get; set; }
	
	/// <summary>BracePairMetadata in the Lexer is what this points to.</summary> 
	public int ParentBracePairIndex { get; set; }
	
	/// <summary>
	/// BracePairMetadata in the Lexer is what this points to.
	///
	/// The 'FirstChildBracePairIndex' is probably this entry's index + 1.
	/// But until this is confirmed, this property will continue to exist.
	/// </summary>
	public int FirstChildBracePairIndex { get; set; }
	
	/// <summary>BracePairMetadata in the Lexer is what this points to.</summary>
	public int LastChildBracePairIndex { get; set; }
}
