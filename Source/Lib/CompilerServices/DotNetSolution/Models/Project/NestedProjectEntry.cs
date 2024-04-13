namespace Luthetus.CompilerServices.Lang.DotNetSolution.Models.Project;

public record NestedProjectEntry(
    Guid ChildProjectIdGuid,
    Guid SolutionFolderIdGuid);
