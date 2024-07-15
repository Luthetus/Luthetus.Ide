using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.CompilerServices.Json;

public class JsonResource : CompilerServiceResource
{
    public JsonResource(ResourceUri resourceUri, JsonCompilerService jsonCompilerService)
        : base(resourceUri, jsonCompilerService)
    {
    }
}