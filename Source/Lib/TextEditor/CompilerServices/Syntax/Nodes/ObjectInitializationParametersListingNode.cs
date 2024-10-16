using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class ObjectInitializationParametersListingNode : ISyntaxNode
{
    public ObjectInitializationParametersListingNode(
        OpenBraceToken openBraceToken,
        ImmutableArray<ObjectInitializationParameterEntryNode> objectInitializationParameterEntryNodeList,
        CloseBraceToken closeBraceToken)
    {
        OpenBraceToken = openBraceToken;
        ObjectInitializationParameterEntryNodeList = objectInitializationParameterEntryNodeList;
        CloseBraceToken = closeBraceToken;

        SetChildList();
    }

    public OpenBraceToken OpenBraceToken { get; }
    public ImmutableArray<ObjectInitializationParameterEntryNode> ObjectInitializationParameterEntryNodeList { get; }
    public CloseBraceToken CloseBraceToken { get; }

    public ISyntax[] ChildList { get; private set; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.ObjectInitializationParametersListingNode;
    
    public void SetChildList()
    {
    	var children = new List<ISyntax>
        {
            OpenBraceToken
        };

        children.AddRange(ObjectInitializationParameterEntryNodeList);

        children.Add(CloseBraceToken);

        ChildList = children.ToImmutableArray();
    	throw new NotImplementedException();
    }
}
