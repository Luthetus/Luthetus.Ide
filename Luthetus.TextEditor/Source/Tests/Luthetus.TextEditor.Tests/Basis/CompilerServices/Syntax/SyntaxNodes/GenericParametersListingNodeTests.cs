using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

public sealed record GenericParametersListingNodeTests
{
    public GenericParametersListingNode(
        OpenAngleBracketToken openAngleBracketToken,
        ImmutableArray<GenericParameterEntryNode> genericParameterEntryNodes,
        CloseAngleBracketToken closeAngleBracketToken)
    {
        OpenAngleBracketToken = openAngleBracketToken;
        GenericParameterEntryNodeBag = genericParameterEntryNodes;
        CloseAngleBracketToken = closeAngleBracketToken;

        var children = new List<ISyntax>
        {
            OpenAngleBracketToken,
        };

        children.AddRange(GenericParameterEntryNodeBag);

        children.Add(CloseAngleBracketToken);

        ChildBag = children.ToImmutableArray();
    }

    public OpenAngleBracketToken OpenAngleBracketToken { get; }
    public ImmutableArray<GenericParameterEntryNode> GenericParameterEntryNodeBag { get; }
    public CloseAngleBracketToken CloseAngleBracketToken { get; }

    public ImmutableArray<ISyntax> ChildBag { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.GenericParametersListingNode;
}