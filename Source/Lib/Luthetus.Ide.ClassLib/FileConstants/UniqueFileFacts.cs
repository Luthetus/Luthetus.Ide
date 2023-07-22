using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.FileConstants;

public static class UniqueFileFacts
{
    public const string Properties = "Properties";
    public const string WwwRoot = "wwwroot";

    /// <summary>
    /// If rendering a .csproj file pass in <see cref="ExtensionNoPeriodFacts.C_SHARP_PROJECT"/>
    ///
    /// Then perhaps the returning array would contain { "Properties", "wwwroot" } as they are unique files
    /// with this context.
    /// </summary>
    /// <returns></returns>
    public static ImmutableArray<string> GetUniqueFilesByContainerFileExtension(string extensionNoPeriod)
    {
        return extensionNoPeriod switch
        {
            ExtensionNoPeriodFacts.C_SHARP_PROJECT => new[] { Properties, WwwRoot }.ToImmutableArray(),
            _ => ImmutableArray<string>.Empty
        };
    }
}