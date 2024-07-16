using Luthetus.Ide.RazorLib.DotNetSolutions.Displays;
using Luthetus.Ide.RazorLib.CSharpProjects.Displays;
using Luthetus.Ide.RazorLib.Nugets.Displays;
using Luthetus.Ide.RazorLib.Nugets.Models;

namespace Luthetus.CompilerServices.RazorLib.Installations.Models;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLuthetusCompilerServicesRazorLibServices(
        this IServiceCollection services,
        LuthetusHostingInformation hostingInformation,
        Func<LuthetusIdeConfig, LuthetusIdeConfig>? configure = null)
    {
    	services
            .AddScoped<INugetPackageManagerProvider, NugetPackageManagerProviderAzureSearchUsnc>()
            .AddScoped<DotNetCliOutputParser>()
    }
    
    private static readonly CompilerServicesTreeViews _compilerServicesTreeViews = new(
        typeof(TreeViewCSharpProjectDependenciesDisplay),
        typeof(TreeViewCSharpProjectNugetPackageReferencesDisplay),
        typeof(TreeViewCSharpProjectToProjectReferencesDisplay),
        typeof(TreeViewCSharpProjectNugetPackageReferenceDisplay),
        typeof(TreeViewCSharpProjectToProjectReferenceDisplay),
        typeof(TreeViewSolutionFolderDisplay));

    private static readonly CompilerServicesComponentRenderers _compilerServicesComponentRenderers = new(
        typeof(NuGetPackageManager),
        typeof(RemoveCSharpProjectFromSolutionDisplay),
        _compilerServicesTreeViews);
}