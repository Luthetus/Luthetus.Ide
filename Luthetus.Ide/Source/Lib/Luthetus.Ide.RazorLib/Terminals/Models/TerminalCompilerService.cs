using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

public sealed class TerminalCompilerService : LuthCompilerService
{
    public TerminalCompilerService(ITextEditorService textEditorService)
        : base(textEditorService)
    {
    }

    public List<TextEditorTextSpan> TerminalDecorationList { get; } = new List<TextEditorTextSpan>();

    public override ImmutableArray<TextEditorTextSpan> GetTokenTextSpansFor(ResourceUri resourceUri)
    {
        return TerminalDecorationList.ToImmutableArray();
    }
}