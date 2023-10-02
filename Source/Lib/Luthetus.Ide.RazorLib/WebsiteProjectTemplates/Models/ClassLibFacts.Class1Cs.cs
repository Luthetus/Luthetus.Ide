namespace Luthetus.Ide.RazorLib.WebsiteProjectTemplates.Models;

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
