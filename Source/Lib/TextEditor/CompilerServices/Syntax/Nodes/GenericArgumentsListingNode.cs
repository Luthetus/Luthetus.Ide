using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

/// <summary>
/// Used when defining a syntax which contains a generic type.
/// </summary>
public sealed record GenericArgumentsListingNode : ISyntaxNode
{
    public GenericArgumentsListingNode(
        OpenAngleBracketToken openAngleBracketToken,
        ImmutableArray<GenericArgumentEntryNode> genericArgumentEntryNodeList,
        CloseAngleBracketToken closeAngleBracketToken)
    {
        OpenAngleBracketToken = openAngleBracketToken;
        GenericArgumentEntryNodeList = genericArgumentEntryNodeList;
        CloseAngleBracketToken = closeAngleBracketToken;

        var children = new List<ISyntax>
        {
            OpenAngleBracketToken,
        };

        children.AddRange(GenericArgumentEntryNodeList);

        children.Add(CloseAngleBracketToken);

        ChildList = children.ToImmutableArray();
    }

    public OpenAngleBracketToken OpenAngleBracketToken { get; }
    public ImmutableArray<GenericArgumentEntryNode> GenericArgumentEntryNodeList { get; }
    public CloseAngleBracketToken CloseAngleBracketToken { get; }

    public ImmutableArray<ISyntax> ChildList { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.GenericArgumentsListingNode;
}