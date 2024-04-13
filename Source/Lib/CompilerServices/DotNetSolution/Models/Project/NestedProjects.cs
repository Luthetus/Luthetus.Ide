using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.Models.Project;

public record NestedProjects(
    ImmutableArray<NestedProjectEntry> NestedProjectEntries);