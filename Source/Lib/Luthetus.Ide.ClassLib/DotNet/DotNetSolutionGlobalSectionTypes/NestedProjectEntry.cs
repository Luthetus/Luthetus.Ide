namespace Luthetus.Ide.ClassLib.DotNet.DotNetSolutionGlobalSectionTypes;

public record NestedProjectEntry(
    Guid ChildProjectIdGuid,
    Guid SolutionFolderIdGuid);