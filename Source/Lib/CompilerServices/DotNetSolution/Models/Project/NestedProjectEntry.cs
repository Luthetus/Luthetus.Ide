namespace Luthetus.CompilerServices.DotNetSolution.Models.Project;

public record NestedProjectEntry(
    Guid ChildProjectIdGuid,
    Guid SolutionFolderIdGuid);
