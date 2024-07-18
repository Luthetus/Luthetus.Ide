namespace Luthetus.Extensions.DotNet.Websites.ProjectTemplates.Models;

public static partial class BlazorWasmEmptyFacts
{
    public const string MAIN_LAYOUT_RAZOR_RELATIVE_FILE_PATH = @"MainLayout.razor";

    public static string GetMainLayoutRazorContents(string projectName) => @$"@inherits LayoutComponentBase

<main>
    @Body
</main>
";
}
