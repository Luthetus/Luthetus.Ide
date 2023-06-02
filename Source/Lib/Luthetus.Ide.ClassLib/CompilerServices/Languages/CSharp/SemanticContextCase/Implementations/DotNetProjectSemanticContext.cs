using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.SemanticContextCase.Keys;
using Luthetus.Ide.ClassLib.DotNet;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.SemanticContextCase.Implementations;

public sealed record DotNetProjectSemanticContext
{
    public DotNetProjectSemanticContext(
        DotNetProjectKey key,
        IDotNetProject dotNetProject,
        ImmutableDictionary<NamespaceKey, NamespaceSemanticContext> namespaceContextMap)
    {
        Key = key;
        DotNetProject = dotNetProject;
        NamespaceContextMap = namespaceContextMap;
    }

    public DotNetProjectKey Key { get; init; }
    public IDotNetProject DotNetProject { get; init; }
    public ImmutableDictionary<NamespaceKey, NamespaceSemanticContext> NamespaceContextMap { get; init; }
}
