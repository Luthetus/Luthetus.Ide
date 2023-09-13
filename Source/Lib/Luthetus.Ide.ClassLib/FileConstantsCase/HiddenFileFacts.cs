using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.FileConstantsCase;

public static class HiddenFileFacts
{
    public const string BIN = "bin";
    public const string OBJ = "obj";

    /// <summary>
    /// If rendering a .csproj file pass in <see cref="ExtensionNoPeriodFacts.C_SHARP_PROJECT"/>
    ///
    /// Then perhaps the returning array would contain { "bin", "obj" } as they should be hidden
    /// with this context.
    /// </summary>
    /// <returns></returns>
    public static ImmutableArray<string> GetHiddenFilesByContainerFileExtension(string extensionNoPeriod)
    {
        return extensionNoPeriod switch
        {
            ExtensionNoPeriodFacts.C_SHARP_PROJECT => new[] { BIN, OBJ }.ToImmutableArray(),
            _ => ImmutableArray<string>.Empty
        };
    }
}