using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CodeAnalysis.C.Syntax.SyntaxNodes;

public interface ISyntaxNode : ISyntax
{
    public ImmutableArray<ISyntax> Children { get; }
    /// <summary><see cref="IsFabricated"/> refers to when the <see cref="ParserSession"/> expected a node to be valid, but it wasn't.<br/><br/>For example, a function invocation on an undefined function will construct a <see cref="BoundFunctionInvocationNode"/> with <see cref="IsFabricated"/> set to true.</summary>
    public bool IsFabricated { get; init; }
}
