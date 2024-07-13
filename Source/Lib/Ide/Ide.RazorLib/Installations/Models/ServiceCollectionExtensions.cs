using Microsoft.Extensions.DependencyInjection;
using Fluxor;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Themes.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.Ide.RazorLib.Nugets.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.Nugets.Displays;
using Luthetus.Ide.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.Gits.Displays;
using Luthetus.Ide.RazorLib.Menus.Models;
using Luthetus.Ide.RazorLib.InputFiles.Displays;
using Luthetus.Ide.RazorLib.FileSystems.Displays;
using Luthetus.Ide.RazorLib.FormsGenerics.Displays;
using Luthetus.Ide.RazorLib.CSharpProjects.Displays;
using Luthetus.Ide.RazorLib.Decorations;
using Luthetus.Ide.RazorLib.CompilerServices.Models;
using Luthetus.Ide.RazorLib.Commands;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.CompilerServices.Displays;
using Luthetus.Ide.RazorLib.Namespaces.Displays;
using Luthetus.Ide.RazorLib.DotNetSolutions.Displays;
using Luthetus.Ide.RazorLib.CommandLines.Models;
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
                RegisterModelFunc = async (registerModelArgs) =>
                {
                    registerModelArgs = new RegisterModelArgs(
                        RemoveDriveFromResourceUri(registerModelArgs.ResourceUri, registerModelArgs.ServiceProvider),
                        registerModelArgs.ServiceProvider);

                    var ideBackgroundTaskApi = registerModelArgs.ServiceProvider.GetRequiredService<IdeBackgroundTaskApi>();
                    await ideBackgroundTaskApi.Editor.RegisterModelFunc(registerModelArgs).ConfigureAwait(false);
                },
                TryRegisterViewModelFunc = async (tryRegisterViewModelArgs) =>
                {
                    tryRegisterViewModelArgs = new TryRegisterViewModelArgs(
                        tryRegisterViewModelArgs.ViewModelKey,
                        RemoveDriveFromResourceUri(tryRegisterViewModelArgs.ResourceUri, tryRegisterViewModelArgs.ServiceProvider),
                        tryRegisterViewModelArgs.Category,
                        tryRegisterViewModelArgs.ShouldSetFocusToEditor,
                        tryRegisterViewModelArgs.ServiceProvider);

                    var ideBackgroundTaskApi = tryRegisterViewModelArgs.ServiceProvider.GetRequiredService<IdeBackgroundTaskApi>();
                    return await ideBackgroundTaskApi.Editor.TryRegisterViewModelFunc(tryRegisterViewModelArgs).ConfigureAwait(false);
                },
                TryShowViewModelFunc = async (tryShowViewModelArgs) =>
                {
                    var ideBackgroundTaskApi = tryShowViewModelArgs.ServiceProvider.GetRequiredService<IdeBackgroundTaskApi>();
                    return await ideBackgroundTaskApi.Editor.TryShowViewModelFunc(tryShowViewModelArgs).ConfigureAwait(false);
                },
            });
        }

        services
            .AddSingleton(ideConfig)
            .AddSingleton<IIdeComponentRenderers>(_ideComponentRenderers)
            .AddScoped<IdeBackgroundTaskApi>()
            .AddScoped<ICommandFactory, CommandFactory>()
            .AddScoped<ICompilerServiceRegistry, CompilerServiceRegistry>()
            .AddScoped<IDecorationMapperRegistry, DecorationMapperRegistry>()
            .AddScoped<IMenuOptionsFactory, MenuOptionsFactory>()
            .AddScoped<IFileTemplateProvider, FileTemplateProvider>()
            .AddScoped<INugetPackageManagerProvider, NugetPackageManagerProviderAzureSearchUsnc>()
            .AddScoped<DotNetCliOutputParser>()
			.AddScoped<GitCliOutputParser>();

        return services;
    }

    private static readonly IdeTreeViews _ideTreeViews = new(
        typeof(TreeViewNamespacePathDisplay),
        typeof(TreeViewAbsolutePathDisplay),
        typeof(TreeViewGitFileDisplay),
        typeof(TreeViewCompilerServiceDisplay),
        typeof(TreeViewCSharpProjectDependenciesDisplay),
        typeof(TreeViewCSharpProjectNugetPackageReferencesDisplay),
        typeof(TreeViewCSharpProjectToProjectReferencesDisplay),
        typeof(TreeViewCSharpProjectNugetPackageReferenceDisplay),
        typeof(TreeViewCSharpProjectToProjectReferenceDisplay),
        typeof(TreeViewSolutionFolderDisplay));

    private static readonly IdeComponentRenderers _ideComponentRenderers = new(
        typeof(BooleanPromptOrCancelDisplay),
        typeof(FileFormDisplay),
        typeof(DeleteFileFormDisplay),
        typeof(NuGetPackageManager),
        typeof(GitChangesDisplay),
        typeof(RemoveCSharpProjectFromSolutionDisplay),
        typeof(InputFileDisplay),
        _ideTreeViews);
}