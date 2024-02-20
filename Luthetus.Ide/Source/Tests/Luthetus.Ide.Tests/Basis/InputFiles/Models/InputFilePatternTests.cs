using Luthetus.Common.RazorLib.FileSystems.Models;

namespace Luthetus.Ide.Tests.Basis.InputFiles.Models;

public class InputFilePatternTests
{
    public InputFilePattern(string patternName, Func<IAbsolutePath, bool> matchesPatternFunc)
    {
        PatternName = patternName;
        MatchesPatternFunc = matchesPatternFunc;
    }

    public string PatternName { get; }
    public Func<IAbsolutePath, bool> MatchesPatternFunc { get; }
}