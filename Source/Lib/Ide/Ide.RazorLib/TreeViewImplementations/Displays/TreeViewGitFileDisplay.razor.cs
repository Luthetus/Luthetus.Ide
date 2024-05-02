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
        get => GitState.SelectedGitFileList.ContainsKey(TreeViewGitFile.Item.AbsolutePath.Value);
        set
        {
            var localGitState = GitState;

            Dispatcher.Dispatch(new GitState.SetGitStateWithAction(inState =>
            {
                if (inState.GitFolderAbsolutePath != localGitState.GitFolderAbsolutePath)
                {
                    // Git folder was changed, throw away the result since it is thereby invalid.
                    return inState;
                }

                var key = TreeViewGitFile.Item.AbsolutePath.Value;
                ImmutableDictionary<string, GitFile> outSelectedGitFileList;

                if (inState.SelectedGitFileList.ContainsKey(key))
                    outSelectedGitFileList = inState.SelectedGitFileList.Remove(key);
                else
                    outSelectedGitFileList = inState.SelectedGitFileList.Add(key, TreeViewGitFile.Item);

                return inState with
                {
                    SelectedGitFileList = outSelectedGitFileList
                };
            }));
        }
    }
}