using System.Collections.Immutable;
using Luthetus.Common.RazorLib.TreeView;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using Luthetus.Ide.ClassLib.Store.GitCase;
using Luthetus.Ide.ClassLib.TreeViewImplementations;
using Luthetus.Ide.ClassLib.ComponentRenderers;
using Luthetus.Ide.ClassLib.ComponentRenderers.Types;
using Luthetus.Ide.ClassLib.Store.TerminalCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Store.NotificationCase;

namespace Luthetus.Ide.RazorLib.Git;

public partial class GitChangesDisplay : FluxorComponent, IGitDisplayRendererType
{
    [Inject]
    private IState<GitState> GitStateWrap { get; set; } = null!;
    [Inject]
    private IState<TerminalSessionsState> TerminalSessionsStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private ILuthetusIdeComponentRenderers LuthetusIdeComponentRenderers { get; set; } = null!;
    [Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;

    private static readonly TreeViewStateKey GitChangesTreeViewStateKey =
        TreeViewStateKey.NewTreeViewStateKey();

    private CancellationTokenSource _gitActionCancellationTokenSource = new();
    private bool _disposed;

    protected override void OnInitialized()
    {
        if (!TreeViewService.TryGetTreeViewState(
                GitChangesTreeViewStateKey,
                out _))
        {
            SetGitChangesTreeViewRoot();
        }

        base.OnInitialized();
    }

    private void GitInitOnClick()
    {
        Dispatcher.Dispatch(
            new GitState.GitInitAction(
                _gitActionCancellationTokenSource.Token));
    }

    private void RefreshGitOnClick()
    {
        Dispatcher.Dispatch(
            new GitState.RefreshGitAction(
                _gitActionCancellationTokenSource.Token));
    }

    private void ResetGitActionCancellationTokenSource()
    {
        _gitActionCancellationTokenSource.Cancel();
        _gitActionCancellationTokenSource = new();
    }

    private void SetGitChangesTreeViewRoot()
    {
        var gitState = GitStateWrap.Value;

        var treeViewNodes = gitState.GitFilesList
            .Select(x => (TreeViewNoType)new TreeViewGitFile(
                x,
                LuthetusIdeComponentRenderers,
                false,
                false))
            .ToArray();

        var adhocRootNode = TreeViewAdhoc.ConstructTreeViewAdhoc(
            treeViewNodes);

        foreach (var child in adhocRootNode.Children)
        {
            child.IsExpandable = false;
        }

        var activeNode = adhocRootNode.Children.FirstOrDefault();

        if (!TreeViewService.TryGetTreeViewState(
                GitChangesTreeViewStateKey,
                out var treeViewState))
        {
            TreeViewService.RegisterTreeViewState(new TreeViewState(
                GitChangesTreeViewStateKey,
                adhocRootNode,
                activeNode,
                ImmutableList<TreeViewNoType>.Empty));
        }
        else
        {
            TreeViewService.SetRoot(
                GitChangesTreeViewStateKey,
                adhocRootNode);

            TreeViewService.SetActiveNode(
                GitChangesTreeViewStateKey,
                activeNode);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _disposed = true;

            _gitActionCancellationTokenSource.Cancel();
        }

        base.Dispose(disposing);
    }
}