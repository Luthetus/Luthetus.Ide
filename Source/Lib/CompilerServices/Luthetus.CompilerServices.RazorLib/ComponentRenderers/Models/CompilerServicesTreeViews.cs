namespace Luthetus.CompilerServices.RazorLib.ComponentRenderers.Models;

public class CompilerServicesTreeViews
{
    public CompilerServicesTreeViews(
        Type treeViewCSharpProjectDependenciesRendererType,
        Type treeViewCSharpProjectNugetPackageReferencesRendererType,
        Type treeViewCSharpProjectToProjectReferencesRendererType,
        Type treeViewCSharpProjectNugetPackageReferenceRendererType,
        Type treeViewCSharpProjectToProjectReferenceRendererType,
        Type treeViewSolutionFolderRendererType)
    {
        TreeViewCSharpProjectDependenciesRendererType = treeViewCSharpProjectDependenciesRendererType;
        TreeViewCSharpProjectNugetPackageReferencesRendererType = treeViewCSharpProjectNugetPackageReferencesRendererType;
        TreeViewCSharpProjectToProjectReferencesRendererType = treeViewCSharpProjectToProjectReferencesRendererType;
        TreeViewCSharpProjectNugetPackageReferenceRendererType = treeViewCSharpProjectNugetPackageReferenceRendererType;
        TreeViewCSharpProjectToProjectReferenceRendererType = treeViewCSharpProjectToProjectReferenceRendererType;
        TreeViewSolutionFolderRendererType = treeViewSolutionFolderRendererType;
    }

    public Type TreeViewCSharpProjectDependenciesRendererType { get; }
    public Type TreeViewCSharpProjectNugetPackageReferencesRendererType { get; }
    public Type TreeViewCSharpProjectToProjectReferencesRendererType { get; }
    public Type TreeViewCSharpProjectNugetPackageReferenceRendererType { get; }
    public Type TreeViewCSharpProjectToProjectReferenceRendererType { get; }
    public Type TreeViewSolutionFolderRendererType { get; }
}