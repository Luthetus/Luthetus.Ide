namespace Luthetus.Ide.RazorLib.WebsiteProjectTemplates.Models;

public partial class XUnitFacts
{
    public const string USINGS_CS_RELATIVE_FILE_PATH = @"Usings.cs";

    public static string GetUsingsCsContents(string projectName) => @$"global using Xunit;";
}
