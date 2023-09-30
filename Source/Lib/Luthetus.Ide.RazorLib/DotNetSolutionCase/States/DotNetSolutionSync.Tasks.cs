using System.Collections.Immutable;
using Luthetus.CompilerServices.Lang.DotNetSolution;
using Luthetus.CompilerServices.Lang.DotNetSolution.CSharp;
using static Luthetus.Ide.RazorLib.DotNetSolutionCase.States.DotNetSolutionState;
using Luthetus.CompilerServices.Lang.DotNetSolution.RewriteForImmutability;
using Luthetus.Ide.RazorLib.WebsiteProjectTemplatesCase.Models;
using Luthetus.Ide.RazorLib.TreeViewImplementationsCase.Models;
using Luthetus.Common.RazorLib.FileSystem.Models;
using Luthetus.Common.RazorLib.Namespaces.Models;
using Luthetus.Common.RazorLib.TreeView.Models;
using Luthetus.TextEditor.RazorLib.TextEditorCase.States;
using Luthetus.TextEditor.RazorLib.Lexing.Models;
using Luthetus.Common.RazorLib.BackgroundTaskCase.Models;
using Luthetus.Common.RazorLib.KeyCase.Models;

namespace Luthetus.Ide.RazorLib.DotNetSolutionCase.States;

public partial class DotNetSolutionSync
{
    public void AddExistingProjectToSolutionAction(
        Key<DotNetSolutionModel> dotNetSolutionModelKey,
        string localProjectTemplateShortName,
        string localCSharpProjectName,
        IAbsolutePath cSharpProjectAbsolutePath,
        IEnvironmentProvider environmentProvider)
    {
        BackgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.Queue.Key,
            "AddExistingProjectToSolutionAction",
            async () => {
                var inDotNetSolutionModel = _dotNetSolutionStateWrap.Value.DotNetSolutions.FirstOrDefault(
                    x => x.DotNetSolutionModelKey == dotNetSolutionModelKey);

                if (inDotNetSolutionModel is null)
                    return;

                var projectTypeGuid = WebsiteProjectTemplateFacts.GetProjectTypeGuid(
                    localProjectTemplateShortName);

                var relativePathFromSlnToProject = PathHelper.GetRelativeFromTwoAbsolutes(
                    inDotNetSolutionModel.NamespacePath.AbsolutePath,
                    cSharpProjectAbsolutePath,
                    environmentProvider);

                var projectIdGuid = Guid.NewGuid();

                var cSharpProject = new CSharpProject(
                    localCSharpProjectName,
                    projectTypeGuid,
                    relativePathFromSlnToProject,
                    projectIdGuid);

                cSharpProject.SetAbsolutePath(cSharpProjectAbsolutePath);

                var dotNetSolutionBuilder = inDotNetSolutionModel.AddDotNetProject(
                    cSharpProject,
                    environmentProvider);

                var outDotNetSolutionModel = dotNetSolutionBuilder.Build();

                await _fileSystemProvider.File.WriteAllTextAsync(
                    inDotNetSolutionModel.NamespacePath.AbsolutePath.FormattedInput,
                    inDotNetSolutionModel.SolutionFileContents);

                var solutionTextEditorModel = _textEditorService.Model.FindOrDefaultByResourceUri(
                    new ResourceUri(inDotNetSolutionModel.NamespacePath.AbsolutePath.FormattedInput));

                if (solutionTextEditorModel is not null)
                {
                    Dispatcher.Dispatch(new TextEditorModelState.ReloadAction(
                        solutionTextEditorModel.ModelKey,
                        inDotNetSolutionModel.SolutionFileContents,
                        DateTime.UtcNow));
                }

                // TODO: Putting a hack for now to overwrite if somehow model was registered already
                Dispatcher.Dispatch(ConstructModelReplacement(
                    outDotNetSolutionModel.DotNetSolutionModelKey,
                    outDotNetSolutionModel));

                await SetDotNetSolutionTreeViewAsync(outDotNetSolutionModel.DotNetSolutionModelKey);
            });
    }

    public void SetDotNetSolution(IAbsolutePath inSolutionAbsolutePath)
    {
        BackgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.Queue.Key,
            "SetDotNetSolution",
            async () => {

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

                var dotNetSolution = DotNetSolutionLexer.Lex(
                    content,
                    solutionNamespacePath,
                    _environmentProvider);

                // TODO: If somehow model was registered already this won't write the state
                Dispatcher.Dispatch(new RegisterAction(dotNetSolution, this));

                Dispatcher.Dispatch(new WithAction(
                    inDotNetSolutionState => inDotNetSolutionState with
                    {
                        DotNetSolutionModelKey = dotNetSolution.DotNetSolutionModelKey
                    }));

                // TODO: Putting a hack for now to overwrite if somehow model was registered already
                Dispatcher.Dispatch(ConstructModelReplacement(
                    dotNetSolution.DotNetSolutionModelKey,
                    dotNetSolution));

                await SetDotNetSolutionTreeViewAsync(dotNetSolution.DotNetSolutionModelKey);
            });
    }

    public void SetDotNetSolutionTreeView(Key<DotNetSolutionModel> dotNetSolutionModelKey)
    {
        BackgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.Queue.Key,
            "SetDotNetSolutionTreeViewAsync",
            async () => await SetDotNetSolutionTreeViewAsync(dotNetSolutionModelKey));
    }

    private async Task SetDotNetSolutionTreeViewAsync(Key<DotNetSolutionModel> dotNetSolutionModelKey)
    {
        var dotNetSolutionState = _dotNetSolutionStateWrap.Value;

        var dotNetSolutionModel = dotNetSolutionState.DotNetSolutions.FirstOrDefault(
            x => x.DotNetSolutionModelKey == dotNetSolutionModelKey);

        if (dotNetSolutionModel is null)
            return;

        var rootNode = new TreeViewSolution(
            dotNetSolutionModel,
            _luthetusIdeComponentRenderers,
            _luthetusCommonComponentRenderers,
            _fileSystemProvider,
            _environmentProvider,
            true,
            true);

        await rootNode.LoadChildrenAsync();

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
            dotNetSolutionModel.DotNetSolutionModelKey,
            dotNetSolutionModel));
    }
}