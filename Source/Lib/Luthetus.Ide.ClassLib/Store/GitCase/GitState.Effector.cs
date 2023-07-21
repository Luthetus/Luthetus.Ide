using Luthetus.Ide.ClassLib.FileSystem.Classes.FilePath;
using Luthetus.Ide.ClassLib.Store.TerminalCase;
using Luthetus.Ide.ClassLib.CommandLine;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;
using Luthetus.Ide.ClassLib.Git;

namespace Luthetus.Ide.ClassLib.Store.GitCase;

public partial record GitState
{
    private class Effector
    {
        private readonly IState<TerminalSessionsState> _terminalSessionsStateWrap;
        private readonly IState<GitState> _gitStateWrap;
        private readonly IFileSystemProvider _fileSystemProvider;
        private readonly IEnvironmentProvider _environmentProvider;

        // Usage:
        /*
           try
           {
               await _handleActionSemaphoreSlim.WaitAsync(
                   refreshGitAction.CancellationToken);

                // Code 
           }
           finally
           {
               _handleActionSemaphoreSlim.Release();
           }
         */
        private readonly SemaphoreSlim _handleActionSemaphoreSlim = new(1, 1);

        public Effector(
            IState<TerminalSessionsState> terminalSessionsStateWrap,
            IState<GitState> gitStateWrap,
            IFileSystemProvider fileSystemProvider,
            IEnvironmentProvider environmentProvider)
        {
            _terminalSessionsStateWrap = terminalSessionsStateWrap;
            _gitStateWrap = gitStateWrap;
            _fileSystemProvider = fileSystemProvider;
            _environmentProvider = environmentProvider;
        }

        [EffectMethod]
        public async Task HandleRefreshGitAction(
            RefreshGitAction refreshGitAction,
            IDispatcher dispatcher)
        {
            var handleRefreshGitTask = new GitTask(
                Guid.NewGuid(),
                nameof(HandleRefreshGitAction),
                refreshGitAction,
                refreshGitAction.CancellationToken);

            if (refreshGitAction.CancellationToken.IsCancellationRequested)
                return;

            try
            {
                await _handleActionSemaphoreSlim.WaitAsync();

                if (refreshGitAction.CancellationToken.IsCancellationRequested)
                    return;

                var gitState = _gitStateWrap.Value;

                dispatcher.Dispatch(new SetGitStateWithAction(withGitState =>
                {
                    var nextActiveGitTasks = withGitState.ActiveGitTasks.Add(
                        handleRefreshGitTask);

                    return withGitState with
                    {
                        ActiveGitTasks = nextActiveGitTasks,
                    };
                }));

                // Do not combine this following Dispatch for GitFilesList replacement
                // with the Dispatch for ActiveGitTasks replacement.
                // It could cause confusion in the future when one gets removed
                // without realizing the other was also part of the Dispatch replacement.
                dispatcher.Dispatch(new SetGitStateWithAction(withGitState => withGitState with
                {
                    GitFilesList = ImmutableList<GitFile>.Empty
                }));

                if (gitState.GitFolderAbsoluteFilePath is null ||
                    !await _fileSystemProvider.Directory.ExistsAsync(gitState.GitFolderAbsoluteFilePath.GetAbsoluteFilePathString()) ||
                    gitState.GitFolderAbsoluteFilePath.ParentDirectory is null)
                {
                    return;
                }

                var generalTerminalSession = _terminalSessionsStateWrap.Value.TerminalSessionMap[
                    TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY];

                var gitStatusCommand = new TerminalCommand(
                    GitFacts.GitStatusTerminalCommandKey,
                    GitCliFacts.TARGET_FILE_NAME,
                    new[] { GitCliFacts.STATUS_COMMAND },
                    gitState.GitFolderAbsoluteFilePath.ParentDirectory.GetAbsoluteFilePathString(),
                    CancellationToken.None,
                    async () =>
                    {
                        var gitStatusOutput = generalTerminalSession.ReadStandardOut(
                            GitFacts.GitStatusTerminalCommandKey);

                        if (gitStatusOutput is null)
                            return;

                        await GetGitOutputSectionAsync(
                            gitState,
                            gitStatusOutput,
                            GitFacts.UNTRACKED_FILES_TEXT_START,
                            GitDirtyReason.Untracked,
                            UntrackedFilesOnAfterCompletedAction);

                        await GetGitOutputSectionAsync(
                            gitState,
                            gitStatusOutput,
                            GitFacts.CHANGES_NOT_STAGED_FOR_COMMIT_TEXT_START,
                            null,
                            ChangesNotStagedOnAfterCompletedAction);
                    });

                await generalTerminalSession
                    .EnqueueCommandAsync(gitStatusCommand);

                void UntrackedFilesOnAfterCompletedAction(ImmutableList<GitFile> gitFiles)
                {
                    dispatcher.Dispatch(new SetGitStateWithAction(withGitState =>
                    {
                        var nextGitFilesList = withGitState.GitFilesList.AddRange(gitFiles);

                        return withGitState with { GitFilesList = nextGitFilesList };
                    }));
                }

                void ChangesNotStagedOnAfterCompletedAction(ImmutableList<GitFile> gitFiles)
                {
                    dispatcher.Dispatch(new SetGitStateWithAction(withGitState =>
                    {
                        var nextGitFilesList = withGitState.GitFilesList.AddRange(gitFiles);

                        return withGitState with { GitFilesList = nextGitFilesList };
                    }));
                }
            }
            finally
            {
                dispatcher.Dispatch(new SetGitStateWithAction(withGitState =>
                {
                    var nextActiveGitTasks = withGitState.ActiveGitTasks.Remove(
                        handleRefreshGitTask);

                    return withGitState with
                    {
                        ActiveGitTasks = nextActiveGitTasks,
                    };
                }));

                _handleActionSemaphoreSlim.Release();
            }
        }

