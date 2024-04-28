using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.CompilerServices.Lang.TypeScript;

public class TypeScriptResource : LuthCompilerServiceResource
{
    public TypeScriptResource(ResourceUri resourceUri, TypeScriptCompilerService jsCompilerService)
        : base(resourceUri, jsCompilerService)
    {
    }
}