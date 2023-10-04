using Luthetus.TextEditor.RazorLib.Commands.Models.Vims;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;
using Luthetus.Common.RazorLib.Keymaps.Models;

namespace Luthetus.TextEditor.RazorLib.Keymaps.Models.Vims;

public class VimSentence
{
    private readonly List<VimGrammarToken> _pendingSentenceBag = new();

    public ImmutableArray<VimGrammarToken> PendingSentenceBag => _pendingSentenceBag.ToImmutableArray();

    public bool TryLex(
        TextEditorKeymapVim textEditorKeymapVim,
        KeymapArgument keymapArgument,
        bool hasTextSelection,
        out CommandTextEditor? textEditorCommand)
    {
        bool sentenceIsSyntacticallyComplete;

        var mostRecentToken = _pendingSentenceBag.LastOrDefault();

        if (mostRecentToken is null)
        {
            sentenceIsSyntacticallyComplete = ContinueFromStart(keymapArgument, hasTextSelection);
        }
        else
        {
            switch (mostRecentToken.VimGrammarKind)
            {
                case VimGrammarKind.Verb:
                    {
                        sentenceIsSyntacticallyComplete = ContinueFromVerb(keymapArgument, hasTextSelection);
                        break;
                    }
                case VimGrammarKind.Modifier:
                    {
                        sentenceIsSyntacticallyComplete = ContinueFromModifier(keymapArgument, hasTextSelection);
                        break;
                    }
                case VimGrammarKind.TextObject:
                    {
                        sentenceIsSyntacticallyComplete = ContinueFromTextObject(keymapArgument, hasTextSelection);
                        break;
                    }
                case VimGrammarKind.Repeat:
                    {
                        sentenceIsSyntacticallyComplete = ContinueFromRepeat(keymapArgument, hasTextSelection);
                        break;
                    }
                default:
                    {
                        throw new ApplicationException($"The {nameof(VimGrammarKind)}: {_pendingSentenceBag.Last().VimGrammarKind} was not recognized.");
                    }
            }
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
        VimGrammarToken? vimGrammarToken;

        _ = SyntaxVerbVim.TryLex(keymapArgument, hasTextSelection, out vimGrammarToken) ||
            SyntaxTextObjectVim.TryLex(keymapArgument, hasTextSelection, out vimGrammarToken) ||
            SyntaxRepeatVim.TryLex(keymapArgument, hasTextSelection, out vimGrammarToken);

        if (vimGrammarToken is null)
        {
            _pendingSentenceBag.Clear();
            return false;
        }

        switch (vimGrammarToken.VimGrammarKind)
        {
            case VimGrammarKind.Verb:
                {
                    if (keymapArgument.CtrlKey)
                    {
                        // This if case relates to 'Ctrl + e' which does not get
                        // double tapped instead it only takes one press of the keymap
                        _pendingSentenceBag.Clear();
                        _pendingSentenceBag.Add(vimGrammarToken);

                        return true;
                    }

                    if (hasTextSelection)
                    {
                        _pendingSentenceBag.Clear();
                        _pendingSentenceBag.Add(vimGrammarToken);

                        return true;
                    }

                    _pendingSentenceBag.Add(vimGrammarToken);
                    return false;
                }
            case VimGrammarKind.TextObject:
                {
                    _pendingSentenceBag.Add(vimGrammarToken);
                    return true;
                }
            case VimGrammarKind.Repeat:
                {
                    _pendingSentenceBag.Add(vimGrammarToken);
                    return false;
                }
        }

        return false;
    }

    private bool ContinueFromVerb(KeymapArgument keymapArgument, bool hasTextSelection)
    {
        VimGrammarToken? vimGrammarToken;

        _ = SyntaxVerbVim.TryLex(keymapArgument, hasTextSelection, out vimGrammarToken) ||
            SyntaxTextObjectVim.TryLex(keymapArgument, hasTextSelection, out vimGrammarToken) ||
            SyntaxRepeatVim.TryLex(keymapArgument, hasTextSelection, out vimGrammarToken);

        if (vimGrammarToken is null)
        {
            _pendingSentenceBag.Clear();
            return false;
        }

        switch (vimGrammarToken.VimGrammarKind)
        {
            case VimGrammarKind.Verb:
                {
                    if (_pendingSentenceBag.Last().KeymapArgument.Code == vimGrammarToken.KeymapArgument.Code)
                    {
                        _pendingSentenceBag.Add(vimGrammarToken);
                        return true;
                    }

                    if (keymapArgument.CtrlKey)
                    {
                        // This if case relates to 'Ctrl + e' which does not get
                        // double tapped instead it only takes one press of the keymap
                        _pendingSentenceBag.Clear();
                        _pendingSentenceBag.Add(vimGrammarToken);

                        return true;
                    }

                    // The verb was overriden so restart sentence
                    _pendingSentenceBag.Clear();

                    return ContinueFromStart(keymapArgument, hasTextSelection);
                }
            case VimGrammarKind.Modifier:
                {
                    _pendingSentenceBag.Add(vimGrammarToken);
                    return false;
                }
            case VimGrammarKind.TextObject:
                {
                    _pendingSentenceBag.Add(vimGrammarToken);
                    return true;
                }
            case VimGrammarKind.Repeat:
                {
                    _pendingSentenceBag.Add(vimGrammarToken);
                    return false;
                }
        }

        return false;
    }

    private bool ContinueFromModifier(KeymapArgument keymapArgument, bool hasTextSelection)
    {
        VimGrammarToken? vimGrammarToken;

        _ = SyntaxTextObjectVim.TryLex(keymapArgument, hasTextSelection, out vimGrammarToken);

        if (vimGrammarToken is null)
        {
            _pendingSentenceBag.Clear();
            return false;
        }

        switch (vimGrammarToken.VimGrammarKind)
        {
            case VimGrammarKind.TextObject:
                {
                    _pendingSentenceBag.Add(vimGrammarToken);
                    return true;
                }
        }

        return false;
    }

    private bool ContinueFromTextObject(KeymapArgument keymapArgument, bool hasTextSelection)
    {
        // This state should not occur as a TextObject always ends a sentence if it is there.
        _pendingSentenceBag.Clear();
        return false;
    }

    private bool ContinueFromRepeat(KeymapArgument keymapArgument, bool hasTextSelection)
    {
        VimGrammarToken? vimGrammarToken;

        _ = SyntaxVerbVim.TryLex(keymapArgument, hasTextSelection, out vimGrammarToken) ||
            SyntaxTextObjectVim.TryLex(keymapArgument, hasTextSelection, out vimGrammarToken) ||
            SyntaxRepeatVim.TryLex(keymapArgument, hasTextSelection, out vimGrammarToken);

        if (vimGrammarToken is null)
        {
            _pendingSentenceBag.Clear();
            return false;
        }

        switch (vimGrammarToken.VimGrammarKind)
        {
            case VimGrammarKind.Verb:
                {
                    _pendingSentenceBag.Add(vimGrammarToken);
                    return false;
                }
            case VimGrammarKind.TextObject:
                {
                    _pendingSentenceBag.Add(vimGrammarToken);
                    return true;
                }
            case VimGrammarKind.Repeat:
                {
                    _pendingSentenceBag.Add(vimGrammarToken);
                    return false;
                }
        }

        return false;
    }

    public static bool TryParseNextToken(
        TextEditorKeymapVim textEditorKeymapVim,
        ImmutableArray<VimGrammarToken> sentenceSnapshotBag,
        int indexInSentence,
        KeymapArgument keymapArgument,
        bool hasTextSelection,
        out CommandTextEditor? command)
    {
        if (indexInSentence >= sentenceSnapshotBag.Length)
        {
            command = null;
            return false;
        }

        var currentToken = sentenceSnapshotBag[indexInSentence];

        var success = false;

        switch (currentToken.VimGrammarKind)
        {
            case VimGrammarKind.Verb:
                success = SyntaxVerbVim.TryParse(
                    textEditorKeymapVim,
                    sentenceSnapshotBag,
                    indexInSentence,
                    keymapArgument,
                    hasTextSelection,
                    out command);

                break;
            case VimGrammarKind.Modifier:
                success = SyntaxModifierVim.TryParse(
                    textEditorKeymapVim,
                    sentenceSnapshotBag,
                    indexInSentence,
                    keymapArgument,
                    hasTextSelection,
                    out command);

                break;
            case VimGrammarKind.TextObject:
                success = SyntaxTextObjectVim.TryParse(
                    textEditorKeymapVim,
                    sentenceSnapshotBag,
                    indexInSentence,
                    keymapArgument,
                    hasTextSelection,
                    out command);

                break;
            case VimGrammarKind.Repeat:
                success = SyntaxRepeatVim.TryParse(
                    textEditorKeymapVim,
                    sentenceSnapshotBag,
                    indexInSentence,
                    keymapArgument,
                    hasTextSelection,
                    out command);

                break;
            default:
                throw new ApplicationException($"The {nameof(VimGrammarKind)}: {sentenceSnapshotBag.Last().VimGrammarKind} was not recognized.");
        }

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