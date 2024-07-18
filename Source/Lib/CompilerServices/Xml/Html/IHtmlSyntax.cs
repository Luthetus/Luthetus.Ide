using Luthetus.CompilerServices.Xml.Html.SyntaxEnums;

namespace Luthetus.CompilerServices.Xml.Html;

public interface IHtmlSyntax
{
    public HtmlSyntaxKind HtmlSyntaxKind { get; }
}