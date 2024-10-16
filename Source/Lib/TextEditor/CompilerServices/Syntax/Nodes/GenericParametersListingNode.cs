using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

/// <summary>
/// Used when invoking a syntax which contains a generic type.
/// </summary>
public sealed class GenericParametersListingNode : ISyntaxNode
{
    public GenericParametersListingNode(
        OpenAngleBracketToken openAngleBracketToken,
        ImmutableArray<GenericParameterEntryNode> genericParameterEntryNodes,
        CloseAngleBracketToken closeAngleBracketToken)
    {
        OpenAngleBracketToken = openAngleBracketToken;
        GenericParameterEntryNodeList = genericParameterEntryNodes;
        CloseAngleBracketToken = closeAngleBracketToken;
        
        SetChildList();
    }

    public OpenAngleBracketToken OpenAngleBracketToken { get; }
    public ImmutableArray<GenericParameterEntryNode> GenericParameterEntryNodeList { get; }
    public CloseAngleBracketToken CloseAngleBracketToken { get; }

    public ISyntax[] ChildList { get; private set; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.GenericParametersListingNode;
    
    public void SetChildList()
    {
    	var children = new List<ISyntax>
        {
            OpenAngleBracketToken,
        };

        children.AddRange(GenericParameterEntryNodeList);

        children.Add(CloseAngleBracketToken);

        ChildList = children.ToImmutableArray();
    	throw new NotImplementedException();
    }
}