namespace Luthetus.Extensions.DotNet.ComponentRenderers.Models;

public class CompilerServicesTreeViews
{
	public CompilerServicesTreeViews(
		Type treeViewCSharpProjectDependenciesRendererType,
		Type treeViewCSharpProjectNugetPackageReferencesRendererType,
		Type treeViewCSharpProjectToProjectReferencesRendererType,
		Type treeViewCSharpProjectNugetPackageReferenceRendererType,
		Type treeViewCSharpProjectToProjectReferenceRendererType,
		Type treeViewSolutionFolderRendererType,
        Type treeViewCompilerServiceRendererType)
	{
		TreeViewCSharpProjectDependenciesRendererType = treeViewCSharpProjectDependenciesRendererType;
		TreeViewCSharpProjectNugetPackageReferencesRendererType = treeViewCSharpProjectNugetPackageReferencesRendererType;
		TreeViewCSharpProjectToProjectReferencesRendererType = treeViewCSharpProjectToProjectReferencesRendererType;
		TreeViewCSharpProjectNugetPackageReferenceRendererType = treeViewCSharpProjectNugetPackageReferenceRendererType;
		TreeViewCSharpProjectToProjectReferenceRendererType = treeViewCSharpProjectToProjectReferenceRendererType;
		TreeViewSolutionFolderRendererType = treeViewSolutionFolderRendererType;
        TreeViewCompilerServiceRendererType = treeViewCompilerServiceRendererType;
    }

    public Type TreeViewCSharpProjectDependenciesRendererType { get; }
	public Type TreeViewCSharpProjectNugetPackageReferencesRendererType { get; }
	public Type TreeViewCSharpProjectToProjectReferencesRendererType { get; }
	public Type TreeViewCSharpProjectNugetPackageReferenceRendererType { get; }
	public Type TreeViewCSharpProjectToProjectReferenceRendererType { get; }
	public Type TreeViewSolutionFolderRendererType { get; }
    public Type TreeViewCompilerServiceRendererType { get; }
}