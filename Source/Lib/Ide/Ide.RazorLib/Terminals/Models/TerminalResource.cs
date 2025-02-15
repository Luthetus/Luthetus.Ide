using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

public class TerminalResource : CompilerServiceResource
{
    /// <summary>
    /// The <see cref="ArgumentsTextSpan"/> and <see cref="TargetFilePathTextSpan"/> are currently
    /// mutable state. If these properties are re-written, then this lock is not needed.<br/><br/>
    /// 
    /// This lock is intended to be used only to read or write to <see cref="ArgumentsTextSpan"/> or <see cref="TargetFilePathTextSpan"/>
    /// and preferably, one would in bulk, read or write both properties from the same lock() { ... }
    /// </summary>
    public readonly object UnsafeStateLock = new();

    public TerminalResource(
	    	ResourceUri resourceUri,
	    	TerminalCompilerService terminalCompilerService)
        : base(resourceUri, terminalCompilerService)
    {
    }

    public override IReadOnlyList<SyntaxToken> SyntaxTokenList { get; set; } = new List<SyntaxToken>();
    public List<TextEditorTextSpan> ManualDecorationTextSpanList { get; } = new List<TextEditorTextSpan>();
    public List<Symbol> ManualSymbolList { get; } = new List<Symbol>();

    public override IReadOnlyList<TextEditorTextSpan> GetTokenTextSpans()
    {
        var tokenTextSpanList = new List<TextEditorTextSpan>();
        tokenTextSpanList.AddRange(ManualDecorationTextSpanList);
        tokenTextSpanList.AddRange(SyntaxTokenList.Select(st => st.TextSpan));

        return tokenTextSpanList;
    }

    public override IReadOnlyList<Symbol> GetSymbols()
    {
        return ManualSymbolList;
    }
}