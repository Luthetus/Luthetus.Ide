namespace Luthetus.TextEditor.RazorLib.Lexers.Models;

public record ResourceUri(string Value)
{
	public static readonly ResourceUri Empty = new ResourceUri(string.Empty);
}