using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using System.Collections.Immutable;
using System.Text;

namespace Luthetus.TextEditor.RazorLib.Keymaps.Models.Vims;

public static class SyntaxRepeatVim
{
    public static bool TryLex(KeymapArgument keymapArgument, bool hasTextSelection, out VimGrammarToken? vimGrammarToken)
    {
        // Suppress unused parameter warnings because these will likely be used once further Vim emulation is implemented.
        _ = hasTextSelection;

        if (keymapArgument.Code is null)
        {
            vimGrammarToken = null;
            return false;
        }

        var possibleDigit = keymapArgument.Code.Last();

        if (char.IsNumber(possibleDigit) && possibleDigit != '0')
        {
            vimGrammarToken = new VimGrammarToken(VimGrammarKind.Repeat, keymapArgument);
            return true;
        }

        vimGrammarToken = null;
        return false;
    }

    public static bool TryParse(TextEditorKeymapVim textEditorKeymapVim,
        ImmutableArray<VimGrammarToken> sentenceSnapshotList,
        int indexInSentence,
        KeymapArgument keymapArgument,
        bool hasTextSelection,
        out TextEditorCommand? textEditorCommand)
    {
        int modifiedIndexInSentence = indexInSentence;

        var numberBuilder = new StringBuilder();

        for (int i = indexInSentence; i < sentenceSnapshotList.Length; i++)
        {
            var currentToken = sentenceSnapshotList[i];

            if (currentToken.VimGrammarKind == VimGrammarKind.Repeat)
            {
                numberBuilder.Append(currentToken.KeymapArgument.Code.Last());
                modifiedIndexInSentence++;
            }
        }

        var intValue = int.Parse(numberBuilder.ToString());

        var success = VimSentence.TryParseNextToken(
            textEditorKeymapVim,
            sentenceSnapshotList,
            modifiedIndexInSentence,
            keymapArgument,
            hasTextSelection,
            out var innerTextEditorCommand);

        if (success && innerTextEditorCommand is not null)
        {
            var textEditorCommandDisplayName = $"Vim::Repeat(count: {intValue}, arg: {innerTextEditorCommand.DisplayName})";

            // Repeat the inner TextEditorCommand using a for loop
            textEditorCommand = new TextEditorCommand(
                textEditorCommandDisplayName, textEditorCommandDisplayName, false, true, TextEditKind.None, null,
                async textEditorCommandArgs =>
                {
                    for (int index = 0; index < intValue; index++)
                    {
                        await innerTextEditorCommand.CommandFunc.Invoke(textEditorCommandArgs).ConfigureAwait(false);
                    }
                })
            {
                TextEditorEditFactory = interfaceCommandArgs =>
                {
                    var textEditorCommandArgs = (TextEditorCommandArgs)interfaceCommandArgs;

                    return async editContext  =>
                    {
                        for (int index = 0; index < intValue; index++)
                        {
                            await innerTextEditorCommand.CommandFunc.Invoke(textEditorCommandArgs).ConfigureAwait(false);
                        }
                    };
                }
            };
        }
        else
        {
            textEditorCommand = null;
        }

        return success;
    }
}
