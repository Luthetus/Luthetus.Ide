using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.ComponentRenderers.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Luthetus.TextEditor.RazorLib.Installations.Models;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLuthetusTextEditor(
        this IServiceCollection services,
        LuthetusHostingInformation hostingInformation,
        Func<LuthetusTextEditorConfig, LuthetusTextEditorConfig>? configure = null)
    {
        var textEditorConfig = new LuthetusTextEditorConfig();

        if (configure is not null)
            textEditorConfig = configure.Invoke(textEditorConfig);

        if (textEditorConfig.AddLuthetusCommon)
            services.AddLuthetusCommonServices(hostingInformation);

        services
            .AddSingleton(textEditorConfig)
            .AddSingleton<ILuthetusTextEditorComponentRenderers>(_textEditorComponentRenderers)
            .AddScoped(serviceProvider => textEditorConfig.AutocompleteServiceFactory.Invoke(serviceProvider))
            .AddScoped(serviceProvider => textEditorConfig.AutocompleteIndexerFactory.Invoke(serviceProvider))
            .AddScoped<ITextEditorService, TextEditorService>()
            .AddScoped<ITextEditorRegistryWrap, TextEditorRegistryWrap>();
        
        return services;
    }

    private static readonly LuthetusTextEditorComponentRenderers _textEditorComponentRenderers = new(
        typeof(TextEditors.Displays.Internals.SymbolDisplay),
        typeof(TextEditors.Displays.Internals.DiagnosticDisplay));
}