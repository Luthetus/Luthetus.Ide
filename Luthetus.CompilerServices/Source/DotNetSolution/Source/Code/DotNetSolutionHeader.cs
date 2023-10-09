namespace Luthetus.CompilerServices.Lang.DotNetSolution.Code;

public record DotNetSolutionHeader
{
    public DotNetSolutionHeader()
    {
    }

    public DotNetSolutionHeader(
            AssociatedEntryPair? formatVersionStartToken,
            AssociatedEntryPair? hashtagVisualStudioVersionStartToken,
            AssociatedEntryPair? exactVisualStudioVersionStartToken,
            AssociatedEntryPair? minimumVisualStudioVersionStartToken)
        : this()
    {
        FormatVersionStartToken = formatVersionStartToken;
        HashtagVisualStudioVersionStartToken = hashtagVisualStudioVersionStartToken;
        ExactVisualStudioVersionStartToken = exactVisualStudioVersionStartToken;
        MinimumVisualStudioVersionStartToken = minimumVisualStudioVersionStartToken;
    }

    public AssociatedEntryPair? FormatVersionStartToken { get; init; }
    public AssociatedEntryPair? HashtagVisualStudioVersionStartToken { get; init; }
    public AssociatedEntryPair? ExactVisualStudioVersionStartToken { get; init; }
    public AssociatedEntryPair? MinimumVisualStudioVersionStartToken { get; init; }
}
