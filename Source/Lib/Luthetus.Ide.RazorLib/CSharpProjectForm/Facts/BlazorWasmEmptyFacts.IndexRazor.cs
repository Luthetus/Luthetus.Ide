using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.Ide.RazorLib.CSharpProjectForm.Facts;

public static partial class BlazorWasmEmptyFacts
{
    public const string INDEX_RAZOR_RELATIVE_FILE_PATH = @"Pages/Index.razor";

    public static string GetIndexRazorContents(string projectName) => @$"@page ""/""

<h1>Hello, world!</h1>
";
}
