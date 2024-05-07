using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Namespaces.Models;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.WatchWindows.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.Models.Project;
using Luthetus.CompilerServices.Lang.DotNetSolution.SyntaxActors;
using Luthetus.Ide.RazorLib.CodeSearches.States;
using Luthetus.Ide.RazorLib.CommandLines.Models;
using Luthetus.Ide.RazorLib.CompilerServices.Models;
using Luthetus.Ide.RazorLib.CompilerServices.States;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.DotNetSolutions.States;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Models;
using Luthetus.Ide.RazorLib.Websites.ProjectTemplates.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.FindAlls.States;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using static Luthetus.Ide.RazorLib.CompilerServices.States.CompilerServiceExplorerState;

namespace Luthetus.Ide.RazorLib.BackgroundTasks.Models;

public class LuthetusIdeBackgroundTaskServiceApi
{
    private readonly IBackgroundTaskService _backgroundTaskService;

    public LuthetusIdeBackgroundTaskServiceApi(IBackgroundTaskService backgroundTaskService)
    {
        _backgroundTaskService = backgroundTaskService;
    }

    public Task WriteToLocalStorage(IStorageService storageService, string key, object value)
    {
        return _backgroundTaskService.EnqueueAsync(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.GetQueueKey(),
            "WriteToStorage",
            async () =>
            {
                var valueJson = System.Text.Json.JsonSerializer.Serialize(value);
                await storageService.SetValue(key, valueJson).ConfigureAwait(false);
            });
    }

    public Task SetCompilerServiceExplorerTreeView(
        IState<CompilerServiceExplorerState> compilerServiceExplorerStateWrap,
        CompilerServiceRegistry compilerServiceRegistry,
        ILuthetusIdeComponentRenderers ideComponentRenderers,
        ILuthetusCommonComponentRenderers commonComponentRenderers,
        ITreeViewService treeViewService,
        IDispatcher dispatcher)
    {
        return _backgroundTaskService.EnqueueAsync(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.GetQueueKey(),
            "Set CompilerServiceExplorer TreeView",
            async () =>
            {
                var compilerServiceExplorerState = compilerServiceExplorerStateWrap.Value;

                var xmlCompilerServiceWatchWindowObject = new WatchWindowObject(
                    compilerServiceRegistry.XmlCompilerService, compilerServiceRegistry.XmlCompilerService.GetType(), "XML", true);

                var dotNetSolutionCompilerServiceWatchWindowObject = new WatchWindowObject(
                    compilerServiceRegistry.DotNetSolutionCompilerService, compilerServiceRegistry.DotNetSolutionCompilerService.GetType(), ".NET Solution", true);

                var cSharpProjectCompilerServiceWatchWindowObject = new WatchWindowObject(
                    compilerServiceRegistry.CSharpProjectCompilerService, compilerServiceRegistry.CSharpProjectCompilerService.GetType(), "C# Project", true);

                var cSharpCompilerServiceWatchWindowObject = new WatchWindowObject(
                    compilerServiceRegistry.CSharpCompilerService, compilerServiceRegistry.CSharpCompilerService.GetType(), "C#", true);

                var razorCompilerServiceWatchWindowObject = new WatchWindowObject(
                    compilerServiceRegistry.RazorCompilerService, compilerServiceRegistry.RazorCompilerService.GetType(), "Razor", true);

                var cssCompilerServiceWatchWindowObject = new WatchWindowObject(
                    compilerServiceRegistry.CssCompilerService, compilerServiceRegistry.CssCompilerService.GetType(), "Css", true);

                var fSharpCompilerServiceWatchWindowObject = new WatchWindowObject(
                    compilerServiceRegistry.FSharpCompilerService, compilerServiceRegistry.FSharpCompilerService.GetType(), "F#", true);

                var javaScriptCompilerServiceWatchWindowObject = new WatchWindowObject(
                    compilerServiceRegistry.JavaScriptCompilerService, compilerServiceRegistry.JavaScriptCompilerService.GetType(), "JavaScript", true);

                var typeScriptCompilerServiceWatchWindowObject = new WatchWindowObject(
                    compilerServiceRegistry.TypeScriptCompilerService, compilerServiceRegistry.TypeScriptCompilerService.GetType(), "TypeScript", true);

                var jsonCompilerServiceWatchWindowObject = new WatchWindowObject(
                    compilerServiceRegistry.JsonCompilerService, compilerServiceRegistry.JsonCompilerService.GetType(), "JSON", true);

                var terminalCompilerServiceWatchWindowObject = new WatchWindowObject(
                    compilerServiceRegistry.TerminalCompilerService, compilerServiceRegistry.TerminalCompilerService.GetType(), "Terminal", true);

                var rootNode = TreeViewAdhoc.ConstructTreeViewAdhoc(
                    new TreeViewReflectionWithView(xmlCompilerServiceWatchWindowObject, true, false, ideComponentRenderers, commonComponentRenderers),
                    new TreeViewReflectionWithView(dotNetSolutionCompilerServiceWatchWindowObject, true, false, ideComponentRenderers, commonComponentRenderers),
                    new TreeViewReflectionWithView(cSharpProjectCompilerServiceWatchWindowObject, true, false, ideComponentRenderers, commonComponentRenderers),
                    new TreeViewReflectionWithView(cSharpCompilerServiceWatchWindowObject, true, false, ideComponentRenderers, commonComponentRenderers),
                    new TreeViewReflectionWithView(razorCompilerServiceWatchWindowObject, true, false, ideComponentRenderers, commonComponentRenderers),
                    new TreeViewReflectionWithView(cssCompilerServiceWatchWindowObject, true, false, ideComponentRenderers, commonComponentRenderers),
                    new TreeViewReflectionWithView(fSharpCompilerServiceWatchWindowObject, true, false, ideComponentRenderers, commonComponentRenderers),
                    new TreeViewReflectionWithView(javaScriptCompilerServiceWatchWindowObject, true, false, ideComponentRenderers, commonComponentRenderers),
                    new TreeViewReflectionWithView(typeScriptCompilerServiceWatchWindowObject, true, false, ideComponentRenderers, commonComponentRenderers),
                    new TreeViewReflectionWithView(jsonCompilerServiceWatchWindowObject, true, false, ideComponentRenderers, commonComponentRenderers),
                    new TreeViewReflectionWithView(terminalCompilerServiceWatchWindowObject, true, false, ideComponentRenderers, commonComponentRenderers));

                await rootNode.LoadChildListAsync();

                if (!treeViewService.TryGetTreeViewContainer(
                        TreeViewCompilerServiceExplorerContentStateKey,
                        out var treeViewState))
                {
                    treeViewService.RegisterTreeViewContainer(new TreeViewContainer(
                        TreeViewCompilerServiceExplorerContentStateKey,
                        rootNode,
                        new TreeViewNoType[] { rootNode }.ToImmutableList()));
                }
                else
                {
                    treeViewService.SetRoot(
                        TreeViewCompilerServiceExplorerContentStateKey,
                        rootNode);

                    treeViewService.SetActiveNode(
                        TreeViewCompilerServiceExplorerContentStateKey,
                        rootNode,
                        true,
                        false);
                }

                dispatcher.Dispatch(new NewAction(inCompilerServiceExplorerState =>
                    new CompilerServiceExplorerState(inCompilerServiceExplorerState.Model)));
            });
    }

