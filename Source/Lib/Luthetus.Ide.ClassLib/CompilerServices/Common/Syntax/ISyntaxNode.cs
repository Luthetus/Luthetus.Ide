using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;

public interface ISyntaxNode : ISyntax
{
    public ImmutableArray<ISyntax> Children { get; init; }
}