        [EffectMethod]
        public async Task HandleGitInitAction(
            GitInitAction gitInitAction,
            IDispatcher dispatcher)
        {
            var handleHandleGitInitAction = new GitTask(
                Guid.NewGuid(),
                nameof(HandleRefreshGitAction),
                gitInitAction,
                gitInitAction.CancellationToken);

            if (gitInitAction.CancellationToken.IsCancellationRequested)
                return;

            try
            {
                await _handleActionSemaphoreSlim.WaitAsync();

                if (gitInitAction.CancellationToken.IsCancellationRequested)
                    return;

                dispatcher.Dispatch(new SetGitStateWithAction(withGitState =>
                {
                    var nextActiveGitTasks = withGitState.ActiveGitTasks.Add(
                        handleHandleGitInitAction);

                    return withGitState with
                    {
                        ActiveGitTasks = nextActiveGitTasks,
                    };
                }));

                var gitState = _gitStateWrap.Value;

                if (gitState.MostRecentTryFindGitFolderInDirectoryAction is null)
                    return;

                var gitInitCommand = new TerminalCommand(
                    GitFacts.GitInitTerminalCommandKey,
                    GitCliFacts.TARGET_FILE_NAME,
                    new[] { GitCliFacts.INIT_COMMAND },
                    gitState.MostRecentTryFindGitFolderInDirectoryAction.DirectoryAbsoluteFilePath.GetAbsoluteFilePathString(),
                    CancellationToken.None,
                    () =>
                    {
                        dispatcher.Dispatch(
                            new GitState.RefreshGitAction(
                                gitInitAction.CancellationToken));

                        return Task.CompletedTask;
                    });

                var generalTerminalSession = _terminalSessionsStateWrap.Value.TerminalSessionMap[
                    TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY];

                await generalTerminalSession
                    .EnqueueCommandAsync(gitInitCommand);
            }
            finally
            {
                dispatcher.Dispatch(new SetGitStateWithAction(withGitState =>
                {
                    var nextActiveGitTasks = withGitState.ActiveGitTasks.Remove(
                        handleHandleGitInitAction);

                    return withGitState with
                    {
                        ActiveGitTasks = nextActiveGitTasks,
                    };
                }));

                _handleActionSemaphoreSlim.Release();
            }
        }

