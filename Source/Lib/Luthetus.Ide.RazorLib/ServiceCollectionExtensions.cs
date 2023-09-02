using Luthetus.Ide.RazorLib.CSharpProjectForm;
using Luthetus.Ide.RazorLib.File;
using Luthetus.Ide.RazorLib.FormsGeneric;
using Luthetus.Ide.RazorLib.Git;
using Luthetus.Ide.RazorLib.InputFile;
using Luthetus.Ide.RazorLib.NuGet;
using Luthetus.Ide.RazorLib.TreeViewImplementations;
using Microsoft.Extensions.DependencyInjection;
using Luthetus.TextEditor.RazorLib;
using Luthetus.Ide.ClassLib.ComponentRenderers;
using Luthetus.Ide.ClassLib;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Common.RazorLib.FileSystem.Classes.Local;
using Luthetus.Common.RazorLib.FileSystem.Classes.InMemoryFileSystem;

namespace Luthetus.Ide.RazorLib;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLuthetusIdeRazorLibServices(
        this IServiceCollection services,
        Func<LuthetusIdeOptions, LuthetusIdeOptions>? configure = null)
    {
        var ideOptions = new LuthetusIdeOptions();

        if (configure is not null)
            ideOptions = configure.Invoke(ideOptions);

        if (ideOptions.AddLuthetusTextEditor)
        {
            services.AddLuthetusTextEditor(inTextEditorOptions => inTextEditorOptions with
            {
                CustomThemeRecords = LuthetusTextEditorCustomThemeFacts.AllCustomThemes,
                InitialThemeKey = LuthetusTextEditorCustomThemeFacts.DarkTheme.ThemeKey,
            });
        }

        return services
            .AddSingleton(ideOptions)
            .AddSingleton<ILuthetusIdeComponentRenderers>(_ideComponentRenderers)
            .AddLuthetusIdeFileSystem(ideOptions)
            .AddLuthetusIdeClassLibServices();
    }

    private static IServiceCollection AddLuthetusIdeFileSystem(
        this IServiceCollection services,
        LuthetusIdeOptions ideOptions)
    {
        Func<IServiceProvider, IEnvironmentProvider> environmentProviderFactory;
        Func<IServiceProvider, IFileSystemProvider> fileSystemProviderFactory;

        if (ideOptions.IsNativeApplication)
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

    private static readonly LuthetusIdeComponentRenderers _ideComponentRenderers = new(
        typeof(BooleanPromptOrCancelDisplay),
        typeof(FileFormDisplay),
        typeof(DeleteFileFormDisplay),
        typeof(TreeViewNamespacePathDisplay),
        typeof(TreeViewAbsoluteFilePathDisplay),
        typeof(TreeViewGitFileDisplay),
        typeof(NuGetPackageManager),
        typeof(GitChangesDisplay),
        typeof(RemoveCSharpProjectFromSolutionDisplay),
        typeof(InputFileDisplay),
        typeof(TreeViewCSharpProjectDependenciesDisplay),
        typeof(TreeViewCSharpProjectNugetPackageReferencesDisplay),
        typeof(TreeViewCSharpProjectToProjectReferencesDisplay),
        typeof(TreeViewCSharpProjectNugetPackageReferenceDisplay),
        typeof(TreeViewCSharpProjectToProjectReferenceDisplay),
        typeof(TreeViewSolutionFolderDisplay));
}