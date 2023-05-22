using BlazorCommon.RazorLib.BackgroundTaskCase;
using BlazorCommon.RazorLib.Clipboard;
using BlazorCommon.RazorLib.ComponentRenderers;
using BlazorCommon.RazorLib.Notification;
using BlazorCommon.RazorLib.Store.AccountCase;
using BlazorCommon.RazorLib.WatchWindow;
using BlazorCommon.RazorLib.WatchWindow.TreeViewDisplays;
using BlazorStudio.ClassLib.ComponentRenderers;
using BlazorStudio.ClassLib.FileSystem.Classes.Local;
using BlazorStudio.ClassLib.FileSystem.Classes.Website;
using BlazorTextEditor.RazorLib;
using Fluxor;
using Luthetus.Ide.ClassLib;
using Luthetus.Ide.ClassLib.ComponentRenderers;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;
using Luthetus.Ide.RazorLib.CSharpProjectForm;
using Luthetus.Ide.RazorLib.File;
using Luthetus.Ide.RazorLib.FormsGeneric;
using Luthetus.Ide.RazorLib.Git;
using Luthetus.Ide.RazorLib.InputFile;
using Luthetus.Ide.RazorLib.NuGet;
using Luthetus.Ide.RazorLib.TreeViewImplementations;
using Microsoft.Extensions.DependencyInjection;
using TreeViewExceptionDisplay = Luthetus.Ide.RazorLib.TreeViewImplementations.TreeViewExceptionDisplay;

namespace Luthetus.Ide.RazorLib;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBlazorStudioRazorLibServices(
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

        var commonRendererTypes = new BlazorCommonComponentRenderers(
            typeof(BackgroundTaskDisplay),
            typeof(CommonErrorNotificationDisplay),
            typeof(CommonInformativeNotificationDisplay),
            typeof(TreeViewExceptionDisplay),
            typeof(TreeViewMissingRendererFallbackDisplay),
            watchWindowTreeViewRenderers);

        // TODO: Move registration of "IBlazorCommonComponentRenderers" to BlazorCommon
        services.AddScoped<IBlazorCommonComponentRenderers>(_ => commonRendererTypes);

        var shouldInitializeFluxor = false;

        services.AddBlazorTextEditor(inTextEditorOptions =>
        {
            var blazorCommonOptions =
                (inTextEditorOptions.BlazorCommonOptions ?? new()) with
                {
                    InitializeFluxor = shouldInitializeFluxor
                };

            if (isNativeApplication)
            {
                var blazorCommonFactories = blazorCommonOptions.BlazorCommonFactories with
                {
                    ClipboardServiceFactory = _ => new InMemoryClipboardService(true),
                };

                blazorCommonOptions = blazorCommonOptions with
                {
                    BlazorCommonFactories = blazorCommonFactories
                };
            }

            return inTextEditorOptions with
            {
                InitializeFluxor = shouldInitializeFluxor,
                CustomThemeRecords = BlazorTextEditorCustomThemeFacts.AllCustomThemes,
                InitialThemeKey = BlazorTextEditorCustomThemeFacts.DarkTheme.ThemeKey,
                BlazorCommonOptions = blazorCommonOptions
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
            new BlazorStudioOptions(isNativeApplication));

        services.AddScoped<ILuthetusIdeComponentRenderers>(serviceProvider =>
        {
            var blazorCommonComponentRenderers = serviceProvider
                .GetRequiredService<IBlazorCommonComponentRenderers>();

            return new BlazorStudioComponentRenderers(
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