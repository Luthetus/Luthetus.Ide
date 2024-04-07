using Fluxor;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

public class TerminalResource : LuthCompilerServiceResource
{
    private readonly IState<TerminalSessionState> _terminalSessionStateWrap;

    public TerminalResource(
            ResourceUri resourceUri,
            TerminalCompilerService terminalCompilerService,
            IState<TerminalSessionState> terminalSessionStateWrap)
        : base(resourceUri, terminalCompilerService)
    {
        _terminalSessionStateWrap = terminalSessionStateWrap;
    }

    public override ImmutableArray<ISyntaxToken> SyntaxTokenList { get; set; } = ImmutableArray<ISyntaxToken>.Empty;
    public List<TextEditorTextSpan> ManualDecorationTextSpanList { get; } = new List<TextEditorTextSpan>();

    public TerminalSession TerminalSession => _terminalSessionStateWrap.Value.TerminalSessionMap.Values.First(
        x => x.ResourceUri == ResourceUri);

    public override ImmutableArray<TextEditorTextSpan> GetTokenTextSpans()
    {
        var tokenTextSpanList = new List<TextEditorTextSpan>();
        tokenTextSpanList.AddRange(ManualDecorationTextSpanList);
        tokenTextSpanList.AddRange(SyntaxTokenList.Select(st => st.TextSpan));

        return tokenTextSpanList.ToImmutableArray();
    }
}