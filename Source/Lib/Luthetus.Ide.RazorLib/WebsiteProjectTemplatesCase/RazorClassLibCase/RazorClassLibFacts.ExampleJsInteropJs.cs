namespace Luthetus.Ide.RazorLib.WebsiteProjectTemplatesCase.RazorClassLibCase;

public partial class RazorClassLibFacts
{
    public const string EXAMPLE_JS_INTEROP_JS_RELATIVE_FILE_PATH = @"wwwroot/ExampleJsInterop.js";

    public static string GetExampleJsInteropJsContents(string projectName) => @$"// This is a JavaScript module that is loaded on demand. It can export any number of
// functions, and may import other JavaScript modules if required.

export function showPrompt(message) {{
  return prompt(message, 'Type anything here');
}}
";
}
