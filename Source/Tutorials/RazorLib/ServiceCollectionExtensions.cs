using Microsoft.Extensions.DependencyInjection;
using Luthetus.Common.RazorLib.Installations.Models;

namespace Luthetus.Tutorials.RazorLib;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLuthetusTutorialsRazorLibServices(
        this IServiceCollection services,
        LuthetusHostingInformation hostingInformation)
    {
        return services.AddLuthetusCommonServices(hostingInformation);
    }
}
