using Luthetus.TextEditor.RazorLib.Decorations.Models;

namespace Luthetus.CompilerServices.Css.Decoration;

public class TextEditorCssDecorationMapper : IDecorationMapper
{
    public string Map(byte decorationByte)
    {
        var decoration = (CssDecorationKind)decorationByte;

        return decoration switch
        {
            CssDecorationKind.None => string.Empty,
            CssDecorationKind.Identifier => "luth_te_css-identifier",
            CssDecorationKind.PropertyName => "luth_te_css-property-name",
            CssDecorationKind.PropertyValue => "luth_te_css-property-value",
            CssDecorationKind.Comment => "luth_te_comment",
            _ => string.Empty,
        };
    }
}