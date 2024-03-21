using Luthetus.TextEditor.RazorLib.Decorations.Models;

namespace Luthetus.Ide.RazorLib.Outputs.Models;

public class OutputDecorationMapper : IDecorationMapper
{
    public string Map(byte decorationByte)
    {
        var decoration = (OutputDecorationKind)decorationByte;

        return decoration switch
        {
            OutputDecorationKind.None => string.Empty,
            OutputDecorationKind.Error => "luth_tree-view-exception",
            OutputDecorationKind.Warning => "luth_te_keyword",
            _ => string.Empty,
        };
    }
}