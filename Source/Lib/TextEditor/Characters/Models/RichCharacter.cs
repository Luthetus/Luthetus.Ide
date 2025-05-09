namespace Luthetus.TextEditor.RazorLib.Characters.Models;

public struct RichCharacter
{
	public RichCharacter(char value, byte decorationByte)
	{
		Value = value;
		DecorationByte = decorationByte;
	}

    public char Value;
    
    /// <summary>
    /// The decoration byte is expected to change "on the fly"
    /// its solely for the UI to color the text and therefore has a setter.
    /// </summary>
    public byte DecorationByte;
}
