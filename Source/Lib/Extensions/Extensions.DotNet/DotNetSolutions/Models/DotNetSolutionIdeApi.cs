using System.Runtime.InteropServices;
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
using Luthetus.TextEditor.RazorLib.FindAlls.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;
using Luthetus.CompilerServices.DotNetSolution.Models.Project;
using Luthetus.CompilerServices.DotNetSolution.Models;
using Luthetus.CompilerServices.DotNetSolution.SyntaxActors;
using Luthetus.CompilerServices.DotNetSolution.CompilerServiceCase;
using Luthetus.CompilerServices.DotNetSolution.Models.Project;
using Luthetus.CompilerServices.Xml.Html.SyntaxActors;
using Luthetus.CompilerServices.Xml.Html.SyntaxEnums;
using Luthetus.CompilerServices.Xml.Html.SyntaxObjects;
using Luthetus.Ide.RazorLib.CodeSearches.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.StartupControls.Models;
using Luthetus.Ide.RazorLib.AppDatas.Models;
// FindAllReferences
// using Luthetus.Ide.RazorLib.FindAllReferences.Models;
using Luthetus.Extensions.DotNet.CompilerServices.Models;
using Luthetus.Extensions.DotNet.Websites.ProjectTemplates.Models;
using Luthetus.Extensions.DotNet.ComponentRenderers.Models;
using Luthetus.Extensions.DotNet.CommandLines.Models;
using Luthetus.Extensions.DotNet.AppDatas.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;

namespace Luthetus.Extensions.DotNet.DotNetSolutions.Models;

public class DotNetSolutionIdeApi : IBackgroundTaskGroup
{
	private readonly IdeBackgroundTaskApi _ideBackgroundTaskApi;
	private readonly BackgroundTaskService _backgroundTaskService;
	private readonly IStorageService _storageService;
	private readonly IAppDataService _appDataService;
	private readonly ICompilerServiceExplorerService _compilerServiceExplorerService;
	private readonly IDotNetComponentRenderers _dotNetComponentRenderers;
	private readonly IIdeComponentRenderers _ideComponentRenderers;
	private readonly ICommonComponentRenderers _commonComponentRenderers;
	private readonly ITreeViewService _treeViewService;
	private readonly INotificationService _notificationService;
	private readonly IEnvironmentProvider _environmentProvider;
	private readonly IDotNetSolutionService _dotNetSolutionService;
	private readonly IFileSystemProvider _fileSystemProvider;
	private readonly TextEditorService _textEditorService;
	private readonly IFindAllService _findAllService;
	private readonly ICodeSearchService _codeSearchService;
	// FindAllReferences
	// private readonly IFindAllReferencesService _findAllReferencesService;
	private readonly IStartupControlService _startupControlService;
	private readonly ICompilerServiceRegistry _compilerServiceRegistry;
	private readonly ITerminalService _terminalService;
	private readonly DotNetCliOutputParser _dotNetCliOutputParser;
	private readonly IServiceProvider _serviceProvider;
	
	private readonly Key<TerminalCommandRequest> _newDotNetSolutionTerminalCommandRequestKey = Key<TerminalCommandRequest>.NewKey();
    private readonly CancellationTokenSource _newDotNetSolutionCancellationTokenSource = new();

	public DotNetSolutionIdeApi(
		IdeBackgroundTaskApi ideBackgroundTaskApi,
		BackgroundTaskService backgroundTaskService,
		IStorageService storageService,
		IAppDataService appDataService,
		ICompilerServiceExplorerService compilerServiceExplorerService,
        IDotNetComponentRenderers dotNetComponentRenderers,
        IIdeComponentRenderers ideComponentRenderers,
		ICommonComponentRenderers commonComponentRenderers,
		ITreeViewService treeViewService,
		INotificationService notificationService,
		IEnvironmentProvider environmentProvider,
		IDotNetSolutionService dotNetSolutionService,
		IFileSystemProvider fileSystemProvider,
		TextEditorService textEditorService,
		IFindAllService findAllService,
		ICodeSearchService codeSearchService,
		// FindAllReferences
		// IFindAllReferencesService findAllReferencesService,
		IStartupControlService startupControlService,
		ICompilerServiceRegistry compilerServiceRegistry,
		ITerminalService terminalService,
		DotNetCliOutputParser dotNetCliOutputParser,
		IServiceProvider serviceProvider)
	{
		_ideBackgroundTaskApi = ideBackgroundTaskApi;
		_backgroundTaskService = backgroundTaskService;
		_storageService = storageService;
		_appDataService = appDataService;
		_compilerServiceExplorerService = compilerServiceExplorerService;
		_compilerServiceRegistry = compilerServiceRegistry;
        _dotNetComponentRenderers = dotNetComponentRenderers;
        _ideComponentRenderers = ideComponentRenderers;
		_commonComponentRenderers = commonComponentRenderers;
		_treeViewService = treeViewService;
		_notificationService = notificationService;
		_environmentProvider = environmentProvider;
		_dotNetSolutionService = dotNetSolutionService;
		_fileSystemProvider = fileSystemProvider;
		_textEditorService = textEditorService;
		_findAllService = findAllService;
		_codeSearchService = codeSearchService;
		// FindAllReferences
		// _findAllReferencesService = findAllReferencesService;
		_startupControlService = startupControlService;
		_compilerServiceRegistry = compilerServiceRegistry;
		_terminalService = terminalService;
		_dotNetCliOutputParser = dotNetCliOutputParser;
		_serviceProvider = serviceProvider;
	}

