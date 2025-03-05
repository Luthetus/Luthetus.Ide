using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Extensions.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax;
using Luthetus.Extensions.Git.BackgroundTasks.Models;

namespace Luthetus.Extensions.Git.Models;

public class GitCliOutputParser
{
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly GitBackgroundTaskApi _gitBackgroundTaskApi;

    public GitCliOutputParser(
		GitBackgroundTaskApi gitBackgroundTaskApi,
        IEnvironmentProvider environmentProvider)
    {
		_gitBackgroundTaskApi = gitBackgroundTaskApi;
        _environmentProvider = environmentProvider;
    }

    private StageKind _stageKind = StageKind.None;
	private string? _origin;
    private string? _branch;
    private List<string> _branchList = new();
    private int? _behindByCommitCount;
    private int? _aheadByCommitCount;
	private GitRepo? _repo;

	public List<GitFile> UntrackedGitFileList { get; } = new();
    public List<GitFile> StagedGitFileList { get; } = new();
    public List<GitFile> UnstagedGitFileList { get; } = new();
    public string? LogFileContent { get; private set; }

	public void DispatchSetStatusAction()
	{
		var localRepo = _repo;
		if (localRepo is null)
			return;

		_gitBackgroundTaskApi.Git.ReduceSetStatusAction(
			localRepo,
			UntrackedGitFileList,
			StagedGitFileList,
			UnstagedGitFileList,
			_behindByCommitCount ?? 0,
			_aheadByCommitCount ?? 0);
	}
	
	public void DispatchSetBranchAction()
	{
		var localRepo = _repo;
		if (localRepo is null)
			return;
			
		var localBranch = _branch;
		
		if (localBranch is not null)
		{
			_gitBackgroundTaskApi.Git.ReduceSetBranchAction(
				localRepo,
				localBranch);
		}
	}
	
	public void DispatchSetOriginAction()
	{
		var localRepo = _repo;
		if (localRepo is null)
			return;
			
		var localOrigin = _origin;
		
		if (localOrigin is not null)
		{
			_gitBackgroundTaskApi.Git.ReduceSetGitOriginAction(
				localRepo,
				localOrigin);
		}
	}
	
	public void DispatchSetBranchListAction()
	{
		var localRepo = _repo;
		if (localRepo is null)
			return;
			
		var localBranchList = _branchList;
		
		if (localBranchList is not null)
		{
			_gitBackgroundTaskApi.Git.ReduceSetBranchListAction(
				localRepo,
				localBranchList);
		}
	}
    
    public List<TextEditorTextSpan> StatusParseEntire(string outputEntire)
    {
    	_stageKind = StageKind.None;
		_repo = _gitBackgroundTaskApi.Git.GetGitState().Repo;
		
		UntrackedGitFileList.Clear();
		StagedGitFileList.Clear();
		UnstagedGitFileList.Clear();
		_behindByCommitCount = null;
		_aheadByCommitCount = null;
    
		var localRepo = _repo;
        if (localRepo is null)
            return new();
            
		var stringWalker = new StringWalker(new ResourceUri("/__LUTHETUS__/GitCliOutputParser.txt"), outputEntire);
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
                var syntaxTokenList = new List<SyntaxToken>();
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
                var syntaxTokenList = new List<SyntaxToken>();
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
            else if (stringWalker.CurrentCharacter == ' ' && stringWalker.NextCharacter == ' ')
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
            }

            _ = stringWalker.ReadCharacter();
        }

        return textSpanList;
    }
    
    public List<TextEditorTextSpan> GetOriginParse(string outputEntire)
    {
		var localRepo = _repo;
        if (localRepo is null)
            return new();

        _origin = outputEntire.Trim();
        
        return new();
    }
    
    public List<TextEditorTextSpan> GetBranchParse(string outputEntire)
    {
		var localRepo = _repo;
		if (localRepo is null)
			return new();

		_branch = outputEntire.Trim();

        return new();
    }
    
    public List<TextEditorTextSpan> GetBranchListEntire(string outputEntire)
	{
		var localRepo = _repo;
		if (localRepo is null)
			return new();

		var stringWalker = new StringWalker(new ResourceUri("/__LUTHETUS__/GitCliOutputParser.txt"), outputEntire);
        var textSpanList = new List<TextEditorTextSpan>();

		while (!stringWalker.IsEof)
		{
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
	        
	        if (stringWalker.IsEof)
	        {
	        	break;
	        }
	        else
	        {
	        	// Finished reading a line, so consume the line ending character(s)
	        	while (WhitespaceFacts.LINE_ENDING_CHARACTER_LIST.Contains(stringWalker.CurrentCharacter))
	        	{
		        	_ = stringWalker.ReadCharacter();
	        	}
	        }
		}

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