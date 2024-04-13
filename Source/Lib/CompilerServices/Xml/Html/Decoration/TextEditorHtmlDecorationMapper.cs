using Luthetus.TextEditor.RazorLib.Decorations.Models;

namespace Luthetus.CompilerServices.Lang.Xml.Html.Decoration;

public class TextEditorHtmlDecorationMapper : IDecorationMapper
{
    public string Map(byte decorationByte)
    {
        var decoration = (HtmlDecorationKind)decorationByte;

        return decoration switch
        {
            HtmlDecorationKind.None => string.Empty,
            HtmlDecorationKind.AttributeName => "luth_te_attribute-name",
            HtmlDecorationKind.AttributeValue => "luth_te_attribute-value",
            HtmlDecorationKind.Comment => "luth_te_comment",
            HtmlDecorationKind.CustomTagName => "luth_te_custom-tag-name",
            HtmlDecorationKind.EntityReference => "luth_te_entity-reference",
            HtmlDecorationKind.HtmlCode => "luth_te_html-code",
            HtmlDecorationKind.InjectedLanguageFragment => "luth_te_injected-language-fragment",
            HtmlDecorationKind.InjectedLanguageComponent => "luth_te_injected-language-component",
            HtmlDecorationKind.TagName => "luth_te_tag-name",
            HtmlDecorationKind.Tag => "luth_te_tag",
            HtmlDecorationKind.Error => "luth_te_error",
            HtmlDecorationKind.InjectedLanguageCodeBlock => "luth_te_injected-language-code-block",
            HtmlDecorationKind.InjectedLanguageCodeBlockTag => "luth_te_injected-language-code-block-tag",
            HtmlDecorationKind.InjectedLanguageKeyword => "luth_te_keyword",
            HtmlDecorationKind.InjectedLanguageTagHelperAttribute => "luth_te_injected-language-tag-helper-attribute",
            HtmlDecorationKind.InjectedLanguageTagHelperElement => "luth_te_injected-language-tag-helper-element",
            HtmlDecorationKind.InjectedLanguageMethod => "luth_te_method",
            HtmlDecorationKind.InjectedLanguageVariable => "luth_te_variable",
            HtmlDecorationKind.InjectedLanguageType => "luth_te_type",
            HtmlDecorationKind.InjectedLanguageStringLiteral => "luth_te_string-literal",
            _ => string.Empty,
        };
    }
}