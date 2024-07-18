using System.Collections.Immutable;
using System.Text;
using Fluxor;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Facts;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.Ide.RazorLib.Gits.States;
using Luthetus.Ide.RazorLib.Terminals.Models;

namespace Luthetus.Ide.RazorLib.Gits.Models;

public class GitCliOutputParser : IOutputParser
{
    private readonly IDispatcher _dispatcher;
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly IState<GitState> _gitStateWrap;

    public GitCliOutputParser(
        IDispatcher dispatcher,
        IState<GitState> gitStateWrap,
        IEnvironmentProvider environmentProvider)
    {
        _dispatcher = dispatcher;
		_gitStateWrap = gitStateWrap;
        _environmentProvider = environmentProvider;
    }

    private StageKind _stageKind = StageKind.None;
	private string? _origin;
    private string? _branch;
    private List<string> _branchList = new();
    private int _count;
    private int? _behindByCommitCount;
    private int? _aheadByCommitCount;
    private StringBuilder? _logFileContentBuilder;
	private GitRepo? _repo;

	public List<GitFile> UntrackedGitFileList { get; } = new();
    public List<GitFile> StagedGitFileList { get; } = new();
    public List<GitFile> UnstagedGitFileList { get; } = new();
    public string? LogFileContent { get; private set; }

	public Task OnAfterCommandStarted(TerminalCommand terminalCommand)
	{
        _stageKind = StageKind.None;
		_repo = _gitStateWrap.Value.Repo;

        // Reset data
        switch (terminalCommand.FormattedCommand.Tag)
        {
			case TagConstants.StatusEnqueue:
				UntrackedGitFileList.Clear();
				StagedGitFileList.Clear();
				UnstagedGitFileList.Clear();
				_behindByCommitCount = null;
				_aheadByCommitCount = null;
				break;
			case TagConstants.GetActiveBranchNameEnqueue:
                _branch = null;
                break;
			case TagConstants.GetOriginNameEnqueue:
                _origin = null;
                break;
			case TagConstants.BranchGetAllEnqueue:
                _branchList = new();
                break;
            case TagConstants.LogFileEnqueue:
                _logFileContentBuilder = null;
				break;
		}

		return Task.CompletedTask;
	}

	public List<TextEditorTextSpan> OnAfterOutputLine(TerminalCommand terminalCommand, string outputLine)
    {
        var localRepo = _repo;
		if (localRepo is null)
            return new();

        return terminalCommand.FormattedCommand.Tag switch
        {
			TagConstants.StatusEnqueue => StatusParseLine(outputLine),
			TagConstants.GetActiveBranchNameEnqueue => GetBranchParseLine(outputLine),
			TagConstants.GetOriginNameEnqueue => GetOriginParseLine(outputLine),
			TagConstants.BranchGetAllEnqueue => GetBranchListLine(outputLine),
			TagConstants.LogFileEnqueue => LogFileParseLine(outputLine),
            _ => new(),
        };
    }

	public Task OnAfterCommandFinished(TerminalCommand terminalCommand)
	{
		var localRepo = _repo;
		if (localRepo is null)
			return Task.CompletedTask;

		switch (terminalCommand.FormattedCommand.Tag)
		{
			case TagConstants.GetOriginNameEnqueue when _origin is not null:
				_dispatcher.Dispatch(new GitState.SetOriginAction(
					localRepo,
					_origin));
				break;
			case TagConstants.GetActiveBranchNameEnqueue when _branch is not null:
				_dispatcher.Dispatch(new GitState.SetBranchAction(
					localRepo,
					_branch));
				break;
			case TagConstants.BranchGetAllEnqueue:
				_dispatcher.Dispatch(new GitState.SetBranchListAction(
					localRepo,
					_branchList));
				break;
			case TagConstants.StatusEnqueue:
				_dispatcher.Dispatch(new GitState.SetStatusAction(
					localRepo,
					UntrackedGitFileList.ToImmutableList(),
					StagedGitFileList.ToImmutableList(),
					UnstagedGitFileList.ToImmutableList(),
					_behindByCommitCount ?? 0,
					_aheadByCommitCount ?? 0));
				break;
			case TagConstants.LogFileEnqueue:
				if (_logFileContentBuilder is not null)
					LogFileContent = _logFileContentBuilder.ToString();
				break;
		}

		return Task.CompletedTask;
	}

