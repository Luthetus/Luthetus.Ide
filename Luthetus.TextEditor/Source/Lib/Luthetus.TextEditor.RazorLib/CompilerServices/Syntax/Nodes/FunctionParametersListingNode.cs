using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

/// <summary>
/// Used when invoking a function.
/// </summary>
public sealed record FunctionParametersListingNode : ISyntaxNode
{
    public FunctionParametersListingNode(
        OpenParenthesisToken openParenthesisToken,
        ImmutableArray<FunctionParameterEntryNode> functionParameterEntryNodes,
        CloseParenthesisToken closeParenthesisToken)
    {
        OpenParenthesisToken = openParenthesisToken;
        FunctionParameterEntryNodeList = functionParameterEntryNodes;
        CloseParenthesisToken = closeParenthesisToken;

        var children = new List<ISyntax>
        {
            OpenParenthesisToken
        };

        children.AddRange(FunctionParameterEntryNodeList);

        children.Add(CloseParenthesisToken);

        ChildList = children.ToImmutableArray();
    }

    public OpenParenthesisToken OpenParenthesisToken { get; }
    public ImmutableArray<FunctionParameterEntryNode> FunctionParameterEntryNodeList { get; }
    public CloseParenthesisToken CloseParenthesisToken { get; }

    public ImmutableArray<ISyntax> ChildList { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.FunctionParametersListingNode;
}
