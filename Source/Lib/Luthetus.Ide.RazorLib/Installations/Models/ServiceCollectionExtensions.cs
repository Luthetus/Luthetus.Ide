using Microsoft.Extensions.DependencyInjection;
using Fluxor;
using Luthetus.Ide.RazorLib.DotNetSolutions.States;
using Luthetus.Ide.RazorLib.CompilerServices.States;
using Luthetus.Ide.RazorLib.Editors.States;
using Luthetus.Ide.RazorLib.FileSystems.States;
using Luthetus.Ide.RazorLib.FolderExplorers.States;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.InputFiles.States;
using Luthetus.Ide.RazorLib.LocalStorages.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Commands.Models.Ide;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Themes.Models;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Displays;
using Luthetus.Ide.RazorLib.Nugets.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.Nugets.Displays;
using Luthetus.Ide.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.Gits.Displays;
using Luthetus.Ide.RazorLib.Menus.Models;
using Luthetus.Ide.RazorLib.InputFiles.Displays;
using Luthetus.Ide.RazorLib.FileSystems.Displays;
using Luthetus.Ide.RazorLib.FormsGenerics.Displays;
using Luthetus.Ide.RazorLib.CSharpProjectForms.Displays;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.Ide.RazorLib.Decorations;
using Luthetus.Ide.RazorLib.CompilerServices.Models;

namespace Luthetus.Ide.RazorLib.Installations.Models;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLuthetusIdeRazorLibServices(
        this IServiceCollection services,
        LuthetusHostingInformation hostingInformation,
        Func<LuthetusIdeOptions, LuthetusIdeOptions>? configure = null)
    {
        var ideOptions = new LuthetusIdeOptions();

        if (configure is not null)
            ideOptions = configure.Invoke(ideOptions);

        if (ideOptions.AddLuthetusTextEditor)
        {
            services.AddLuthetusTextEditor(hostingInformation, inTextEditorOptions => inTextEditorOptions with
            {
                CustomThemeRecordBag = LuthetusTextEditorCustomThemeFacts.AllCustomThemesBag,
                InitialThemeKey = ThemeFacts.VisualStudioDarkThemeClone.Key,
            });
        }

        return services
            .AddSingleton(ideOptions)
            .AddSingleton<ILuthetusIdeComponentRenderers>(_ideComponentRenderers)
            .AddScoped<DotNetSolutionSync>()
            .AddScoped<CompilerServiceExplorerSync>()
            .AddScoped<EditorSync>()
            .AddScoped<FileSystemSync>()
            .AddScoped<FolderExplorerSync>()
            .AddScoped<InputFileSync>()
            .AddScoped<LocalStorageSync>()
            .AddLuthetusIdeFileSystem(hostingInformation, ideOptions)
            .AddLuthetusIdeClassLibServices(hostingInformation);
    }

    private static IServiceCollection AddLuthetusIdeFileSystem(
        this IServiceCollection services,
        LuthetusHostingInformation hostingInformation,
        LuthetusIdeOptions ideOptions)
    {
        Func<IServiceProvider, IEnvironmentProvider> environmentProviderFactory;
        Func<IServiceProvider, IFileSystemProvider> fileSystemProviderFactory;

        if (hostingInformation.LuthetusHostingKind == LuthetusHostingKind.Photino)
        {
            environmentProviderFactory = _ => new LocalEnvironmentProvider();
            fileSystemProviderFactory = _ => new LocalFileSystemProvider();
        }
        else
        {
            environmentProviderFactory = _ => new InMemoryEnvironmentProvider();

            fileSystemProviderFactory = serviceProvider => new InMemoryFileSystemProvider(
                serviceProvider.GetRequiredService<IEnvironmentProvider>());
        }

        return services
            .AddSingleton(environmentProviderFactory.Invoke)
            .AddSingleton(fileSystemProviderFactory.Invoke);
    }

    public static IServiceCollection AddLuthetusIdeClassLibServices(
        this IServiceCollection services,
        LuthetusHostingInformation hostingInformation)
    {
        services
            .AddScoped<ICommandFactory, CommandFactory>()
            .AddScoped<ICompilerServiceRegistry, CompilerServiceRegistry>()
            .AddScoped<IDecorationMapperRegistry, DecorationMapperRegistry>()
            .AddScoped<IMenuOptionsFactory, MenuOptionsFactory>()
            .AddScoped<IFileTemplateProvider, FileTemplateProvider>()
            .AddScoped<INugetPackageManagerProvider, NugetPackageManagerProviderAzureSearchUsnc>();

        services
            .AddFluxor(options =>
                options.ScanAssemblies(
                    typeof(ServiceCollectionExtensions).Assembly,
                    typeof(LuthetusCommonOptions).Assembly,
                    typeof(LuthetusTextEditorOptions).Assembly));

        return services;
    }

    private static readonly LuthetusIdeTreeViews _ideTreeViews = new(
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

    private static readonly LuthetusIdeComponentRenderers _ideComponentRenderers = new(
        typeof(BooleanPromptOrCancelDisplay),
        typeof(FileFormDisplay),
        typeof(DeleteFileFormDisplay),
        typeof(NuGetPackageManager),
        typeof(GitChangesDisplay),
        typeof(RemoveCSharpProjectFromSolutionDisplay),
        typeof(InputFileDisplay),
        _ideTreeViews);
}