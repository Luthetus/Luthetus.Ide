using Microsoft.Extensions.DependencyInjection;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.Ide.RazorLib.Installations.Models;
using Luthetus.Extensions.DotNet.Installations.Models;
using Luthetus.Extensions.Config.CompilerServices;
using Luthetus.Extensions.Config.Decorations;
// using Luthetus.Extensions.Git.Installations.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;

namespace Luthetus.Extensions.Config.Installations.Models;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLuthetusConfigServices(
        this IServiceCollection services,
        LuthetusHostingInformation hostingInformation,
        Func<LuthetusIdeConfig, LuthetusIdeConfig>? configure = null)
    {
        return services
            .AddLuthetusExtensionsDotNetServices(hostingInformation, configure)
            // .AddLuthetusExtensionsGitServices(hostingInformation, configure)
            .AddScoped<ICompilerServiceRegistry, ConfigCompilerServiceRegistry>()
            .AddScoped<IDecorationMapperRegistry, DecorationMapperRegistry>();
    }
}