using Luthetus.Common.RazorLib.FileSystem.Models;

namespace Luthetus.Ide.RazorLib.InputFileCase.Models;

public class InputFilePattern
{
    public InputFilePattern(
        string patternName,
        Func<IAbsolutePath, bool> matchesPatternFunc)
    {
        PatternName = patternName;
        MatchesPatternFunc = matchesPatternFunc;
    }

    public string PatternName { get; }
    public Func<IAbsolutePath, bool> MatchesPatternFunc { get; }
}