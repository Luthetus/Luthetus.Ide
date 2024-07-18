using Luthetus.TextEditor.RazorLib.Decorations.Models;

namespace Luthetus.CompilerServices.Json.Decoration;

public class TextEditorJsonDecorationMapper : IDecorationMapper
{
    public string Map(byte decorationByte)
    {
        var decoration = (JsonDecorationKind)decorationByte;

        return decoration switch
        {
            JsonDecorationKind.PropertyKey => "luth_te_json-property-key",
            JsonDecorationKind.String => "luth_te_string-literal",
            JsonDecorationKind.Number => "luth_te_number",
            JsonDecorationKind.Integer => "luth_te_integer",
            JsonDecorationKind.Keyword => "luth_te_keyword",
            JsonDecorationKind.LineComment => "luth_te_comment",
            JsonDecorationKind.BlockComment => "luth_te_comment",
            JsonDecorationKind.None => string.Empty,
            JsonDecorationKind.Null => string.Empty,
            JsonDecorationKind.Document => string.Empty,
            JsonDecorationKind.Error => string.Empty,
            _ => string.Empty,
        };
    }
}