using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;

public class BoundNamespaceEntry : ISyntaxNode
{
    public BoundNamespaceEntry(string namespaceName)
    {
    }

    public ImmutableArray<ISyntax> Children => throw new NotImplementedException();

    public bool IsFabricated { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }

    public SyntaxKind SyntaxKind => throw new NotImplementedException();
}