	public List<TextEditorTextSpan> StatusParseLine(string output)
    {
		var localRepo = _repo;
        if (localRepo is null)
            return new();

		var stringWalker = new StringWalker(new ResourceUri("/__LUTHETUS__/GitCliOutputParser.txt"), output);
        var textSpanList = new List<TextEditorTextSpan>();

        while (!stringWalker.IsEof)
        {
            if (stringWalker.CurrentCharacter == 'U' && stringWalker.PeekForSubstring("Untracked files:"))
            {
                // Found: "Untracked files:"
                var startPositionInclusive = stringWalker.PositionIndex;

                // Read: "Untracked files:" (literally)
                while (!stringWalker.IsEof)
                {
                    var character = stringWalker.ReadCharacter();

                    if (character == ':')
                    {
                        textSpanList.Add(new TextEditorTextSpan(
                            startPositionInclusive,
                            stringWalker,
                            (byte)TerminalDecorationKind.StringLiteral));

                        _stageKind = StageKind.IsReadingUntrackedFiles;
                        break;
                    }
                }
            }
            else if (stringWalker.CurrentCharacter == 'C' && stringWalker.PeekForSubstring("Changes to be committed:"))
            {
                // Found: "Changes to be committed:"
                var startPositionInclusive = stringWalker.PositionIndex;

                // Read: "Changes to be committed:" (literally)
                while (!stringWalker.IsEof)
                {
                    var character = stringWalker.ReadCharacter();

                    if (character == ':')
                    {
                        textSpanList.Add(new TextEditorTextSpan(
                            startPositionInclusive,
                            stringWalker,
                            (byte)TerminalDecorationKind.StringLiteral));

                        _stageKind = StageKind.IsReadingStagedFiles;
                        break;
                    }
                }
            }
            else if (stringWalker.CurrentCharacter == 'C' && stringWalker.PeekForSubstring("Changes not staged for commit:"))
            {
                // Found: "Changes not staged for commit:"
                var startPositionInclusive = stringWalker.PositionIndex;

                // Read: "Changes not staged for commit:" (literally)
                while (!stringWalker.IsEof)
                {
                    var character = stringWalker.ReadCharacter();

                    if (character == ':')
                    {
                        textSpanList.Add(new TextEditorTextSpan(
                            startPositionInclusive,
                            stringWalker,
                            (byte)TerminalDecorationKind.StringLiteral));

                        _stageKind = StageKind.IsReadingUnstagedFiles;
                        break;
                    }
                }
            }
            else if (stringWalker.CurrentCharacter == 'Y' && stringWalker.PeekForSubstring("Your branch is behind "))
            {
                // Found: "Your branch is behind 'origin/master' by 1 commit, and can be fast-forwarded."
                //
                // Read: "Your branch is behind " (literally)
                _ = stringWalker.ReadRange("Your branch is behind ".Length);

                if (stringWalker.CurrentCharacter != '\'')
                    return textSpanList;

                // Skip opening single-quote
                _ = stringWalker.ReadCharacter();

                // Skip until and including the closing single-quote
                while (!stringWalker.IsEof)
                {
                    var character = stringWalker.ReadCharacter();

                    if (character == '\'')
                        break;
                }

                // Read: " by "
                _ = stringWalker.ReadRange(" by ".Length);

                // Read the unsigned-integer
                var syntaxTokenList = new List<ISyntaxToken>();
                LexerUtils.LexNumericLiteralToken(stringWalker, syntaxTokenList);
                var numberTextSpan = syntaxTokenList.Single().TextSpan;
                var numberString = numberTextSpan.GetText();

                if (int.TryParse(numberString, out var localBehindByCommitCount))
                    _behindByCommitCount = localBehindByCommitCount;
                else
                    _behindByCommitCount = null;

                textSpanList.Add(numberTextSpan with
                {
                    DecorationByte = (byte)TerminalDecorationKind.StringLiteral,
                });

                return textSpanList;
            }
            else if (stringWalker.CurrentCharacter == 'Y' && stringWalker.PeekForSubstring("Your branch is ahead of "))
            {
                // Found: "Your branch is ahead of 'origin/master' by 1 commit."
                //
                // Read: "Your branch is ahead of " (literally)
                _ = stringWalker.ReadRange("Your branch is ahead of ".Length);

                if (stringWalker.CurrentCharacter != '\'')
                    return textSpanList;

                // Skip opening single-quote
                _ = stringWalker.ReadCharacter();

                // Skip until and including the closing single-quote
                while (!stringWalker.IsEof)
                {
                    var character = stringWalker.ReadCharacter();

                    if (character == '\'')
                        break;
                }

                // Read: " by "
                _ = stringWalker.ReadRange(" by ".Length);

                // Read the unsigned-integer
                var syntaxTokenList = new List<ISyntaxToken>();
                LexerUtils.LexNumericLiteralToken(stringWalker, syntaxTokenList);
                var numberTextSpan = syntaxTokenList.Single().TextSpan;
                var numberString = numberTextSpan.GetText();

                if (int.TryParse(numberString, out var localBehindByCommitCount))
                    _aheadByCommitCount = localBehindByCommitCount;
                else
                    _aheadByCommitCount = null;

                textSpanList.Add(numberTextSpan with
                {
                    DecorationByte = (byte)TerminalDecorationKind.StringLiteral,
                });

                return textSpanList;
            }

            if (_stageKind == StageKind.IsReadingUntrackedFiles ||
                _stageKind == StageKind.IsReadingStagedFiles ||
                _stageKind == StageKind.IsReadingUnstagedFiles)
            {
                while (!stringWalker.IsEof)
                {

                    if (stringWalker.CurrentCharacter == ' ' && stringWalker.NextCharacter == ' ')
                    {
                        // Read comments line by line
                        while (!stringWalker.IsEof)
                        {
                            if (stringWalker.CurrentCharacter != ' ' || stringWalker.NextCharacter != ' ')
                                break;

                            // Discard the leading whitespace on the line (two spaces)
                            _ = stringWalker.ReadRange(2);

                            var startPositionInclusive = stringWalker.PositionIndex;

                            while (!stringWalker.IsEof && !WhitespaceFacts.LINE_ENDING_CHARACTER_LIST.Contains(stringWalker.CurrentCharacter))
                            {
                                _ = stringWalker.ReadCharacter();
                            }

                            textSpanList.Add(new TextEditorTextSpan(
                                startPositionInclusive,
                                stringWalker,
                                (byte)TerminalDecorationKind.Comment));
                        }
                    }
                    else if (stringWalker.CurrentCharacter == WhitespaceFacts.TAB)
                    {
                        // Read untracked files line by line
                        while (!stringWalker.IsEof)
                        {
                            if (stringWalker.CurrentCharacter != WhitespaceFacts.TAB)
                                break;

                            // Discard the leading whitespace on the line (one tab)
                            _ = stringWalker.ReadCharacter();

							var gitDirtyString = string.Empty;

                            if (_stageKind == StageKind.IsReadingStagedFiles ||
                                _stageKind == StageKind.IsReadingUnstagedFiles)
                            {
                                // Read the git description
                                //
                                // Example: "new file:   BlazorApp4NetCoreDbg/Persons/Abc.cs"
                                //           ^^^^^^^^^^^^

								var gitDirtyStartPositionInclusive = stringWalker.PositionIndex;

                                while (!stringWalker.IsEof)
                                {
                                    if (stringWalker.CurrentCharacter == ':')
                                    {
										var gitDirtyTextSpan = new TextEditorTextSpan(
			                                gitDirtyStartPositionInclusive,
			                                stringWalker,
			                                (byte)TerminalDecorationKind.None);

										gitDirtyString = gitDirtyTextSpan.GetText();

                                        // Read the ':'
                                        _ = stringWalker.ReadCharacter();

                                        // Read the 3 ' ' characters (space characters)
                                        _ = stringWalker.ReadRange(3);

                                        break;
                                    }

                                    _ = stringWalker.ReadCharacter();
                                }
                            }

                            var startPositionInclusive = stringWalker.PositionIndex;

                            while (!stringWalker.IsEof && !WhitespaceFacts.LINE_ENDING_CHARACTER_LIST.Contains(stringWalker.CurrentCharacter))
                            {
                                _ = stringWalker.ReadCharacter();
                            }

                            var textSpan = new TextEditorTextSpan(
                                startPositionInclusive,
                                stringWalker,
                                (byte)TerminalDecorationKind.Warning);
                            textSpanList.Add(textSpan);

                            var relativePathString = textSpan.GetText();

							var absolutePathString = PathHelper.GetAbsoluteFromAbsoluteAndRelative(
								localRepo.AbsolutePath,
                                relativePathString,
                                _environmentProvider);

                            var isDirectory = relativePathString.EndsWith(_environmentProvider.DirectorySeparatorChar) ||
                                relativePathString.EndsWith(_environmentProvider.AltDirectorySeparatorChar);

                            var absolutePath = _environmentProvider.AbsolutePathFactory(absolutePathString, isDirectory);

                            GitDirtyReason gitDirtyReason;

							if (_stageKind == StageKind.IsReadingUntrackedFiles)
							{
								gitDirtyReason = GitDirtyReason.Untracked;
							}
							else
							{
								if (gitDirtyString == "modified")
									gitDirtyReason = GitDirtyReason.Modified;
								else if (gitDirtyString == "added") // There is no "added" its "new file" in the output.
									gitDirtyReason = GitDirtyReason.Added;
								else if (gitDirtyString == "new file")
									gitDirtyReason = GitDirtyReason.Added;
								else if (gitDirtyString == "deleted")
									gitDirtyReason = GitDirtyReason.Deleted;
								else
									gitDirtyReason = GitDirtyReason.None;
							}

                            var gitFile = new GitFile(
                                absolutePath,
                                relativePathString,
                                gitDirtyReason);

                            if (_stageKind == StageKind.IsReadingUntrackedFiles)
                                UntrackedGitFileList.Add(gitFile);
                            else if (_stageKind == StageKind.IsReadingStagedFiles)
                                StagedGitFileList.Add(gitFile);
                            else if (_stageKind == StageKind.IsReadingUnstagedFiles)
                                UnstagedGitFileList.Add(gitFile);
                        }

                        break;
                    }

                    _ = stringWalker.ReadCharacter();
                }
            }

            _ = stringWalker.ReadCharacter();
        }

        return textSpanList;
    }

