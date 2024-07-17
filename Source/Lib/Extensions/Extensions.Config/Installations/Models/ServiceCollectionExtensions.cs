using Microsoft.Extensions.DependencyInjection;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Ide.RazorLib.Installations.Models;
using Luthetus.Extensions.DotNet.Installations.Models;
using Luthetus.Extensions.Config.CompilerServices;
using Luthetus.Extensions.Config.Decorations;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.Decorations.Models;

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
            .AddScoped<ICompilerServiceRegistry, CompilerServiceRegistry>()
            .AddScoped<IDecorationMapperRegistry, DecorationMapperRegistry>();
    }
}