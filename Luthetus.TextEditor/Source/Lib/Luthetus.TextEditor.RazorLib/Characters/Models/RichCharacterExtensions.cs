using Luthetus.Common.RazorLib.Keyboards.Models;

namespace Luthetus.TextEditor.RazorLib.Characters.Models;

public static class RichCharacterExtensions
{
    public static CharacterKind GetCharacterKind(this RichCharacter richCharacter)
    {
        if (KeyboardKeyFacts.IsWhitespaceCharacter(richCharacter.Value))
            return CharacterKind.Whitespace;
        if (KeyboardKeyFacts.IsPunctuationCharacter(richCharacter.Value))
            return CharacterKind.Punctuation;
        return CharacterKind.LetterOrDigit;
    }
}