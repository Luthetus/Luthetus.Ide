using Luthetus.TextEditor.RazorLib.Decorations.Models;

namespace Luthetus.Ide.RazorLib.ErrorLists.Models;

public class ErrorListDecorationMapper : IDecorationMapper
{
    public string Map(byte decorationByte)
    {
        var decoration = (ErrorListDecorationKind)decorationByte;

        return decoration switch
        {
            ErrorListDecorationKind.None => string.Empty,
            ErrorListDecorationKind.Keyword => "luth_te_keyword",
            _ => string.Empty,
        };
    }
}