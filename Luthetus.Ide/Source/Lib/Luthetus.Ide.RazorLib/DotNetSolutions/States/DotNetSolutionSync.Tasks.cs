using System.Collections.Immutable;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Namespaces.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.FindAlls.States;
using Luthetus.CompilerServices.Lang.DotNetSolution.SyntaxActors;
using Luthetus.CompilerServices.Lang.DotNetSolution.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.Models.Project;
using Luthetus.Ide.RazorLib.Websites.ProjectTemplates.Models;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Models;
using Luthetus.Ide.RazorLib.CodeSearches.States;
using static Luthetus.Ide.RazorLib.DotNetSolutions.States.DotNetSolutionState;
using Luthetus.Ide.RazorLib.CommandLines.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;

namespace Luthetus.Ide.RazorLib.DotNetSolutions.States;

public partial class DotNetSolutionSync
{
    private async Task Website_AddExistingProjectToSolutionAsync(
        Key<DotNetSolutionModel> dotNetSolutionModelKey,
        string projectTemplateShortName,
        string cSharpProjectName,
        IAbsolutePath cSharpProjectAbsolutePath,
        IEnvironmentProvider environmentProvider)
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

        await _fileSystemProvider.File.WriteAllTextAsync(
            outDotNetSolutionModel.NamespacePath.AbsolutePath.Value,
            outDotNetSolutionModel.SolutionFileContents);

        var solutionTextEditorModel = _textEditorService.ModelApi.GetOrDefault(
            new ResourceUri(inDotNetSolutionModel.NamespacePath.AbsolutePath.Value));

        if (solutionTextEditorModel is not null)
        {
            _textEditorService.Post(
                nameof(Website_AddExistingProjectToSolutionAsync),
                _textEditorService.ModelApi.ReloadFactory(
                    solutionTextEditorModel.ResourceUri,
                    outDotNetSolutionModel.SolutionFileContents,
                    DateTime.UtcNow));
        }

        // TODO: Putting a hack for now to overwrite if somehow model was registered already
        Dispatcher.Dispatch(ConstructModelReplacement(
            outDotNetSolutionModel.Key,
            outDotNetSolutionModel));

        await SetDotNetSolutionTreeViewAsync(outDotNetSolutionModel.Key);
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

        var parentDirectory = solutionAbsolutePath.ParentDirectory;

        if (parentDirectory is not null)
		{
            _environmentProvider.DeletionPermittedRegister(new(parentDirectory.Value, true));

			Dispatcher.Dispatch(new TextEditorFindAllState.SetStartingDirectoryPathAction(
				parentDirectory.Value));

			Dispatcher.Dispatch(new CodeSearchState.WithAction(inState => inState with
			{
				StartingAbsolutePathForSearch = parentDirectory.Value
			}));

            // Set 'generalTerminalSession' working directory
            {
                var generalTerminalSession = _terminalSessionStateWrap.Value.TerminalSessionMap[TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY];
                var formattedCommand = TerminalCommandFormatter.FormatChangeDirectory(parentDirectory.Value);

                var changeDirectoryCommand = new TerminalCommand(
                    Key<TerminalCommand>.NewKey(),
                    formattedCommand,
                    parentDirectory.Value,
                    CancellationToken.None);

                await generalTerminalSession.EnqueueCommandAsync(changeDirectoryCommand);
            }

            // Set 'executionTerminalSession' working directory
            {
                var executionTerminalSession = _terminalSessionStateWrap.Value.TerminalSessionMap[TerminalSessionFacts.EXECUTION_TERMINAL_SESSION_KEY];
                var formattedCommand = TerminalCommandFormatter.FormatChangeDirectory(parentDirectory.Value);

                var changeDirectoryCommand = new TerminalCommand(
                    Key<TerminalCommand>.NewKey(),
                    formattedCommand,
                    parentDirectory.Value,
                    CancellationToken.None);

                await executionTerminalSession.EnqueueCommandAsync(changeDirectoryCommand);
            }
        }

        await SetDotNetSolutionTreeViewAsync(dotNetSolutionModel.Key);
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

            _treeViewService.SetActiveNode(
				TreeViewSolutionExplorerStateKey,
				rootNode,
				true,
				false);
        }

        if (dotNetSolutionModel is null)
            return;

        Dispatcher.Dispatch(ConstructModelReplacement(
            dotNetSolutionModel.Key,
            dotNetSolutionModel));
    }
}