using Luthetus.TextEditor.RazorLib.Commands.Models.Vims;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;
using Luthetus.Common.RazorLib.Keymaps.Models;

namespace Luthetus.TextEditor.Tests.Basis.Keymaps.Models.Vims;

public class VimSentenceTests
{
    private readonly List<VimGrammarToken> _pendingSentenceBag = new();

    public ImmutableArray<VimGrammarToken> PendingSentenceBag => _pendingSentenceBag.ToImmutableArray();

    public bool TryLex(
        TextEditorKeymapVim textEditorKeymapVim,
        KeymapArgument keymapArgument,
        bool hasTextSelection,
        out TextEditorCommand? textEditorCommand)
    {
        bool sentenceIsSyntacticallyComplete;

        var mostRecentToken = _pendingSentenceBag.LastOrDefault();

        if (mostRecentToken is null)
        {
            sentenceIsSyntacticallyComplete = ContinueFromStart(keymapArgument, hasTextSelection);
        }
        else
        {
            sentenceIsSyntacticallyComplete = mostRecentToken.VimGrammarKind switch
            {
                VimGrammarKind.Verb => ContinueFromVerb(keymapArgument, hasTextSelection),
                VimGrammarKind.Modifier => ContinueFromModifier(keymapArgument, hasTextSelection),
                VimGrammarKind.TextObject => ContinueFromTextObject(keymapArgument, hasTextSelection),
                VimGrammarKind.Repeat => ContinueFromRepeat(keymapArgument, hasTextSelection),
                _ => throw new ApplicationException($"The {nameof(VimGrammarKind)}: {_pendingSentenceBag.Last().VimGrammarKind} was not recognized."),
            };
        }

        if (sentenceIsSyntacticallyComplete)
        {
            var sentenceSnapshot = PendingSentenceBag;
            _pendingSentenceBag.Clear();

            return TryParseNextToken(
                textEditorKeymapVim,
                sentenceSnapshot,
                0,
                keymapArgument,
                hasTextSelection,
                out textEditorCommand);
        }

        textEditorCommand = TextEditorCommandDefaultFacts.DoNothingDiscard;
        return true;
    }

    private bool ContinueFromStart(KeymapArgument keymapArgument, bool hasTextSelection)
    {
        if (SyntaxVerbVim.TryLex(keymapArgument, hasTextSelection, out var verbToken) && verbToken is not null)
        {
            if (keymapArgument.CtrlKey)
            {
                // This if case relates to 'Ctrl + e' which does not get
                // double tapped instead it only takes one press of the keymap
                _pendingSentenceBag.Clear();
                _pendingSentenceBag.Add(verbToken);

                return true;
            }

            if (hasTextSelection)
            {
                _pendingSentenceBag.Clear();
                _pendingSentenceBag.Add(verbToken);

                return true;
            }

            _pendingSentenceBag.Add(verbToken);
            return false;
        }

        if (SyntaxTextObjectVim.TryLex(keymapArgument, hasTextSelection, out var textObjectToken) && textObjectToken is not null)
        {
            _pendingSentenceBag.Add(textObjectToken);
            return true;
        }

        if (SyntaxRepeatVim.TryLex(keymapArgument, hasTextSelection, out var repeatToken) && repeatToken is not null)
        {
            _pendingSentenceBag.Add(repeatToken);
            return false;
        }

        _pendingSentenceBag.Clear();
        return false;
    }

    private bool ContinueFromVerb(KeymapArgument keymapArgument, bool hasTextSelection)
    {
        if (SyntaxVerbVim.TryLex(keymapArgument, hasTextSelection, out var verbToken) && verbToken is not null)
        {
            if (_pendingSentenceBag.Last().KeymapArgument.Code == verbToken.KeymapArgument.Code)
            {
                _pendingSentenceBag.Add(verbToken);
                return true;
            }

            if (keymapArgument.CtrlKey)
            {
                // This if case relates to 'Ctrl + e' which does not get
                // double tapped instead it only takes one press of the keymap
                _pendingSentenceBag.Clear();
                _pendingSentenceBag.Add(verbToken);

                return true;
            }

            // The verb was overriden so restart sentence
            _pendingSentenceBag.Clear();

            return ContinueFromStart(keymapArgument, hasTextSelection);
        }
        
        if (SyntaxModifierVim.TryLex(keymapArgument, hasTextSelection, out var modifierToken) && modifierToken is not null)
        {
            _pendingSentenceBag.Add(modifierToken);
            return false;
        }
        
        if (SyntaxTextObjectVim.TryLex(keymapArgument, hasTextSelection, out var textObjectToken) && textObjectToken is not null)
        {
            _pendingSentenceBag.Add(textObjectToken);
            return true;
        }
        
        if (SyntaxRepeatVim.TryLex(keymapArgument, hasTextSelection, out var repeatToken) && repeatToken is not null)
        {
            _pendingSentenceBag.Add(repeatToken);
            return false;
        }

        _pendingSentenceBag.Clear();
        return false;
    }

