using Microsoft.Extensions.DependencyInjection;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Themes.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.Gits.Displays;
using Luthetus.Ide.RazorLib.Menus.Models;
using Luthetus.Ide.RazorLib.InputFiles.Displays;
using Luthetus.Ide.RazorLib.FileSystems.Displays;
using Luthetus.Ide.RazorLib.FormsGenerics.Displays;
using Luthetus.Ide.RazorLib.Commands;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.Namespaces.Displays;
using Luthetus.Ide.RazorLib.Gits.Models;

namespace Luthetus.Ide.RazorLib.Installations.Models;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLuthetusIdeRazorLibServices(
        this IServiceCollection services,
        LuthetusHostingInformation hostingInformation,
        Func<LuthetusIdeConfig, LuthetusIdeConfig>? configure = null)
    {
        var ideConfig = new LuthetusIdeConfig();

        if (configure is not null)
            ideConfig = configure.Invoke(ideConfig);

        if (ideConfig.AddLuthetusTextEditor)
        {
            ResourceUri RemoveDriveFromResourceUri(ResourceUri resourceUri, IServiceProvider serviceProvider)
            {
                var environmentProvider = serviceProvider.GetRequiredService<IEnvironmentProvider>();

                if (resourceUri.Value.StartsWith(environmentProvider.DriveExecutingFromNoDirectorySeparator))
                {
                    var removeDriveFromResourceUriValue = resourceUri.Value[
                        environmentProvider.DriveExecutingFromNoDirectorySeparator.Length..];

                    return new ResourceUri(removeDriveFromResourceUriValue);
                }

                return resourceUri;
            }

            services.AddLuthetusTextEditor(hostingInformation, inTextEditorOptions => inTextEditorOptions with
            {
                CustomThemeRecordList = LuthetusTextEditorCustomThemeFacts.AllCustomThemesList,
                InitialThemeKey = ThemeFacts.VisualStudioDarkThemeClone.Key,
                RegisterModelFunc = (registerModelArgs) =>
                {
                    registerModelArgs = new RegisterModelArgs(
                        RemoveDriveFromResourceUri(registerModelArgs.ResourceUri, registerModelArgs.ServiceProvider),
                        registerModelArgs.ServiceProvider)
                    {
                    	ShouldBlockUntilBackgroundTaskIsCompleted = registerModelArgs.ShouldBlockUntilBackgroundTaskIsCompleted
                    };

                    var ideBackgroundTaskApi = registerModelArgs.ServiceProvider.GetRequiredService<IdeBackgroundTaskApi>();
                    return ideBackgroundTaskApi.Editor.RegisterModelFunc(registerModelArgs);
                },
                TryRegisterViewModelFunc = (tryRegisterViewModelArgs) =>
                {
                    tryRegisterViewModelArgs = new TryRegisterViewModelArgs(
                        tryRegisterViewModelArgs.ViewModelKey,
                        RemoveDriveFromResourceUri(tryRegisterViewModelArgs.ResourceUri, tryRegisterViewModelArgs.ServiceProvider),
                        tryRegisterViewModelArgs.Category,
                        tryRegisterViewModelArgs.ShouldSetFocusToEditor,
                        tryRegisterViewModelArgs.ServiceProvider);

                    var ideBackgroundTaskApi = tryRegisterViewModelArgs.ServiceProvider.GetRequiredService<IdeBackgroundTaskApi>();
                    return ideBackgroundTaskApi.Editor.TryRegisterViewModelFunc(tryRegisterViewModelArgs);
                },
                TryShowViewModelFunc = (tryShowViewModelArgs) =>
                {
                    var ideBackgroundTaskApi = tryShowViewModelArgs.ServiceProvider.GetRequiredService<IdeBackgroundTaskApi>();
                    return ideBackgroundTaskApi.Editor.TryShowViewModelFunc(tryShowViewModelArgs);
                },
            });
        }

        services
            .AddSingleton(ideConfig)
            .AddSingleton<IIdeComponentRenderers>(_ideComponentRenderers)
            .AddScoped<IdeBackgroundTaskApi>()
            .AddScoped<ICommandFactory, CommandFactory>()
            .AddScoped<IMenuOptionsFactory, MenuOptionsFactory>()
            .AddScoped<IFileTemplateProvider, FileTemplateProvider>()
			.AddScoped<GitCliOutputParser>();

        return services;
    }

    private static readonly IdeTreeViews _ideTreeViews = new(
        typeof(TreeViewNamespacePathDisplay),
        typeof(TreeViewAbsolutePathDisplay),
        typeof(TreeViewGitFileDisplay));

    private static readonly IdeComponentRenderers _ideComponentRenderers = new(
        typeof(BooleanPromptOrCancelDisplay),
        typeof(FileFormDisplay),
        typeof(DeleteFileFormDisplay),
        typeof(GitChangesDisplay),
        typeof(InputFileDisplay),
        _ideTreeViews);
}