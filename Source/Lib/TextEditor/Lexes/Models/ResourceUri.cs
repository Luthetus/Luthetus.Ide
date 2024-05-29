namespace Luthetus.TextEditor.RazorLib.Lexes.Models;

public record ResourceUri(string Value)
{
	public static readonly ResourceUri Empty = new ResourceUri(string.Empty);
}