using System.Collections.Immutable;
using Luthetus.CompilerServices.Lang.Xml.Html.SyntaxEnums;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.CompilerServices.Lang.Xml.Html.SyntaxObjects;

public class TagNode : IHtmlSyntaxNode
{
    public TagNode(
        TagNameNode? openTagNameNode,
        TagNameNode? closeTagNameNode,
        ImmutableArray<AttributeNode> attributeNodes,
        ImmutableArray<IHtmlSyntax> childHtmlSyntaxes,
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

        Children = childrenAsList.ToImmutableArray();
    }

    public TagNameNode? OpenTagNameNode { get; }
    public TagNameNode? CloseTagNameNode { get; }
    public ImmutableArray<AttributeNode> AttributeNodes { get; }
    public bool HasSpecialHtmlCharacter { get; }

    public ImmutableArray<IHtmlSyntax> ChildContent { get; }
    public ImmutableArray<IHtmlSyntax> Children { get; }
    public HtmlSyntaxKind HtmlSyntaxKind { get; }

    public TextEditorTextSpan TextEditorTextSpan => new(
        0,
        0,
        (byte)GenericDecorationKind.None,
        new ResourceUri(string.Empty),
        string.Empty);
}