namespace Luthetus.Ide.ClassLib.WebsiteProjectTemplates.RazorClassLibCase;

public partial class RazorClassLibFacts
{
    public const string COMPONENT_1_RAZOR_RELATIVE_FILE_PATH = @"Component1.razor";

    public static string GetComponent1RazorContents(string projectName) => @$"<div class=""my-component"">
    This component is defined in the <strong>{projectName}</strong> library.
</div>
";
}
