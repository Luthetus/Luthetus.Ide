using Microsoft.Extensions.DependencyInjection;
using Fluxor;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Ide.RazorLib.Installations.Models;
using Luthetus.Extensions.Config.Installations.Models;

namespace Luthetus.Website.RazorLib;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLuthetusWebsiteServices(
        this IServiceCollection services,
        LuthetusHostingInformation hostingInformation)
    {
        services.AddLuthetusIdeRazorLibServices(hostingInformation);
        services.AddLuthetusConfigServices(hostingInformation);

        return services.AddFluxor(options => options.ScanAssemblies(
            typeof(LuthetusIdeConfig).Assembly,
            typeof(Luthetus.Extensions.DotNet.Installations.Models.ServiceCollectionExtensions).Assembly,
            typeof(Luthetus.Extensions.Git.Installations.Models.ServiceCollectionExtensions).Assembly));
    }
}
