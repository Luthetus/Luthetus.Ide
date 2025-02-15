using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.Ide.RazorLib.FileSystems.Models;

public static class HiddenFileFacts
{
    public const string BIN = "bin";
    public const string OBJ = "obj";

    private static readonly List<string> _empty = new();

    /// <summary>
    /// If rendering a .csproj file pass in <see cref="ExtensionNoPeriodFacts.C_SHARP_PROJECT"/>
    ///
    /// Then perhaps the returning array would contain { "bin", "obj" } as they should be hidden
    /// with this context.
    /// </summary>
    /// <returns></returns>
    public static List<string> GetHiddenFilesByContainerFileExtension(string extensionNoPeriod)
    {
        return extensionNoPeriod switch
        {
            ExtensionNoPeriodFacts.C_SHARP_PROJECT => new() { BIN, OBJ },
            _ => _empty
		};
    }
}