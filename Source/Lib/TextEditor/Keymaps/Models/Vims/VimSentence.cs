using Luthetus.TextEditor.RazorLib.Commands.Models.Vims;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.Exceptions;

namespace Luthetus.TextEditor.RazorLib.Keymaps.Models.Vims;

public class VimSentence
{
    private readonly List<VimGrammarToken> _pendingSentenceList = new();

    public ImmutableArray<VimGrammarToken> PendingSentenceList => _pendingSentenceList.ToImmutableArray();
    public ImmutableArray<VimGrammarToken> MostRecentSyntacticallyCompleteSentence { get; set; } = ImmutableArray<VimGrammarToken>.Empty;

    public bool TryLex(
        TextEditorKeymapVim textEditorKeymapVim,
        KeymapArgs keymapArgs,
        bool hasTextSelection,
        out TextEditorCommand? textEditorCommand)
    {
        bool sentenceIsSyntacticallyComplete;

        var mostRecentToken = _pendingSentenceList.LastOrDefault();

        if (mostRecentToken is null)
        {
            sentenceIsSyntacticallyComplete = ContinueFromStart(keymapArgs, hasTextSelection);
        }
        else
        {
            sentenceIsSyntacticallyComplete = mostRecentToken.VimGrammarKind switch
            {
                VimGrammarKind.Verb => ContinueFromVerb(keymapArgs, hasTextSelection),
                VimGrammarKind.Modifier => ContinueFromModifier(keymapArgs, hasTextSelection),
                VimGrammarKind.TextObject => ContinueFromTextObject(keymapArgs, hasTextSelection),
                VimGrammarKind.Repeat => ContinueFromRepeat(keymapArgs, hasTextSelection),
                _ => throw new LuthetusTextEditorException($"The {nameof(VimGrammarKind)}: {_pendingSentenceList.Last().VimGrammarKind} was not recognized."),
            };
        }

        if (sentenceIsSyntacticallyComplete)
        {
            var sentenceSnapshot = PendingSentenceList;
            MostRecentSyntacticallyCompleteSentence = sentenceSnapshot;
            _pendingSentenceList.Clear();

            return TryParseNextToken(
                textEditorKeymapVim,
                sentenceSnapshot,
                0,
                keymapArgs,
                hasTextSelection,
                out textEditorCommand);
        }

        textEditorCommand = TextEditorCommandDefaultFacts.DoNothingDiscard;
        return true;
    }

    private bool ContinueFromStart(KeymapArgs keymapArgs, bool hasTextSelection)
    {
        if (SyntaxVerbVim.TryLex(keymapArgs, hasTextSelection, out var verbToken) && verbToken is not null)
        {
            if (keymapArgs.CtrlKey)
            {
                // This if case relates to 'Ctrl + e' which does not get
                // double tapped instead it only takes one press of the keymap
                _pendingSentenceList.Clear();
                _pendingSentenceList.Add(verbToken);

                return true;
            }

            if (hasTextSelection)
            {
                _pendingSentenceList.Clear();
                _pendingSentenceList.Add(verbToken);

                return true;
            }

            _pendingSentenceList.Add(verbToken);
            return false;
        }

        if (SyntaxTextObjectVim.TryLex(keymapArgs, hasTextSelection, out var textObjectToken) && textObjectToken is not null)
        {
            _pendingSentenceList.Add(textObjectToken);
            return true;
        }

        if (SyntaxRepeatVim.TryLex(keymapArgs, hasTextSelection, out var repeatToken) && repeatToken is not null)
        {
            _pendingSentenceList.Add(repeatToken);
            return false;
        }

        _pendingSentenceList.Clear();
        return false;
    }

