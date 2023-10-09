namespace Luthetus.CompilerServices.Lang.DotNetSolution.Code;

public record DotNetSolutionHeader
{
    public DotNetSolutionHeader(
        string formatVersionStartToken,
        string hashtagVisualStudioVersionStartToken,
        string exactVisualStudioVersionStartToken,
        string minimumVisualStudioVersionStartToken)
    {
        FormatVersionStartToken = formatVersionStartToken;
        HashtagVisualStudioVersionStartToken = hashtagVisualStudioVersionStartToken;
        ExactVisualStudioVersionStartToken = exactVisualStudioVersionStartToken;
        MinimumVisualStudioVersionStartToken = minimumVisualStudioVersionStartToken;
    }

    public string FormatVersionStartToken { get; init; }
    public string HashtagVisualStudioVersionStartToken { get; init; }
    public string ExactVisualStudioVersionStartToken { get; init; }
    public string MinimumVisualStudioVersionStartToken { get; init; }
}