    public Task Website_AddExistingProjectToSolution(
        Key<DotNetSolutionModel> dotNetSolutionModelKey,
        string projectTemplateShortName,
        string cSharpProjectName,
        IAbsolutePath cSharpProjectAbsolutePath,
        IEnvironmentProvider environmentProvider,
        IState<DotNetSolutionState> dotNetSolutionStateWrap,
        ILuthetusIdeComponentRenderers ideComponentRenderers,
        ILuthetusCommonComponentRenderers commonComponentRenderers,
        IFileSystemProvider fileSystemProvider,
        ITextEditorService textEditorService,
        IDispatcher dispatcher,
        ITreeViewService treeViewService)
    {
        return _backgroundTaskService.EnqueueAsync(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.GetQueueKey(),
            "Add Existing-Project To Solution",
            async () => await Website_AddExistingProjectToSolutionAsync(
                dotNetSolutionModelKey,
                projectTemplateShortName,
                cSharpProjectName,
                cSharpProjectAbsolutePath,
                environmentProvider,
                dotNetSolutionStateWrap,
                ideComponentRenderers,
                commonComponentRenderers,
                fileSystemProvider,
                textEditorService,
                dispatcher,
                treeViewService));
    }

    private async Task Website_AddExistingProjectToSolutionAsync(
        Key<DotNetSolutionModel> dotNetSolutionModelKey,
        string projectTemplateShortName,
        string cSharpProjectName,
        IAbsolutePath cSharpProjectAbsolutePath,
        IEnvironmentProvider environmentProvider,
        IState<DotNetSolutionState> dotNetSolutionStateWrap,
        ILuthetusIdeComponentRenderers ideComponentRenderers,
        ILuthetusCommonComponentRenderers commonComponentRenderers,
        IFileSystemProvider fileSystemProvider,
        ITextEditorService textEditorService,
        IDispatcher dispatcher,
        ITreeViewService treeViewService)
    {
        var inDotNetSolutionModel = dotNetSolutionStateWrap.Value.DotNetSolutionsList.FirstOrDefault(
            x => x.Key == dotNetSolutionModelKey);

        if (inDotNetSolutionModel is null)
            return;

        var projectTypeGuid = WebsiteProjectTemplateFacts.GetProjectTypeGuid(
            projectTemplateShortName);

        var relativePathFromSlnToProject = PathHelper.GetRelativeFromTwoAbsolutes(
            inDotNetSolutionModel.NamespacePath.AbsolutePath,
            cSharpProjectAbsolutePath,
            environmentProvider);

        var projectIdGuid = Guid.NewGuid();

        var dotNetSolutionModelBuilder = new DotNetSolutionModelBuilder(inDotNetSolutionModel);

        var cSharpProject = new CSharpProject(
            cSharpProjectName,
            projectTypeGuid,
            relativePathFromSlnToProject,
            projectIdGuid,
            // TODO: 'openAssociatedGroupToken' gets set when 'AddDotNetProject(...)' is ran, which is hacky and should be changed. Until then passing in 'null!'
            null!,
            null,
            cSharpProjectAbsolutePath);

        dotNetSolutionModelBuilder.AddDotNetProject(cSharpProject, environmentProvider);

        var outDotNetSolutionModel = dotNetSolutionModelBuilder.Build();

        await fileSystemProvider.File.WriteAllTextAsync(
            outDotNetSolutionModel.NamespacePath.AbsolutePath.Value,
            outDotNetSolutionModel.SolutionFileContents);

        var solutionTextEditorModel = textEditorService.ModelApi.GetOrDefault(
            new ResourceUri(inDotNetSolutionModel.NamespacePath.AbsolutePath.Value));

        if (solutionTextEditorModel is not null)
        {
            textEditorService.PostIndependent(
                nameof(Website_AddExistingProjectToSolutionAsync),
                textEditorService.ModelApi.ReloadFactory(
                    solutionTextEditorModel.ResourceUri,
                    outDotNetSolutionModel.SolutionFileContents,
                    DateTime.UtcNow));
        }

        // TODO: Putting a hack for now to overwrite if somehow model was registered already
        dispatcher.Dispatch(ConstructModelReplacement(
            outDotNetSolutionModel.Key,
            outDotNetSolutionModel));

        await SetDotNetSolutionTreeViewAsync(
            outDotNetSolutionModel.Key,
            dotNetSolutionStateWrap,
            ideComponentRenderers,
            commonComponentRenderers,
            fileSystemProvider,
            environmentProvider,
            treeViewService,
            dispatcher);
    }

