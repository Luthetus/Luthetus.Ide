using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.Ide.RazorLib.CSharpProjectForm.Facts;

public static partial class BlazorWasmEmptyFacts
{
    public const string MAIN_LAYOUT_RAZOR_RELATIVE_FILE_PATH = @"MainLayout.razor";

    public static string GetMainLayoutRazorContents(string projectName) => @$"@inherits LayoutComponentBase

<main>
    @Body
</main>
";
}
