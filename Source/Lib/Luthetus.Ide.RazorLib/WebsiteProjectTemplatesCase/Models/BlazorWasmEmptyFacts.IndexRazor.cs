namespace Luthetus.Ide.RazorLib.WebsiteProjectTemplatesCase.Models;

public static partial class BlazorWasmEmptyFacts
{
    public const string INDEX_RAZOR_RELATIVE_FILE_PATH = @"Pages/Index.razor";

    public static string GetIndexRazorContents(string projectName) => @$"@page ""/""

<h1>Hello, world!</h1>
";
}
