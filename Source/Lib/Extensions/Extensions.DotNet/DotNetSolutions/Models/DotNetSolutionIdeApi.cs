using System.Collections.Immutable;
using System.Runtime.InteropServices;
using Fluxor;
using CliWrap.EventStream;
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
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.CompilerServices.DotNetSolution.Models.Project;
using Luthetus.CompilerServices.DotNetSolution.Models;
using Luthetus.CompilerServices.DotNetSolution.SyntaxActors;
using Luthetus.CompilerServices.DotNetSolution.CompilerServiceCase;
using Luthetus.Ide.RazorLib.CodeSearches.States;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.StartupControls.Models;
using Luthetus.Ide.RazorLib.StartupControls.States;
using Luthetus.Ide.RazorLib.AppDatas.Models;
using Luthetus.Extensions.DotNet.DotNetSolutions.States;
using Luthetus.Extensions.DotNet.CompilerServices.States;
using Luthetus.Extensions.DotNet.Websites.ProjectTemplates.Models;
using Luthetus.Extensions.DotNet.ComponentRenderers.Models;
using Luthetus.Extensions.DotNet.CommandLines.Models;
using Luthetus.Extensions.DotNet.AppDatas.Models;

namespace Luthetus.Extensions.DotNet.DotNetSolutions.Models;

public class DotNetSolutionIdeApi
{
	private readonly IdeBackgroundTaskApi _ideBackgroundTaskApi;
	private readonly IBackgroundTaskService _backgroundTaskService;
	private readonly IStorageService _storageService;
	private readonly IAppDataService _appDataService;
	private readonly IState<CompilerServiceExplorerState> _compilerServiceExplorerStateWrap;
	private readonly IDotNetComponentRenderers _dotNetComponentRenderers;
	private readonly IIdeComponentRenderers _ideComponentRenderers;
	private readonly ICommonComponentRenderers _commonComponentRenderers;
	private readonly ITreeViewService _treeViewService;
	private readonly IDispatcher _dispatcher;
	private readonly IEnvironmentProvider _environmentProvider;
	private readonly IState<DotNetSolutionState> _dotNetSolutionStateWrap;
	private readonly IFileSystemProvider _fileSystemProvider;
	private readonly ITextEditorService _textEditorService;
	private readonly ICompilerServiceRegistry _compilerServiceRegistry;
	private readonly IState<TerminalState> _terminalStateWrap;
	private readonly DotNetCliOutputParser _dotNetCliOutputParser;
	private readonly IServiceProvider _serviceProvider;
	
	private readonly Key<TerminalCommandRequest> _newDotNetSolutionTerminalCommandRequestKey = Key<TerminalCommandRequest>.NewKey();
    private readonly CancellationTokenSource _newDotNetSolutionCancellationTokenSource = new();

	public DotNetSolutionIdeApi(
		IdeBackgroundTaskApi ideBackgroundTaskApi,
		IBackgroundTaskService backgroundTaskService,
		IStorageService storageService,
		IAppDataService appDataService,
		IState<CompilerServiceExplorerState> compilerServiceExplorerStateWrap,
        IDotNetComponentRenderers dotNetComponentRenderers,
        IIdeComponentRenderers ideComponentRenderers,
		ICommonComponentRenderers commonComponentRenderers,
		ITreeViewService treeViewService,
		IDispatcher dispatcher,
		IEnvironmentProvider environmentProvider,
		IState<DotNetSolutionState> dotNetSolutionStateWrap,
		IFileSystemProvider fileSystemProvider,
		ITextEditorService textEditorService,
		ICompilerServiceRegistry compilerServiceRegistry,
		IState<TerminalState> terminalStateWrap,
		DotNetCliOutputParser dotNetCliOutputParser,
		IServiceProvider serviceProvider)
	{
		_ideBackgroundTaskApi = ideBackgroundTaskApi;
		_backgroundTaskService = backgroundTaskService;
		_storageService = storageService;
		_appDataService = appDataService;
		_compilerServiceExplorerStateWrap = compilerServiceExplorerStateWrap;
		_compilerServiceRegistry = compilerServiceRegistry;
        _dotNetComponentRenderers = dotNetComponentRenderers;
        _ideComponentRenderers = ideComponentRenderers;
		_commonComponentRenderers = commonComponentRenderers;
		_treeViewService = treeViewService;
		_dispatcher = dispatcher;
		_environmentProvider = environmentProvider;
		_dotNetSolutionStateWrap = dotNetSolutionStateWrap;
		_fileSystemProvider = fileSystemProvider;
		_textEditorService = textEditorService;
		_compilerServiceRegistry = compilerServiceRegistry;
		_terminalStateWrap = terminalStateWrap;
		_dotNetCliOutputParser = dotNetCliOutputParser;
		_serviceProvider = serviceProvider;
	}

