using Microsoft.Extensions.DependencyInjection;
using Fluxor;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Ide.RazorLib.Installations.Models;

namespace Luthetus.Website.RazorLib;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLuthetusWebsiteServices(
        this IServiceCollection services,
        LuthetusHostingInformation hostingInformation)
    {
        services.AddLuthetusIdeRazorLibServices(hostingInformation);

        return services.AddFluxor(options => options.ScanAssemblies(
            typeof(LuthetusCommonOptions).Assembly,
            typeof(LuthetusTextEditorOptions).Assembly,
            typeof(Ide.RazorLib.Installations.Models.ServiceCollectionExtensions).Assembly));
    }
}
