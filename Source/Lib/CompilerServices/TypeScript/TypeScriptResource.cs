using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.CompilerServices.TypeScript;

public class TypeScriptResource : CompilerServiceResource
{
    public TypeScriptResource(ResourceUri resourceUri, TypeScriptCompilerService jsCompilerService)
        : base(resourceUri, jsCompilerService)
    {
    }
}