    /// <summary>Don't have the implementation <see cref="WithAction"/> as public scope.</summary>
    public interface IWithAction
    {
    }

    /// <summary>Don't have <see cref="WithAction"/> itself as public scope.</summary>
    public record WithAction(Func<DotNetSolutionState, DotNetSolutionState> WithFunc)
        : IWithAction;

    public static IWithAction ConstructModelReplacement(
            Key<DotNetSolutionModel> dotNetSolutionModelKey,
            DotNetSolutionModel outDotNetSolutionModel)
    {
        return new WithAction(dotNetSolutionState =>
        {
            var indexOfSln = dotNetSolutionState.DotNetSolutionsList.FindIndex(
                sln => sln.Key == dotNetSolutionModelKey);

            if (indexOfSln == -1)
                return dotNetSolutionState;

            var outDotNetSolutions = dotNetSolutionState.DotNetSolutionsList.SetItem(
                indexOfSln,
                outDotNetSolutionModel);

            return dotNetSolutionState with
            {
                DotNetSolutionsList = outDotNetSolutions
            };
        });
    }

    public Task SetDotNetSolution(
        IAbsolutePath inSolutionAbsolutePath,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider,
        IDispatcher dispatcher,
        ICompilerServiceRegistry interfaceCompilerServiceRegistry,
        IState<TerminalState> terminalStateWrap,
        IState<DotNetSolutionState> dotNetSolutionStateWrap,
        ILuthetusIdeComponentRenderers ideComponentRenderers,
        ILuthetusCommonComponentRenderers commonComponentRenderers,
        ITreeViewService treeViewService)
    {
        return _backgroundTaskService.EnqueueAsync(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.GetQueueKey(),
            "Set .NET Solution",
            async () => await SetDotNetSolutionAsync(
                inSolutionAbsolutePath,
                fileSystemProvider,
                environmentProvider,
                dispatcher,
                interfaceCompilerServiceRegistry,
                terminalStateWrap,
                dotNetSolutionStateWrap,
                ideComponentRenderers,
                commonComponentRenderers,
                treeViewService));
    }