        [EffectMethod]
        public async Task ReduceTryFindGitFolderInDirectoryAction(
            TryFindGitFolderInDirectoryAction tryFindGitFolderInDirectoryAction,
            IDispatcher dispatcher)
        {
            var handleTryFindGitFolderInDirectoryAction = new GitTask(
                Guid.NewGuid(),
                nameof(HandleRefreshGitAction),
                tryFindGitFolderInDirectoryAction,
                tryFindGitFolderInDirectoryAction.CancellationToken);

            if (tryFindGitFolderInDirectoryAction.CancellationToken.IsCancellationRequested)
                return;

            try
            {
                await _handleActionSemaphoreSlim.WaitAsync();

                if (tryFindGitFolderInDirectoryAction.CancellationToken.IsCancellationRequested)
                    return;

                dispatcher.Dispatch(new SetGitStateWithAction(withGitState =>
                {
                    var nextActiveGitTasks = withGitState.ActiveGitTasks.Add(
                        handleTryFindGitFolderInDirectoryAction);

                    return withGitState with
                    {
                        ActiveGitTasks = nextActiveGitTasks,
                    };
                }));

                if (!tryFindGitFolderInDirectoryAction.DirectoryAbsoluteFilePath.IsDirectory)
                    return;

                var directoryAbsoluteFilePathString =
                    tryFindGitFolderInDirectoryAction.DirectoryAbsoluteFilePath
                        .GetAbsoluteFilePathString();

                var childDirectoryAbsoluteFilePathStrings = await _fileSystemProvider.Directory
                    .GetDirectoriesAsync(
                        directoryAbsoluteFilePathString);

                var gitFolderAbsoluteFilePathString = childDirectoryAbsoluteFilePathStrings.FirstOrDefault(
                    x => x.EndsWith(GitFacts.GIT_FOLDER_NAME));

                if (gitFolderAbsoluteFilePathString is not null)
                {
                    var gitFolderAbsoluteFilePath = new AbsoluteFilePath(
                        gitFolderAbsoluteFilePathString,
                        true,
                        _environmentProvider);

                    dispatcher.Dispatch(
                        new SetGitStateWithAction(withGitState => withGitState with
                        {
                            GitFolderAbsoluteFilePath = gitFolderAbsoluteFilePath,
                            MostRecentTryFindGitFolderInDirectoryAction = tryFindGitFolderInDirectoryAction
                        }));

                    dispatcher.Dispatch(
                        new GitState.RefreshGitAction(
                            tryFindGitFolderInDirectoryAction.CancellationToken));
                }
                else
                {
                    dispatcher.Dispatch(
                        new SetGitStateWithAction(withGitState => withGitState with
                        {
                            GitFolderAbsoluteFilePath = null,
                            MostRecentTryFindGitFolderInDirectoryAction = tryFindGitFolderInDirectoryAction
                        }));
                }
            }
            finally
            {
                dispatcher.Dispatch(new SetGitStateWithAction(withGitState =>
                {
                    var nextActiveGitTasks = withGitState.ActiveGitTasks.Remove(
                        handleTryFindGitFolderInDirectoryAction);

                    return withGitState with
                    {
                        ActiveGitTasks = nextActiveGitTasks,
                    };
                }));

                _handleActionSemaphoreSlim.Release();
            }
        }

