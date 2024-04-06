using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

public class TerminalResource : LuthCompilerServiceResource
{
    public TerminalResource(ResourceUri resourceUri, TerminalCompilerService terminalCompilerService)
        : base(resourceUri, terminalCompilerService)
    {
    }

    public override ImmutableArray<ISyntaxToken> SyntaxTokenList { get; set; } = ImmutableArray<ISyntaxToken>.Empty;
    public List<TextEditorTextSpan> ManualDecorationList { get; } = new List<TextEditorTextSpan>();

    public override ImmutableArray<TextEditorTextSpan> GetTokenTextSpans()
    {
        var tokenTextSpanList = new List<TextEditorTextSpan>();
        tokenTextSpanList.AddRange(ManualDecorationList);
        tokenTextSpanList.AddRange(SyntaxTokenList.Select(st => st.TextSpan));

        return tokenTextSpanList.ToImmutableArray();
    }
}