    public List<TextEditorTextSpan> LogFileParseLine(string output)
    {
		/*
         git log ...redacted
         commit ...redacted
         Author: ...redacted
         Date:   ...redacted
         
             Abc123 3
         
         diff --git a/BlazorApp4NetCoreDbg/Shared/NavMenu.razor b/BlazorApp4NetCoreDbg/Shared/NavMenu.razor
         new file mode ...redacted
         index ...redacted
         --- /dev/null
         +++ ...redacted
         @@ -0,0 +1,39 @@
         +﻿<div class="top-row ps-3 navbar navbar-dark">
         +    <div class="container-fluid">
...For brevity much of the file's contents
...have been left out of this C# comment. 
         +        collapseNavMenu = !collapseNavMenu;
         +    }
         +}
         Process exited; Code: 0
         */

		// The output has every line from the file to start with a single '+' character.
		//
		// For brevity much of the file's contents were left out.
		//
		// But, the pattern can be seen that the file is written out as contiguous
		// lines, where each starts with a '+' character.

		var localRepo = _repo;
        if (localRepo is null)
            return new();

		var stringWalker = new StringWalker(new ResourceUri("/__LUTHETUS__/GitCliOutputParser.txt"), output);
        var textSpanList = new List<TextEditorTextSpan>();
        _logFileContentBuilder ??= new();

        // TODO: This code is incorrect. For example, the line "++1"...
        //       ...The first '+' is from the git CLI. But, then the file's text is "+1".
        //       This second '+' is erroneously taken to mean the line isn't the logged file's content.
        if (stringWalker.CurrentCharacter == '+' && stringWalker.NextCharacter != '+')
        {
            // Do not include the '+' as part of the logged file's content.
            _ = stringWalker.ReadCharacter();

            // Skip the 'UTF-8 with BOM'
            if (stringWalker.CurrentCharacter == '�' && stringWalker.PeekForSubstring("﻿"))
                _ = stringWalker.ReadRange("﻿".Length);

            var loggedLineTextSpan = new TextEditorTextSpan(
                stringWalker.PositionIndex,
                output.Length,
                (byte)TerminalDecorationKind.StringLiteral,
                new ResourceUri("/__LUTHETUS__/GitCliOutputParser.txt"),
                output);

            _logFileContentBuilder.Append(loggedLineTextSpan.GetText());
            textSpanList.Add(loggedLineTextSpan);
        }

        return textSpanList;
    }
    
