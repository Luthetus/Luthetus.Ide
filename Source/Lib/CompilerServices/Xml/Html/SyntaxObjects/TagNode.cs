using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.CompilerServices.Xml.Html.SyntaxEnums;

namespace Luthetus.CompilerServices.Xml.Html.SyntaxObjects;

public class TagNode : IHtmlSyntaxNode
{
    public TagNode(
        TagNameNode? openTagNameNode,
        TagNameNode? closeTagNameNode,
		IReadOnlyList<AttributeNode> attributeNodes,
		IReadOnlyList<IHtmlSyntax> childHtmlSyntaxes,
        HtmlSyntaxKind htmlSyntaxKind,
        bool hasSpecialHtmlCharacter = false)
    {
        ChildContent = childHtmlSyntaxes;
        HtmlSyntaxKind = htmlSyntaxKind;
        HasSpecialHtmlCharacter = hasSpecialHtmlCharacter;
        AttributeNodes = attributeNodes;
        OpenTagNameNode = openTagNameNode;
        CloseTagNameNode = closeTagNameNode;

        var childrenAsList = new List<IHtmlSyntax>();

        if (OpenTagNameNode is not null)
            childrenAsList.Add(OpenTagNameNode);

        foreach (var attribute in AttributeNodes)
        {
            childrenAsList.Add(attribute);
        }

        foreach (var child in ChildContent)
        {
            childrenAsList.Add(child);
        }

        if (CloseTagNameNode is not null)
            childrenAsList.Add(CloseTagNameNode);

        Children = childrenAsList;
    }

    public TagNameNode? OpenTagNameNode { get; }
    public TagNameNode? CloseTagNameNode { get; }
    public IReadOnlyList<AttributeNode> AttributeNodes { get; }
    public bool HasSpecialHtmlCharacter { get; }

    public IReadOnlyList<IHtmlSyntax> ChildContent { get; }
    public IReadOnlyList<IHtmlSyntax> Children { get; }
    public HtmlSyntaxKind HtmlSyntaxKind { get; }

    public TextEditorTextSpan TextEditorTextSpan => new(
        0,
        0,
        (byte)GenericDecorationKind.None,
        ResourceUri.Empty,
        string.Empty);
}