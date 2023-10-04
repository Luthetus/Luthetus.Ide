namespace Luthetus.TextEditor.RazorLib.Decorations.Models;

public interface IDecorationMapper
{
    /// <summary>
    /// Maps a <see cref="decorationByte" /> to a CSS class that is in the form of a string
    /// </summary>
    public string Map(byte decorationByte);
}