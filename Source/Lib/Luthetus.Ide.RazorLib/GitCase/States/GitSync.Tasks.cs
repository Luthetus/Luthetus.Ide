using System.Text;
using System.Collections.Immutable;
using Luthetus.Ide.RazorLib.CommandLineCase.Models;
using Luthetus.Ide.RazorLib.GitCase.Models;
using Luthetus.Ide.RazorLib.TerminalCase.Models;
using Luthetus.Common.RazorLib.FileSystem.Models;
using static Luthetus.Ide.RazorLib.GitCase.States.GitState;

namespace Luthetus.Ide.RazorLib.GitCase.States;

public partial class GitSync
{
    public async Task RefreshGit(RefreshGitTask refreshGitAction)
    {
        var handleRefreshGitTask = new GitTask(
            Guid.NewGuid(),
            nameof(RefreshGit),
            refreshGitAction,
            refreshGitAction.CancellationToken);

        if (refreshGitAction.CancellationToken.IsCancellationRequested)
            return;

        if (refreshGitAction.CancellationToken.IsCancellationRequested)
            return;

        var gitState = _gitStateWrap.Value;

        Dispatcher.Dispatch(new SetGitStateWithAction(withGitState =>
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
        Dispatcher.Dispatch(new SetGitStateWithAction(withGitState => withGitState with
        {
            GitFilesList = ImmutableList<GitFile>.Empty
        }));

        if (gitState.GitFolderAbsolutePath is null ||
            !await _fileSystemProvider.Directory.ExistsAsync(gitState.GitFolderAbsolutePath.FormattedInput) ||
            gitState.GitFolderAbsolutePath.ParentDirectory is null)
        {
            return;
        }

        var generalTerminalSession = _terminalSessionStateWrap.Value.TerminalSessionMap[
            TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY];

        var formattedCommand = new FormattedCommand(
            GitCliFacts.TARGET_FILE_NAME,
            new[]
            {
                    GitCliFacts.STATUS_COMMAND
            });

        var gitStatusCommand = new TerminalCommand(
            GitFacts.GitStatusTerminalCommandKey,
            formattedCommand,
            gitState.GitFolderAbsolutePath.ParentDirectory.FormattedInput,
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

        await generalTerminalSession.EnqueueCommandAsync(gitStatusCommand);

        Dispatcher.Dispatch(new SetGitStateWithAction(withGitState =>
        {
            var nextActiveGitTasks = withGitState.ActiveGitTasks.Remove(
                handleRefreshGitTask);

            return withGitState with
            {
                ActiveGitTasks = nextActiveGitTasks,
            };
        }));

        void UntrackedFilesOnAfterCompletedAction(ImmutableList<GitFile> gitFiles)
        {
            Dispatcher.Dispatch(new SetGitStateWithAction(withGitState =>
            {
                var nextGitFilesList = withGitState.GitFilesList.AddRange(gitFiles);

                return withGitState with { GitFilesList = nextGitFilesList };
            }));
        }

        void ChangesNotStagedOnAfterCompletedAction(ImmutableList<GitFile> gitFiles)
        {
            Dispatcher.Dispatch(new SetGitStateWithAction(withGitState =>
            {
                var nextGitFilesList = withGitState.GitFilesList.AddRange(gitFiles);

                return withGitState with { GitFilesList = nextGitFilesList };
            }));
        }
    }

    public async Task GitInit(GitInitTask gitInitAction)
    {
        var handleHandleGitInitAction = new GitTask(
            Guid.NewGuid(),
            nameof(RefreshGit),
            gitInitAction,
            gitInitAction.CancellationToken);

        if (gitInitAction.CancellationToken.IsCancellationRequested)
            return;

        
        if (gitInitAction.CancellationToken.IsCancellationRequested)
            return;

        Dispatcher.Dispatch(new SetGitStateWithAction(withGitState =>
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

        var formattedCommand = new FormattedCommand(
            GitCliFacts.TARGET_FILE_NAME,
            new[]
            {
                    GitCliFacts.INIT_COMMAND
            });

        var gitInitCommand = new TerminalCommand(
            GitFacts.GitInitTerminalCommandKey,
            formattedCommand,
            gitState.MostRecentTryFindGitFolderInDirectoryAction.DirectoryAbsolutePath.FormattedInput,
            CancellationToken.None,
            () =>
            {
                Dispatcher.Dispatch(new RefreshGitTask(
                    this,
                    gitInitAction.CancellationToken));

                return Task.CompletedTask;
            });

        var generalTerminalSession = _terminalSessionStateWrap.Value.TerminalSessionMap[
            TerminalSessionFacts.GENERAL_TERMINAL_SESSION_KEY];

        await generalTerminalSession.EnqueueCommandAsync(gitInitCommand);

        Dispatcher.Dispatch(new SetGitStateWithAction(withGitState =>
        {
            var nextActiveGitTasks = withGitState.ActiveGitTasks.Remove(
                handleHandleGitInitAction);

            return withGitState with
            {
                ActiveGitTasks = nextActiveGitTasks,
            };
        }));
    }

    public async Task TryFindGitFolderInDirectory(TryFindGitFolderInDirectoryTask tryFindGitFolderInDirectoryAction)
    {
        var handleTryFindGitFolderInDirectoryAction = new GitTask(
            Guid.NewGuid(),
            nameof(RefreshGit),
            tryFindGitFolderInDirectoryAction,
            tryFindGitFolderInDirectoryAction.CancellationToken);

        if (tryFindGitFolderInDirectoryAction.CancellationToken.IsCancellationRequested)
            return;

        if (tryFindGitFolderInDirectoryAction.CancellationToken.IsCancellationRequested)
            return;

        Dispatcher.Dispatch(new SetGitStateWithAction(withGitState =>
        {
            var nextActiveGitTasks = withGitState.ActiveGitTasks.Add(
                handleTryFindGitFolderInDirectoryAction);

            return withGitState with
            {
                ActiveGitTasks = nextActiveGitTasks,
            };
        }));

        if (!tryFindGitFolderInDirectoryAction.DirectoryAbsolutePath.IsDirectory)
            return;

        var directoryAbsolutePathString =
            tryFindGitFolderInDirectoryAction.DirectoryAbsolutePath
                .FormattedInput;

        var childDirectoryAbsolutePathStrings = await _fileSystemProvider.Directory
            .GetDirectoriesAsync(
                directoryAbsolutePathString);

        var gitFolderAbsolutePathString = childDirectoryAbsolutePathStrings.FirstOrDefault(
            x => x.EndsWith(GitFacts.GIT_FOLDER_NAME));

        if (gitFolderAbsolutePathString is not null)
        {
            var gitFolderAbsolutePath = new AbsolutePath(
                gitFolderAbsolutePathString,
                true,
                _environmentProvider);

            Dispatcher.Dispatch(new SetGitStateWithAction(
                withGitState => withGitState with
                {
                    GitFolderAbsolutePath = gitFolderAbsolutePath,
                    MostRecentTryFindGitFolderInDirectoryAction = tryFindGitFolderInDirectoryAction
                }));

            Dispatcher.Dispatch(new RefreshGitTask(
                this,
                tryFindGitFolderInDirectoryAction.CancellationToken));
        }
        else
        {
            Dispatcher.Dispatch(new SetGitStateWithAction(
                withGitState => withGitState with
                {
                    GitFolderAbsolutePath = null,
                    MostRecentTryFindGitFolderInDirectoryAction = tryFindGitFolderInDirectoryAction
                }));
        }
        Dispatcher.Dispatch(new SetGitStateWithAction(
            withGitState =>
            {
                var nextActiveGitTasks = withGitState.ActiveGitTasks.Remove(
                    handleTryFindGitFolderInDirectoryAction);

                return withGitState with
                {
                    ActiveGitTasks = nextActiveGitTasks,
                };
            }));
    }

    private async Task GetGitOutputSectionAsync(
        GitState gitState,
        string gitStatusOutput,
        string sectionStart,
        GitDirtyReason? gitDirtyReason,
        Action<ImmutableList<GitFile>> onAfterCompletedAction)
    {
        if (gitState.GitFolderAbsolutePath?.ParentDirectory is null)
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
                    var absolutePathString =
                        gitState.GitFolderAbsolutePath.ParentDirectory.FormattedInput +
                        x.relativePath;

                    var isDirectory = _environmentProvider.IsDirectorySeparator(x.relativePath.LastOrDefault());

                    var absolutePath = new AbsolutePath(
                        absolutePathString,
                        isDirectory,
                        _environmentProvider);

                    return new GitFile(absolutePath, x.gitDirtyReason);
                })
                .ToImmutableList();

            onAfterCompletedAction.Invoke(changesNotStagedForCommitGitFiles);
        }
    }
}
