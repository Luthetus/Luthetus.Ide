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

public class LuthetusIdeBackgroundTaskApi
{
    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly IStorageService _storageService;
    private readonly IState<CompilerServiceExplorerState> _compilerServiceExplorerStateWrap;
    private readonly CompilerServiceRegistry _compilerServiceRegistry;
    private readonly ILuthetusIdeComponentRenderers _ideComponentRenderers;
    private readonly ILuthetusCommonComponentRenderers _commonComponentRenderers;
    private readonly ITreeViewService _treeViewService;
    private readonly IDispatcher _dispatcher;
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly IState<DotNetSolutionState> _dotNetSolutionStateWrap;
    private readonly IFileSystemProvider _fileSystemProvider;
    private readonly ITextEditorService _textEditorService;
    private readonly ICompilerServiceRegistry _interfaceCompilerServiceRegistry;
    private readonly IState<TerminalState> _terminalStateWrap;

    public LuthetusIdeBackgroundTaskApi(
        IBackgroundTaskService backgroundTaskService,
        IStorageService storageService,
        IState<CompilerServiceExplorerState> compilerServiceExplorerStateWrap,
        CompilerServiceRegistry compilerServiceRegistry,
        ILuthetusIdeComponentRenderers ideComponentRenderers,
        ILuthetusCommonComponentRenderers commonComponentRenderers,
        ITreeViewService treeViewService,
        IDispatcher dispatcher,
        IEnvironmentProvider environmentProvider,
        IState<DotNetSolutionState> dotNetSolutionStateWrap,
        IFileSystemProvider fileSystemProvider,
        ITextEditorService textEditorService,
        ICompilerServiceRegistry interfaceCompilerServiceRegistry,
        IState<TerminalState> terminalStateWrap)
    {
        _backgroundTaskService = backgroundTaskService;
        _storageService = storageService;
        _compilerServiceExplorerStateWrap = compilerServiceExplorerStateWrap;
        _compilerServiceRegistry = compilerServiceRegistry;
        _ideComponentRenderers = ideComponentRenderers;
        _commonComponentRenderers = commonComponentRenderers;
        _treeViewService = treeViewService;
        _dispatcher = dispatcher;
        _environmentProvider = environmentProvider;
        _dotNetSolutionStateWrap = dotNetSolutionStateWrap;
        _fileSystemProvider = fileSystemProvider;
        _textEditorService = textEditorService;
        _interfaceCompilerServiceRegistry = interfaceCompilerServiceRegistry;
        _terminalStateWrap = terminalStateWrap;
    }

