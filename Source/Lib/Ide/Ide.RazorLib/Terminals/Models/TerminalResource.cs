using System.Collections.Immutable;
using Fluxor;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
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
            TerminalCompilerService terminalCompilerService,
            IState<TerminalState> terminalStateWrap)
        : base(resourceUri, terminalCompilerService)
    {
        _terminalStateWrap = terminalStateWrap;
    }

    public override ImmutableArray<ISyntaxToken> SyntaxTokenList { get; set; } = ImmutableArray<ISyntaxToken>.Empty;
    public TextEditorTextSpan ArgumentsTextSpan { get; set; } = new TextEditorTextSpan(0, 0, 0, new ResourceUri(string.Empty), string.Empty);
    public TextEditorTextSpan TargetFilePathTextSpan { get; set; } = new TextEditorTextSpan(0, 0, 0, new ResourceUri(string.Empty), string.Empty);
    public List<TextEditorTextSpan> ManualDecorationTextSpanList { get; } = new List<TextEditorTextSpan>();
    public List<ITextEditorSymbol> ManualSymbolList { get; } = new List<ITextEditorSymbol>();

    public Terminal Terminal => _terminalStateWrap.Value.TerminalMap.Values.First(
        x => x.ResourceUri == ResourceUri);

    public override ImmutableArray<TextEditorTextSpan> GetTokenTextSpans()
    {
        var tokenTextSpanList = new List<TextEditorTextSpan>();
        tokenTextSpanList.AddRange(ManualDecorationTextSpanList);
        tokenTextSpanList.AddRange(SyntaxTokenList.Select(st => st.TextSpan));

        return tokenTextSpanList.ToImmutableArray();
    }

    public override ImmutableArray<ITextEditorSymbol> GetSymbols()
    {
        return ManualSymbolList.ToImmutableArray();
    }
}