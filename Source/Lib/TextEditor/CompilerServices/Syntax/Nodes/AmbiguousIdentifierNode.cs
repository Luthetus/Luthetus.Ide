using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

/// <summary>
/// Example usage: One finds a <see cref="IdentifierToken"/>, but must
/// continue parsing in order to know if it is a reference to a
/// function, type, variable, or etc...
///
/// TODO: Permit this type to have a nullable GenericParametersListingNode
/// </summary>
public sealed class AmbiguousIdentifierNode : ISyntaxNode
{
    public AmbiguousIdentifierNode(IdentifierToken identifierToken)
    {
        IdentifierToken = identifierToken;

        SetChildList();
    }

    public IdentifierToken IdentifierToken { get; }

    public ISyntax[] ChildList { get; private set; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.AmbiguousIdentifierNode;
    
    public void SetChildList()
    {
    	ChildList = new ISyntax[]
        {
            IdentifierToken,
        };
    }
}