    private async Task SetDotNetSolutionAsync(
        IAbsolutePath inSolutionAbsolutePath,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider,
        IDispatcher dispatcher,
        ICompilerServiceRegistry interfaceCompilerServiceRegistry,
        IState<TerminalState> terminalStateWrap,
        IState<DotNetSolutionState> dotNetSolutionStateWrap,
        ILuthetusIdeComponentRenderers ideComponentRenderers,
        ILuthetusCommonComponentRenderers commonComponentRenderers,
        ITreeViewService treeViewService)
    {
        var dotNetSolutionAbsolutePathString = inSolutionAbsolutePath.Value;

        var content = await fileSystemProvider.File.ReadAllTextAsync(
            dotNetSolutionAbsolutePathString,
            CancellationToken.None);

        var solutionAbsolutePath = environmentProvider.AbsolutePathFactory(
            dotNetSolutionAbsolutePathString,
            false);

        var solutionNamespacePath = new NamespacePath(
            string.Empty,
            solutionAbsolutePath);

        var lexer = new DotNetSolutionLexer(
            new ResourceUri(solutionAbsolutePath.Value),
            content);

        lexer.Lex();

        var parser = new DotNetSolutionParser(lexer);

        var compilationUnit = parser.Parse();

        foreach (var project in parser.DotNetProjectList)
        {
            var relativePathFromSolutionFileString = project.RelativePathFromSolutionFileString;

            // Debugging Linux-Ubuntu (2024-04-28)
            // -----------------------------------
            // It is believed, that Linux-Ubuntu is not fully working correctly,
            // due to the directory separator character at the os level being '/',
            // meanwhile the .NET solution has as its directory separator character '\'.
            //
            // Will perform a string.Replace("\\", "/") here. And if it solves the issue,
            // then some standard way of doing this needs to be made available in the IEnvironmentProvider.
            //
            // Okay, this single replacement fixes 99% of the solution explorer issue.
            // And I say 99% instead of 100% just because I haven't tested every single part of it yet.
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                relativePathFromSolutionFileString = relativePathFromSolutionFileString.Replace("\\", "/");

            // Solution Folders do not exist on the filesystem. Therefore their absolute path is not guaranteed to be unique
            // One can use the ProjectIdGuid however, when working with a SolutionFolder to make the absolute path unique.
            if (project.DotNetProjectKind == DotNetProjectKind.SolutionFolder)
                relativePathFromSolutionFileString = $"{project.ProjectIdGuid}_{relativePathFromSolutionFileString}";

            var absolutePathString = PathHelper.GetAbsoluteFromAbsoluteAndRelative(
                solutionAbsolutePath,
                relativePathFromSolutionFileString,
                environmentProvider);

            project.AbsolutePath = environmentProvider.AbsolutePathFactory(absolutePathString, false);
        }

        var solutionFolderList = parser.DotNetProjectList
            .Where(x => x.DotNetProjectKind == DotNetProjectKind.SolutionFolder)
            .Select(x => (SolutionFolder)x).ToImmutableArray();

        var dotNetSolutionModel = new DotNetSolutionModel(
            solutionAbsolutePath,
            parser.DotNetSolutionHeader,
            parser.DotNetProjectList.ToImmutableArray(),
            solutionFolderList,
            parser.NestedProjectEntryList.ToImmutableArray(),
            parser.DotNetSolutionGlobal,
            content);

        // TODO: If somehow model was registered already this won't write the state
        dispatcher.Dispatch(new DotNetSolutionState.RegisterAction(dotNetSolutionModel, this));

        dispatcher.Dispatch(new WithAction(
            inDotNetSolutionState => inDotNetSolutionState with
            {
                DotNetSolutionModelKey = dotNetSolutionModel.Key
            }));

        // TODO: Putting a hack for now to overwrite if somehow model was registered already
        dispatcher.Dispatch(ConstructModelReplacement(
            dotNetSolutionModel.Key,
            dotNetSolutionModel));

        var dotNetSolutionCompilerService = interfaceCompilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.DOT_NET_SOLUTION);

        dotNetSolutionCompilerService.ResourceWasModified(
            new ResourceUri(solutionAbsolutePath.Value),
            ImmutableArray<TextEditorTextSpan>.Empty);

