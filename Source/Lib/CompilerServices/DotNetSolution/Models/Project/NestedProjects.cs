using System.Collections.Immutable;

namespace Luthetus.CompilerServices.DotNetSolution.Models.Project;

public record NestedProjects(
    ImmutableArray<NestedProjectEntry> NestedProjectEntries);