    public Task SetCompilerServiceExplorerTreeView()
    {
        return _backgroundTaskService.EnqueueAsync(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.GetQueueKey(),
            "Set CompilerServiceExplorer TreeView",
            async () =>
            {
                var compilerServiceExplorerState = _compilerServiceExplorerStateWrap.Value;

                var xmlCompilerServiceWatchWindowObject = new WatchWindowObject(
                    _compilerServiceRegistry.XmlCompilerService, _compilerServiceRegistry.XmlCompilerService.GetType(), "XML", true);

                var dotNetSolutionCompilerServiceWatchWindowObject = new WatchWindowObject(
                    _compilerServiceRegistry.DotNetSolutionCompilerService, _compilerServiceRegistry.DotNetSolutionCompilerService.GetType(), ".NET Solution", true);

                var cSharpProjectCompilerServiceWatchWindowObject = new WatchWindowObject(
                    _compilerServiceRegistry.CSharpProjectCompilerService, _compilerServiceRegistry.CSharpProjectCompilerService.GetType(), "C# Project", true);

                var cSharpCompilerServiceWatchWindowObject = new WatchWindowObject(
                    _compilerServiceRegistry.CSharpCompilerService, _compilerServiceRegistry.CSharpCompilerService.GetType(), "C#", true);

                var razorCompilerServiceWatchWindowObject = new WatchWindowObject(
                    _compilerServiceRegistry.RazorCompilerService, _compilerServiceRegistry.RazorCompilerService.GetType(), "Razor", true);

                var cssCompilerServiceWatchWindowObject = new WatchWindowObject(
                    _compilerServiceRegistry.CssCompilerService, _compilerServiceRegistry.CssCompilerService.GetType(), "Css", true);

                var fSharpCompilerServiceWatchWindowObject = new WatchWindowObject(
                    _compilerServiceRegistry.FSharpCompilerService, _compilerServiceRegistry.FSharpCompilerService.GetType(), "F#", true);

                var javaScriptCompilerServiceWatchWindowObject = new WatchWindowObject(
                    _compilerServiceRegistry.JavaScriptCompilerService, _compilerServiceRegistry.JavaScriptCompilerService.GetType(), "JavaScript", true);

                var typeScriptCompilerServiceWatchWindowObject = new WatchWindowObject(
                    _compilerServiceRegistry.TypeScriptCompilerService, _compilerServiceRegistry.TypeScriptCompilerService.GetType(), "TypeScript", true);

                var jsonCompilerServiceWatchWindowObject = new WatchWindowObject(
                    _compilerServiceRegistry.JsonCompilerService, _compilerServiceRegistry.JsonCompilerService.GetType(), "JSON", true);

                var terminalCompilerServiceWatchWindowObject = new WatchWindowObject(
                    _compilerServiceRegistry.TerminalCompilerService, _compilerServiceRegistry.TerminalCompilerService.GetType(), "Terminal", true);

                var rootNode = TreeViewAdhoc.ConstructTreeViewAdhoc(
                    new TreeViewReflectionWithView(xmlCompilerServiceWatchWindowObject, true, false, _ideComponentRenderers, _commonComponentRenderers),
                    new TreeViewReflectionWithView(dotNetSolutionCompilerServiceWatchWindowObject, true, false, _ideComponentRenderers, _commonComponentRenderers),
                    new TreeViewReflectionWithView(cSharpProjectCompilerServiceWatchWindowObject, true, false, _ideComponentRenderers, _commonComponentRenderers),
                    new TreeViewReflectionWithView(cSharpCompilerServiceWatchWindowObject, true, false, _ideComponentRenderers, _commonComponentRenderers),
                    new TreeViewReflectionWithView(razorCompilerServiceWatchWindowObject, true, false, _ideComponentRenderers, _commonComponentRenderers),
                    new TreeViewReflectionWithView(cssCompilerServiceWatchWindowObject, true, false, _ideComponentRenderers, _commonComponentRenderers),
                    new TreeViewReflectionWithView(fSharpCompilerServiceWatchWindowObject, true, false, _ideComponentRenderers, _commonComponentRenderers),
                    new TreeViewReflectionWithView(javaScriptCompilerServiceWatchWindowObject, true, false, _ideComponentRenderers, _commonComponentRenderers),
                    new TreeViewReflectionWithView(typeScriptCompilerServiceWatchWindowObject, true, false, _ideComponentRenderers, _commonComponentRenderers),
                    new TreeViewReflectionWithView(jsonCompilerServiceWatchWindowObject, true, false, _ideComponentRenderers, _commonComponentRenderers),
                    new TreeViewReflectionWithView(terminalCompilerServiceWatchWindowObject, true, false, _ideComponentRenderers, _commonComponentRenderers));

                await rootNode.LoadChildListAsync();

                if (!_treeViewService.TryGetTreeViewContainer(
                        TreeViewCompilerServiceExplorerContentStateKey,
                        out var treeViewState))
                {
                    _treeViewService.RegisterTreeViewContainer(new TreeViewContainer(
                        TreeViewCompilerServiceExplorerContentStateKey,
                        rootNode,
                        new TreeViewNoType[] { rootNode }.ToImmutableList()));
                }
                else
                {
                    _treeViewService.SetRoot(
                        TreeViewCompilerServiceExplorerContentStateKey,
                        rootNode);

                    _treeViewService.SetActiveNode(
                        TreeViewCompilerServiceExplorerContentStateKey,
                        rootNode,
                        true,
                        false);
                }

                _dispatcher.Dispatch(new NewAction(inCompilerServiceExplorerState =>
                    new CompilerServiceExplorerState(inCompilerServiceExplorerState.Model)));
            });
    }

