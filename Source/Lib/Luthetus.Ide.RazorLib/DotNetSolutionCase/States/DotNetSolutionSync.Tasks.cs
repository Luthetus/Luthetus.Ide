using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Namespaces;
using Luthetus.CompilerServices.Lang.DotNetSolution;
using Luthetus.CompilerServices.Lang.DotNetSolution.CSharp;
using static Luthetus.Ide.RazorLib.DotNetSolutionCase.States.DotNetSolutionState;
using Luthetus.CompilerServices.Lang.DotNetSolution.RewriteForImmutability;
using Luthetus.TextEditor.RazorLib.Lexing;
using Luthetus.TextEditor.RazorLib.Store.Model;
using Luthetus.Ide.RazorLib.WebsiteProjectTemplatesCase.Models;
using Luthetus.Ide.RazorLib.TreeViewImplementationsCase.Models;
using Luthetus.Common.RazorLib.FileSystem.Models;
using Luthetus.Common.RazorLib.TreeView.Models.TreeViewClasses;

namespace Luthetus.Ide.RazorLib.DotNetSolutionCase.States;

public partial class DotNetSolutionSync
{
    public async Task<DotNetSolutionModel?> SetDotNetSolutionAsync(SetDotNetSolutionTask setDotNetSolutionAction)
    {
        var dotNetSolutionAbsolutePathString = setDotNetSolutionAction.SolutionAbsolutePath.FormattedInput;

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

        return await SetDotNetSolutionTreeViewAsync(new SetDotNetSolutionTreeViewTask(dotNetSolution.DotNetSolutionModelKey, this));
    }

    public async Task<DotNetSolutionModel?> SetDotNetSolutionTreeViewAsync(
        SetDotNetSolutionTreeViewTask setDotNetSolutionTreeViewAction)
    {
        var dotNetSolutionState = _dotNetSolutionStateWrap.Value;

        var dotNetSolutionModel = dotNetSolutionState.DotNetSolutions.FirstOrDefault(
            x => x.DotNetSolutionModelKey == setDotNetSolutionTreeViewAction.DotNetSolutionModelKey);

        if (dotNetSolutionModel is null)
            return null;

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
            _treeViewService.RegisterTreeViewState(new TreeViewState(
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

        return dotNetSolutionModel;
    }
    
    public async Task<DotNetSolutionModel?> AddExistingProjectToSolutionAsync(AddExistingProjectToSolutionTask addExistingProjectToSolutionTask)
    {
        var inDotNetSolutionModel = _dotNetSolutionStateWrap.Value.DotNetSolutions.FirstOrDefault(
            x => x.DotNetSolutionModelKey == addExistingProjectToSolutionTask.DotNetSolutionModelKey);

        if (inDotNetSolutionModel is null)
            return null;

        var projectTypeGuid = WebsiteProjectTemplateRegistry.GetProjectTypeGuid(
            addExistingProjectToSolutionTask.LocalProjectTemplateShortName);

        var relativePathFromSlnToProject = PathHelper.GetRelativeFromTwoAbsolutes(
            inDotNetSolutionModel.NamespacePath.AbsolutePath,
            addExistingProjectToSolutionTask.CSharpProjectAbsolutePath,
            addExistingProjectToSolutionTask.EnvironmentProvider);

        var projectIdGuid = Guid.NewGuid();

        var cSharpProject = new CSharpProject(
            addExistingProjectToSolutionTask.LocalCSharpProjectName,
            projectTypeGuid,
            relativePathFromSlnToProject,
            projectIdGuid);

        cSharpProject.SetAbsolutePath(addExistingProjectToSolutionTask.CSharpProjectAbsolutePath);

        var dotNetSolutionBuilder = inDotNetSolutionModel.AddDotNetProject(
            cSharpProject,
            addExistingProjectToSolutionTask.EnvironmentProvider);

        var outDotNetSolutionModel = dotNetSolutionBuilder.Build();

        await _fileSystemProvider.File.WriteAllTextAsync(
            inDotNetSolutionModel.NamespacePath.AbsolutePath.FormattedInput,
            inDotNetSolutionModel.SolutionFileContents);

        var solutionTextEditorModel = _textEditorService.Model.FindOrDefaultByResourceUri(
            new ResourceUri(inDotNetSolutionModel.NamespacePath.AbsolutePath.FormattedInput));

        if (solutionTextEditorModel is not null)
        {
            Dispatcher.Dispatch(new TextEditorModelRegistry.ReloadAction(
                solutionTextEditorModel.ModelKey,
                inDotNetSolutionModel.SolutionFileContents,
                DateTime.UtcNow));
        }

        // TODO: Putting a hack for now to overwrite if somehow model was registered already
        Dispatcher.Dispatch(ConstructModelReplacement(
            outDotNetSolutionModel.DotNetSolutionModelKey,
            outDotNetSolutionModel));
        
        return await SetDotNetSolutionTreeViewAsync(new SetDotNetSolutionTreeViewTask(outDotNetSolutionModel.DotNetSolutionModelKey, this));
    }
}