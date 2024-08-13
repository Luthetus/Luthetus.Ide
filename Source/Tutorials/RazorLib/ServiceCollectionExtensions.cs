using Microsoft.Extensions.DependencyInjection;
using Fluxor;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;

namespace Luthetus.Tutorials.RazorLib;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLuthetusTutorialsRazorLibServices(
        this IServiceCollection services,
        LuthetusHostingInformation hostingInformation)
    {
        services.AddLuthetusTextEditor(hostingInformation);
        
        return services.AddFluxor(options => options.ScanAssemblies(
            typeof(LuthetusCommonConfig).Assembly,
            typeof(LuthetusTextEditorConfig).Assembly));
    }
}
