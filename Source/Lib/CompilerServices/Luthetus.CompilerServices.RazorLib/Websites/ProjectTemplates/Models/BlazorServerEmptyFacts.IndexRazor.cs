namespace Luthetus.CompilerServices.RazorLib.Websites.ProjectTemplates.Models;

public static partial class BlazorServerEmptyFacts
{
    public const string INDEX_RAZOR_RELATIVE_FILE_PATH = @"Pages/Index.razor";

    public static string GetIndexRazorContents(string projectName) => @$"@page ""/""

<h1>Hello, world!</h1>
";
}
