using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.CompilerServices.Lang.JavaScript;

public class JavaScriptResource : LuthCompilerServiceResource
{
    public JavaScriptResource(ResourceUri resourceUri, JavaScriptCompilerService jsCompilerService)
        : base(resourceUri, jsCompilerService)
    {
    }
}