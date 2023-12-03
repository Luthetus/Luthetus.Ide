using System.Collections.Immutable;
using static Luthetus.Ide.RazorLib.TestExplorers.States.TestExplorerState;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Namespaces.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.SyntaxActors;
using Luthetus.CompilerServices.Lang.DotNetSolution.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.Models.Project;
using Luthetus.Ide.RazorLib.Websites.ProjectTemplates.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.Ide.RazorLib.TestExplorers.States;

public partial class TestExplorerSync
{
	private async Task DotNetSolutionStateWrap_StateChangedAsync(object? sender, EventArgs e)
	{
		var dotNetSolutionState = DotNetSolutionStateWrap.Value;
		var dotNetSolutionModel = dotNetSolutionState.DotNetSolutionModel;

		try
		{
			await _projectTestModelBagSemaphoreSlim.WaitAsync();

			if (dotNetSolutionModel is null)
				return;

			var localDotNetProjectBag = dotNetSolutionModel.DotNetProjectBag
				.Where(x => x.DotNetProjectKind == DotNetProjectKind.CSharpProject);

			var localProjectTestModelBag = localDotNetProjectBag.Select(x => new ProjectTestModel
			{
				ProjectIdGuid = x.ProjectIdGuid,
				AbsolutePath = x.AbsolutePath,
				EnqueueDiscoverTestsFunc = callback => Task.CompletedTask,
				ReRenderNodeAction = node => TreeViewService.ReRenderNode(
					TestExplorerState.TreeViewTestExplorerKey,
					node)
			});

			var localFormattedCommand = DotNetCliCommandFormatter.FormatDotNetTestListTests();
	
			var localTreeViewProjectTestModelBag = localProjectTestModelBag.Select(x =>
					(TreeViewNoType)new TreeViewProjectTestModel(
						x,
						CommonComponentRenderers,
						true,
						false))
				.ToArray();

			foreach (var entry in localTreeViewProjectTestModelBag)
			{
				var treeViewProjectTestModel = (TreeViewProjectTestModel)entry;
				
				if (String.IsNullOrWhiteSpace(treeViewProjectTestModel.Item.DirectoryNameForTestDiscovery))
					return;

				treeViewProjectTestModel.Item.EnqueueDiscoverTestsFunc = async callback =>
				{
					var executionTerminalSession = TerminalSessionStateWrap.Value.TerminalSessionMap[
		            	TerminalSessionFacts.EXECUTION_TERMINAL_SESSION_KEY];
		
			        var dotNetTestListTestsCommand = new TerminalCommand(
			            treeViewProjectTestModel.Item.DotNetTestListTestsTerminalCommandKey,
			            localFormattedCommand,
			            treeViewProjectTestModel.Item.DirectoryNameForTestDiscovery,
			            DotNetTestListTestsCancellationTokenSource.Token,
			            async () => 
						{
							try
							{
								var output = executionTerminalSession.ReadStandardOut(treeViewProjectTestModel.Item.DotNetTestListTestsTerminalCommandKey);
			
								treeViewProjectTestModel.Item.DotNetTestListTestsCommandOutput = DotNetCliOutputLexer.LexDotNetTestListTestsTerminalOutput(output);
				
				    			// THINKING_ABOUT_TREE_VIEW();
								{
									var splitOutputBag = treeViewProjectTestModel.Item.DotNetTestListTestsCommandOutput
										.Select(x => x.Split('.'));
							
									var rootMap = new Dictionary<string, StringFragment>();
	
									foreach (var splitOutput in splitOutputBag)
									{
										var targetMap = rootMap;
										var lastSeenStringFragment = (StringFragment?)null;
							
										foreach (var fragment in splitOutput)
										{
											if (!targetMap.ContainsKey(fragment))
												targetMap.Add(fragment, new(fragment));
							
											lastSeenStringFragment = targetMap[fragment];
											targetMap = lastSeenStringFragment.Map;
										}
										
										lastSeenStringFragment.IsEndpoint = true;
									}
	
									treeViewProjectTestModel.Item.RootStringFragmentMap = rootMap;
									await callback.Invoke(rootMap);
								}
							}
							catch (Exception)
							{
								await callback.Invoke(new());
								throw;
							}
						});

			        await executionTerminalSession.EnqueueCommandAsync(dotNetTestListTestsCommand);
				};				
			}
	
			var adhocRoot = TreeViewAdhoc.ConstructTreeViewAdhoc(localTreeViewProjectTestModelBag);
			var firstNode = localTreeViewProjectTestModelBag.FirstOrDefault();
	
			var activeNodes = firstNode is null
				? Array.Empty<TreeViewNoType>()
				: new []{ firstNode };
				
			if (!TreeViewService.TryGetTreeViewContainer(TreeViewTestExplorerKey, out _))
	        {
	            TreeViewService.RegisterTreeViewContainer(new TreeViewContainer(
	                TestExplorerState.TreeViewTestExplorerKey,
	                adhocRoot,
	                activeNodes.ToImmutableList()));
	        }
	        else
	        {
	            TreeViewService.SetRoot(TestExplorerState.TreeViewTestExplorerKey, adhocRoot);
	            TreeViewService.SetActiveNode(TestExplorerState.TreeViewTestExplorerKey, firstNode);
	        }

			_projectTestModelBag = localProjectTestModelBag.ToList();
		}
		finally
		{
			_projectTestModelBagSemaphoreSlim.Release();
		}

		await InvokeAsync(StateHasChanged);
	}

