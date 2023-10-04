namespace Luthetus.CompilerServices.Lang.DotNetSolution.Obsolete.DotNetSolutionGlobalSectionTypes;

public record NestedProjectEntry(
    Guid ChildProjectIdGuid,
    Guid SolutionFolderIdGuid);