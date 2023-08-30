using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.Ide.ClassLib.WebsiteProjectTemplates.ClassLibCase;

public partial class ClassLibFacts
{
    public const string CLASS_1_CS_RELATIVE_FILE_PATH = @"Class1.cs";

    public static string GetClass1CsContents(string projectName) => @$"namespace {projectName}
{{
    public class Class1
    {{

    }}
}}";
}
