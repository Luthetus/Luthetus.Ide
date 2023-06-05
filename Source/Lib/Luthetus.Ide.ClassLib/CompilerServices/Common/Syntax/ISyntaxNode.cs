using System.Collections.Immutable;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.C.ParserCase;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;

public interface ISyntaxNode : ISyntax
{
    public ImmutableArray<ISyntax> Children { get; init; }
}
