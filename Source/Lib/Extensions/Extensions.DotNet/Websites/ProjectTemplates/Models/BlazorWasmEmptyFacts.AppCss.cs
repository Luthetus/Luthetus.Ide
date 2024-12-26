namespace Luthetus.Extensions.DotNet.Websites.ProjectTemplates.Models;

public static partial class BlazorWasmEmptyFacts
{
    public const string APP_CSS_RELATIVE_FILE_PATH = @"wwwroot/css/app.css";

    public static string GetAppCssContents(string projectName) =>
@"h1:focus {
    outline: none;
}
";
}
