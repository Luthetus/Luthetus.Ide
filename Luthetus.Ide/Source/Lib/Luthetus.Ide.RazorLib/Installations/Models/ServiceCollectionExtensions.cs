using Microsoft.Extensions.DependencyInjection;
using Fluxor;
using Luthetus.Ide.RazorLib.DotNetSolutions.States;
using Luthetus.Ide.RazorLib.CompilerServices.States;
using Luthetus.Ide.RazorLib.Editors.States;
using Luthetus.Ide.RazorLib.FileSystems.States;
using Luthetus.Ide.RazorLib.FolderExplorers.States;
using Luthetus.Ide.RazorLib.InputFiles.States;
using Luthetus.Ide.RazorLib.LocalStorages.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Themes.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
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
using Luthetus.Ide.RazorLib.Commands;
using Luthetus.Ide.RazorLib.TestExplorers.States;

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
                CustomThemeRecordList = LuthetusTextEditorCustomThemeFacts.AllCustomThemesList,
                InitialThemeKey = ThemeFacts.VisualStudioDarkThemeClone.Key,
				OpenInEditorAsyncFunc = (absolutePathString, serviceProvider) =>
				{
					var environmentProvider = serviceProvider.GetRequiredService<IEnvironmentProvider>();
					var editorSync = serviceProvider.GetRequiredService<EditorSync>();

					editorSync.OpenInEditor(
                        environmentProvider.AbsolutePathFactory(absolutePathString, false),
						false);

					return Task.CompletedTask;
				}
            });
        }

        services
            .AddSingleton(ideOptions)
            .AddSingleton<ILuthetusIdeComponentRenderers>(_ideComponentRenderers)
            .AddScoped<DotNetSolutionSync>()
            .AddScoped<CompilerServiceExplorerSync>()
            .AddScoped<EditorSync>()
            .AddScoped<FileSystemSync>()
            .AddScoped<FolderExplorerSync>()
            .AddScoped<InputFileSync>()
            .AddScoped<LocalStorageSync>()
            .AddScoped<TestExplorerSync>()
            .AddScoped<ICommandFactory, CommandFactory>()
            .AddScoped<ICompilerServiceRegistry, CompilerServiceRegistry>()
            .AddScoped<IDecorationMapperRegistry, DecorationMapperRegistry>()
            .AddScoped<IMenuOptionsFactory, MenuOptionsFactory>()
            .AddScoped<IFileTemplateProvider, FileTemplateProvider>()
            .AddScoped<INugetPackageManagerProvider, NugetPackageManagerProviderAzureSearchUsnc>();

        services.AddFluxor(options => options.ScanAssemblies(
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