	public void SetDotNetSolution(IAbsolutePath inSolutionAbsolutePath)
	{
		_backgroundTaskService.Enqueue(
			Key<IBackgroundTask>.NewKey(),
			ContinuousBackgroundTaskWorker.GetQueueKey(),
			"Set .NET Solution",
			() => SetDotNetSolutionAsync(inSolutionAbsolutePath));
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

			_compilerServiceRegistry
				.GetCompilerService(ExtensionNoPeriodFacts.DOT_NET_SOLUTION)
				.RegisterResource(
					resourceUri,
					shouldTriggerResourceWasModified: true);
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

		var dotNetSolutionCompilerService = (DotNetSolutionCompilerService)_compilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.DOT_NET_SOLUTION);

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
				var terminalCommandRequest = new TerminalCommandRequest(
		        	TerminalInteractive.RESERVED_TARGET_FILENAME_PREFIX + nameof(DotNetSolutionIdeApi),
		        	parentDirectory.Value)
		        {
		        	BeginWithFunc = parsedCommand =>
		        	{
		        		_terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_KEY].TerminalOutput.WriteOutput(
							parsedCommand,
							new StandardOutputCommandEvent(@$"Sln found: '{solutionAbsolutePath.Value}'
Sln-Directory: '{parentDirectory.Value}'
General Terminal"));
		        		return Task.CompletedTask;
		        	}
		        };
		        	
		        _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
			}

			// Set 'executionTerminal' working directory
			{
				var terminalCommandRequest = new TerminalCommandRequest(
		        	TerminalInteractive.RESERVED_TARGET_FILENAME_PREFIX + nameof(DotNetSolutionIdeApi),
		        	parentDirectory.Value)
		        {
		        	BeginWithFunc = parsedCommand =>
		        	{
		        		_terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_KEY].TerminalOutput.WriteOutput(
							parsedCommand,
							new StandardOutputCommandEvent(@$"Sln found: '{solutionAbsolutePath.Value}'
Sln-Directory: '{parentDirectory.Value}'
Execution Terminal"));
		        		return Task.CompletedTask;
		        	}
		        };

				_terminalStateWrap.Value.TerminalMap[TerminalFacts.EXECUTION_KEY].EnqueueCommand(terminalCommandRequest);
			}
		}
		
		// _appDataService.WriteAppDataAsync
		_ = Task.Run(() =>
		{
			try
			{
				return _appDataService.WriteAppDataAsync(new DotNetAppData
				{
					SolutionMostRecent = solutionAbsolutePath.Value
				});
			}
			catch (Exception e)
			{
				NotificationHelper.DispatchError(
			        $"ERROR: nameof(_appDataService.WriteAppDataAsync)",
			        e.ToString(),
			        _commonComponentRenderers,
			        _dispatcher,
			        TimeSpan.FromSeconds(5));
			    return Task.CompletedTask;
			}
		});

		await ParseSolution(dotNetSolutionModel.Key).ConfigureAwait(false);

