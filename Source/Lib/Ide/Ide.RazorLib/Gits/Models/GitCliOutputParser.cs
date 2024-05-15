using Fluxor;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.Gits.States;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Facts;
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
    private int _count;

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

    public void Dispose()
    {
        if (_gitState.Repo is null)
            return;

        if (_gitCommandKind == GitCommandKind.GetOrigin && _origin is not null)
        {
            _dispatcher.Dispatch(new GitState.SetOriginAction(
                _gitState.Repo,
                _origin));
        }
        else if (_gitCommandKind == GitCommandKind.Status)
        {
            _dispatcher.Dispatch(new GitState.SetFileListAction(
                _gitState.Repo,
                UntrackedGitFileList.ToImmutableList(),
                StagedGitFileList.ToImmutableList(),
                UnstagedGitFileList.ToImmutableList()));
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