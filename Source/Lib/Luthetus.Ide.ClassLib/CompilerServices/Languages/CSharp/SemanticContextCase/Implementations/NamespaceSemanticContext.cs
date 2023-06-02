using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.SemanticContextCase.Keys;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.SemanticContextCase.Implementations;

public sealed record NamespaceSemanticContext
{
    public NamespaceSemanticContext(
        NamespaceKey key,
        ImmutableDictionary<FileKey, FileSemanticContext> fileContextMap)
    {
        Key = key;
        FileContexts = fileContextMap;
    }

    public NamespaceKey Key { get; init; }
    public ImmutableDictionary<FileKey, FileSemanticContext> FileContexts { get; set; }
}
