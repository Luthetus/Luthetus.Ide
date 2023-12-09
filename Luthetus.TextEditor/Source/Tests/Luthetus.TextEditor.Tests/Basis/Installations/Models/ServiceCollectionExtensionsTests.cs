using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.ComponentRenderers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Microsoft.Extensions.DependencyInjection;

namespace Luthetus.TextEditor.RazorLib.Installations.Models;

public static class ServiceCollectionExtensionsTests
{
    public static IServiceCollection AddLuthetusTextEditor(
        this IServiceCollection services,
        LuthetusHostingInformation hostingInformation,
        Func<LuthetusTextEditorOptions, LuthetusTextEditorOptions>? configure = null)
    {
        var textEditorOptions = new LuthetusTextEditorOptions();

        if (configure is not null)
            textEditorOptions = configure.Invoke(textEditorOptions);

        if (textEditorOptions.AddLuthetusCommon)
            services.AddLuthetusCommonServices(hostingInformation);

        services
            .AddSingleton(textEditorOptions)
            .AddSingleton<ILuthetusTextEditorComponentRenderers>(_textEditorComponentRenderers)
            .AddScoped(serviceProvider => textEditorOptions.AutocompleteServiceFactory.Invoke(serviceProvider))
            .AddScoped(serviceProvider => textEditorOptions.AutocompleteIndexerFactory.Invoke(serviceProvider))
            .AddScoped<ITextEditorService, TextEditorService>();

        return services;
    }

    private static readonly LuthetusTextEditorComponentRenderers _textEditorComponentRenderers = new(
        typeof(TextEditorSymbolDisplay),
        typeof(TextEditorDiagnosticDisplay));
}