using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.SemanticContextCase.Keys;
using Luthetus.Ide.ClassLib.DotNet;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.SemanticContextCase.Implementations;

public sealed record DotNetSolutionSemanticContext : ISemanticContext
{
    public DotNetSolutionSemanticContext(
        DotNetSolutionKey key,
        DotNetSolution dotNetSolution)
    {
        Key = key;
        DotNetSolution = dotNetSolution;

        foreach (var dotNetProject in DotNetSolution.DotNetProjects)
        {
            if (dotNetProject.DotNetProjectKind == DotNetProjectKind.CSharpProject)
            {
                var dotNetProjectContext = new DotNetProjectSemanticContext(
                    DotNetProjectKey.NewProjectKey(),
                    dotNetProject,
                    ImmutableDictionary<NamespaceKey, NamespaceSemanticContext>.Empty);

                DotNetProjectContextMap = DotNetProjectContextMap.Add(
                    dotNetProjectContext.Key,
                    dotNetProjectContext);
            }
        }
    }

    public DotNetSolutionKey Key { get; init; }
    public DotNetSolution DotNetSolution { get; init; }
    public ImmutableDictionary<DotNetProjectKey, DotNetProjectSemanticContext> DotNetProjectContextMap { get; init; } = ImmutableDictionary<DotNetProjectKey, DotNetProjectSemanticContext>.Empty;
}


