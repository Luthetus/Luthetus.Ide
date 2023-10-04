using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;

public sealed record GenericArgumentsListingNode : ISyntaxNode
{
    public GenericArgumentsListingNode(
        OpenAngleBracketToken openAngleBracketToken,
        ImmutableArray<GenericArgumentEntryNode> genericArgumentEntryNodes,
        CloseAngleBracketToken closeAngleBracketToken)
    {
        OpenAngleBracketToken = openAngleBracketToken;
        GenericArgumentEntryNodeBag = genericArgumentEntryNodes;
        CloseAngleBracketToken = closeAngleBracketToken;

        var children = new List<ISyntax>
        {
            OpenAngleBracketToken,
        };

        children.AddRange(GenericArgumentEntryNodeBag);

        children.Add(CloseAngleBracketToken);

        ChildBag = children.ToImmutableArray();
    }

    public OpenAngleBracketToken OpenAngleBracketToken { get; }
    public ImmutableArray<GenericArgumentEntryNode> GenericArgumentEntryNodeBag { get; }
    public CloseAngleBracketToken CloseAngleBracketToken { get; }

    public ImmutableArray<ISyntax> ChildBag { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.GenericArgumentsListingNode;
}