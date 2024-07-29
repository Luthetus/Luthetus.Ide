using Microsoft.Extensions.DependencyInjection;

namespace Luthetus.Tutorials.RazorLib;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLuthetusTutorialsRazorLibServices(
        this IServiceCollection services)
    {
        return services;
    }
}
