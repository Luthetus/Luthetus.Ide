using Luthetus.TextEditor.RazorLib.Decorations.Models;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

public class TerminalDecorationMapper : IDecorationMapper
{
    public string Map(byte decorationByte)
    {
        var decoration = (TerminalDecorationKind)decorationByte;

        return decoration switch
        {
            TerminalDecorationKind.None => string.Empty,
            TerminalDecorationKind.Comment => "luth_te_comment",
            TerminalDecorationKind.Keyword => "luth_te_keyword",
            TerminalDecorationKind.StringLiteral => "luth_te_string-literal",
            TerminalDecorationKind.TargetFilePath => "luth_te_type",
            _ => string.Empty,
        };
    }
}
