using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.CompilerServices.CSharp.LexerCase;

public struct CSharpLexerOutput
{
	public CSharpLexerOutput()
    {
    	SyntaxTokenList = new();
    	MiscTextSpanList = new();
    	TriviaTextSpanList = new();
    	DiagnosticBag = new();
    }
    
    public List<ISyntaxToken> SyntaxTokenList { get; }
    /// <summary>
    /// MiscTextSpanList contains the comments and the escape characters.
    /// </summary>
    public List<TextEditorTextSpan> MiscTextSpanList { get; }
    /// <summary>
    /// TriviaTextSpanList is likely going to fully replace 'MiscTextSpanList'.
    /// I have to try some things out first.
    ///
    /// The first thing that will be tracked in this list
    /// are the 'interpolated string expressions'.
    /// </summary>
    public List<TextEditorTextSpan> TriviaTextSpanList { get; }
    public DiagnosticBag DiagnosticBag { get; }
}
