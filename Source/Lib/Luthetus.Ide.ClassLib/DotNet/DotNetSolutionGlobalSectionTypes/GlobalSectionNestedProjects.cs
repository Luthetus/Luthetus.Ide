namespace Luthetus.Ide.ClassLib.DotNet.DotNetSolutionGlobalSectionTypes;

public record GlobalSectionNestedProjects(
    ImmutableArray<NestedProjectEntry> NestedProjectEntries);