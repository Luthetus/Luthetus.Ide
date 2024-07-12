using System.Collections.Immutable;
using System.Runtime.InteropServices;
using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Namespaces.Models;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.FindAlls.States;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.Models.Project;
using Luthetus.CompilerServices.Lang.DotNetSolution.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.SyntaxActors;
using Luthetus.Ide.RazorLib.CodeSearches.States;
using Luthetus.Ide.RazorLib.CommandLines.Models;
using Luthetus.Ide.RazorLib.CompilerServices.Models;
using Luthetus.Ide.RazorLib.CompilerServices.States;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.DotNetSolutions.States;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Ide.RazorLib.Websites.ProjectTemplates.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Ide.RazorLib.DotNetSolutions.Models;

public class DotNetSolutionIdeApi
{
    private readonly IdeBackgroundTaskApi _ideBackgroundTaskApi;
    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly IStorageService _storageService;
    private readonly IState<CompilerServiceExplorerState> _compilerServiceExplorerStateWrap;
    private readonly CompilerServiceRegistry _compilerServiceRegistry;
    private readonly IIdeComponentRenderers _ideComponentRenderers;
    private readonly ICommonComponentRenderers _commonComponentRenderers;
    private readonly ITreeViewService _treeViewService;
    private readonly IDispatcher _dispatcher;
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly IState<DotNetSolutionState> _dotNetSolutionStateWrap;
    private readonly IFileSystemProvider _fileSystemProvider;
    private readonly ITextEditorService _textEditorService;
    private readonly ICompilerServiceRegistry _interfaceCompilerServiceRegistry;
    private readonly IState<TerminalState> _terminalStateWrap;
    private readonly IServiceProvider _serviceProvider;

    public DotNetSolutionIdeApi(
        IdeBackgroundTaskApi ideBackgroundTaskApi,
        IBackgroundTaskService backgroundTaskService,
        IStorageService storageService,
        IState<CompilerServiceExplorerState> compilerServiceExplorerStateWrap,
        CompilerServiceRegistry compilerServiceRegistry,
        IIdeComponentRenderers ideComponentRenderers,
        ICommonComponentRenderers commonComponentRenderers,
        ITreeViewService treeViewService,
        IDispatcher dispatcher,
        IEnvironmentProvider environmentProvider,
        IState<DotNetSolutionState> dotNetSolutionStateWrap,
        IFileSystemProvider fileSystemProvider,
        ITextEditorService textEditorService,
        ICompilerServiceRegistry interfaceCompilerServiceRegistry,
        IState<TerminalState> terminalStateWrap,
		IServiceProvider serviceProvider)
    {
        _ideBackgroundTaskApi = ideBackgroundTaskApi;
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
		_serviceProvider = serviceProvider;
    }

    public void Website_AddExistingProjectToSolution(
        Key<DotNetSolutionModel> dotNetSolutionModelKey,
        string projectTemplateShortName,
        string cSharpProjectName,
        IAbsolutePath cSharpProjectAbsolutePath)
    {
        _backgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            ContinuousBackgroundTaskWorker.GetQueueKey(),
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
                outDotNetSolutionModel.SolutionFileContents)
            .ConfigureAwait(false);

        var solutionTextEditorModel = _textEditorService.ModelApi.GetOrDefault(
            new ResourceUri(inDotNetSolutionModel.NamespacePath.AbsolutePath.Value));

        if (solutionTextEditorModel is not null)
        {
            _textEditorService.PostSimpleBatch(
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

        await SetDotNetSolutionTreeViewAsync(outDotNetSolutionModel.Key).ConfigureAwait(false);
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

    public void SetDotNetSolution(IAbsolutePath inSolutionAbsolutePath)
    {
        _backgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            ContinuousBackgroundTaskWorker.GetQueueKey(),
            "Set .NET Solution",
            async () => await SetDotNetSolutionAsync(inSolutionAbsolutePath).ConfigureAwait(false));
    }

    private async Task SetDotNetSolutionAsync(IAbsolutePath inSolutionAbsolutePath)
    {
        var dotNetSolutionAbsolutePathString = inSolutionAbsolutePath.Value;

        var content = await _fileSystemProvider.File.ReadAllTextAsync(
                dotNetSolutionAbsolutePathString,
                CancellationToken.None)
            .ConfigureAwait(false);

        var solutionAbsolutePath = _environmentProvider.AbsolutePathFactory(
            dotNetSolutionAbsolutePathString,
            false);

        var solutionNamespacePath = new NamespacePath(
            string.Empty,
            solutionAbsolutePath);

        var resourceUri = new ResourceUri(solutionAbsolutePath.Value);

        if (_textEditorService.ModelApi.GetOrDefault(resourceUri) is null)
        {
            _textEditorService.ModelApi.RegisterTemplated(
                ExtensionNoPeriodFacts.DOT_NET_SOLUTION,
                resourceUri,
                DateTime.UtcNow,
                content);

            _compilerServiceRegistry.DotNetSolutionCompilerService.RegisterResource(resourceUri);
        }

        var lexer = new DotNetSolutionLexer(
            resourceUri,
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

                generalTerminal.EnqueueCommand(changeDirectoryCommand);
            }

            // Set 'executionTerminal' working directory
            {
                var executionTerminal = _terminalStateWrap.Value.TerminalMap[TerminalFacts.EXECUTION_TERMINAL_KEY];

                var changeDirectoryCommand = new TerminalCommand(
                    Key<TerminalCommand>.NewKey(),
                    new FormattedCommand("cd", new string[] { }),
                    parentDirectory.Value,
                    CancellationToken.None);

                executionTerminal.EnqueueCommand(changeDirectoryCommand);
            }
        }

		await ParseSolutionAsync(dotNetSolutionModel.Key).ConfigureAwait(false);

        await SetDotNetSolutionTreeViewAsync(dotNetSolutionModel.Key).ConfigureAwait(false);
    }

