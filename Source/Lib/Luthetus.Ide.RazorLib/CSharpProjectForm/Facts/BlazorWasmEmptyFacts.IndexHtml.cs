using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.Ide.RazorLib.CSharpProjectForm.Facts;

public static partial class BlazorWasmEmptyFacts
{
    public const string INDEX_HTML_ABSOLUTE_FILE_PATH = @"/BlazorWasmEmpty/wwwroot/index.html";
    public const string INDEX_HTML_CONTENTS = @"<!DOCTYPE html>
<html lang=""en"">

<head>
    <meta charset=""utf-8"" />
    <title>BlazorWasmApp-empty</title>
    <base href=""/"" />
    <link href=""css/app.css"" rel=""stylesheet"" />

    <!-- If you add any scoped CSS files, uncomment the following to load them
    <link href=""BlazorWasmApp_empty.styles.css"" rel=""stylesheet"" /> -->
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
