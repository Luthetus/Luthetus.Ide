using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax.SyntaxTokens;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;

public class BoundInheritanceStatementNode : ISyntaxNode
{
    public BoundInheritanceStatementNode(
        IdentifierToken parentClassIdentifierToken)
    {
        ParentClassIdentifierToken = parentClassIdentifierToken;
        
        Children = new ISyntax[]
        {
            ParentClassIdentifierToken
        }.ToImmutableArray();
    }

    public IdentifierToken ParentClassIdentifierToken { get; }

    public ImmutableArray<ISyntax> Children { get; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BoundInheritanceStatementNode;
}
