namespace Luthetus.TextEditor.RazorLib.CompilerServices;

public class LexerKeywords
{
    public static readonly LexerKeywords Empty = new LexerKeywords(
        Array.Empty<string>(),
        Array.Empty<string>(),
        Array.Empty<string>());

    public LexerKeywords(
        string[] nonContextualKeywords,
        string[] controlKeywords,
        string[] contextualKeywords)
    {
        NonContextualKeywords = nonContextualKeywords;
        ControlKeywords = controlKeywords;
        ContextualKeywords = contextualKeywords;
        
        AllKeywords = NonContextualKeywords
	        .Union(ContextualKeywords)
	        .ToArray();
    }

    /// <summary>
    /// The <see cref="ControlKeywords"/> are INCLUDED within the <see cref="NonContextualKeywords"/>.
    /// There is a separate <see cref="ControlKeywords"/> list only to provide a different color to the
    /// text.
    /// </summary>
    public string[] NonContextualKeywords { get; }
    /// <summary>
    /// The <see cref="ControlKeywords"/> are INCLUDED within the <see cref="NonContextualKeywords"/>.
    /// There is a separate <see cref="ControlKeywords"/> list only to provide a different color to the
    /// text.
    /// </summary>
    public string[] ControlKeywords { get; }
    public string[] ContextualKeywords { get; }

    public string[] AllKeywords { get; }
}