    public Task Website_AddExistingProjectToSolution(
        Key<DotNetSolutionModel> dotNetSolutionModelKey,
        string projectTemplateShortName,
        string cSharpProjectName,
        IAbsolutePath cSharpProjectAbsolutePath)
    {
        return _backgroundTaskService.EnqueueAsync(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.GetQueueKey(),
            "Add Existing-Project To Solution",
            async () => await Website_AddExistingProjectToSolutionAsync(
                dotNetSolutionModelKey,
                projectTemplateShortName,
                cSharpProjectName,
                cSharpProjectAbsolutePath));
    }

    private async Task Website_AddExistingProjectToSolutionAsync(
        Key<DotNetSolutionModel> dotNetSolutionModelKey,
        string projectTemplateShortName,
        string cSharpProjectName,
        IAbsolutePath cSharpProjectAbsolutePath)
    {
        var inDotNetSolutionModel = _dotNetSolutionStateWrap.Value.DotNetSolutionsList.FirstOrDefault(
            x => x.Key == dotNetSolutionModelKey);

        if (inDotNetSolutionModel is null)
            return;

        var projectTypeGuid = WebsiteProjectTemplateFacts.GetProjectTypeGuid(
            projectTemplateShortName);

        var relativePathFromSlnToProject = PathHelper.GetRelativeFromTwoAbsolutes(
            inDotNetSolutionModel.NamespacePath.AbsolutePath,
            cSharpProjectAbsolutePath,
            _environmentProvider);

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

        dotNetSolutionModelBuilder.AddDotNetProject(cSharpProject, _environmentProvider);

        var outDotNetSolutionModel = dotNetSolutionModelBuilder.Build();

        await _fileSystemProvider.File.WriteAllTextAsync(
            outDotNetSolutionModel.NamespacePath.AbsolutePath.Value,
            outDotNetSolutionModel.SolutionFileContents);

        var solutionTextEditorModel = _textEditorService.ModelApi.GetOrDefault(
            new ResourceUri(inDotNetSolutionModel.NamespacePath.AbsolutePath.Value));

        if (solutionTextEditorModel is not null)
        {
            _textEditorService.PostIndependent(
                nameof(Website_AddExistingProjectToSolutionAsync),
                _textEditorService.ModelApi.ReloadFactory(
                    solutionTextEditorModel.ResourceUri,
                    outDotNetSolutionModel.SolutionFileContents,
                    DateTime.UtcNow));
        }

        // TODO: Putting a hack for now to overwrite if somehow model was registered already
        _dispatcher.Dispatch(ConstructModelReplacement(
            outDotNetSolutionModel.Key,
            outDotNetSolutionModel));

        await SetDotNetSolutionTreeViewAsync(outDotNetSolutionModel.Key);
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

    public Task SetDotNetSolution(IAbsolutePath inSolutionAbsolutePath)
    {
        return _backgroundTaskService.EnqueueAsync(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.GetQueueKey(),
            "Set .NET Solution",
            async () => await SetDotNetSolutionAsync(inSolutionAbsolutePath));
    }

    private async Task SetDotNetSolutionAsync(IAbsolutePath inSolutionAbsolutePath)
    {
        var dotNetSolutionAbsolutePathString = inSolutionAbsolutePath.Value;

        var content = await _fileSystemProvider.File.ReadAllTextAsync(
            dotNetSolutionAbsolutePathString,
            CancellationToken.None);

        var solutionAbsolutePath = _environmentProvider.AbsolutePathFactory(
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
                _environmentProvider);

            project.AbsolutePath = _environmentProvider.AbsolutePathFactory(absolutePathString, false);
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
        _dispatcher.Dispatch(new DotNetSolutionState.RegisterAction(dotNetSolutionModel, this));

        _dispatcher.Dispatch(new WithAction(
            inDotNetSolutionState => inDotNetSolutionState with
            {
                DotNetSolutionModelKey = dotNetSolutionModel.Key
            }));

        // TODO: Putting a hack for now to overwrite if somehow model was registered already
        _dispatcher.Dispatch(ConstructModelReplacement(
            dotNetSolutionModel.Key,
            dotNetSolutionModel));

        var dotNetSolutionCompilerService = _interfaceCompilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.DOT_NET_SOLUTION);

        dotNetSolutionCompilerService.ResourceWasModified(
            new ResourceUri(solutionAbsolutePath.Value),
            ImmutableArray<TextEditorTextSpan>.Empty);

        var parentDirectory = solutionAbsolutePath.ParentDirectory;

        if (parentDirectory is not null)
        {
            _environmentProvider.DeletionPermittedRegister(new(parentDirectory.Value, true));

            _dispatcher.Dispatch(new TextEditorFindAllState.SetStartingDirectoryPathAction(
                parentDirectory.Value));

            _dispatcher.Dispatch(new CodeSearchState.WithAction(inState => inState with
            {
                StartingAbsolutePathForSearch = parentDirectory.Value
            }));

            // Set 'generalTerminal' working directory
            {
                var generalTerminal = _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_TERMINAL_KEY];

                var changeDirectoryCommand = new TerminalCommand(
                    Key<TerminalCommand>.NewKey(),
                    new FormattedCommand("cd", new string[] { }),
                    parentDirectory.Value,
                    CancellationToken.None);

                await generalTerminal.EnqueueCommandAsync(changeDirectoryCommand);
            }

            // Set 'executionTerminal' working directory
            {
                var executionTerminal = _terminalStateWrap.Value.TerminalMap[TerminalFacts.EXECUTION_TERMINAL_KEY];

                var changeDirectoryCommand = new TerminalCommand(
                    Key<TerminalCommand>.NewKey(),
                    new FormattedCommand("cd", new string[] { }),
                    parentDirectory.Value,
                    CancellationToken.None);

                await executionTerminal.EnqueueCommandAsync(changeDirectoryCommand);
            }
        }

