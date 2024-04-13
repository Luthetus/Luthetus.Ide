namespace Luthetus.TextEditor.RazorLib.Characters.Models;

public static class RichCharacterExtensions
{
    public static CharacterKind GetCharacterKind(this RichCharacter richCharacter)
    {
        return CharacterKindHelper.CharToCharacterKind(richCharacter.Value);
    }
}