using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed record ObjectInitializationParametersListingNode : ISyntaxNode
{
    public ObjectInitializationParametersListingNode(
        OpenBraceToken openBraceToken,
        ImmutableArray<ObjectInitializationParameterEntryNode> objectInitializationParameterEntryNodeList,
        CloseBraceToken closeBraceToken)
    {
        OpenBraceToken = openBraceToken;
        ObjectInitializationParameterEntryNodeList = objectInitializationParameterEntryNodeList;
        CloseBraceToken = closeBraceToken;

        var children = new List<ISyntax>
        {
            OpenBraceToken
        };

        children.AddRange(ObjectInitializationParameterEntryNodeList);

        children.Add(CloseBraceToken);

        ChildList = children.ToImmutableArray();
    }

    public OpenBraceToken OpenBraceToken { get; }
    public ImmutableArray<ObjectInitializationParameterEntryNode> ObjectInitializationParameterEntryNodeList { get; }
    public CloseBraceToken CloseBraceToken { get; }

    public ImmutableArray<ISyntax> ChildList { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.ObjectInitializationParametersListingNode;
}
