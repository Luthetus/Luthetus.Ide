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
    
    public List<ISyntaxToken> SyntaxTokenList { get; }
    public List<TextEditorTextSpan> MiscTextSpanList { get; }
    public DiagnosticBag DiagnosticBag { get; }
}
