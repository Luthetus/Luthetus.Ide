using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;

/// <summary>
/// Used when invoking a syntax which contains a generic type.
/// </summary>
public sealed record GenericParametersListingNode : ISyntaxNode
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