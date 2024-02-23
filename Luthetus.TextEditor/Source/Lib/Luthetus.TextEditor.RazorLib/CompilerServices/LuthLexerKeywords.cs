using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices;

public class LuthLexerKeywords
{
    public LuthLexerKeywords(
        ImmutableArray<string> nonContextualKeywords,
        ImmutableArray<string> controlKeywords,
        ImmutableArray<string> contextualKeywords)
    {
        NonContextualKeywords = nonContextualKeywords;
        ControlKeywords = controlKeywords;
        ContextualKeywords = contextualKeywords;
    }

    /// <summary>
    /// The <see cref="ControlKeywords"/> are INCLUDED within the <see cref="NonContextualKeywords"/>.
    /// There is a separate <see cref="ControlKeywords"/> list only to provide a different color to the
    /// text.
    /// </summary>
    public ImmutableArray<string> NonContextualKeywords { get; }
    /// <summary>
    /// The <see cref="ControlKeywords"/> are INCLUDED within the <see cref="NonContextualKeywords"/>.
    /// There is a separate <see cref="ControlKeywords"/> list only to provide a different color to the
    /// text.
    /// </summary>
    public ImmutableArray<string> ControlKeywords { get; }
    public ImmutableArray<string> ContextualKeywords { get; }

    public ImmutableArray<string> AllKeywords => NonContextualKeywords
        .Union(ContextualKeywords)
        .ToImmutableArray();
}