	private async Task ParseSolutionAsync(Key<DotNetSolutionModel> dotNetSolutionModelKey)
	{
		var dotNetSolutionState = _dotNetSolutionStateWrap.Value;

        var dotNetSolutionModel = dotNetSolutionState.DotNetSolutionsList.FirstOrDefault(
            x => x.Key == dotNetSolutionModelKey);

        if (dotNetSolutionModel is null)
            return;

		var progressBarModel = new ProgressBarModel(0, "parsing...");

		NotificationHelper.DispatchProgress(
	        $"Parse: {dotNetSolutionModel.AbsolutePath.NameWithExtension}",
	        progressBarModel,
	        _commonComponentRenderers,
	        _dispatcher,
	        TimeSpan.FromMilliseconds(-1));

		_ = Task.Run(async () =>
		{
			try
			{
				if (_textEditorService.TextEditorConfig.RegisterModelFunc is null)
		            return;
		
				progressBarModel.SetProgress(0.05, "Discovering projects...");
				foreach (var project in dotNetSolutionModel.DotNetProjectList)
				{
					var resourceUri = new ResourceUri(project.AbsolutePath.Value);
		
					if (!(await _fileSystemProvider.File.ExistsAsync(resourceUri.Value)))
						continue; // TODO: This can still cause a race condition exception if the file is removed before the next line runs.

			        await _textEditorService.TextEditorConfig.RegisterModelFunc.Invoke(new RegisterModelArgs(
			                resourceUri,
			                _serviceProvider))
			            .ConfigureAwait(false);
				}

				var previousStageProgress = 0.05;
				var dotNetProjectListLength = dotNetSolutionModel.DotNetProjectList.Length;
				var projectsParsedCount = 0;
				foreach (var project in dotNetSolutionModel.DotNetProjectList)
				{
					// foreach project in solution
					// 	foreach C# file in project
					// 		EnqueueBackgroundTask(async () =>
					// 		{
					// 			ParseCSharpFile();
					// 			UpdateProgressBar();
					// 		});
					//
					// Treat every project as an equal weighting with relation to remaining percent to complete
					// on the progress bar.
					//
					// If the project were to be parsed, how much would it move the percent progress completed by?
					//
					// Then, in order to see progress while each C# file in the project gets parsed,
					// multiply the percent progress this project can provide by the proportion
					// of the project's C# files which have been parsed.
					var maximumProgressAvailableToProject = (1 - previousStageProgress) * ((double)1.0 / (double)dotNetProjectListLength);
					var currentProgress = Math.Min(1.0, previousStageProgress + (maximumProgressAvailableToProject * projectsParsedCount));

					progressBarModel.SetProgress(
						currentProgress,
						$"{projectsParsedCount + 1}/{dotNetProjectListLength}: {project.AbsolutePath.NameWithExtension}");

					await DiscoverClassesInProject(project, progressBarModel, currentProgress, maximumProgressAvailableToProject);
					projectsParsedCount++;
				}
	
				progressBarModel.SetProgress(1, $"Finished parsing: {dotNetSolutionModel.AbsolutePath.NameWithExtension}", string.Empty);
			}
			catch (Exception e)
			{
				var currentProgress = progressBarModel.GetProgress();
				progressBarModel.SetProgress(currentProgress, e.ToString());
			}
			finally
			{
				progressBarModel.Dispose();
			}
		});
	}

