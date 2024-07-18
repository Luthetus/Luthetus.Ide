namespace Luthetus.Extensions.DotNet.Websites.ProjectTemplates.Models;

public partial class RazorClassLibFacts
{
    public const string IMPORTS_RAZOR_RELATIVE_FILE_PATH = @"_Imports.razor";

    public static string GetImportsRazorContents(string projectName) => @$"@using Microsoft.AspNetCore.Components.Web
";
}
