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
    	DiagnosticBag = new();
    }
    
    /// <summary>If not 'DEBUG' then these are set to null after the compilation unit is finished.</summary>
    public List<ISyntaxToken> SyntaxTokenList { get; }
    /// <summary>
    /// MiscTextSpanList contains the comments and the escape characters.
    /// </summary>
    public List<TextEditorTextSpan> MiscTextSpanList { get; }
    public DiagnosticBag DiagnosticBag { get; }
}
