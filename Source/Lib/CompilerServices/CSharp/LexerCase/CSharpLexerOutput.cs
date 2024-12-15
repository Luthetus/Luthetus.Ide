using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.CompilerServices.CSharp.LexerCase;

public struct CSharpLexerOutput
{
	public CSharpLexerOutput()
    {
    	#if DEBUG
    	++LuthetusDebugSomething.Lexer_ConstructorInvocationCount;
    	#endif
    	
    	SyntaxTokenList = new();
    	EscapeCharacterList = new();
    	DiagnosticBag = new();
    }
    
    public List<ISyntaxToken> SyntaxTokenList { get; }
    public List<TextEditorTextSpan> EscapeCharacterList { get; }
    public DiagnosticBag DiagnosticBag { get; }
}
