using Fluxor;
using Luthetus.Ide.RazorLib.Gits.Models;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.Gits.States;

public partial record GitState
{
    public class Reducer
    {
        [ReducerMethod]
        public static GitState ReduceSetGitFileListAction(
            GitState inState,
            SetGitFileListAction setGitFileListAction)
        {
            if (inState.GitFolderAbsolutePath != setGitFileListAction.ExpectedGitFolderAbsolutePath)
            {
                // Git folder was changed while the text was being parsed,
                // throw away the result since it is thereby invalid.
                return inState;
            }

            return inState with
            {
                GitFileList = setGitFileListAction.GitFileList
            };
        }
        
        [ReducerMethod]
        public static GitState ReduceSetGitFolderAction(
            GitState inState,
            SetGitFolderAction setGitFolderAction)
        {
            return inState with
            {
                GitFolderAbsolutePath = setGitFolderAction.GitFolderAbsolutePath,
                GitFileList = ImmutableList<GitFile>.Empty,
                SelectedGitFileMap = ImmutableDictionary<string, GitFile>.Empty,
                ActiveGitTasks = ImmutableList<GitTask>.Empty,
            };
        }

        [ReducerMethod]
        public static GitState ReduceSetGitStateWithAction(
            GitState inState,
            SetGitStateWithAction setGitStateWithAction)
        {
            return setGitStateWithAction.GitStateWithFunc.Invoke(inState);
        }
    }
}