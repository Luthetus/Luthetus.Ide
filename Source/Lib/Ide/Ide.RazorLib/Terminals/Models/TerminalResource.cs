using System.Collections.Immutable;
using Fluxor;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.Ide.RazorLib.Terminals.States;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

public class TerminalResource : CompilerServiceResource
{
    private readonly IState<TerminalState> _terminalStateWrap;

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

    public override IReadOnlyList<ISyntaxToken> SyntaxTokenList { get; set; } = ImmutableArray<ISyntaxToken>.Empty;
    public List<TextEditorTextSpan> ManualDecorationTextSpanList { get; } = new List<TextEditorTextSpan>();
    public List<ITextEditorSymbol> ManualSymbolList { get; } = new List<ITextEditorSymbol>();

    public override IReadOnlyList<TextEditorTextSpan> GetTokenTextSpans()
    {
        var tokenTextSpanList = new List<TextEditorTextSpan>();
        tokenTextSpanList.AddRange(ManualDecorationTextSpanList);
        tokenTextSpanList.AddRange(SyntaxTokenList.Select(st => st.TextSpan));

        return tokenTextSpanList;
    }

    public override IReadOnlyList<ITextEditorSymbol> GetSymbols()
    {
        return ManualSymbolList;
    }
}