namespace Luthetus.Ide.RazorLib.WebsiteProjectTemplatesCase.Models;

public static partial class BlazorServerEmptyFacts
{
    public const string HOST_CSHTML_RELATIVE_FILE_PATH = @"Pages/_Host.cshtml";

    public static string GetHostCshtmlContents(string projectName) => @$"@page ""/""
@using Microsoft.AspNetCore.Components.Web
@namespace {projectName}.Pages
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers

<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""utf-8"" />
    <base href=""~/"" />
    <link href=""css/site.css"" rel=""stylesheet"" />
    <component type=""typeof(HeadOutlet)"" render-mode=""ServerPrerendered"" />
</head>
<body>
    <component type=""typeof(App)"" render-mode=""ServerPrerendered"" />

    <div id=""blazor-error-ui"">
        <environment include=""Staging,Production"">
            An error has occurred. This application may no longer respond until reloaded.
        </environment>
        <environment include=""Development"">
            An unhandled exception has occurred. See browser dev tools for details.
        </environment>
        <a href="""" class=""reload"">Reload</a>
        <a class=""dismiss"">🗙</a>
    </div>

    <script src=""_framework/blazor.server.js""></script>
</body>
</html>
";
}
