using Microsoft.Extensions.DependencyInjection;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Themes.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.CodeSearches.Models;
using Luthetus.Ide.RazorLib.StartupControls.Models;
using Luthetus.Ide.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.Menus.Models;
using Luthetus.Ide.RazorLib.InputFiles.Displays;
using Luthetus.Ide.RazorLib.InputFiles.Models;
using Luthetus.Ide.RazorLib.FileSystems.Displays;
using Luthetus.Ide.RazorLib.FormsGenerics.Displays;
using Luthetus.Ide.RazorLib.Commands;
using Luthetus.Ide.RazorLib.CommandBars.Models;
using Luthetus.Ide.RazorLib.FindAllReferences.Models;
using Luthetus.Ide.RazorLib.Shareds.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.FolderExplorers.Models;
using Luthetus.Ide.RazorLib.Namespaces.Displays;
using Luthetus.Ide.RazorLib.AppDatas.Models;

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
            services.AddLuthetusTextEditor(hostingInformation, inTextEditorOptions => inTextEditorOptions with
            {
                CustomThemeRecordList = LuthetusTextEditorCustomThemeFacts.AllCustomThemesList,
                InitialThemeKey = ThemeFacts.VisualStudioDarkThemeClone.Key,
                AbsolutePathStandardizeFunc = AbsolutePathStandardizeFunc,
                FastParseFunc = async (fastParseArgs) =>
                {
                	var standardizedAbsolutePathString = await AbsolutePathStandardizeFunc(
                		fastParseArgs.ResourceUri.Value, fastParseArgs.ServiceProvider);
                		
                	var standardizedResourceUri = new ResourceUri(standardizedAbsolutePathString);
                
                    fastParseArgs = new FastParseArgs(
                        standardizedResourceUri,
                        fastParseArgs.ExtensionNoPeriod,
                        fastParseArgs.ServiceProvider)
                    {
                    	ShouldBlockUntilBackgroundTaskIsCompleted = fastParseArgs.ShouldBlockUntilBackgroundTaskIsCompleted
                    };

                    var ideBackgroundTaskApi = fastParseArgs.ServiceProvider.GetRequiredService<IdeBackgroundTaskApi>();
                    await ideBackgroundTaskApi.Editor.FastParseFunc(fastParseArgs);
                },
                RegisterModelFunc = async (registerModelArgs) =>
                {
                	var standardizedAbsolutePathString = await AbsolutePathStandardizeFunc(
                		registerModelArgs.ResourceUri.Value, registerModelArgs.ServiceProvider);
                		
                	var standardizedResourceUri = new ResourceUri(standardizedAbsolutePathString);
                
                    registerModelArgs = new RegisterModelArgs(
                    	registerModelArgs.EditContext,
                        standardizedResourceUri,
                        registerModelArgs.ServiceProvider)
                    {
                    	ShouldBlockUntilBackgroundTaskIsCompleted = registerModelArgs.ShouldBlockUntilBackgroundTaskIsCompleted
                    };

                    var ideBackgroundTaskApi = registerModelArgs.ServiceProvider.GetRequiredService<IdeBackgroundTaskApi>();
                    await ideBackgroundTaskApi.Editor.RegisterModelFunc(registerModelArgs);
                },
                TryRegisterViewModelFunc = async (tryRegisterViewModelArgs) =>
                {
                	var standardizedAbsolutePathString = await AbsolutePathStandardizeFunc(
                		tryRegisterViewModelArgs.ResourceUri.Value, tryRegisterViewModelArgs.ServiceProvider);
                		
                	var standardizedResourceUri = new ResourceUri(standardizedAbsolutePathString);
                	
                    tryRegisterViewModelArgs = new TryRegisterViewModelArgs(
                    	tryRegisterViewModelArgs.EditContext,
                        tryRegisterViewModelArgs.ViewModelKey,
                        standardizedResourceUri,
                        tryRegisterViewModelArgs.Category,
                        tryRegisterViewModelArgs.ShouldSetFocusToEditor,
                        tryRegisterViewModelArgs.ServiceProvider);

                    var ideBackgroundTaskApi = tryRegisterViewModelArgs.ServiceProvider.GetRequiredService<IdeBackgroundTaskApi>();
                    return await ideBackgroundTaskApi.Editor.TryRegisterViewModelFunc(tryRegisterViewModelArgs);
                },
                TryShowViewModelFunc = (tryShowViewModelArgs) =>
                {
                    var ideBackgroundTaskApi = tryShowViewModelArgs.ServiceProvider.GetRequiredService<IdeBackgroundTaskApi>();
                    return ideBackgroundTaskApi.Editor.TryShowViewModelFunc(tryShowViewModelArgs);
                },
            });
        }
        
        if (hostingInformation.LuthetusHostingKind == LuthetusHostingKind.Photino)
        	services.AddScoped<IAppDataService, NativeAppDataService>();
        else
        	services.AddScoped<IAppDataService, DoNothingAppDataService>();

        services
            .AddSingleton(ideConfig)
            .AddSingleton<IIdeComponentRenderers>(_ideComponentRenderers)
            .AddScoped<IdeBackgroundTaskApi>()
            .AddScoped<ICommandFactory, CommandFactory>()
            .AddScoped<IMenuOptionsFactory, MenuOptionsFactory>()
            .AddScoped<IFileTemplateProvider, FileTemplateProvider>()
            .AddScoped<ICodeSearchService, CodeSearchService>()
            .AddScoped<IInputFileService, InputFileService>()
            .AddScoped<IStartupControlService, StartupControlService>()
            .AddScoped<ITerminalService, TerminalService>()
            .AddScoped<ITerminalGroupService, TerminalGroupService>()
            .AddScoped<IFolderExplorerService, FolderExplorerService>()
            .AddScoped<IIdeMainLayoutService, IdeMainLayoutService>()
            .AddScoped<IIdeHeaderService, IdeHeaderService>()
            .AddScoped<ICommandBarService, CommandBarService>()
            .AddScoped<IFindAllReferencesService, FindAllReferencesService>();

        return services;
    }
    
    public static Task<string> AbsolutePathStandardizeFunc(string absolutePathString, IServiceProvider serviceProvider)
    {
        var environmentProvider = serviceProvider.GetRequiredService<IEnvironmentProvider>();

        if (absolutePathString.StartsWith(environmentProvider.DriveExecutingFromNoDirectorySeparator))
        {
            var removeDriveFromResourceUriValue = absolutePathString[
                environmentProvider.DriveExecutingFromNoDirectorySeparator.Length..];

            return Task.FromResult(removeDriveFromResourceUriValue);
        }

        return Task.FromResult(absolutePathString);
    }

    private static readonly IdeTreeViews _ideTreeViews = new(
        typeof(TreeViewNamespacePathDisplay),
        typeof(TreeViewAbsolutePathDisplay));

    private static readonly IdeComponentRenderers _ideComponentRenderers = new(
        typeof(BooleanPromptOrCancelDisplay),
        typeof(FileFormDisplay),
        typeof(DeleteFileFormDisplay),
        typeof(InputFileDisplay),
        _ideTreeViews);
}