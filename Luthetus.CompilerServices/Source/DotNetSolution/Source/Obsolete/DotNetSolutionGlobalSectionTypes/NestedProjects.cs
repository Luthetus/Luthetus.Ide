using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.Obsolete.DotNetSolutionGlobalSectionTypes;

public record NestedProjects(
    ImmutableArray<NestedProjectEntry> NestedProjectEntries);