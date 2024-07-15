using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.CompilerServices.FSharp;

public class FSharpResource : CompilerServiceResource
{
    public FSharpResource(ResourceUri resourceUri, FSharpCompilerService fSharpCompilerService)
        : base(resourceUri, fSharpCompilerService)
    {
    }
}