using System.Collections.Immutable;
using Fluxor;
using Luthetus.Extensions.Git.Models;

namespace Luthetus.Extensions.Git.States;

public partial record GitState
{
    public class Reducer
    {
        [ReducerMethod]
        public static GitState ReduceSetStatusAction(
            GitState inState,
            SetStatusAction setStatusAction)
        {
            if (inState.Repo != setStatusAction.Repo)
            {
                // Git folder was changed while the text was being parsed,
                // throw away the result since it is thereby invalid.
                return inState;
            }

            return inState with
            {
                UntrackedFileList = setStatusAction.UntrackedFileList,
                StagedFileList = setStatusAction.StagedFileList,
                UnstagedFileList = setStatusAction.UnstagedFileList,
                BehindByCommitCount = setStatusAction.BehindByCommitCount,
                AheadByCommitCount = setStatusAction.AheadByCommitCount,
            };
        }

        [ReducerMethod]
        public static GitState ReduceSetGitOriginAction(
            GitState inState,
            SetOriginAction setOriginAction)
        {
            if (inState.Repo != setOriginAction.Repo)
            {
                // Git folder was changed while the text was being parsed,
                // throw away the result since it is thereby invalid.
                return inState;
            }

            return inState with
            {
                Origin = setOriginAction.Origin
            };
        }
        
        [ReducerMethod]
        public static GitState ReduceSetBranchAction(
            GitState inState,
            SetBranchAction setBranchAction)
        {
            if (inState.Repo != setBranchAction.Repo)
            {
                // Git folder was changed while the text was being parsed,
                // throw away the result since it is thereby invalid.
                return inState;
            }

            return inState with
            {
                Branch = setBranchAction.Branch
            };
        }
        
        [ReducerMethod]
        public static GitState ReduceSetBranchListAction(
            GitState inState,
            SetBranchListAction setBranchListAction)
        {
            if (inState.Repo != setBranchListAction.Repo)
            {
                // Git folder was changed while the text was being parsed,
                // throw away the result since it is thereby invalid.
                return inState;
            }

            return inState with
            {
                BranchList = setBranchListAction.BranchList.ToImmutableList()
            };
        }
        
        [ReducerMethod]
        public static GitState ReduceSetGitFolderAction(
            GitState inState,
            SetRepoAction setRepoAction)
        {
            return inState with
            {
                Repo = setRepoAction.Repo,
                UntrackedFileList = ImmutableList<GitFile>.Empty,
                StagedFileList = ImmutableList<GitFile>.Empty,
                UnstagedFileList = ImmutableList<GitFile>.Empty,
                SelectedFileList = ImmutableList<GitFile>.Empty,
                ActiveTasks = ImmutableList<GitTask>.Empty,
                Branch = null,
                Origin = null,
                AheadByCommitCount = null,
                BehindByCommitCount = null,
                BranchList = ImmutableList<string>.Empty,
                Upstream = null,
            };
        }

        [ReducerMethod]
        public static GitState ReduceSetGitStateWithAction(
            GitState inState,
            WithAction withAction)
        {
            return withAction.WithFunc.Invoke(inState);
        }
    }
}