using Fluxor;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.CompilerServices.Lang.CSharp.LexerCase;
using Luthetus.Ide.RazorLib.Gits.States;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Facts;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.Gits.Models;

public class GitCliOutputParser : IOutputParser
{
    private readonly IDispatcher _dispatcher;
    private readonly GitState _gitState;
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly GitCommandKind _gitCommandKind;

    public GitCliOutputParser(
        IDispatcher dispatcher,
        GitState gitState,
        IEnvironmentProvider environmentProvider,
		GitCommandKind gitCommandKind,
        StageKind stageKind = StageKind.None)
    {
        _dispatcher = dispatcher;
        _gitState = gitState;
        _environmentProvider = environmentProvider;
        _gitCommandKind = gitCommandKind;
        _stageKind = stageKind;
    }

    private StageKind _stageKind = StageKind.None;
    private string? _origin;
    private string? _branch;
    private List<string> _branchList = new();
    private int _count;
    private int? _behindByCommitCount;

    public List<GitFile> UntrackedGitFileList { get; } = new();
    public List<GitFile> StagedGitFileList { get; } = new();
    public List<GitFile> UnstagedGitFileList { get; } = new();

    public List<TextEditorTextSpan> ParseLine(string output)
    {
        if (_gitState.Repo is null)
            return new();

        return _gitCommandKind switch
        {
            GitCommandKind.Status => StatusParseLine(output),
            GitCommandKind.GetOrigin => GetOriginParseLine(output),
            GitCommandKind.GetBranch => GetBranchParseLine(output),
            GitCommandKind.GetBranchList => GetBranchListLine(output),
            _ => new(),
        };
    }

    public List<TextEditorTextSpan> StatusParseLine(string output)
    {
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
                var startPositionInclusive = stringWalker.PositionIndex;

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
                LuthLexerUtils.LexNumericLiteralToken(stringWalker, syntaxTokenList);
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

            if (_stageKind == StageKind.IsReadingUntrackedFiles)
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
                                _gitState.Repo.AbsolutePath,
                                relativePathString,
                                _environmentProvider);

                            var isDirectory = relativePathString.EndsWith(_environmentProvider.DirectorySeparatorChar) ||
                                relativePathString.EndsWith(_environmentProvider.AltDirectorySeparatorChar);

                            var absolutePath = _environmentProvider.AbsolutePathFactory(absolutePathString, isDirectory);

                            UntrackedGitFileList.Add(new GitFile(
                                absolutePath,
                                relativePathString,
                                GitDirtyReason.Untracked));
                        }

                        break;
                    }

                    _ = stringWalker.ReadCharacter();
                }
            }
            else if (_stageKind == StageKind.IsReadingStagedFiles)
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

                            // Skip the git description
                            //
                            // Example: "new file:   BlazorApp4NetCoreDbg/Persons/Abc.cs"
                            //           ^^^^^^^^^^^^
                            while (!stringWalker.IsEof)
                            {
                                if (stringWalker.CurrentCharacter == ':')
                                {
                                    // Read the ':'
                                    _ = stringWalker.ReadCharacter();

                                    // Read the 3 ' ' characters (space characters)
                                    _ = stringWalker.ReadRange(3);

                                    break;
                                }

                                _ = stringWalker.ReadCharacter();
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
                                _gitState.Repo.AbsolutePath,
                                relativePathString,
                                _environmentProvider);

                            var isDirectory = relativePathString.EndsWith(_environmentProvider.DirectorySeparatorChar) ||
                                relativePathString.EndsWith(_environmentProvider.AltDirectorySeparatorChar);

                            var absolutePath = _environmentProvider.AbsolutePathFactory(absolutePathString, isDirectory);

                            StagedGitFileList.Add(new GitFile(
                                absolutePath,
                                relativePathString,
                                GitDirtyReason.Added));
                        }

                        break;
                    }

                    _ = stringWalker.ReadCharacter();
                }
            }
            else if (_stageKind == StageKind.IsReadingUnstagedFiles)
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

                            // Skip the git description
                            //
                            // Example: "new file:   BlazorApp4NetCoreDbg/Persons/Abc.cs"
                            //           ^^^^^^^^^^^^
                            while (!stringWalker.IsEof)
                            {
                                if (stringWalker.CurrentCharacter == ':')
                                {
                                    // Read the ':'
                                    _ = stringWalker.ReadCharacter();

                                    // Read the 3 ' ' characters (space characters)
                                    _ = stringWalker.ReadRange(3);

                                    break;
                                }

                                _ = stringWalker.ReadCharacter();
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
                                _gitState.Repo.AbsolutePath,
                                relativePathString,
                                _environmentProvider);

                            var isDirectory = relativePathString.EndsWith(_environmentProvider.DirectorySeparatorChar) ||
                                relativePathString.EndsWith(_environmentProvider.AltDirectorySeparatorChar);

                            var absolutePath = _environmentProvider.AbsolutePathFactory(absolutePathString, isDirectory);

                            UnstagedGitFileList.Add(new GitFile(
                                absolutePath,
                                relativePathString,
                                GitDirtyReason.Added));
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

    public List<TextEditorTextSpan> GetOriginParseLine(string output)
    {
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

    public void Dispose()
    {
        if (_gitState.Repo is null)
            return;

        switch (_gitCommandKind)
        {
            case GitCommandKind.GetOrigin when _origin is not null:
                _dispatcher.Dispatch(new GitState.SetOriginAction(
                    _gitState.Repo,
                    _origin));
                break;
            case GitCommandKind.GetBranch when _branch is not null:
                _dispatcher.Dispatch(new GitState.SetBranchAction(
                    _gitState.Repo,
                    _branch));
                break;
            case GitCommandKind.GetBranchList:
                _dispatcher.Dispatch(new GitState.SetBranchListAction(
                    _gitState.Repo,
                    _branchList));
                break;
            case GitCommandKind.Status:
                _dispatcher.Dispatch(new GitState.SetStatusAction(
                    _gitState.Repo,
                    UntrackedGitFileList.ToImmutableList(),
                    StagedGitFileList.ToImmutableList(),
                    UnstagedGitFileList.ToImmutableList(),
                    _behindByCommitCount ?? 0));
                break;
        }
    }

    /// <summary>
    /// Macro-filter
    /// </summary>
	public enum GitCommandKind
    {
        None,
        Status,
        GetOrigin,
        GetBranch,
        GetBranchList,
        PushToOriginWithTracking,
        Pull,
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
}