    private bool ContinueFromModifier(KeymapArgument keymapArgument, bool hasTextSelection)
    {
        if (SyntaxTextObjectVim.TryLex(keymapArgument, hasTextSelection, out var textObjectToken) && textObjectToken is not null)
        {
            _pendingSentenceBag.Add(textObjectToken);
            return true;
        }

        _pendingSentenceBag.Clear();
        return false;
    }

    private bool ContinueFromTextObject(KeymapArgument keymapArgument, bool hasTextSelection)
    {
        // Suppress unused parameter warnings because these will likely be used once further Vim emulation is implemented.
        _ = keymapArgument;
        _ = hasTextSelection;

        // This state should not occur as a TextObject always ends a sentence if it is there.
        _pendingSentenceBag.Clear();
        return false;
    }

    private bool ContinueFromRepeat(KeymapArgument keymapArgument, bool hasTextSelection)
    {
        if (SyntaxVerbVim.TryLex(keymapArgument, hasTextSelection, out var verbToken) && verbToken is not null)
        {
            _pendingSentenceBag.Add(verbToken);
            return false;
        }
        
        if (SyntaxTextObjectVim.TryLex(keymapArgument, hasTextSelection, out var textObjectToken) && textObjectToken is not null)
        {
            _pendingSentenceBag.Add(textObjectToken);
            return true;
        }
        
        if (SyntaxRepeatVim.TryLex(keymapArgument, hasTextSelection, out var repeatToken) && repeatToken is not null)
        {
            _pendingSentenceBag.Add(repeatToken);
            return false;
        }

        _pendingSentenceBag.Clear();
        return false;
    }

    public static bool TryParseNextToken(
        TextEditorKeymapVim textEditorKeymapVim,
        ImmutableArray<VimGrammarToken> sentenceSnapshotBag,
        int indexInSentence,
        KeymapArgument keymapArgument,
        bool hasTextSelection,
        out TextEditorCommand? command)
    {
        if (indexInSentence >= sentenceSnapshotBag.Length)
        {
            command = null;
            return false;
        }

        var currentToken = sentenceSnapshotBag[indexInSentence];

        var success = false;

        success = currentToken.VimGrammarKind switch
        {
            VimGrammarKind.Verb => SyntaxVerbVim.TryParse(
                textEditorKeymapVim, sentenceSnapshotBag, indexInSentence, keymapArgument, hasTextSelection, out command),
            VimGrammarKind.Modifier => SyntaxModifierVim.TryParse(
                textEditorKeymapVim, sentenceSnapshotBag, indexInSentence, keymapArgument, hasTextSelection, out command),
            VimGrammarKind.TextObject => SyntaxTextObjectVim.TryParse(
                textEditorKeymapVim, sentenceSnapshotBag, indexInSentence, keymapArgument, hasTextSelection, out command),
            VimGrammarKind.Repeat => SyntaxRepeatVim.TryParse(
                textEditorKeymapVim, sentenceSnapshotBag, indexInSentence, keymapArgument, hasTextSelection, out command),
            _ => throw new ApplicationException($"The {nameof(VimGrammarKind)}: {sentenceSnapshotBag.Last().VimGrammarKind} was not recognized."),
        };

        if (success && command is not null)
        {
            if (textEditorKeymapVim.ActiveVimMode == VimMode.Visual)
                command = TextEditorCommandVimFacts.Motions.GetVisual(command, command.DisplayName);
            if (textEditorKeymapVim.ActiveVimMode == VimMode.VisualLine)
                command = TextEditorCommandVimFacts.Motions.GetVisualLine(command, command.DisplayName);
        }

        return success;
    }
}