using Luthetus.Common.RazorLib.FileSystem.Interfaces;

namespace Luthetus.Ide.ClassLib.InputFile;

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