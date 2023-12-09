using System.Collections.Immutable;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices;

public static class WhitespaceFactsTests
{
    public const char SPACE = ' ';
    public const char TAB = '\t';
    public const char CARRIAGE_RETURN = '\r';
    public const char LINE_FEED = '\n';

    public static readonly ImmutableArray<char> ALL_BAG = new[]
    {
        SPACE,
        TAB,
        CARRIAGE_RETURN,
        LINE_FEED,
    }.ToImmutableArray();

    public static readonly ImmutableArray<char> LINE_ENDING_CHARACTERS = new[]
    {
        CARRIAGE_RETURN,
        LINE_FEED,
    }.ToImmutableArray();
}