		await SetDotNetSolutionTreeViewAsync(dotNetSolutionModel.Key).ConfigureAwait(false);
	}
	
	private enum ParseSolutionStageKind
	{
		A,
		B,
		C,
		D,
		E,
	}

	private Task ParseSolution(Key<DotNetSolutionModel> dotNetSolutionModelKey)
	{
		var dotNetSolutionState = _dotNetSolutionStateWrap.Value;

		var dotNetSolutionModel = dotNetSolutionState.DotNetSolutionsList.FirstOrDefault(
			x => x.Key == dotNetSolutionModelKey);

		if (dotNetSolutionModel is null)
			return Task.CompletedTask;
			
		_ = Task.Run(async () =>
		{
			var cancellationTokenSource = new CancellationTokenSource();
			var cancellationToken = cancellationTokenSource.Token;
			
			var progressBarModel = new ProgressBarModel(0, "parsing...")
			{
				OnCancelFunc = () =>
				{
					cancellationTokenSource.Cancel();
					cancellationTokenSource.Dispose();
					return Task.CompletedTask;
				}
			};

			NotificationHelper.DispatchProgress(
				$"Parse: {dotNetSolutionModel.AbsolutePath.NameWithExtension}",
				progressBarModel,
				_commonComponentRenderers,
				_dispatcher,
				TimeSpan.FromMilliseconds(-1));
				
			// var progressThrottle = new Throttle(TimeSpan.FromMilliseconds(100));
			var progressThrottle = new ThrottleOptimized<(ParseSolutionStageKind StageKind, double? Progress, string? MessageOne, string? MessageTwo)>(TimeSpan.FromMilliseconds(300), (tuple, _) =>
			{
				switch (tuple.StageKind)
				{
					case ParseSolutionStageKind.A:
						progressBarModel.SetProgress(tuple.Progress, tuple.MessageOne);
						return Task.CompletedTask;
					case ParseSolutionStageKind.B:
						progressBarModel.SetProgress(tuple.Progress, tuple.MessageOne, tuple.MessageTwo);
						progressBarModel.Dispose();
						return Task.CompletedTask;
					case ParseSolutionStageKind.C:
						progressBarModel.SetProgress(tuple.Progress, tuple.MessageOne);
						progressBarModel.Dispose();
						return Task.CompletedTask;
					case ParseSolutionStageKind.D:
						progressBarModel.SetProgress(tuple.Progress, tuple.MessageOne, tuple.MessageTwo);
						return Task.CompletedTask;
					case ParseSolutionStageKind.E:
						progressBarModel.SetProgress(tuple.Progress, tuple.MessageOne, tuple.MessageTwo);
						return Task.CompletedTask;
					default:
						return Task.CompletedTask;
				}
			});
		
			try
			{
				if (_textEditorService.TextEditorConfig.RegisterModelFunc is null)
					return;
				
				progressThrottle.Run((ParseSolutionStageKind.A, 0.05, "Discovering projects...", null));
				
				foreach (var project in dotNetSolutionModel.DotNetProjectList)
				{
					RegisterStartupControl(project);
				
					var resourceUri = new ResourceUri(project.AbsolutePath.Value);

					if (!await _fileSystemProvider.File.ExistsAsync(resourceUri.Value))
						continue; // TODO: This can still cause a race condition exception if the file is removed before the next line runs.

					var registerModelArgs = new RegisterModelArgs(resourceUri, _serviceProvider)
					{
						ShouldBlockUntilBackgroundTaskIsCompleted = true,
					};

					await _textEditorService.TextEditorConfig.RegisterModelFunc.Invoke(registerModelArgs)
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
					var maximumProgressAvailableToProject = (1 - previousStageProgress) * ((double)1.0 / dotNetProjectListLength);
					var currentProgress = Math.Min(1.0, previousStageProgress + maximumProgressAvailableToProject * projectsParsedCount);

					// This 'SetProgress' is being kept out the throttle, since it sets message 1
					// whereas the per class progress updates set message 2.
					//
					// Otherwise an update to message 2 could result in this message 1 update never being written.
					progressBarModel.SetProgress(
						currentProgress,
						$"{projectsParsedCount + 1}/{dotNetProjectListLength}: {project.AbsolutePath.NameWithExtension}");
					
					cancellationToken.ThrowIfCancellationRequested();

					await DiscoverClassesInProject(project, progressBarModel, progressThrottle, currentProgress, maximumProgressAvailableToProject);
					projectsParsedCount++;
				}

				progressThrottle.Run((ParseSolutionStageKind.B, 1, $"Finished parsing: {dotNetSolutionModel.AbsolutePath.NameWithExtension}", string.Empty));
			}
			catch (Exception e)
			{
				if (e is OperationCanceledException)
					progressBarModel.IsCancelled = true;
					
				var currentProgress = progressBarModel.GetProgress();
				
				progressThrottle.Run((ParseSolutionStageKind.C, currentProgress, e.ToString(), null));
			}
		});

		return Task.CompletedTask;
	}

	private async Task DiscoverClassesInProject(
		IDotNetProject dotNetProject,
		ProgressBarModel progressBarModel,
		ThrottleOptimized<(ParseSolutionStageKind StageKind, double? Progress, string? MessageOne, string? MessageTwo)> progressThrottle,
		double currentProgress,
		double maximumProgressAvailableToProject)
	{
		if (!await _fileSystemProvider.File.ExistsAsync(dotNetProject.AbsolutePath.Value))
			return; // TODO: This can still cause a race condition exception if the file is removed before the next line runs.

		var parentDirectory = dotNetProject.AbsolutePath.ParentDirectory;
		if (parentDirectory is null)
			return;

		var startingAbsolutePathForSearch = parentDirectory.Value;
		var discoveredFileList = new List<string>();

		progressThrottle.Run((ParseSolutionStageKind.D, null, null, "discovering files"));
		
		await DiscoverFilesRecursively(startingAbsolutePathForSearch, discoveredFileList, true).ConfigureAwait(false);

		await ParseClassesInProject(
			dotNetProject,
			progressBarModel,
			progressThrottle,
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
				if (directoryPathChild.Contains(".git") ||
					directoryPathChild.Contains(".vs") ||
					directoryPathChild.Contains(".vscode") ||
					directoryPathChild.Contains(".idea") ||
					directoryPathChild.Contains("bin") ||
					directoryPathChild.Contains("obj"))
				{
					continue;
				}

				//if (isFirstInvocation)
				//{
				//	var currentProgress = progressBarModel.GetProgress();
				// progressThrottle.Run(_ => 
				// {
				//	progressBarModel.SetProgress(currentProgress, $"{directoryPathChild} " + progressMessage);
				//	return Task.CompletedTask;
				// });
				//	
				//}

				await DiscoverFilesRecursively(directoryPathChild, discoveredFileList, isFirstInvocation: false).ConfigureAwait(false);
			}
		}
	}

	private async Task ParseClassesInProject(
		IDotNetProject dotNetProject,
		ProgressBarModel progressBarModel,
		ThrottleOptimized<(ParseSolutionStageKind StageKind, double? Progress, string? MessageOne, string? MessageTwo)> progressThrottle,
		double currentProgress,
		double maximumProgressAvailableToProject,
		List<string> discoveredFileList)
	{
		var fileParsedCount = 0;
		
		foreach (var file in discoveredFileList)
		{
			var fileAbsolutePath = _environmentProvider.AbsolutePathFactory(file, false);

			var progress = currentProgress + maximumProgressAvailableToProject * (fileParsedCount / (double)discoveredFileList.Count);

			progressThrottle.Run((ParseSolutionStageKind.E, progress, null, $"{fileParsedCount + 1}/{discoveredFileList.Count}: {fileAbsolutePath.NameWithExtension}"));

			var resourceUri = new ResourceUri(file);
			
			var registerModelArgs = new RegisterModelArgs(resourceUri, _serviceProvider)
			{
				ShouldBlockUntilBackgroundTaskIsCompleted = true,
			};

			await _textEditorService.TextEditorConfig.RegisterModelFunc.Invoke(registerModelArgs)
				.ConfigureAwait(false);
				
			fileParsedCount++;
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
			_dotNetComponentRenderers,
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
	
	private void RegisterStartupControl(IDotNetProject project)
	{
		_dispatcher.Dispatch(new StartupControlState.RegisterStartupControlAction(
			new StartupControlModel(
				Key<IStartupControlModel>.NewKey(),
				project.DisplayName,
				project.AbsolutePath.Value,
				project.AbsolutePath,
				null,
				null,
				startupControlModel => StartButtonOnClick(startupControlModel, project),
				StopButtonOnClick)));
	}
	
	private Task StartButtonOnClick(IStartupControlModel interfaceStartupControlModel, IDotNetProject project)
    {
    	var startupControlModel = (StartupControlModel)interfaceStartupControlModel;
    	
        var ancestorDirectory = project.AbsolutePath.ParentDirectory;

        if (ancestorDirectory is null)
            return Task.CompletedTask;

        var formattedCommand = DotNetCliCommandFormatter.FormatStartProjectWithoutDebugging(
            project.AbsolutePath);
            
        var terminalCommandRequest = new TerminalCommandRequest(
        	formattedCommand.Value,
        	ancestorDirectory.Value,
        	_newDotNetSolutionTerminalCommandRequestKey)
        {
        	BeginWithFunc = parsedCommand =>
        	{
        		_dotNetCliOutputParser.ParseOutputEntireDotNetRun(
        			string.Empty,
        			"Run-Project_started");
        			
        		return Task.CompletedTask;
        	},
        	ContinueWithFunc = parsedCommand =>
        	{
        		startupControlModel.ExecutingTerminalCommandRequest = null;
        		_dispatcher.Dispatch(new StartupControlState.StateChangedAction());
        	
        		_dotNetCliOutputParser.ParseOutputEntireDotNetRun(
        			parsedCommand.OutputCache.ToString(),
        			"Run-Project_completed");
        			
        		return Task.CompletedTask;
        	}
        };
        
        startupControlModel.ExecutingTerminalCommandRequest = terminalCommandRequest;
        
		_terminalStateWrap.Value.TerminalMap[TerminalFacts.EXECUTION_KEY].EnqueueCommand(terminalCommandRequest);
    	return Task.CompletedTask;
    }
    
    private Task StopButtonOnClick(IStartupControlModel interfaceStartupControlModel)
    {
    	var startupControlModel = (StartupControlModel)interfaceStartupControlModel;
    	
		_terminalStateWrap.Value.TerminalMap[TerminalFacts.EXECUTION_KEY].KillProcess();
		startupControlModel.ExecutingTerminalCommandRequest = null;
		
        _dispatcher.Dispatch(new StartupControlState.StateChangedAction());
        return Task.CompletedTask;
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

		var cSharpProject = new CSharpProjectModel(
			cSharpProjectName,
			projectTypeGuid,
			relativePathFromSlnToProject,
			projectIdGuid,
			// TODO: 'openAssociatedGroupToken' gets set when 'AddDotNetProject(...)' is ran, which is hacky and should be changed. Until then passing in 'null!'
			default,
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
			_textEditorService.PostUnique(
				nameof(Website_AddExistingProjectToSolutionAsync),
				editContext =>
				{
					var modelModifier = editContext.GetModelModifier(solutionTextEditorModel.ResourceUri);
					if (modelModifier is null)
						return Task.CompletedTask;
				
					_textEditorService.ModelApi.Reload(
						editContext,
				        modelModifier,
				        outDotNetSolutionModel.SolutionFileContents,
				        DateTime.UtcNow);
					return Task.CompletedTask;
				});
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
}