    public Key<IBackgroundTaskGroup> BackgroundTaskKey { get; } = Key<IBackgroundTaskGroup>.NewKey();
    public string Name { get; } = nameof(CompilerServiceIdeApi);
    public bool EarlyBatchEnabled { get; } = false;

    public bool __TaskCompletionSourceWasCreated { get; set; }

    private readonly Queue<DotNetSolutionIdeWorkKind> _workKindQueue = new();

    private readonly object _workLock = new();

    private readonly Queue<AbsolutePath> _queue_SetDotNetSolution = new();

    public void Enqueue_SetDotNetSolution(AbsolutePath inSolutionAbsolutePath)
	{
        lock (_workLock)
        {
            _workKindQueue.Enqueue(DotNetSolutionIdeWorkKind.SetDotNetSolution);
			_queue_SetDotNetSolution.Enqueue(inSolutionAbsolutePath);
            _backgroundTaskService.Continuous_EnqueueGroup(this);
        }
	}

	private async ValueTask Do_SetDotNetSolution(AbsolutePath inSolutionAbsolutePath)
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
			_textEditorService.WorkerArbitrary.PostUnique(nameof(DotNetSolutionIdeApi), editContext =>
			{
				var extension = ExtensionNoPeriodFacts.DOT_NET_SOLUTION;
				
				if (dotNetSolutionAbsolutePathString.EndsWith(ExtensionNoPeriodFacts.DOT_NET_SOLUTION_X))
					extension = ExtensionNoPeriodFacts.DOT_NET_SOLUTION_X;
			
				_textEditorService.ModelApi.RegisterTemplated(
					editContext,
					extension,
					resourceUri,
					DateTime.UtcNow,
					content);
	
				_compilerServiceRegistry
					.GetCompilerService(extension)
					.RegisterResource(
						resourceUri,
						shouldTriggerResourceWasModified: true);
			
				return ValueTask.CompletedTask;
			});
		}

		DotNetSolutionModel dotNetSolutionModel;

		if (dotNetSolutionAbsolutePathString.EndsWith(ExtensionNoPeriodFacts.DOT_NET_SOLUTION_X))
			dotNetSolutionModel = ParseSlnx(solutionAbsolutePath, resourceUri, content);
		else
			dotNetSolutionModel = ParseSln(solutionAbsolutePath, resourceUri, content);
		
		var sortedByProjectReferenceDependenciesDotNetProjectList = await SortProjectReferences(dotNetSolutionModel);
		dotNetSolutionModel.DotNetProjectList = sortedByProjectReferenceDependenciesDotNetProjectList;
		
		/*	
		// FindAllReferences
		var pathGroupList = new List<(string Name, string Path)>();
		foreach (var project in sortedByProjectReferenceDependenciesDotNetProjectList)
		{
			if (project.AbsolutePath.ParentDirectory is not null)
			{
				pathGroupList.Add((project.DisplayName, project.AbsolutePath.ParentDirectory));
			}
		}
		_findAllReferencesService.PathGroupList = pathGroupList;
		*/

		// TODO: If somehow model was registered already this won't write the state
		_dotNetSolutionService.ReduceRegisterAction(dotNetSolutionModel, this);

		_dotNetSolutionService.ReduceWithAction(new WithAction(
			inDotNetSolutionState => inDotNetSolutionState with
			{
				DotNetSolutionModelKey = dotNetSolutionModel.Key
			}));

		// TODO: Putting a hack for now to overwrite if somehow model was registered already
		_dotNetSolutionService.ReduceWithAction(ConstructModelReplacement(
			dotNetSolutionModel.Key,
			dotNetSolutionModel));

		var dotNetSolutionCompilerService = (DotNetSolutionCompilerService)_compilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.DOT_NET_SOLUTION);

		dotNetSolutionCompilerService.ResourceWasModified(
			new ResourceUri(solutionAbsolutePath.Value),
			Array.Empty<TextEditorTextSpan>());

		var parentDirectory = solutionAbsolutePath.ParentDirectory;

		if (parentDirectory is not null)
		{
			_environmentProvider.DeletionPermittedRegister(new(parentDirectory, true));
			
			foreach (var project in dotNetSolutionModel.DotNetProjectList)
			{
				var innerParentDirectory = project.AbsolutePath.ParentDirectory;
				if (innerParentDirectory is not null)
					_environmentProvider.DeletionPermittedRegister(new(innerParentDirectory, true));
			}

			_findAllService.SetStartingDirectoryPath(parentDirectory);

			_codeSearchService.With(inState => inState with
			{
				StartingAbsolutePathForSearch = parentDirectory
			});

			// Set 'generalTerminal' working directory
			{
				var terminalCommandRequest = new TerminalCommandRequest(
		        	TerminalInteractive.RESERVED_TARGET_FILENAME_PREFIX + nameof(DotNetSolutionIdeApi),
		        	parentDirectory)
		        {
		        	BeginWithFunc = parsedCommand =>
		        	{
		        		_terminalService.GetTerminalState().TerminalMap[TerminalFacts.GENERAL_KEY].TerminalOutput.WriteOutput(
							parsedCommand,
							new StandardOutputCommandEvent(@$"Sln found: '{solutionAbsolutePath.Value}'
Sln-Directory: '{parentDirectory}'
General Terminal".ReplaceLineEndings("\n")));
		        		return Task.CompletedTask;
		        	}
		        };
		        	
		        _terminalService.GetTerminalState().TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
			}

			// Set 'executionTerminal' working directory
			{
				var terminalCommandRequest = new TerminalCommandRequest(
		        	TerminalInteractive.RESERVED_TARGET_FILENAME_PREFIX + nameof(DotNetSolutionIdeApi),
		        	parentDirectory)
		        {
		        	BeginWithFunc = parsedCommand =>
		        	{
		        		_terminalService.GetTerminalState().TerminalMap[TerminalFacts.GENERAL_KEY].TerminalOutput.WriteOutput(
							parsedCommand,
							new StandardOutputCommandEvent(@$"Sln found: '{solutionAbsolutePath.Value}'
Sln-Directory: '{parentDirectory}'
Execution Terminal".ReplaceLineEndings("\n")));
		        		return Task.CompletedTask;
		        	}
		        };

				_terminalService.GetTerminalState().TerminalMap[TerminalFacts.EXECUTION_KEY].EnqueueCommand(terminalCommandRequest);
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
			        _notificationService,
			        TimeSpan.FromSeconds(5));
			    return Task.CompletedTask;
			}
		});
		
		_textEditorService.WorkerArbitrary.EnqueueUniqueTextEditorWork(
			new UniqueTextEditorWork(
	            nameof(ParseSolution),
	            _textEditorService,
	            async editContext =>
	            {
	            	await ParseSolution(editContext, dotNetSolutionModel.Key);
	            	await ParseSolution(editContext, dotNetSolutionModel.Key);
            	}));

		await Do_SetDotNetSolutionTreeView(dotNetSolutionModel.Key).ConfigureAwait(false);
	}
	
	public DotNetSolutionModel ParseSlnx(
		AbsolutePath solutionAbsolutePath,
		ResourceUri resourceUri,
		string content)
	{
    	var htmlSyntaxUnit = HtmlSyntaxTree.ParseText(
			new(solutionAbsolutePath.Value),
			content);

		var syntaxNodeRoot = htmlSyntaxUnit.RootTagSyntax;

		var cSharpProjectSyntaxWalker = new CSharpProjectSyntaxWalker();

		cSharpProjectSyntaxWalker.Visit(syntaxNodeRoot);

		var dotNetProjectList = new List<IDotNetProject>();
		var solutionFolderList = new List<SolutionFolder>();

		var folderTagList = cSharpProjectSyntaxWalker.TagNodes
			.Where(ts => (ts.OpenTagNameNode?.TextEditorTextSpan.GetText() ?? string.Empty) == "Folder")
			.ToList();
    	
    	var projectTagList = cSharpProjectSyntaxWalker.TagNodes
			.Where(ts => (ts.OpenTagNameNode?.TextEditorTextSpan.GetText() ?? string.Empty) == "Project")
			.ToList();
		
		var solutionFolderPathHashSet = new HashSet<string>();
		
		var stringNestedProjectEntryList = new List<StringNestedProjectEntry>();
		
		foreach (var folder in folderTagList)
		{
			var attributeNameValueTuples = folder
				.AttributeNodes
				.Select(x => (
					x.AttributeNameSyntax.TextEditorTextSpan
						.GetText()
						.Trim(),
					x.AttributeValueSyntax.TextEditorTextSpan
						.GetText()
						.Replace("\"", string.Empty)
						.Replace("=", string.Empty)
						.Trim()))
				.ToArray();

			var attribute = attributeNameValueTuples.FirstOrDefault(x => x.Item1 == "Name");
			if (attribute.Item2 is null)
				continue;

			var ancestorDirectoryList = new List<string>();

			var absolutePath = new AbsolutePath(
				attribute.Item2,
				isDirectory: true,
				_environmentProvider,
				ancestorDirectoryList);

			solutionFolderPathHashSet.Add(absolutePath.Value);
			
			for (int i = 0; i < ancestorDirectoryList.Count; i++)
			{
				if (i == 0)
					continue;
					
				solutionFolderPathHashSet.Add(ancestorDirectoryList[i]);
			}
			
			foreach (var child in folder.ChildContent)
			{
				if (child.HtmlSyntaxKind == HtmlSyntaxKind.TagSelfClosingNode ||
					child.HtmlSyntaxKind == HtmlSyntaxKind.TagClosingNode)
				{
					var tagNode = (TagNode)child;
					
					attributeNameValueTuples = tagNode
						.AttributeNodes
						.Select(x => (
							x.AttributeNameSyntax.TextEditorTextSpan
								.GetText()
								.Trim(),
							x.AttributeValueSyntax.TextEditorTextSpan
								.GetText()
								.Replace("\"", string.Empty)
								.Replace("=", string.Empty)
								.Trim()))
						.ToArray();
		
					attribute = attributeNameValueTuples.FirstOrDefault(x => x.Item1 == "Path");
					if (attribute.Item2 is null)
						continue;
						
					stringNestedProjectEntryList.Add(new StringNestedProjectEntry(
		    			ChildIsSolutionFolder: false,
					    attribute.Item2,
					    absolutePath.Value));
				}
			}
		}
		
		// I'm too tired to decide if enumerating a HashSet is safe
		var temporarySolutionFolderList = solutionFolderPathHashSet.ToList();
		
		foreach (var solutionFolderPath in temporarySolutionFolderList)
		{
			var absolutePath = new AbsolutePath(
				solutionFolderPath,
				isDirectory: true,
				_environmentProvider);
			
			solutionFolderList.Add(new SolutionFolder(
		        absolutePath.NameNoExtension,
		        solutionFolderPath));
		}
		
		foreach (var project in projectTagList)
		{
			var attributeNameValueTuples = project
				.AttributeNodes
				.Select(x => (
					x.AttributeNameSyntax.TextEditorTextSpan
						.GetText()
						.Trim(),
					x.AttributeValueSyntax.TextEditorTextSpan
						.GetText()
						.Replace("\"", string.Empty)
						.Replace("=", string.Empty)
						.Trim()))
				.ToArray();

			var attribute = attributeNameValueTuples.FirstOrDefault(x => x.Item1 == "Path");
			if (attribute.Item2 is null)
				continue;

			var relativePath = new RelativePath(attribute.Item2, isDirectory: false, _environmentProvider);

			dotNetProjectList.Add(new CSharpProjectModel(
		        relativePath.NameNoExtension,
		        Guid.Empty,
		        attribute.Item2,
		        Guid.Empty,
		        new(),
		        new(),
		        default(AbsolutePath)));
		}

    	var dotNetSolutionHeader = new DotNetSolutionHeader();
    	var dotNetSolutionGlobal = new DotNetSolutionGlobal();
    	
    	// You have to iterate in reverse so ascending will put longest words to shortest (when iterating reverse).
    	var childSolutionFolderList = solutionFolderList.OrderBy(x => x.ActualName).ToList();
    	var parentSolutionFolderList = new List<SolutionFolder>(childSolutionFolderList);
    	
    	for (int parentIndex = parentSolutionFolderList.Count - 1; parentIndex >= 0; parentIndex--)
    	{
    		var parentSolutionFolder = parentSolutionFolderList[parentIndex];
    		
	    	for (int childIndex = childSolutionFolderList.Count - 1; childIndex >= 0; childIndex--)
	    	{
	    		var childSolutionFolder = childSolutionFolderList[childIndex];
	    		
	    		if (childSolutionFolder.ActualName != parentSolutionFolder.ActualName &&
	    			childSolutionFolder.ActualName.StartsWith(parentSolutionFolder.ActualName))
	    		{
	    			stringNestedProjectEntryList.Add(new StringNestedProjectEntry(
		    			ChildIsSolutionFolder: true,
					    childSolutionFolder.ActualName,
					    parentSolutionFolder.ActualName));
					    
				    childSolutionFolderList.RemoveAt(childIndex);
	    		}
	    	}
    	}
    	
    	/*foreach (var stringNestedProjectEntry in stringNestedProjectEntryList)
    	{
    		Console.WriteLine($"ci_{stringNestedProjectEntry.ChildIdentifier} -- {stringNestedProjectEntry.SolutionFolderActualName}");
    	}*/
	
		return ParseSharedSteps(
			dotNetProjectList,
			solutionFolderList,
			solutionAbsolutePath,
			resourceUri,
			content,
			dotNetSolutionHeader,
			guidNestedProjectEntryList: null,
			stringNestedProjectEntryList: stringNestedProjectEntryList,
			dotNetSolutionGlobal);
	}
		
	public DotNetSolutionModel ParseSln(
		AbsolutePath solutionAbsolutePath,
		ResourceUri resourceUri,
		string content)
	{
		var lexer = new DotNetSolutionLexer(
			resourceUri,
			content);

		lexer.Lex();

		var parser = new DotNetSolutionParser(lexer);

		var compilationUnit = parser.Parse();

		return ParseSharedSteps(
			parser.DotNetProjectList,
			parser.SolutionFolderList,
			solutionAbsolutePath,
			resourceUri,
			content,
			parser.DotNetSolutionHeader,
			guidNestedProjectEntryList: parser.NestedProjectEntryList,
			stringNestedProjectEntryList: null,
			parser.DotNetSolutionGlobal);
	}
	
	public DotNetSolutionModel ParseSharedSteps(
		List<IDotNetProject> dotNetProjectList,
		List<SolutionFolder> solutionFolderList,
		AbsolutePath solutionAbsolutePath,
		ResourceUri resourceUri,
		string content,
		DotNetSolutionHeader dotNetSolutionHeader,
		List<GuidNestedProjectEntry>? guidNestedProjectEntryList,
		List<StringNestedProjectEntry>? stringNestedProjectEntryList,
		DotNetSolutionGlobal dotNetSolutionGlobal)
	{
		foreach (var project in dotNetProjectList)
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

			var absolutePathString = PathHelper.GetAbsoluteFromAbsoluteAndRelative(
				solutionAbsolutePath,
				relativePathFromSolutionFileString,
				_environmentProvider);

			project.AbsolutePath = _environmentProvider.AbsolutePathFactory(absolutePathString, false);
		}

		return new DotNetSolutionModel(
			solutionAbsolutePath,
			dotNetSolutionHeader,
			dotNetProjectList,
			solutionFolderList,
			guidNestedProjectEntryList,
			stringNestedProjectEntryList,
			dotNetSolutionGlobal,
			content);
	}
	
	private enum ParseSolutionStageKind
	{
		A,
		B,
		C,
		D,
		E,
	}
	
	/// <summary>
	/// This solution is incomplete, the current code for this was just to get a "feel" for things.
	/// </summary>
	private async ValueTask<List<IDotNetProject>> SortProjectReferences(DotNetSolutionModel dotNetSolutionModel)
	{
		List<(IDotNetProject Project, List<AbsolutePath> ReferenceProjectAbsolutePathList)> enumeratingProjectTupleList = dotNetSolutionModel.DotNetProjectList
			.Select(project => (project, new List<AbsolutePath>()))
			.OrderBy(projectTuple => projectTuple.project.AbsolutePath.Value)
			.ToList();
		
		for (int i = enumeratingProjectTupleList.Count - 1; i >= 0; i--)
		{
			var projectTuple = enumeratingProjectTupleList[i];
		
			if (!await _fileSystemProvider.File.ExistsAsync(projectTuple.Project.AbsolutePath.Value))
			{
				enumeratingProjectTupleList.RemoveAt(i);
				continue;
			}
				
			var content = await _fileSystemProvider.File.ReadAllTextAsync(
					projectTuple.Project.AbsolutePath.Value)
				.ConfigureAwait(false);
	
			var htmlSyntaxUnit = HtmlSyntaxTree.ParseText(
				new(projectTuple.Project.AbsolutePath.Value),
				content);
	
			var syntaxNodeRoot = htmlSyntaxUnit.RootTagSyntax;
	
			var cSharpProjectSyntaxWalker = new CSharpProjectSyntaxWalker();
	
			cSharpProjectSyntaxWalker.Visit(syntaxNodeRoot);
	
			var projectReferences = cSharpProjectSyntaxWalker.TagNodes
				.Where(ts => (ts.OpenTagNameNode?.TextEditorTextSpan.GetText() ?? string.Empty) == "ProjectReference")
				.ToList();
	
			foreach (var projectReference in projectReferences)
			{
				var attributeNameValueTuples = projectReference
					.AttributeNodes
					.Select(x => (
						x.AttributeNameSyntax.TextEditorTextSpan
							.GetText()
							.Trim(),
						x.AttributeValueSyntax.TextEditorTextSpan
							.GetText()
							.Replace("\"", string.Empty)
							.Replace("=", string.Empty)
							.Trim()))
					.ToArray();
	
				var includeAttribute = attributeNameValueTuples.FirstOrDefault(x => x.Item1 == "Include");
	
				var referenceProjectAbsolutePathString = PathHelper.GetAbsoluteFromAbsoluteAndRelative(
					projectTuple.Project.AbsolutePath,
					includeAttribute.Item2,
					_environmentProvider);
	
				var referenceProjectAbsolutePath = _environmentProvider.AbsolutePathFactory(
					referenceProjectAbsolutePathString,
					false);
	
				projectTuple.ReferenceProjectAbsolutePathList.Add(referenceProjectAbsolutePath);
			}
		}
		
		var upperLimit = enumeratingProjectTupleList.Count;
		for (int outerIndex = 0; outerIndex < upperLimit; outerIndex++)
		{
			for (int i = 0; i < enumeratingProjectTupleList.Count; i++)
			{
				var projectTuple = enumeratingProjectTupleList[i];
				
				foreach (var referenceAbsolutePath in projectTuple.ReferenceProjectAbsolutePathList)
				{
					var referenceIndex = enumeratingProjectTupleList
						.FindIndex(x => x.Project.AbsolutePath.Value == referenceAbsolutePath.Value);
				
					if (referenceIndex > i)
					{
						var indexDestination = i - 1;
						if (indexDestination == -1)
							indexDestination = 0;
					
						MoveAndShiftList(
							enumeratingProjectTupleList,
							indexSource: referenceIndex,
							indexDestination);
					}
				}
			}
		}
		
		return enumeratingProjectTupleList.Select(x =>
		{
			x.Project.ReferencedAbsolutePathList = x.ReferenceProjectAbsolutePathList;
			return x.Project;
		}).ToList();
	}
	
	private void MoveAndShiftList(
		List<(IDotNetProject Project, List<AbsolutePath> ReferenceProjectAbsolutePathList)> enumeratingProjectTupleList,
		int indexSource,
		int indexDestination)
	{
		if (indexSource == 1 && indexDestination == 0)
		{
			var otherTemporary = enumeratingProjectTupleList[indexDestination];
			enumeratingProjectTupleList[indexDestination] = enumeratingProjectTupleList[indexSource];
			enumeratingProjectTupleList[indexSource] = otherTemporary;
			return;
		}
	
		var temporary = enumeratingProjectTupleList[indexDestination];
		enumeratingProjectTupleList[indexDestination] = enumeratingProjectTupleList[indexSource];
		
		for (int i = indexSource; i > indexDestination; i--)
		{
			if (i - 1 == indexDestination)
				enumeratingProjectTupleList[i] = temporary;
			else
				enumeratingProjectTupleList[i] = enumeratingProjectTupleList[i - 1];
		}
	}

	private async ValueTask ParseSolution(TextEditorEditContext editContext, Key<DotNetSolutionModel> dotNetSolutionModelKey)
	{
		var dotNetSolutionState = _dotNetSolutionService.GetDotNetSolutionState();

		var dotNetSolutionModel = dotNetSolutionState.DotNetSolutionsList.FirstOrDefault(
			x => x.Key == dotNetSolutionModelKey);

		if (dotNetSolutionModel is null)
			return;
		
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
			_notificationService,
			TimeSpan.FromMilliseconds(-1));
			
		// var progressThrottle = new Throttle(TimeSpan.FromMilliseconds(100));
		/*var progressThrottle = new ThrottleOptimized<(ParseSolutionStageKind StageKind, double? Progress, string? MessageOne, string? MessageTwo)>(TimeSpan.FromMilliseconds(1_000), (tuple, _) =>
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
		});*/
	
		try
		{
			// progressThrottle.Run((ParseSolutionStageKind.A, 0.05, "Discovering projects...", null));
			
			foreach (var project in dotNetSolutionModel.DotNetProjectList)
			{
				RegisterStartupControl(project);
			
				var resourceUri = new ResourceUri(project.AbsolutePath.Value);

				if (!await _fileSystemProvider.File.ExistsAsync(resourceUri.Value))
					continue; // TODO: This can still cause a race condition exception if the file is removed before the next line runs.

				/*var registerModelArgs = new RegisterModelArgs(resourceUri, _serviceProvider)
				{
					ShouldBlockUntilBackgroundTaskIsCompleted = true,
				};

				await _textEditorService.TextEditorConfig.RegisterModelFunc.Invoke(registerModelArgs)
					.ConfigureAwait(false);*/
			}

			var previousStageProgress = 0.05;
			var dotNetProjectListLength = dotNetSolutionModel.DotNetProjectList.Count;
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

				await DiscoverClassesInProject(editContext, project, progressBarModel, currentProgress, maximumProgressAvailableToProject);
				projectsParsedCount++;
			}

			// progressThrottle.Run((ParseSolutionStageKind.B, 1, $"Finished parsing: {dotNetSolutionModel.AbsolutePath.NameWithExtension}", string.Empty));
			progressBarModel.SetProgress(1, $"Finished parsing: {dotNetSolutionModel.AbsolutePath.NameWithExtension}", string.Empty);
			progressBarModel.Dispose();
		}
		catch (Exception e)
		{
			if (e is OperationCanceledException)
				progressBarModel.IsCancelled = true;
				
			var currentProgress = progressBarModel.GetProgress();
			
			// progressThrottle.Run((ParseSolutionStageKind.C, currentProgress, e.ToString(), null));
			progressBarModel.SetProgress(currentProgress, e.ToString());
			progressBarModel.Dispose();
		}
	}

	private async Task DiscoverClassesInProject(
		TextEditorEditContext editContext, 
		IDotNetProject dotNetProject,
		ProgressBarModel progressBarModel,
		double currentProgress,
		double maximumProgressAvailableToProject)
	{
		if (!await _fileSystemProvider.File.ExistsAsync(dotNetProject.AbsolutePath.Value))
			return; // TODO: This can still cause a race condition exception if the file is removed before the next line runs.

		var parentDirectory = dotNetProject.AbsolutePath.ParentDirectory;
		if (parentDirectory is null)
			return;

		var startingAbsolutePathForSearch = parentDirectory;
		var discoveredFileList = new List<string>();

		// progressThrottle.Run((ParseSolutionStageKind.D, null, null, "discovering files"));
		
		await DiscoverFilesRecursively(startingAbsolutePathForSearch, discoveredFileList, true).ConfigureAwait(false);

		await ParseClassesInProject(
			editContext,
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
				if (IFileSystemProvider.IsDirectoryIgnored(directoryPathChild))
					continue;

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
		TextEditorEditContext editContext,
		IDotNetProject dotNetProject,
		ProgressBarModel progressBarModel,
		double currentProgress,
		double maximumProgressAvailableToProject,
		List<string> discoveredFileList)
	{
		var fileParsedCount = 0;
		
		foreach (var file in discoveredFileList)
		{
			var fileAbsolutePath = _environmentProvider.AbsolutePathFactory(file, false);

			var progress = currentProgress + maximumProgressAvailableToProject * (fileParsedCount / (double)discoveredFileList.Count);

			// progressThrottle.Run((ParseSolutionStageKind.E, progress, null, $"{fileParsedCount + 1}/{discoveredFileList.Count}: {fileAbsolutePath.NameWithExtension}"));

			var resourceUri = new ResourceUri(file);
			
	        var compilerService = _compilerServiceRegistry.GetCompilerService(fileAbsolutePath.ExtensionNoPeriod);
			
			compilerService.RegisterResource(
				resourceUri,
				shouldTriggerResourceWasModified: false);
				
			await compilerService.FastParseAsync(editContext, resourceUri, _fileSystemProvider)
				.ConfigureAwait(false);
				
			fileParsedCount++;
		}
	}

	private readonly Queue<Key<DotNetSolutionModel>> _queue_SetDotNetSolutionTreeView = new();

    public void Enqueue_SetDotNetSolutionTreeView(Key<DotNetSolutionModel> dotNetSolutionModelKey)
	{
        lock (_workLock)
        {
            _workKindQueue.Enqueue(DotNetSolutionIdeWorkKind.SetDotNetSolutionTreeView);
			_queue_SetDotNetSolutionTreeView.Enqueue(dotNetSolutionModelKey);
            _backgroundTaskService.Continuous_EnqueueGroup(this);
        }
	}

	private async ValueTask Do_SetDotNetSolutionTreeView(Key<DotNetSolutionModel> dotNetSolutionModelKey)
	{
		var dotNetSolutionState = _dotNetSolutionService.GetDotNetSolutionState();

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
			_treeViewService.ReduceRegisterContainerAction(new TreeViewContainer(
				DotNetSolutionState.TreeViewSolutionExplorerStateKey,
				rootNode,
				new List<TreeViewNoType> { rootNode }));
		}
		else
		{
			_treeViewService.ReduceWithRootNodeAction(DotNetSolutionState.TreeViewSolutionExplorerStateKey, rootNode);

			_treeViewService.ReduceSetActiveNodeAction(
				DotNetSolutionState.TreeViewSolutionExplorerStateKey,
				rootNode,
				true,
				false);
		}

		if (dotNetSolutionModel is null)
			return;

		_dotNetSolutionService.ReduceWithAction(ConstructModelReplacement(
			dotNetSolutionModel.Key,
			dotNetSolutionModel));
	}
	
	private void RegisterStartupControl(IDotNetProject project)
	{
		_startupControlService.RegisterStartupControl(
			new StartupControlModel(
				Key<IStartupControlModel>.NewKey(),
				project.DisplayName,
				project.AbsolutePath.Value,
				project.AbsolutePath,
				null,
				null,
				startupControlModel => StartButtonOnClick(startupControlModel, project),
				StopButtonOnClick));
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
        	ancestorDirectory,
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
        		_startupControlService.StateChanged();
        	
        		_dotNetCliOutputParser.ParseOutputEntireDotNetRun(
        			parsedCommand.OutputCache.ToString(),
        			"Run-Project_completed");
        			
        		return Task.CompletedTask;
        	}
        };
        
        startupControlModel.ExecutingTerminalCommandRequest = terminalCommandRequest;
        
		_terminalService.GetTerminalState().TerminalMap[TerminalFacts.EXECUTION_KEY].EnqueueCommand(terminalCommandRequest);
    	return Task.CompletedTask;
    }
    
    private Task StopButtonOnClick(IStartupControlModel interfaceStartupControlModel)
    {
    	var startupControlModel = (StartupControlModel)interfaceStartupControlModel;
    	
		_terminalService.GetTerminalState().TerminalMap[TerminalFacts.EXECUTION_KEY].KillProcess();
		startupControlModel.ExecutingTerminalCommandRequest = null;
		
        _startupControlService.StateChanged();
        return Task.CompletedTask;
    }

	private readonly
		Queue<(Key<DotNetSolutionModel> dotNetSolutionModelKey, string projectTemplateShortName, string cSharpProjectName, AbsolutePath cSharpProjectAbsolutePath)>
		_queue_Website_AddExistingProjectToSolution = new();

    public void Enqueue_Website_AddExistingProjectToSolution(
		Key<DotNetSolutionModel> dotNetSolutionModelKey,
		string projectTemplateShortName,
		string cSharpProjectName,
		AbsolutePath cSharpProjectAbsolutePath)
	{
        lock (_workLock)
        {
            _workKindQueue.Enqueue(DotNetSolutionIdeWorkKind.Website_AddExistingProjectToSolution);

			_queue_Website_AddExistingProjectToSolution.Enqueue(
                (dotNetSolutionModelKey, projectTemplateShortName, cSharpProjectName, cSharpProjectAbsolutePath));

            _backgroundTaskService.Continuous_EnqueueGroup(this);
        }
	}

	private ValueTask Do_Website_AddExistingProjectToSolution(
		Key<DotNetSolutionModel> dotNetSolutionModelKey,
		string projectTemplateShortName,
		string cSharpProjectName,
		AbsolutePath cSharpProjectAbsolutePath)
	{
		return ValueTask.CompletedTask;
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

			var outDotNetSolutions = new List<DotNetSolutionModel>(dotNetSolutionState.DotNetSolutionsList);
			outDotNetSolutions[indexOfSln] = outDotNetSolutionModel;

			return dotNetSolutionState with
			{
				DotNetSolutionsList = outDotNetSolutions
			};
		});
	}

    public IBackgroundTaskGroup? EarlyBatchOrDefault(IBackgroundTaskGroup oldEvent)
    {
        return null;
    }

    public ValueTask HandleEvent(CancellationToken cancellationToken)
    {
        DotNetSolutionIdeWorkKind workKind;

        lock (_workLock)
        {
            if (!_workKindQueue.TryDequeue(out workKind))
                return ValueTask.CompletedTask;
        }

        switch (workKind)
        {
            case DotNetSolutionIdeWorkKind.SetDotNetSolution:
            {
				var args = _queue_SetDotNetSolution.Dequeue();
                return Do_SetDotNetSolution(args);
            }
			case DotNetSolutionIdeWorkKind.SetDotNetSolutionTreeView:
            {
				var args = _queue_SetDotNetSolutionTreeView.Dequeue();
                return Do_SetDotNetSolutionTreeView(args);
            }
			case DotNetSolutionIdeWorkKind.Website_AddExistingProjectToSolution:
            {
				var args = _queue_Website_AddExistingProjectToSolution.Dequeue();
                return Do_Website_AddExistingProjectToSolution(
                    args.dotNetSolutionModelKey,
					args.projectTemplateShortName,
					args.cSharpProjectName,
                    args.cSharpProjectAbsolutePath);
            }
            default:
            {
                Console.WriteLine($"{nameof(DotNetSolutionIdeApi)} {nameof(HandleEvent)} default case");
				return ValueTask.CompletedTask;
            }
        }
    }
}
