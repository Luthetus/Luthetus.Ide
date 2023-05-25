using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using System.Collections.Immutable;
using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase;

/// <summary>
/// Many files in C# can have the same namespace.
/// Therefore, my thought process is that the <see cref="BoundNamespace"/>
/// will be a CompilationUnit in a sense.
/// <br/><br/>
/// Then, the BoundNamespace will have a <see cref="BoundNamespaceEntry"/>
/// for each C# file which has a namespace declaration with a matching
/// namespace name. Be it a block namespace or file namespace declaration.
/// <br/><br/>
/// I feel as though I'm making everything a compilation unit so that is
/// weird.
/// <br/><br/>
/// I'd furthermore treat each <see cref="BoundNamespaceEntry"/> as a
/// CompilationUnit.
/// <br/><br/>
/// A scope could be created by iterating over all the using statements
/// and looking up into a dictionary the <see cref="BoundNamespace"/>
/// and then creating one large compilation unit which is the aggregation
/// of all SyntaxNodes in each respective namespace.
/// </br><br/>
/// In short I don't know, but going to continue on trying to figure it out.
/// </summary>
public class BoundNamespace : ISyntaxNode
{
    public BoundNamespace(string namespaceName)
    {
    }

    public ImmutableArray<ISyntax> Children => throw new NotImplementedException();
    public bool IsFabricated { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
    public SyntaxKind SyntaxKind => throw new NotImplementedException();
}
