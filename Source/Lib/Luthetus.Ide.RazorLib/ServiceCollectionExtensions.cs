using Luthetus.Ide.RazorLib.CSharpProjectForm;
using Luthetus.Ide.RazorLib.File;
using Luthetus.Ide.RazorLib.FormsGeneric;
using Luthetus.Ide.RazorLib.Git;
using Luthetus.Ide.RazorLib.InputFile;
using Luthetus.Ide.RazorLib.NuGet;
using Luthetus.Ide.RazorLib.TreeViewImplementations;
using Luthetus.Ide.RazorLib.HostedServiceCase;
using Microsoft.Extensions.DependencyInjection;
using Luthetus.TextEditor.RazorLib.HostedServiceCase.CompilerServiceCase;
using Luthetus.TextEditor.RazorLib;
using Luthetus.Common.RazorLib.Store.AccountCase;
using Fluxor;
using Luthetus.Ide.ClassLib.ComponentRenderers;
using Luthetus.Ide.ClassLib;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Common.RazorLib.FileSystem.Classes.Local;
using Luthetus.Common.RazorLib.FileSystem.Classes.Website;

namespace Luthetus.Ide.RazorLib;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLuthetusIdeRazorLibServices(
        this IServiceCollection services,
        bool isNativeApplication,
        Func<LuthetusTextEditorOptions, LuthetusTextEditorOptions>? configureTextEditorOptions = null)
    {
        services.AddLuthetusTextEditor(inTextEditorOptions =>
        {
            inTextEditorOptions = inTextEditorOptions with
            {
                CustomThemeRecords = LuthetusTextEditorCustomThemeFacts.AllCustomThemes,
                InitialThemeKey = LuthetusTextEditorCustomThemeFacts.DarkTheme.ThemeKey,
            };

            return configureTextEditorOptions is null 
                ? inTextEditorOptions
                : configureTextEditorOptions.Invoke(inTextEditorOptions);
        });

        Func<IServiceProvider, IEnvironmentProvider> environmentProviderFactory;
        Func<IServiceProvider, IFileSystemProvider> fileSystemProviderFactory;

        if (isNativeApplication)
        {
            environmentProviderFactory = _ => new LocalEnvironmentProvider();

            fileSystemProviderFactory = _ => new LocalFileSystemProvider();
        }
        else
        {
            environmentProviderFactory = serviceProvider =>
                new WebsiteEnvironmentProvider(
                    serviceProvider.GetRequiredService<IState<AccountState>>());

            fileSystemProviderFactory = serviceProvider =>
                new WebsiteFileSystemProvider(
                    serviceProvider.GetRequiredService<IEnvironmentProvider>(),
                    serviceProvider.GetRequiredService<IState<AccountState>>(),
                    serviceProvider.GetRequiredService<HttpClient>());
        }

        services
            .AddSingleton(environmentProviderFactory.Invoke)
            .AddSingleton(fileSystemProviderFactory.Invoke);

        services.AddSingleton(_ =>
            new LuthetusIdeOptions(isNativeApplication));

        services.AddSingleton<ILuthetusIdeComponentRenderers>(serviceProvider =>
        {
            return new LuthetusIdeComponentRenderers(
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
                typeof(CompilerServiceBackgroundTaskDisplay),
                typeof(FileSystemBackgroundTaskDisplay),
                typeof(TreeViewCSharpProjectDependenciesDisplay),
                typeof(TreeViewCSharpProjectNugetPackageReferencesDisplay),
                typeof(TreeViewCSharpProjectToProjectReferencesDisplay),
                typeof(TreeViewCSharpProjectNugetPackageReferenceDisplay),
                typeof(TreeViewCSharpProjectToProjectReferenceDisplay),
                typeof(TreeViewSolutionFolderDisplay));
        });

        return services.AddLuthetusIdeClassLibServices();
    }
}