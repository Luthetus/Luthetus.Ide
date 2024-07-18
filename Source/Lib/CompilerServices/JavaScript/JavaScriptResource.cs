using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.CompilerServices.JavaScript;

public class JavaScriptResource : CompilerServiceResource
{
    public JavaScriptResource(ResourceUri resourceUri, JavaScriptCompilerService jsCompilerService)
        : base(resourceUri, jsCompilerService)
    {
    }
}