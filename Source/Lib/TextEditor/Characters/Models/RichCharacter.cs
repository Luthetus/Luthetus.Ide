namespace Luthetus.TextEditor.RazorLib.Characters.Models;

public class RichCharacter
{
    public char Value { get; init; }
    public byte DecorationByte { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is not RichCharacter otherRichCharacter)
            return false;

        return otherRichCharacter.Value == Value &&
            otherRichCharacter.DecorationByte == DecorationByte;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}