    public List<TextEditorTextSpan> GetOriginParseLine(string output)
    {
		var localRepo = _repo;
        if (localRepo is null)
            return new();

		// TODO: Parsing origin line is super hacky, and should be re-written.
		if (_count++ == 1)
            _origin ??= output;

        var stringWalker = new StringWalker(new ResourceUri("/__LUTHETUS__/GitCliOutputParser.txt"), output);
        var textSpanList = new List<TextEditorTextSpan>();

        while (!stringWalker.IsEof)
        {
            _ = stringWalker.ReadCharacter();
        }

        return textSpanList;
    }
    
    public List<TextEditorTextSpan> GetBranchParseLine(string output)
    {
		var localRepo = _repo;
		if (localRepo is null)
			return new();

		// TODO: Parsing branch line is super hacky, and should be re-written.
		if (_count++ == 1)
            _branch ??= output.Trim();

        var stringWalker = new StringWalker(new ResourceUri("/__LUTHETUS__/GitCliOutputParser.txt"), output);
        var textSpanList = new List<TextEditorTextSpan>();

        while (!stringWalker.IsEof)
        {
            _ = stringWalker.ReadCharacter();
        }

        return textSpanList;
    }
    
    public List<TextEditorTextSpan> GetBranchListLine(string output)
	{
		var localRepo = _repo;
		if (localRepo is null)
			return new();

		var stringWalker = new StringWalker(new ResourceUri("/__LUTHETUS__/GitCliOutputParser.txt"), output);
        var textSpanList = new List<TextEditorTextSpan>();

        // "* Abc"    <-- Line 1 with quotes added to show where it starts and ends
        // "  master" <-- Line 2 with quotes added to show where it starts and ends
        //
        // Every branch seems to start with 2 characters, where the first is whether it's the active branch,
        // and the second is just a whitespace to separate whether its the active branch from its name.
        //
        // Therefore, naively skip 2 characters then readline.
        var isValid = false;

        if (stringWalker.CurrentCharacter == '*' || stringWalker.CurrentCharacter == ' ')
        {
            if (stringWalker.NextCharacter == ' ')
                isValid = true;
        }

        if (!isValid)
            return textSpanList;

        _ = stringWalker.ReadRange(2);

        var startPositionInclusive = stringWalker.PositionIndex;

        while (!stringWalker.IsEof && !WhitespaceFacts.LINE_ENDING_CHARACTER_LIST.Contains(stringWalker.CurrentCharacter))
        {
            _ = stringWalker.ReadCharacter();
        }

        var textSpan = new TextEditorTextSpan(
            startPositionInclusive,
            stringWalker,
            (byte)TerminalDecorationKind.StringLiteral);

        textSpanList.Add(textSpan);
        _branchList.Add(textSpan.GetText());

        return textSpanList;
    }

    /// <summary>
    /// Micro-filter
    /// </summary>
    public enum StageKind
    {
        None,
        IsReadingUntrackedFiles,
        IsReadingUnstagedFiles,
        IsReadingStagedFiles,
    }

	public static class TagConstants
	{
		public const string SetGitOrigin = "SetGitOrigin";
		public const string StatusEnqueue = "StatusEnqueue";
		public const string GetActiveBranchNameEnqueue = "GetActiveBranchNameEnqueue";
		public const string GetOriginNameEnqueue = "GetOriginNameEnqueue";
		public const string BranchGetAllEnqueue = "BranchGetAllEnqueue";
		public const string BranchSetEnqueue = "BranchSetEnqueue";
		public const string PushToOriginWithTrackingEnqueue = "PushToOriginWithTrackingEnqueue";
		public const string PullEnqueue = "PullEnqueue";
		public const string FetchEnqueue = "FetchEnqueue";
		public const string LogFileEnqueue = "LogFileEnqueue";
	}
}