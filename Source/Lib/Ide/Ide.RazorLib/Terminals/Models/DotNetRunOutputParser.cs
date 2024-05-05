using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;
using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

public class DotNetRunOutputParser : IOutputParser
{
    public List<TextEditorTextSpan> ParseLine(string output)
    {
        var stringWalker = new StringWalker(new ResourceUri("/__LUTHETUS__/DotNetRunOutputParser.txt"), output);

        var textSpanList = new List<TextEditorTextSpan>();

        TextEditorTextSpan errorKeywordAndErrorCodeTextSpan = new(0, 0, 0, new ResourceUri(string.Empty), string.Empty);

        while (!stringWalker.IsEof)
        {
            // Step 1: Read filePathTextSpan
            {
                var startPositionInclusiveFilePath = stringWalker.PositionIndex;

                while (true)
                {
                    var character = stringWalker.ReadCharacter();

                    if (character == '(')
                    {
                        _ = stringWalker.BacktrackCharacter();

                        textSpanList.Add(new TextEditorTextSpan(
                            startPositionInclusiveFilePath,
                            stringWalker,
                            (byte)GenericDecorationKind.None));

                        break;
                    }
                    else if (stringWalker.IsEof)
                    {
                        break;
                    }
                }
            }

            // Step 2: Read rowAndColumnNumberTextSpan
            {
                var startPositionInclusiveRowAndColumnNumber = stringWalker.PositionIndex;

                while (true)
                {
                    var character = stringWalker.ReadCharacter();

                    if (character == ')')
                    {
                        textSpanList.Add(new TextEditorTextSpan(
                            startPositionInclusiveRowAndColumnNumber,
                            stringWalker,
                            (byte)GenericDecorationKind.None));

                        break;
                    }
                    else if (stringWalker.IsEof)
                    {
                        break;
                    }
                }
            }

            // Step 3: Read errorKeywordAndErrorCode
            {
                // Consider having Step 2 use ':' as its exclusive delimiter.
                // Because now a step is needed to skip over some text.
                {
                    if (stringWalker.CurrentCharacter == ':')
                        _ = stringWalker.ReadCharacter();

                    _ = stringWalker.ReadWhitespace();
                }

                var startPositionInclusiveErrorKeywordAndErrorCode = stringWalker.PositionIndex;

                while (true)
                {
                    var character = stringWalker.ReadCharacter();

                    if (character == ':')
                    {
                        _ = stringWalker.BacktrackCharacter();

                        errorKeywordAndErrorCodeTextSpan = new TextEditorTextSpan(
                            startPositionInclusiveErrorKeywordAndErrorCode,
                            stringWalker,
                            (byte)TerminalDecorationKind.Warning);

                        // I would rather a warning be incorrectly syntax highlighted as an error,
                        // than for an error to be incorrectly syntax highlighted as a warning.
                        // Therefore, presume warning, then check if the text isn't "warning".
                        if (!errorKeywordAndErrorCodeTextSpan.GetText().StartsWith("warning", StringComparison.InvariantCultureIgnoreCase))
                        {
                            errorKeywordAndErrorCodeTextSpan = errorKeywordAndErrorCodeTextSpan with
                            {
                                DecorationByte = (byte)TerminalDecorationKind.Error
                            };
                        }

                        break;
                    }
                    else if (stringWalker.IsEof)
                    {
                        break;
                    }
                }
            }

            // Step 4: Read errorMessage
            {
                // A step is needed to skip over some text.
                {
                    if (stringWalker.CurrentCharacter == ':')
                        _ = stringWalker.ReadCharacter();

                    _ = stringWalker.ReadWhitespace();
                }

                var startPositionInclusiveErrorMessage = stringWalker.PositionIndex;

                while (true)
                {
                    var character = stringWalker.ReadCharacter();

                    if (character == '[')
                    {
                        _ = stringWalker.BacktrackCharacter();

                        textSpanList.Add(new TextEditorTextSpan(
                            startPositionInclusiveErrorMessage,
                            stringWalker,
                            errorKeywordAndErrorCodeTextSpan.DecorationByte));

                        break;
                    }
                    else if (stringWalker.IsEof)
                    {
                        break;
                    }
                }
            }

            // Step 5: Read project file path
            {
                var startPositionInclusiveProjectFilePath = stringWalker.PositionIndex;

                while (true)
                {
                    var character = stringWalker.ReadCharacter();

                    if (character == ']')
                    {
                        textSpanList.Add(new TextEditorTextSpan(
                            startPositionInclusiveProjectFilePath,
                            stringWalker,
                            (byte)GenericDecorationKind.None));

                        break;
                    }
                    else if (stringWalker.IsEof)
                    {
                        break;
                    }
                }
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

        return textSpanList;
    }

    public void Dispose()
    {
        return;
    }
}