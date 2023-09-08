namespace Luthetus.Ide.ClassLib.ComponentRenderers;

public class LuthetusIdeComponentRenderers : ILuthetusIdeComponentRenderers
{
    public LuthetusIdeComponentRenderers(
        Type? booleanPromptOrCancelRendererType,
        Type? fileFormRendererType,
        Type? deleteFileFormRendererType,
        Type? nuGetPackageManagerRendererType,
        Type? gitDisplayRendererType,
        Type? removeCSharpProjectFromSolutionRendererType,
        Type? inputFileRendererType,
        LuthetusIdeTreeViews luthetusIdeTreeViews)
    {
        BooleanPromptOrCancelRendererType = booleanPromptOrCancelRendererType;
        FileFormRendererType = fileFormRendererType;
        DeleteFileFormRendererType = deleteFileFormRendererType;
        NuGetPackageManagerRendererType = nuGetPackageManagerRendererType;
        GitDisplayRendererType = gitDisplayRendererType;
        RemoveCSharpProjectFromSolutionRendererType = removeCSharpProjectFromSolutionRendererType;
        InputFileRendererType = inputFileRendererType;
        LuthetusIdeTreeViews = luthetusIdeTreeViews;
    }

    public Type? BooleanPromptOrCancelRendererType { get; }
    public Type? FileFormRendererType { get; }
    public Type? DeleteFileFormRendererType { get; }
    public Type? NuGetPackageManagerRendererType { get; }
    public Type? GitDisplayRendererType { get; }
    public Type? RemoveCSharpProjectFromSolutionRendererType { get; }
    public Type? InputFileRendererType { get; }
    public LuthetusIdeTreeViews LuthetusIdeTreeViews { get; }
}
