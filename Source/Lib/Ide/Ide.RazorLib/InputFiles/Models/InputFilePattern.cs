using Luthetus.Common.RazorLib.FileSystems.Models;

namespace Luthetus.Ide.RazorLib.InputFiles.Models;

/// <summary>
/// If either `PatternName` or `MatchesPatternFunc` is null then this is default.
/// </summary>
public struct InputFilePattern
{
    public InputFilePattern(string patternName, Func<AbsolutePath, bool> matchesPatternFunc)
    {
        PatternName = patternName;
        MatchesPatternFunc = matchesPatternFunc;
    }

    public string PatternName { get; }
    public Func<AbsolutePath, bool> MatchesPatternFunc { get; }
    
    public bool ConstructorWasInvoked => PatternName is null || MatchesPatternFunc is null;
}