    private bool ContinueFromVerb(KeymapArgs keymapArgs, bool hasTextSelection)
    {
        if (SyntaxVerbVim.TryLex(keymapArgs, hasTextSelection, out var verbToken) && verbToken is not null)
        {
            if (_pendingSentenceList.Last().KeymapArgument.Code == verbToken.KeymapArgument.Code)
            {
                _pendingSentenceList.Add(verbToken);
                return true;
            }

            if (keymapArgs.CtrlKey)
            {
                // This if case relates to 'Ctrl + e' which does not get
                // double tapped instead it only takes one press of the keymap
                _pendingSentenceList.Clear();
                _pendingSentenceList.Add(verbToken);

                return true;
            }

            // The verb was overriden so restart sentence
            _pendingSentenceList.Clear();

            return ContinueFromStart(keymapArgs, hasTextSelection);
        }
        
        if (SyntaxModifierVim.TryLex(keymapArgs, hasTextSelection, out var modifierToken) && modifierToken is not null)
        {
            _pendingSentenceList.Add(modifierToken);
            return false;
        }
        
        if (SyntaxTextObjectVim.TryLex(keymapArgs, hasTextSelection, out var textObjectToken) && textObjectToken is not null)
        {
            _pendingSentenceList.Add(textObjectToken);
            return true;
        }
        
        if (SyntaxRepeatVim.TryLex(keymapArgs, hasTextSelection, out var repeatToken) && repeatToken is not null)
        {
            _pendingSentenceList.Add(repeatToken);
            return false;
        }

        _pendingSentenceList.Clear();
        return false;
    }

    private bool ContinueFromModifier(KeymapArgs keymapArgs, bool hasTextSelection)
    {
        if (SyntaxTextObjectVim.TryLex(keymapArgs, hasTextSelection, out var textObjectToken) && textObjectToken is not null)
        {
            _pendingSentenceList.Add(textObjectToken);
            return true;
        }

        _pendingSentenceList.Clear();
        return false;
    }

    private bool ContinueFromTextObject(KeymapArgs keymapArgs, bool hasTextSelection)
    {
        // Suppress unused parameter warnings because these will likely be used once further Vim emulation is implemented.
        _ = keymapArgs;
        _ = hasTextSelection;

        // This state should not occur as a TextObject always ends a sentence if it is there.
        _pendingSentenceList.Clear();
        return false;
    }

    private bool ContinueFromRepeat(KeymapArgs keymapArgs, bool hasTextSelection)
    {
        if (SyntaxVerbVim.TryLex(keymapArgs, hasTextSelection, out var verbToken) && verbToken is not null)
        {
            _pendingSentenceList.Add(verbToken);
            return false;
        }
        
        if (SyntaxTextObjectVim.TryLex(keymapArgs, hasTextSelection, out var textObjectToken) && textObjectToken is not null)
        {
            _pendingSentenceList.Add(textObjectToken);
            return true;
        }
        
        if (SyntaxRepeatVim.TryLex(keymapArgs, hasTextSelection, out var repeatToken) && repeatToken is not null)
        {
            _pendingSentenceList.Add(repeatToken);
            return false;
        }

        _pendingSentenceList.Clear();
        return false;
    }

    public static bool TryParseNextToken(
        TextEditorKeymapVim textEditorKeymapVim,
        ImmutableArray<VimGrammarToken> sentenceSnapshotList,
        int indexInSentence,
        KeymapArgs keymapArgs,
        bool hasTextSelection,
        out TextEditorCommand? command)
    {
        if (indexInSentence >= sentenceSnapshotList.Length)
        {
            command = null;
            return false;
        }

        var currentToken = sentenceSnapshotList[indexInSentence];

        var success = false;

        success = currentToken.VimGrammarKind switch
        {
            VimGrammarKind.Verb => SyntaxVerbVim.TryParse(
                textEditorKeymapVim, sentenceSnapshotList, indexInSentence, keymapArgs, hasTextSelection, out command),
            VimGrammarKind.Modifier => SyntaxModifierVim.TryParse(
                textEditorKeymapVim, sentenceSnapshotList, indexInSentence, keymapArgs, hasTextSelection, out command),
            VimGrammarKind.TextObject => SyntaxTextObjectVim.TryParse(
                textEditorKeymapVim, sentenceSnapshotList, indexInSentence, keymapArgs, hasTextSelection, out command),
            VimGrammarKind.Repeat => SyntaxRepeatVim.TryParse(
                textEditorKeymapVim, sentenceSnapshotList, indexInSentence, keymapArgs, hasTextSelection, out command),
            _ => throw new LuthetusTextEditorException($"The {nameof(VimGrammarKind)}: {sentenceSnapshotList.Last().VimGrammarKind} was not recognized."),
        };

        if (success && command is not null)
        {
            if (textEditorKeymapVim.ActiveVimMode == VimMode.Visual)
                command = TextEditorCommandVimFacts.Motions.GetVisualFactory(command, command.DisplayName);
            if (textEditorKeymapVim.ActiveVimMode == VimMode.VisualLine)
                command = TextEditorCommandVimFacts.Motions.GetVisualLineFactory(command, command.DisplayName);
        }

        return success;
    }
}