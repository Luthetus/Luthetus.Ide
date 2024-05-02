using System.Text;
using System.Collections.Immutable;
using Luthetus.Ide.RazorLib.Gits.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.CommandLines.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using static Luthetus.Ide.RazorLib.Gits.States.GitState;

namespace Luthetus.Ide.RazorLib.Gits.States;

public partial class GitSync
{
    private async Task RefreshGitAsync(CancellationToken cancellationToken)
    {
        var handleRefreshGitTask = new GitTask(
            Guid.NewGuid(),
            nameof(RefreshGit),
            cancellationToken);

        if (cancellationToken.IsCancellationRequested)
            return;

        var gitState = _gitStateWrap.Value;

        Dispatcher.Dispatch(new SetGitStateWithAction(inGitState =>
        {
            var nextActiveGitTasks = inGitState.ActiveGitTasks.Add(handleRefreshGitTask);
            return inGitState with { ActiveGitTasks = nextActiveGitTasks };
        }));

        // Do not combine this following Dispatch for GitFilesList replacement
        // with the Dispatch for ActiveGitTasks replacement.
        // It could cause confusion in the future when one gets removed
        // without realizing the other was also part of the Dispatch replacement.
        Dispatcher.Dispatch(new SetGitStateWithAction(inGitState => inGitState with
        {
            GitFileList = ImmutableList<GitFile>.Empty
        }));

        if (gitState.GitFolderAbsolutePath is null ||
            !await _fileSystemProvider.Directory.ExistsAsync(gitState.GitFolderAbsolutePath.Value) ||
            gitState.GitFolderAbsolutePath.ParentDirectory is null)
        {
            return;
        }

        var generalTerminal = _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_TERMINAL_KEY];

        var formattedCommand = new FormattedCommand(GitCliFacts.TARGET_FILE_NAME, new[]
        {
            GitCliFacts.STATUS_COMMAND
        });

        var gitStatusCommand = new TerminalCommand(
            GitFacts.GitStatusTerminalCommandKey,
            formattedCommand,
            gitState.GitFolderAbsolutePath.ParentDirectory.Value,
            CancellationToken.None,
            async () =>
            {
				var success = generalTerminal.TryGetTerminalCommandTextSpan(
					GitFacts.GitStatusTerminalCommandKey,
					out var terminalCommandTextSpan);

				var gitStatusOutput = terminalCommandTextSpan.GetText();
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

        await generalTerminal.EnqueueCommandAsync(gitStatusCommand);

        Dispatcher.Dispatch(new SetGitStateWithAction(inGitState =>
        {
            var nextActiveGitTasks = inGitState.ActiveGitTasks.Remove(handleRefreshGitTask);
            return inGitState with { ActiveGitTasks = nextActiveGitTasks };
        }));

        void UntrackedFilesOnAfterCompletedAction(ImmutableList<GitFile> gitFiles)
        {
            Dispatcher.Dispatch(new SetGitStateWithAction(inGitState =>
            {
                var nextGitFilesList = inGitState.GitFileList.AddRange(gitFiles);
                return inGitState with { GitFileList = nextGitFilesList };
            }));
        }

        void ChangesNotStagedOnAfterCompletedAction(ImmutableList<GitFile> gitFiles)
        {
            Dispatcher.Dispatch(new SetGitStateWithAction(inGitState =>
            {
                var nextGitFilesList = inGitState.GitFileList.AddRange(gitFiles);
                return inGitState with { GitFileList = nextGitFilesList };
            }));
        }
    }

    private async Task GitInitAsync(CancellationToken cancellationToken)
    {
        var handleHandleGitInitAction = new GitTask(
            Guid.NewGuid(),
            nameof(RefreshGit),
            cancellationToken);

        if (cancellationToken.IsCancellationRequested)
            return;

        Dispatcher.Dispatch(new SetGitStateWithAction(inGitState =>
        {
            var nextActiveGitTasks = inGitState.ActiveGitTasks.Add(handleHandleGitInitAction);
            return inGitState with { ActiveGitTasks = nextActiveGitTasks };
        }));

        var gitState = _gitStateWrap.Value;

        if (gitState.GitFolderAbsolutePath is null)
            return;

        var formattedCommand = new FormattedCommand(GitCliFacts.TARGET_FILE_NAME, new[]
        {
            GitCliFacts.INIT_COMMAND
        });

        var gitInitCommand = new TerminalCommand(
            GitFacts.GitInitTerminalCommandKey,
            formattedCommand,
            gitState.GitFolderAbsolutePath.Value,
            CancellationToken.None,
            async () => await RefreshGitAsync(cancellationToken));

        var generalTerminal = _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_TERMINAL_KEY];
        await generalTerminal.EnqueueCommandAsync(gitInitCommand);

        Dispatcher.Dispatch(new SetGitStateWithAction(inGitState =>
        {
            var nextActiveGitTasks = inGitState.ActiveGitTasks.Remove(handleHandleGitInitAction);
            return inGitState with { ActiveGitTasks = nextActiveGitTasks };
        }));
    }

    private async Task TryFindGitFolderInDirectoryAsync(
        IAbsolutePath directoryAbsolutePath,
        CancellationToken cancellationToken)
    {
        var handleTryFindGitFolderInDirectoryAction = new GitTask(
            Guid.NewGuid(),
            nameof(RefreshGit),
            cancellationToken);

        if (cancellationToken.IsCancellationRequested)
            return;

        Dispatcher.Dispatch(new SetGitStateWithAction(inGitState =>
        {
            var nextActiveGitTasks = inGitState.ActiveGitTasks.Add(handleTryFindGitFolderInDirectoryAction);
            return inGitState with { ActiveGitTasks = nextActiveGitTasks };
        }));

        if (!directoryAbsolutePath.IsDirectory)
            return;

        var directoryAbsolutePathString = directoryAbsolutePath.Value;

        var childDirectoryAbsolutePathStrings = await _fileSystemProvider.Directory.GetDirectoriesAsync(
            directoryAbsolutePathString);

        var gitFolderAbsolutePathString = childDirectoryAbsolutePathStrings.FirstOrDefault(
            x => x.EndsWith(GitFacts.GIT_FOLDER_NAME));

        if (gitFolderAbsolutePathString is not null)
        {
            var gitFolderAbsolutePath = _environmentProvider.AbsolutePathFactory(
                gitFolderAbsolutePathString,
                true);

            Dispatcher.Dispatch(new SetGitStateWithAction(inGitState => inGitState with
            {
                GitFolderAbsolutePath = gitFolderAbsolutePath
            }));

            await RefreshGitAsync(cancellationToken);
        }
        else
        {
            Dispatcher.Dispatch(new SetGitStateWithAction(inGitState => inGitState with
            {
                GitFolderAbsolutePath = null,
            }));
        }
        Dispatcher.Dispatch(new SetGitStateWithAction(inGitState =>
        {
            var nextActiveGitTasks = inGitState.ActiveGitTasks.Remove(handleTryFindGitFolderInDirectoryAction);
            return inGitState with { ActiveGitTasks = nextActiveGitTasks };
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
                        gitState.GitFolderAbsolutePath.ParentDirectory +
                        x.relativePath;

                    var isDirectory = _environmentProvider.IsDirectorySeparator(x.relativePath.LastOrDefault());

                    var absolutePath = _environmentProvider.AbsolutePathFactory(
                        absolutePathString,
                        isDirectory);

                    return new GitFile(absolutePath, x.gitDirtyReason);
                })
                .ToImmutableList();

            onAfterCompletedAction.Invoke(changesNotStagedForCommitGitFiles);
        }
    }
}
