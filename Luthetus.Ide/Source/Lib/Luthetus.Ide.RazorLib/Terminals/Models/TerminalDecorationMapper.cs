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
            _ => string.Empty,
        };
    }
}
