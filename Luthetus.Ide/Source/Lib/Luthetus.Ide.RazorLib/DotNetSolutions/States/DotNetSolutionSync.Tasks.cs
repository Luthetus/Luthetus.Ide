using System.Collections.Immutable;
using static Luthetus.Ide.RazorLib.DotNetSolutions.States.DotNetSolutionState;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Namespaces.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.WebsiteProjectTemplates.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.SyntaxActors;
using Luthetus.CompilerServices.Lang.DotNetSolution.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.Models.Project;

namespace Luthetus.Ide.RazorLib.DotNetSolutions.States;

public partial class DotNetSolutionSync
{
    private async Task AddExistingProjectToSolutionAsync(
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

        var cSharpProject = new CSharpProject(
            cSharpProjectName,
            projectTypeGuid,
            relativePathFromSlnToProject,
            projectIdGuid,
            null,
            null,
            cSharpProjectAbsolutePath);

        var outDotNetProjectBag = inDotNetSolutionModel.DotNetProjectBag.Add(cSharpProject);

        var outDotNetSolutionModel = inDotNetSolutionModel with
        {
            DotNetProjectBag = outDotNetProjectBag
        };

        // TODO: WriteAllTextAsync for the new .sln file
        //
        //await _fileSystemProvider.File.WriteAllTextAsync(
        //    outDotNetSolutionModel.NamespacePath.AbsolutePath.FormattedInput,
        //    outDotNetSolutionModel..SolutionFileContents);

        var solutionTextEditorModel = _textEditorService.Model.FindOrDefaultByResourceUri(
            new ResourceUri(inDotNetSolutionModel.NamespacePath.AbsolutePath.FormattedInput));

        if (solutionTextEditorModel is not null)
        {
            Dispatcher.Dispatch(new TextEditorModelState.ReloadAction(
                solutionTextEditorModel.ResourceUri,
                inDotNetSolutionModel.SolutionFileContents,
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
        var dotNetSolutionAbsolutePathString = inSolutionAbsolutePath.FormattedInput;

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
            new ResourceUri(solutionAbsolutePath.FormattedInput),
            content);

        lexer.Lex();

        var parser = new DotNetSolutionParser(lexer);

        var compilationUnit = parser.Parse();

        var dotNetSolutionModel = new DotNetSolutionModel(
            solutionAbsolutePath,
            parser.DotNetSolutionHeader,
            parser.DotNetProjectBag.ToImmutableArray(),
            ImmutableArray<SolutionFolder>.Empty, // TODO: Pass in these arguments
            ImmutableArray<NestedProjectEntry>.Empty, // TODO: Pass in these arguments
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

        if (!_treeViewService.TryGetTreeViewState(TreeViewSolutionExplorerStateKey, out _))
        {
            _treeViewService.RegisterTreeViewState(new TreeViewContainer(
                TreeViewSolutionExplorerStateKey,
                rootNode,
                rootNode,
                ImmutableList<TreeViewNoType>.Empty));
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