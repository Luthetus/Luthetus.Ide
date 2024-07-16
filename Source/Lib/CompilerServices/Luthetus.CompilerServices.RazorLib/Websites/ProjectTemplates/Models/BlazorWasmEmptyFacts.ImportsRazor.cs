namespace Luthetus.CompilerServices.RazorLib.Websites.ProjectTemplates.Models;

public static partial class BlazorWasmEmptyFacts
{
    public const string IMPORTS_RAZOR_RELATIVE_FILE_PATH = @"_Imports.razor";

    public static string GetImportsRazorContents(string projectName) => @$"@using System.Net.Http
@using System.Net.Http.Json
@using Microsoft.AspNetCore.Components.Routing
@using Microsoft.AspNetCore.Components.Web
@using Microsoft.AspNetCore.Components.WebAssembly.Http
@using Microsoft.JSInterop
@using {projectName}
";
}
