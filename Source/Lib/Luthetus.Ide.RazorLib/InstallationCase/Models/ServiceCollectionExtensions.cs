using Microsoft.Extensions.DependencyInjection;
using Fluxor;
using Luthetus.Common.RazorLib;
using Luthetus.TextEditor.RazorLib;
using Luthetus.CompilerServices.Lang.CSharp.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.CSharpProject.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.DotNetSolution.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.FSharp;
using Luthetus.CompilerServices.Lang.JavaScript;
using Luthetus.CompilerServices.Lang.Json;
using Luthetus.CompilerServices.Lang.Razor.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.TypeScript;
using Luthetus.CompilerServices.Lang.Xml;
using Luthetus.Common.RazorLib.FileSystem.Classes.InMemoryFileSystem;
using Luthetus.Common.RazorLib.FileSystem.Classes.Local;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Common.RazorLib.Theme;
using Luthetus.CompilerServices.Lang.Css;
using Luthetus.Ide.RazorLib.DotNetSolutionCase.States;
using Luthetus.Ide.RazorLib.CommandCase.Models;
using Luthetus.Ide.RazorLib.ComponentRenderersCase.Models;
using Luthetus.Ide.RazorLib.FileSystemCase.Models;
using Luthetus.Ide.RazorLib.MenuCase.Models;
using Luthetus.Ide.RazorLib.NugetCase.Models;
using Luthetus.Ide.RazorLib.NugetCase.Displays;
using Luthetus.Ide.RazorLib.FileSystemCase.Displays;
using Luthetus.Ide.RazorLib.FormsGenericCase.Displays;
using Luthetus.Ide.RazorLib.GitCase.Displays;
using Luthetus.Ide.RazorLib.CSharpProjectFormCase.Displays;
using Luthetus.Ide.RazorLib.InputFileCase.Displays;
using Luthetus.Ide.RazorLib.TreeViewImplementationsCase.Displays;
using Luthetus.Ide.RazorLib.CompilerServiceExplorerCase.States;
using Luthetus.Ide.RazorLib.EditorCase.States;

namespace Luthetus.Ide.RazorLib.InstallationCase.Models;

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
                CustomThemeRecords = LuthetusTextEditorCustomThemeFacts.AllCustomThemes,
                InitialThemeKey = ThemeFacts.VisualStudioDarkThemeClone.Key,
            });
        }

        return services
            .AddSingleton(ideOptions)
            .AddSingleton<ILuthetusIdeComponentRenderers>(_ideComponentRenderers)
            .AddScoped<DotNetSolutionSync>()
            .AddScoped<CompilerServiceExplorerSync>()
            .AddScoped<EditorSync>()
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
            .AddScoped<XmlCompilerService>()
            .AddScoped<DotNetSolutionCompilerService>()
            .AddScoped<CSharpProjectCompilerService>()
            .AddScoped<CSharpCompilerService>()
            .AddScoped<RazorCompilerService>()
            .AddScoped<CssCompilerService>()
            .AddScoped<FSharpCompilerService>()
            .AddScoped<JavaScriptCompilerService>()
            .AddScoped<TypeScriptCompilerService>()
            .AddScoped<JsonCompilerService>()
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