    private async Task SetDotNetSolutionAsync(IAbsolutePath inSolutionAbsolutePath)
    {
        var dotNetSolutionAbsolutePathString = inSolutionAbsolutePath.Value;

        var content = await _fileSystemProvider.File.ReadAllTextAsync(
            dotNetSolutionAbsolutePathString,
            CancellationToken.None);

        var solutionAbsolutePath = new AbsolutePath(
            dotNetSolutionAbsolutePathString,
            false,
            _environmentProvider);

        var solutionNamespacePath = new NamespacePath(
            string.Empty,
            solutionAbsolutePath);

        var lexer = new DotNetSolutionLexer(
            new ResourceUri(solutionAbsolutePath.Value),
            content);

        lexer.Lex();

        var parser = new DotNetSolutionParser(lexer);

        var compilationUnit = parser.Parse();

        foreach (var project in parser.DotNetProjectBag)
        {
            var relativePathFromSolutionFileString = project.RelativePathFromSolutionFileString;
            
            // Solution Folders do not exist on the filesystem. Therefore their absolute path is not guaranteed to be unique
            // One can use the ProjectIdGuid however, when working with a SolutionFolder to make the absolute path unique.
            if (project.DotNetProjectKind == DotNetProjectKind.SolutionFolder)
                relativePathFromSolutionFileString = $"{project.ProjectIdGuid}_{relativePathFromSolutionFileString}";

            var absolutePathString = PathHelper.GetAbsoluteFromAbsoluteAndRelative(
                solutionAbsolutePath,
                relativePathFromSolutionFileString,
                _environmentProvider);

            project.AbsolutePath = new AbsolutePath(absolutePathString, false, _environmentProvider);
        }

        var solutionFolderBag = parser.DotNetProjectBag
            .Where(x => x.DotNetProjectKind == DotNetProjectKind.SolutionFolder)
            .Select(x => (SolutionFolder)x).ToImmutableArray();

        var dotNetSolutionModel = new DotNetSolutionModel(
            solutionAbsolutePath,
            parser.DotNetSolutionHeader,
            parser.DotNetProjectBag.ToImmutableArray(),
            solutionFolderBag,
            parser.NestedProjectEntryBag.ToImmutableArray(),
            parser.DotNetSolutionGlobal,
            content);

        // TODO: If somehow model was registered already this won't write the state
        Dispatcher.Dispatch(new RegisterAction(dotNetSolutionModel, this));

        Dispatcher.Dispatch(new WithAction(
            inDotNetSolutionState => inDotNetSolutionState with
            {
                DotNetSolutionModelKey = dotNetSolutionModel.Key
            }));

        // TODO: Putting a hack for now to overwrite if somehow model was registered already
        Dispatcher.Dispatch(ConstructModelReplacement(
            dotNetSolutionModel.Key,
            dotNetSolutionModel));

        var dotNetSolutionCompilerService = _interfaceCompilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.DOT_NET_SOLUTION);

        dotNetSolutionCompilerService.ResourceWasModified(
            new ResourceUri(solutionAbsolutePath.Value),
            ImmutableArray<TextEditorTextSpan>.Empty);

        await SetDotNetSolutionTreeViewAsync(dotNetSolutionModel.Key);
    }

    private async Task SetDotNetSolutionTreeViewAsync(Key<DotNetSolutionModel> dotNetSolutionModelKey)
    {
        var dotNetSolutionState = _dotNetSolutionStateWrap.Value;

        var dotNetSolutionModel = dotNetSolutionState.DotNetSolutionsBag.FirstOrDefault(
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

        await rootNode.LoadChildBagAsync();

        if (!_treeViewService.TryGetTreeViewContainer(TreeViewSolutionExplorerStateKey, out _))
        {
            _treeViewService.RegisterTreeViewContainer(new TreeViewContainer(
                TreeViewSolutionExplorerStateKey,
                rootNode,
                new TreeViewNoType[] { rootNode }.ToImmutableList()));
        }
        else
        {
            _treeViewService.SetRoot(TreeViewSolutionExplorerStateKey, rootNode);
            _treeViewService.SetActiveNode(TreeViewSolutionExplorerStateKey, rootNode);
        }

        if (dotNetSolutionModel is null)
            return;

        Dispatcher.Dispatch(ConstructModelReplacement(
            dotNetSolutionModel.Key,
            dotNetSolutionModel));
    }
}