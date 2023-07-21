namespace Luthetus.Ide.ClassLib.ComponentRenderers;

public interface ILuthetusIdeComponentRenderers
{
    public ILuthetusCommonComponentRenderers? LuthetusCommonComponentRenderers { get; }
    public Type? BooleanPromptOrCancelRendererType { get; }
    public Type? FileFormRendererType { get; }
    public Type? DeleteFileFormRendererType { get; }
    public Type? TreeViewNamespacePathRendererType { get; }
    public Type? TreeViewSolutionFolderRendererType { get; }
    public Type? TreeViewCSharpProjectDependenciesRendererType { get; }
    public Type? TreeViewCSharpProjectNugetPackageReferencesRendererType { get; }
    public Type? TreeViewCSharpProjectToProjectReferencesRendererType { get; }
    public Type? TreeViewLightWeightNugetPackageRecordRendererType { get; }
    public Type? TreeViewCSharpProjectToProjectReferenceRendererType { get; }
    public Type? TreeViewAbsoluteFilePathRendererType { get; }
    public Type? TreeViewGitFileRendererType { get; }
    public Type? NuGetPackageManagerRendererType { get; }
    public Type? GitDisplayRendererType { get; }
    public Type? RemoveCSharpProjectFromSolutionRendererType { get; }
    public Type? InputFileRendererType { get; }
    public Type? CompilerServiceBackgroundTaskDisplayRendererType { get; }
    public Type? FileSystemBackgroundTaskDisplayRendererType { get; }
}