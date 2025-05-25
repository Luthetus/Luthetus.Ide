namespace Luthetus.CompilerServices.DotNetSolution.Models.Project;

public record GuidNestedProjectEntry(
    Guid ChildProjectIdGuid,
    Guid SolutionFolderIdGuid);
