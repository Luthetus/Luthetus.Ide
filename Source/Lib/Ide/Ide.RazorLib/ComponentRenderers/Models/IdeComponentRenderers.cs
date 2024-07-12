namespace Luthetus.Ide.RazorLib.ComponentRenderers.Models;

public class IdeComponentRenderers : IIdeComponentRenderers
{
    public IdeComponentRenderers(
        Type booleanPromptOrCancelRendererType,
        Type fileFormRendererType,
        Type deleteFileFormRendererType,
        Type nuGetPackageManagerRendererType,
        Type gitDisplayRendererType,
        Type removeCSharpProjectFromSolutionRendererType,
        Type inputFileRendererType,
        IdeTreeViews ideTreeViews)
    {
        BooleanPromptOrCancelRendererType = booleanPromptOrCancelRendererType;
        FileFormRendererType = fileFormRendererType;
        DeleteFileFormRendererType = deleteFileFormRendererType;
        NuGetPackageManagerRendererType = nuGetPackageManagerRendererType;
        GitDisplayRendererType = gitDisplayRendererType;
        RemoveCSharpProjectFromSolutionRendererType = removeCSharpProjectFromSolutionRendererType;
        InputFileRendererType = inputFileRendererType;
        IdeTreeViews = ideTreeViews;
    }

    public Type BooleanPromptOrCancelRendererType { get; }
    public Type FileFormRendererType { get; }
    public Type DeleteFileFormRendererType { get; }
    public Type NuGetPackageManagerRendererType { get; }
    public Type GitDisplayRendererType { get; }
    public Type RemoveCSharpProjectFromSolutionRendererType { get; }
    public Type InputFileRendererType { get; }
    public IdeTreeViews IdeTreeViews { get; }
}