	private async Task DiscoverClassesInProject(
		IDotNetProject dotNetProject,
		ProgressBarModel progressBarModel,
		double currentProgress,
		double maximumProgressAvailableToProject)
	{
		if (!(await _fileSystemProvider.File.ExistsAsync(dotNetProject.AbsolutePath.Value)))
			return; // TODO: This can still cause a race condition exception if the file is removed before the next line runs.

		var parentDirectory = dotNetProject.AbsolutePath.ParentDirectory;
		if (parentDirectory is null)
	        return;

		var startingAbsolutePathForSearch = parentDirectory.Value;
		var discoveredFileList = new List<string>();

		progressBarModel.SetProgress(null, null, "discovering files");
		await DiscoverFilesRecursively(startingAbsolutePathForSearch, discoveredFileList, true).ConfigureAwait(false);

		await ParseClassesInProject(
			dotNetProject,
			progressBarModel,
			currentProgress,
			maximumProgressAvailableToProject,
			discoveredFileList);

        async Task DiscoverFilesRecursively(string directoryPathParent, List<string> discoveredFileList, bool isFirstInvocation)
        {
            var directoryPathChildList = await _fileSystemProvider.Directory.GetDirectoriesAsync(
                    directoryPathParent,
                    CancellationToken.None)
                .ConfigureAwait(false);

            var filePathChildList = await _fileSystemProvider.Directory.GetFilesAsync(
                    directoryPathParent,
                    CancellationToken.None)
                .ConfigureAwait(false);

            foreach (var filePathChild in filePathChildList)
            {
                if (filePathChild.EndsWith(".cs"))
                    discoveredFileList.Add(filePathChild);
            }

			//var progressMessage = progressBarModel.Message ?? string.Empty;

            foreach (var directoryPathChild in directoryPathChildList)
            {
                if (directoryPathChild.Contains(".vs") || directoryPathChild.Contains(".git") || directoryPathChild.Contains("bin") || directoryPathChild.Contains("obj"))
                    continue;

				//if (isFirstInvocation)
				//{
				//	var currentProgress = progressBarModel.GetProgress();
				//	progressBarModel.SetProgress(currentProgress, $"{directoryPathChild} " + progressMessage);
				//}

                await DiscoverFilesRecursively(directoryPathChild, discoveredFileList, isFirstInvocation: false).ConfigureAwait(false);
            }
        }
	}

	private async Task ParseClassesInProject(
		IDotNetProject dotNetProject,
		ProgressBarModel progressBarModel,
		double currentProgress,
		double maximumProgressAvailableToProject,
		List<string> discoveredFileList)
	{
		// TODO: Do not increment until enqueued task is finished.
		var fileParsedCount = 0;
		foreach (var file in discoveredFileList)
		{
			var fileAbsolutePath = _environmentProvider.AbsolutePathFactory(file, false);

			var progress = currentProgress + (maximumProgressAvailableToProject * ((double)fileParsedCount / (double)discoveredFileList.Count));

			progressBarModel.SetProgress(
				progress,
				null,
				$"{fileParsedCount + 1}/{discoveredFileList.Count}: {fileAbsolutePath.NameWithExtension}");

			var resourceUri = new ResourceUri(file);

			await _textEditorService.TextEditorConfig.RegisterModelFunc.Invoke(new RegisterModelArgs(
	                resourceUri,
	                _serviceProvider))
	            .ConfigureAwait(false);

			var model = _textEditorService.ModelApi.GetOrDefault(resourceUri);

			if (model is null)
			{
				Console.WriteLine($"Model with {nameof(resourceUri)}: '{resourceUri}' was null");
				continue;
			}

			var consecutiveMissCounter = 0;

			while (true)
			{
				var compilerService = model.CompilerService;
				if (compilerService is null)
					break;

				if (compilerService.GetCompilerServiceResourceFor(resourceUri) is not null)
				{
					fileParsedCount++;
					consecutiveMissCounter = 0;
					break;
				}
				else
				{
					consecutiveMissCounter++;
					if (consecutiveMissCounter > 50)
						Console.WriteLine($"CompilerService did not contain {nameof(resourceUri)}: '{resourceUri}'");
					await Task.Delay(20 * (consecutiveMissCounter));
				}
			}
		}
	}

    public void SetDotNetSolutionTreeView(Key<DotNetSolutionModel> dotNetSolutionModelKey)
    {
        _backgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            ContinuousBackgroundTaskWorker.GetQueueKey(),
            "Set .NET Solution TreeView",
            async () => await SetDotNetSolutionTreeViewAsync(dotNetSolutionModelKey).ConfigureAwait(false));
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

        await rootNode.LoadChildListAsync().ConfigureAwait(false);

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