        await SetDotNetSolutionTreeViewAsync(dotNetSolutionModel.Key);
    }

    public Task SetDotNetSolutionTreeView(Key<DotNetSolutionModel> dotNetSolutionModelKey)
    {
        return _backgroundTaskService.EnqueueAsync(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.GetQueueKey(),
            "Set .NET Solution TreeView",
            async () => await SetDotNetSolutionTreeViewAsync(dotNetSolutionModelKey));
    }

    private async Task SetDotNetSolutionTreeViewAsync(Key<DotNetSolutionModel> dotNetSolutionModelKey)
    {
        var dotNetSolutionState = _dotNetSolutionStateWrap.Value;

        var dotNetSolutionModel = dotNetSolutionState.DotNetSolutionsList.FirstOrDefault(
            x => x.Key == dotNetSolutionModelKey);

        if (dotNetSolutionModel is null)
            return;

        var rootNode = new TreeViewSolution(
            dotNetSolutionModel,
            _ideComponentRenderers,
            _commonComponentRenderers,
            _fileSystemProvider,
            _environmentProvider,
            true,
            true);

        await rootNode.LoadChildListAsync();

        if (!_treeViewService.TryGetTreeViewContainer(DotNetSolutionState.TreeViewSolutionExplorerStateKey, out _))
        {
            _treeViewService.RegisterTreeViewContainer(new TreeViewContainer(
                DotNetSolutionState.TreeViewSolutionExplorerStateKey,
                rootNode,
                new TreeViewNoType[] { rootNode }.ToImmutableList()));
        }
        else
        {
            _treeViewService.SetRoot(DotNetSolutionState.TreeViewSolutionExplorerStateKey, rootNode);

            _treeViewService.SetActiveNode(
                DotNetSolutionState.TreeViewSolutionExplorerStateKey,
                rootNode,
                true,
                false);
        }

        if (dotNetSolutionModel is null)
            return;

        _dispatcher.Dispatch(ConstructModelReplacement(
            dotNetSolutionModel.Key,
            dotNetSolutionModel));
    }
}
