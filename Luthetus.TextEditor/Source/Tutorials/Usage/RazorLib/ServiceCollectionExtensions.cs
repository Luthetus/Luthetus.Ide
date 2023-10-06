using Fluxor;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.CompilerServices.Lang.CSharp.CompilerServiceCase;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Luthetus.TextEditor.Usage.RazorLib;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLuthetusTextEditorUsageServices(
        this IServiceCollection services,
        LuthetusHostingInformation luthetusHostingInformation)
    {
        services.AddScoped<CSharpCompilerService>();

        services
            .AddLuthetusTextEditor(luthetusHostingInformation)
            .AddFluxor(options => options.ScanAssemblies(
                typeof(LuthetusCommonOptions).Assembly,
                typeof(LuthetusTextEditorOptions).Assembly));

        return services;
    }
}
