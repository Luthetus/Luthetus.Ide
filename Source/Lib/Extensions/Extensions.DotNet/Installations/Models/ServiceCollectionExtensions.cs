using Microsoft.Extensions.DependencyInjection;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Ide.RazorLib.Installations.Models;
using Luthetus.Extensions.DotNet.CSharpProjects.Displays;
using Luthetus.Extensions.DotNet.DotNetSolutions.Displays;
using Luthetus.Extensions.DotNet.Menus.Models;
using Luthetus.Extensions.DotNet.Nugets.Models;
using Luthetus.Extensions.DotNet.ComponentRenderers.Models;
using Luthetus.Extensions.DotNet.Nugets.Displays;
using Luthetus.Extensions.DotNet.CommandLines.Models;
using Luthetus.Extensions.DotNet.BackgroundTasks.Models;
using Luthetus.Extensions.DotNet.CompilerServices.Displays;
using Luthetus.Extensions.DotNet.Commands;

namespace Luthetus.Extensions.DotNet.Installations.Models;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddLuthetusExtensionsDotNetServices(
		this IServiceCollection services,
		LuthetusHostingInformation hostingInformation,
		Func<LuthetusIdeConfig, LuthetusIdeConfig>? configure = null)
	{
		return services
			.AddScoped<INugetPackageManagerProvider, NugetPackageManagerProviderAzureSearchUsnc>()
			.AddScoped<DotNetCliOutputParser>()
			.AddScoped<DotNetBackgroundTaskApi>()
			.AddScoped<IDotNetCommandFactory, DotNetCommandFactory>()
			.AddScoped<IDotNetMenuOptionsFactory, DotNetMenuOptionsFactory>()
			.AddScoped<IDotNetComponentRenderers>(_ => _dotNetComponentRenderers);
	}

	private static readonly CompilerServicesTreeViews _dotNetTreeViews = new(
		typeof(TreeViewCSharpProjectDependenciesDisplay),
		typeof(TreeViewCSharpProjectNugetPackageReferencesDisplay),
		typeof(TreeViewCSharpProjectToProjectReferencesDisplay),
		typeof(TreeViewCSharpProjectNugetPackageReferenceDisplay),
		typeof(TreeViewCSharpProjectToProjectReferenceDisplay),
		typeof(TreeViewSolutionFolderDisplay),
        typeof(TreeViewCompilerServiceDisplay));

	private static readonly DotNetComponentRenderers _dotNetComponentRenderers = new(
		typeof(NuGetPackageManager),
		typeof(RemoveCSharpProjectFromSolutionDisplay),
		_dotNetTreeViews);
}