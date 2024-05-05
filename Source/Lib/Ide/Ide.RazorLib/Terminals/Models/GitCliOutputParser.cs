using Fluxor;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.Gits.Models;
using Luthetus.Ide.RazorLib.Gits.States;
using Luthetus.TextEditor.RazorLib.CompilerServices.Facts;
using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

public class GitCliOutputParser : IOutputParser
{
    private readonly IDispatcher _dispatcher;
    private readonly GitState _gitState;
    private readonly IEnvironmentProvider _environmentProvider;
    
    public GitCliOutputParser(
        IDispatcher dispatcher,
        GitState gitState,
        IEnvironmentProvider environmentProvider,
        StageKind stageKind = StageKind.None)
    {
        _dispatcher = dispatcher;
        _gitState = gitState;
        _environmentProvider = environmentProvider;
        _stageKind = stageKind;
    }

    private StageKind _stageKind = StageKind.None;

    private string? _origin;

    private int _count;

    public List<GitFile> GitFileList { get; } = new();

    public List<TextEditorTextSpan> ParseLine(string output)
    {
        if (_gitState.Repo is null)
            return new();

        if (_stageKind == StageKind.GetOrigin)
            return ParseOriginLine(output);

        var stringWalker = new StringWalker(new ResourceUri("/__LUTHETUS__/GitCliOutputParser.txt"), output);
        var textSpanList = new List<TextEditorTextSpan>();

        while (!stringWalker.IsEof)
        {
            if (_stageKind == StageKind.None)
            {
                // Find: "Untracked files:"
                if (stringWalker.CurrentCharacter == 'U' && stringWalker.PeekForSubstring("Untracked files:"))
                {
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
                                (byte)TerminalDecorationKind.Warning));

                            _stageKind = StageKind.IsReadingUntrackedFiles;
                            break;
                        }
                    }
                }
            }
            else if (_stageKind == StageKind.IsReadingUntrackedFiles)
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

                            GitFileList.Add(new GitFile(
                                absolutePath,
                                relativePathString,
                                GitDirtyReason.Untracked));
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

    private List<TextEditorTextSpan> ParseOriginLine(string output)
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

        if (_stageKind == StageKind.GetOrigin && _origin is not null)
        {
            _dispatcher.Dispatch(new GitState.SetOriginAction(
                _gitState.Repo,
                _origin));
        }
        else
        {
            _dispatcher.Dispatch(new GitState.SetFileListAction(
                _gitState.Repo,
                GitFileList.ToImmutableList()));
        }
    }

    public enum StageKind
    {
        None,
        IsReadingUntrackedFiles,
        GetOrigin,
    }
}