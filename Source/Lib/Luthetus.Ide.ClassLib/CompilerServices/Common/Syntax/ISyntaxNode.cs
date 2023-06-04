using System.Collections.Immutable;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.C.ParserCase;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;

public interface ISyntaxNode : ISyntax
{
    public ImmutableArray<ISyntax> Children { get; init; }
    /// <summary><see cref="IsFabricated"/> refers to when the <see cref="ParserSession"/> expected a node to be valid, but it wasn't.<br/><br/>For example, a function invocation on an undefined function will construct a <see cref="BoundFunctionInvocationNode"/> with <see cref="IsFabricated"/> set to true.</summary>
    public bool IsFabricated { get; init; }
}
