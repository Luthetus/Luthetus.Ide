using Microsoft.Extensions.DependencyInjection;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Ide.RazorLib.Installations.Models;
using Luthetus.CompilerServices.RazorLib.Installations.Models;

namespace Luthetus.Ide.Config.Installations.Models;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLuthetusConfigServices(
        this IServiceCollection services,
        LuthetusHostingInformation hostingInformation,
        Func<LuthetusIdeConfig, LuthetusIdeConfig>? configure = null)
    {
    	return services
            .AddLuthetusCompilerServicesRazorLibServices(hostingInformation, configure);
    }
}