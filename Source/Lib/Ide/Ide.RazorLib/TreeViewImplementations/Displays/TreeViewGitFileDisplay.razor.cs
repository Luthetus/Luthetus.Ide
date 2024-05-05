using Fluxor;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.Gits.Models;
using Luthetus.Ide.RazorLib.Gits.States;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Models;
using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.TreeViewImplementations.Displays;

public partial class TreeViewGitFileDisplay : ComponentBase, ITreeViewGitFileRendererType
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [CascadingParameter]
    public GitState GitState { get; set; } = null!;

    [Parameter, EditorRequired]
    public TreeViewGitFile TreeViewGitFile { get; set; } = null!;

    private bool IsChecked
    {
        get => GitState.StagedFileMap.ContainsKey(TreeViewGitFile.Item.AbsolutePath.Value);
        set
        {
            var localGitState = GitState;

            Dispatcher.Dispatch(new GitState.WithAction(inState =>
            {
                if (inState.Repo != localGitState.Repo)
                {
                    // Git folder was changed, throw away the result since it is thereby invalid.
                    return inState;
                }

                var key = TreeViewGitFile.Item.AbsolutePath.Value;
                ImmutableDictionary<string, GitFile> outStagedFileMap;

                if (inState.StagedFileMap.ContainsKey(key))
                    outStagedFileMap = inState.StagedFileMap.Remove(key);
                else
                    outStagedFileMap = inState.StagedFileMap.Add(key, TreeViewGitFile.Item);

                return inState with
                {
                    StagedFileMap = outStagedFileMap
                };
            }));
        }
    }
}