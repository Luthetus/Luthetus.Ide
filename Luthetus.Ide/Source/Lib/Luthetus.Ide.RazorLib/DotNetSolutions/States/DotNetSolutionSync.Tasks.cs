using System.Collections.Immutable;
using static Luthetus.Ide.RazorLib.DotNetSolutions.States.DotNetSolutionState;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Namespaces.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.SyntaxActors;
using Luthetus.CompilerServices.Lang.DotNetSolution.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.Models.Project;
using Luthetus.Ide.RazorLib.Websites.ProjectTemplates.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

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
        var inDotNetSolutionModel = _dotNetSolutionStateWrap.Value.DotNetSolutionsBag.FirstOrDefault(
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