        var parentDirectory = solutionAbsolutePath.ParentDirectory;

        if (parentDirectory is not null)
        {
            environmentProvider.DeletionPermittedRegister(new(parentDirectory.Value, true));

            dispatcher.Dispatch(new TextEditorFindAllState.SetStartingDirectoryPathAction(
                parentDirectory.Value));

            dispatcher.Dispatch(new CodeSearchState.WithAction(inState => inState with
            {
                StartingAbsolutePathForSearch = parentDirectory.Value
            }));

            // Set 'generalTerminal' working directory
            {
                var generalTerminal = terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_TERMINAL_KEY];

                var changeDirectoryCommand = new TerminalCommand(
                    Key<TerminalCommand>.NewKey(),
                    new FormattedCommand("cd", new string[] { }),
                    parentDirectory.Value,
                    CancellationToken.None);

                await generalTerminal.EnqueueCommandAsync(changeDirectoryCommand);
            }

            // Set 'executionTerminal' working directory
            {
                var executionTerminal = terminalStateWrap.Value.TerminalMap[TerminalFacts.EXECUTION_TERMINAL_KEY];

                var changeDirectoryCommand = new TerminalCommand(
                    Key<TerminalCommand>.NewKey(),
                    new FormattedCommand("cd", new string[] { }),
                    parentDirectory.Value,
                    CancellationToken.None);

                await executionTerminal.EnqueueCommandAsync(changeDirectoryCommand);
            }
        }

        await SetDotNetSolutionTreeViewAsync(
            dotNetSolutionModel.Key,
            dotNetSolutionStateWrap,
            ideComponentRenderers,
            commonComponentRenderers,
            fileSystemProvider,
            environmentProvider,
            treeViewService,
            dispatcher);
    }

    public Task SetDotNetSolutionTreeView(
        Key<DotNetSolutionModel> dotNetSolutionModelKey,
        IState<DotNetSolutionState> dotNetSolutionStateWrap,
        ILuthetusIdeComponentRenderers ideComponentRenderers,
        ILuthetusCommonComponentRenderers commonComponentRenderers,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider,
        ITreeViewService treeViewService,
        IDispatcher dispatcher)
    {
        return _backgroundTaskService.EnqueueAsync(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.GetQueueKey(),
            "Set .NET Solution TreeView",
            async () => await SetDotNetSolutionTreeViewAsync(
                dotNetSolutionModelKey,
                dotNetSolutionStateWrap,
                ideComponentRenderers,
                commonComponentRenderers,
                fileSystemProvider,
                environmentProvider,
                treeViewService,
                dispatcher));
    }

    private async Task SetDotNetSolutionTreeViewAsync(
        Key<DotNetSolutionModel> dotNetSolutionModelKey,
        IState<DotNetSolutionState> dotNetSolutionStateWrap,
        ILuthetusIdeComponentRenderers ideComponentRenderers,
        ILuthetusCommonComponentRenderers commonComponentRenderers,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider,
        ITreeViewService treeViewService,
        IDispatcher dispatcher)
    {
        var dotNetSolutionState = dotNetSolutionStateWrap.Value;

        var dotNetSolutionModel = dotNetSolutionState.DotNetSolutionsList.FirstOrDefault(
            x => x.Key == dotNetSolutionModelKey);

        if (dotNetSolutionModel is null)
            return;

        var rootNode = new TreeViewSolution(
            dotNetSolutionModel,
            ideComponentRenderers,
            commonComponentRenderers,
            fileSystemProvider,
            environmentProvider,
            true,
            true);

        await rootNode.LoadChildListAsync();

        if (!treeViewService.TryGetTreeViewContainer(DotNetSolutionState.TreeViewSolutionExplorerStateKey, out _))
        {
            treeViewService.RegisterTreeViewContainer(new TreeViewContainer(
                DotNetSolutionState.TreeViewSolutionExplorerStateKey,
                rootNode,
                new TreeViewNoType[] { rootNode }.ToImmutableList()));
        }
        else
        {
            treeViewService.SetRoot(DotNetSolutionState.TreeViewSolutionExplorerStateKey, rootNode);

            treeViewService.SetActiveNode(
                DotNetSolutionState.TreeViewSolutionExplorerStateKey,
                rootNode,
                true,
                false);
        }

        if (dotNetSolutionModel is null)
            return;

        dispatcher.Dispatch(ConstructModelReplacement(
            dotNetSolutionModel.Key,
            dotNetSolutionModel));
    }
}
