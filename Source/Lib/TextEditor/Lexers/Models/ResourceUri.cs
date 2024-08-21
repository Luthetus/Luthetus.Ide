namespace Luthetus.TextEditor.RazorLib.Lexers.Models;

public record struct ResourceUri(string Value)
{
	public static readonly ResourceUri Empty = new(string.Empty);
}