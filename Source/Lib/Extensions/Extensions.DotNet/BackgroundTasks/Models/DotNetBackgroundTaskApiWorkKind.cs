namespace Luthetus.Extensions.DotNet.BackgroundTasks.Models;

public enum DotNetBackgroundTaskApiWorkKind
{
	None,
    SolutionExplorer_TreeView_MultiSelect_DeleteFiles,
    LuthetusExtensionsDotNetInitializerOnInit,
    LuthetusExtensionsDotNetInitializerOnAfterRender,
    SubmitNuGetQuery,
    RunTestByFullyQualifiedName
}
