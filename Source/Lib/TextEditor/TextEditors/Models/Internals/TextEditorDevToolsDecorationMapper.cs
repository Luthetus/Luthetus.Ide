using Luthetus.TextEditor.RazorLib.Decorations.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

public sealed class TextEditorDevToolsDecorationMapper : IDecorationMapper
{
    public string Map(byte decorationByte)
    {
        var decoration = (TextEditorDevToolsDecorationKind)decorationByte;

        return decoration switch
        {
            TextEditorDevToolsDecorationKind.None => string.Empty,
            TextEditorDevToolsDecorationKind.Scope => "luth_te_brace_matching",
            _ => string.Empty,
        };
    }
}
