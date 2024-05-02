using Fluxor;
using Luthetus.Ide.RazorLib.Gits.States;
using Luthetus.TextEditor.RazorLib.CompilerServices.Facts;
using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Linq;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

public class GitCliOutputParser
{
    private readonly IDispatcher _dispatcher;
    private readonly GitState _gitState;

    public GitCliOutputParser(IDispatcher dispatcher, GitState gitState)
    {
        _dispatcher = dispatcher;
        _gitState = gitState;
    }

    private StageKind _stageKind = StageKind.None;

    public List<TextEditorTextSpan> Parse(string output)
    {
        var stringWalker = new StringWalker(new ResourceUri("/__LUTHETUS__/GitCliOutputParser.txt"), output);

        var textSpanList = new List<TextEditorTextSpan>();

        TextEditorTextSpan errorKeywordAndErrorCodeTextSpan = new(0, 0, 0, new ResourceUri(string.Empty), string.Empty);

        

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

                            textSpanList.Add(new TextEditorTextSpan(
                                startPositionInclusive,
                                stringWalker,
                                (byte)TerminalDecorationKind.Warning));
                        }

                        break;
                    }

                    _ = stringWalker.ReadCharacter();
                }

                //// Read each untracked file
                //while (!stringWalker.IsEof)
                //{
                //    var character = stringWalker.ReadCharacter();

                //    if (character == ':')
                //    {
                //        textSpanList.Add(new TextEditorTextSpan(
                //            startPositionInclusive,
                //            stringWalker,
                //            (byte)TerminalDecorationKind.Warning));

                //        break;
                //    }
                //}
            }


            _ = stringWalker.ReadCharacter();
        }

        if (errorKeywordAndErrorCodeTextSpan.DecorationByte != 0)
        {
            for (int i = textSpanList.Count - 1; i >= 0; i--)
            {
                textSpanList[i] = textSpanList[i] with
                {
                    DecorationByte = errorKeywordAndErrorCodeTextSpan.DecorationByte
                };
            }
        }

        //_dispatcher.Dispatch(new GitState.SetGitStateWithAction(inState =>
        //{
        //    if (inState.GitFolderAbsolutePath != localGitState.GitFolderAbsolutePath)
        //    {
        //        // Git folder was changed while the text was being parsed,
        //        // throw away the result since it is thereby invalid.
        //        return inState;
        //    }
        //
        //    inState.GitFilesList.Add(new GitFile());
        //}));

        return textSpanList;
    }

    private enum StageKind
    {
        None,
        IsReadingUntrackedFiles,
    }
}