using Microsoft.Extensions.DependencyInjection;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.CompilerServices.RazorLib.DotNetSolutions.Displays;
using Luthetus.CompilerServices.RazorLib.CSharpProjects.Displays;
using Luthetus.CompilerServices.RazorLib.Nugets.Displays;
using Luthetus.CompilerServices.RazorLib.Nugets.Models;
using Luthetus.CompilerServices.RazorLib.CommandLines.Models;
using Luthetus.CompilerServices.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.Installations.Models;

namespace Luthetus.CompilerServices.RazorLib.Installations.Models;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLuthetusCompilerServicesRazorLibServices(
        this IServiceCollection services,
        LuthetusHostingInformation hostingInformation,
        Func<LuthetusIdeConfig, LuthetusIdeConfig>? configure = null)
    {
    	return services
            .AddScoped<INugetPackageManagerProvider, NugetPackageManagerProviderAzureSearchUsnc>()
            .AddScoped<DotNetCliOutputParser>();
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