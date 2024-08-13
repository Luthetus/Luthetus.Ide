using Microsoft.Extensions.DependencyInjection;
using Fluxor;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Tutorials.RazorLib.CompilerServices;
using Luthetus.Tutorials.RazorLib.Decorations;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.Decorations.Models;

namespace Luthetus.Tutorials.RazorLib;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLuthetusTutorialsRazorLibServices(
        this IServiceCollection services,
        LuthetusHostingInformation hostingInformation)
    {
        services.AddLuthetusTextEditor(hostingInformation);
        
        services
            .AddScoped<ICompilerServiceRegistry, CompilerServiceRegistry>()
            .AddScoped<IDecorationMapperRegistry, DecorationMapperRegistry>();
        
        return services.AddFluxor(options => options.ScanAssemblies(
            typeof(LuthetusCommonConfig).Assembly,
            typeof(LuthetusTextEditorConfig).Assembly));
    }
}
