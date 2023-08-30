namespace Luthetus.Ide.ClassLib.WebsiteProjectTemplates.BlazorWasmEmptyCase;

public static partial class BlazorWasmEmptyFacts
{
    public const string INDEX_HTML_RELATIVE_FILE_PATH = @"wwwroot/index.html";

    public static string GetIndexHtmlContents(string projectName) => @$"<!DOCTYPE html>
<html lang=""en"">

<head>
    <meta charset=""utf-8"" />
    <title>{projectName}</title>
    <base href=""/"" />
    <link href=""css/app.css"" rel=""stylesheet"" />

    <!-- If you add any scoped CSS files, uncomment the following to load them
    <link href=""{projectName}.styles.css"" rel=""stylesheet"" /> -->
</head>

<body>
    <div id=""app"">Loading...</div>

    <div id=""blazor-error-ui"">
        An unhandled error has occurred.
        <a href="""" class=""reload"">Reload</a>
        <a class=""dismiss"">🗙</a>
    </div>
    <script src=""_framework/blazor.webassembly.js""></script>
</body>

</html>
";
}