        private async Task GetGitOutputSectionAsync(
            GitState gitState,
            string gitStatusOutput,
            string sectionStart,
            GitDirtyReason? gitDirtyReason,
            Action<ImmutableList<GitFile>> onAfterCompletedAction)
        {
            if (gitState.GitFolderAbsoluteFilePath?.ParentDirectory is null)
                return;

            var indexOfChangesNotStagedForCommitTextStart = gitStatusOutput.IndexOf(
                sectionStart,
                StringComparison.InvariantCulture);

            if (indexOfChangesNotStagedForCommitTextStart != -1)
            {
                var startOfChangesNotStagedForCommitIndex = indexOfChangesNotStagedForCommitTextStart +
                                                            sectionStart.Length;

                var gitStatusOutputReader = new StringReader(
                    gitStatusOutput.Substring(startOfChangesNotStagedForCommitIndex));

                var changesNotStagedForCommitBuilder = new StringBuilder();

                // This skips the second newline when seeing: "\n\n"
                string? currentLine = await gitStatusOutputReader.ReadLineAsync();

                while ((currentLine = await gitStatusOutputReader.ReadLineAsync()) is not null &&
                       currentLine.Length > 0)
                {
                    // It is presumed that git CLI provides comments on lines
                    // which start with two space characters.
                    //
                    // Whereas output for this command starts with a tab.
                    //
                    // TODO: I imagine this is a very naive presumption and this should be revisited but I am still feeling out how to write this git logic.

                    /*
                     * Changes not staged for commit:
                     *   (use "git add/rm <file>..." to update what will be committed)
                     *   (use "git restore <file>..." to discard changes in working directory)
                     *       modified:   BlazorCrudApp.ServerSide/Pages/Counter.razor
                     *       deleted:    BlazorCrudApp.ServerSide/Shared/SurveyPrompt.razor
                     */
                    if (currentLine.StartsWith(new string(' ', 2)))
                        continue;

                    changesNotStagedForCommitBuilder.Append(currentLine);
                }

                var changesNotStagedForCommitText = changesNotStagedForCommitBuilder.ToString();

                var changesNotStagedForCommitCollection = changesNotStagedForCommitText
                    .Split('\t')
                    .Select(x => x.Trim())
                    .OrderBy(x => x)
                    .ToArray();

                if (changesNotStagedForCommitCollection.First() == string.Empty)
                {
                    changesNotStagedForCommitCollection = changesNotStagedForCommitCollection
                        .Skip(1)
                        .ToArray();
                }

                (string relativePath, GitDirtyReason gitDirtyReason)[] changesNotStagedForCommitRelativePathAndGitDirtyReasonTuples;

                if (gitDirtyReason is not null)
                {
                    changesNotStagedForCommitRelativePathAndGitDirtyReasonTuples = changesNotStagedForCommitCollection
                        .Select(x => (x, gitDirtyReason.Value))
                        .ToArray();
                }
                else
                {
                    changesNotStagedForCommitRelativePathAndGitDirtyReasonTuples = changesNotStagedForCommitCollection
                        .Select(x =>
                        {
                            var relativePath = x;
                            GitDirtyReason innerGitDirtyReason = GitDirtyReason.None;

                            if (x.StartsWith(GitFacts.GIT_DIRTY_REASON_MODIFIED))
                            {
                                innerGitDirtyReason = GitDirtyReason.Modified;

                                relativePath = new string(relativePath
                                    .Skip(GitFacts.GIT_DIRTY_REASON_MODIFIED.Length)
                                    .ToArray());
                            }
                            else if (x.StartsWith(GitFacts.GIT_DIRTY_REASON_DELETED))
                            {
                                innerGitDirtyReason = GitDirtyReason.Deleted;

                                relativePath = new string(relativePath
                                    .Skip(GitFacts.GIT_DIRTY_REASON_DELETED.Length)
                                    .ToArray());
                            }

                            return (relativePath, innerGitDirtyReason);
                        })
                        .ToArray();
                }

                var changesNotStagedForCommitGitFiles = changesNotStagedForCommitRelativePathAndGitDirtyReasonTuples
                    .Select(x =>
                    {
                        var absoluteFilePathString =
                            gitState.GitFolderAbsoluteFilePath.ParentDirectory.GetAbsoluteFilePathString() +
                            x.relativePath;

                        var isDirectory = x.relativePath.EndsWith(_environmentProvider.DirectorySeparatorChar) ||
                                          x.relativePath.EndsWith(_environmentProvider.AltDirectorySeparatorChar);

                        var absoluteFilePath = new AbsoluteFilePath(
                            absoluteFilePathString,
                            isDirectory,
                            _environmentProvider);

                        return new GitFile(absoluteFilePath, x.gitDirtyReason);
                    })
                    .ToImmutableList();

                onAfterCompletedAction.Invoke(changesNotStagedForCommitGitFiles);
            }
        }
    }
}