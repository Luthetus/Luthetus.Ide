namespace Luthetus.Ide.RazorLib.WebsiteProjectTemplates.Models;

public partial class RazorClassLibFacts
{
    public const string COMPONENT_1_RAZOR_CSS_RELATIVE_FILE_PATH = @"Component1.razor.css";

    public static string GetComponent1RazorCssContents(string projectName) => @$".my-component {{
    border: 2px dashed red;
    padding: 1em;
    margin: 1em 0;
    background-image: url('background.png');
}}
";
}
