using Fluxor;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.Gits.Models;
using Luthetus.Ide.RazorLib.Gits.States;
using Luthetus.TextEditor.RazorLib.CompilerServices.Facts;
using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

public class GitCliOutputParser
{
    private readonly IDispatcher _dispatcher;
    private readonly GitState _gitState;
    private readonly IAbsolutePath _workingDirectory;
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly List<GitFile> _gitFileList = new();

    public GitCliOutputParser(
        IDispatcher dispatcher,
        GitState gitState,
        IAbsolutePath workingDirectory,
        IEnvironmentProvider environmentProvider)
    {
        _dispatcher = dispatcher;
        _gitState = gitState;
        _workingDirectory = workingDirectory;
        _environmentProvider = environmentProvider;
    }

    private StageKind _stageKind = StageKind.None;

    public List<TextEditorTextSpan> Parse(string output)
    {
        if (_gitState.GitFolderAbsolutePath is null)
            return new();

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

                            var text = textSpan.GetText();

                            var absolutePathString = PathHelper.GetAbsoluteFromAbsoluteAndRelative(
                                _workingDirectory,
                                text,
                                _environmentProvider);

                            var isDirectory = text.EndsWith(_environmentProvider.DirectorySeparatorChar) ||
                                text.EndsWith(_environmentProvider.AltDirectorySeparatorChar);

                            var absolutePath = _environmentProvider.AbsolutePathFactory(absolutePathString, isDirectory);

                            _gitFileList.Add(new GitFile(absolutePath, GitDirtyReason.Untracked));
                        }

                        break;
                    }

                    _ = stringWalker.ReadCharacter();
                }
            }

            _ = stringWalker.ReadCharacter();
        }

        _dispatcher.Dispatch(new GitState.SetGitFileListAction(
            _gitState.GitFolderAbsolutePath,
            _gitFileList.ToImmutableList()));

        return textSpanList;
    }

    private enum StageKind
    {
        None,
        IsReadingUntrackedFiles,
    }
}