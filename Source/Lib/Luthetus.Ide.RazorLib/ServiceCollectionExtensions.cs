using Luthetus.Common.RazorLib.ComponentRenderers;
using Luthetus.Common.RazorLib.Notification;
using Luthetus.Common.RazorLib.Store.AccountCase;
using Luthetus.Common.RazorLib.WatchWindow;
using Luthetus.Common.RazorLib.WatchWindow.TreeViewDisplays;
using Luthetus.Ide.ClassLib.ComponentRenderers;
using Luthetus.Ide.ClassLib.FileSystem.Classes.Local;
using Luthetus.Ide.ClassLib.FileSystem.Classes.Website;
using Luthetus.TextEditor.RazorLib;
using Luthetus.Ide.ClassLib;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;
using Luthetus.Ide.RazorLib.CSharpProjectForm;
using Luthetus.Ide.RazorLib.File;
using Luthetus.Ide.RazorLib.FormsGeneric;
using Luthetus.Ide.RazorLib.Git;
using Luthetus.Ide.RazorLib.InputFile;
using Luthetus.Ide.RazorLib.NuGet;
using Luthetus.Ide.RazorLib.TreeViewImplementations;
using Fluxor;
using Microsoft.Extensions.DependencyInjection;
using TreeViewExceptionDisplay = Luthetus.Ide.RazorLib.TreeViewImplementations.TreeViewExceptionDisplay;
using Luthetus.Common.RazorLib.BackgroundTaskCase.Usage;
using Luthetus.Ide.RazorLib.HostedServiceCase;

namespace Luthetus.Ide.RazorLib;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLuthetusIdeRazorLibServices(
        this IServiceCollection services,
        bool isNativeApplication)
    {
        var watchWindowTreeViewRenderers = new WatchWindowTreeViewRenderers(
            typeof(TreeViewTextDisplay),
            typeof(TreeViewReflectionDisplay),
            typeof(TreeViewPropertiesDisplay),
            typeof(TreeViewInterfaceImplementationDisplay),
            typeof(TreeViewFieldsDisplay),
            typeof(TreeViewExceptionDisplay),
            typeof(TreeViewEnumerableDisplay));

        var commonRendererTypes = new LuthetusCommonComponentRenderers(
            typeof(CommonBackgroundTaskDisplay),
            typeof(CommonErrorNotificationDisplay),
            typeof(CommonInformativeNotificationDisplay),
            typeof(TreeViewExceptionDisplay),
            typeof(TreeViewMissingRendererFallbackDisplay),
            watchWindowTreeViewRenderers,
            null);

        // TODO: Move registration of "ILuthetusCommonComponentRenderers" to LuthetusCommon
        services.AddScoped<ILuthetusCommonComponentRenderers>(_ => commonRendererTypes);

        var shouldInitializeFluxor = false;

        services.AddLuthetusTextEditor(inTextEditorOptions =>
        {
            var luthetusCommonOptions =
                (inTextEditorOptions.LuthetusCommonOptions ?? new()) with
                {
                    InitializeFluxor = shouldInitializeFluxor
                };

            return inTextEditorOptions with
            {
                InitializeFluxor = shouldInitializeFluxor,
                CustomThemeRecords = LuthetusTextEditorCustomThemeFacts.AllCustomThemes,
                InitialThemeKey = LuthetusTextEditorCustomThemeFacts.DarkTheme.ThemeKey,
                LuthetusCommonOptions = luthetusCommonOptions
            };
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
            .AddScoped(environmentProviderFactory.Invoke)
            .AddScoped(fileSystemProviderFactory.Invoke);

        if (isNativeApplication)
            services.AddAuthorizationCore();

        services.AddSingleton(_ =>
            new LuthetusIdeOptions(isNativeApplication));

        services.AddScoped<ILuthetusIdeComponentRenderers>(serviceProvider =>
        {
            var blazorCommonComponentRenderers = serviceProvider
                .GetRequiredService<ILuthetusCommonComponentRenderers>();

            return new LuthetusIdeComponentRenderers(
                blazorCommonComponentRenderers,
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
                typeof(TreeViewLightWeightNugetPackageRecordDisplay),
                typeof(TreeViewCSharpProjectToProjectReferenceDisplay),
                typeof(TreeViewSolutionFolderDisplay));
        });

        return services.AddLuthetusIdeClassLibServices();
    }
}