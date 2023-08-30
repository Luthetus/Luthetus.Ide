using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.Ide.ClassLib.WebsiteProjectTemplates.BlazorServerEmptyCase;

public static partial class BlazorServerEmptyFacts
{
    public const string IMPORTS_RAZOR_RELATIVE_FILE_PATH = @"_Imports.razor";

    public static string GetImportsRazorContents(string projectName) => @$"@using Microsoft.AspNetCore.Components.Routing
@using Microsoft.AspNetCore.Components.Web
@using Microsoft.JSInterop
@using {projectName}
";
}
