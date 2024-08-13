namespace Luthetus.TextEditor.RazorLib.Characters.Models;

public struct RichCharacter
{
	public RichCharacter(char value, byte decorationByte)
	{
		Value = value;
		DecorationByte = decorationByte;
	}

    public char Value { get; }
    
    /// <summary>
    /// The decoration byte is expected to change "on the fly"
    /// its solely for the UI to color the text and therefore has a setter.
    /// </summary>
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