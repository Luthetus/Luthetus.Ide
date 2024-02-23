using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.CompilerServices.Lang.Json;

public class JsonResource : LuthCompilerServiceResource
{
    public JsonResource(ResourceUri resourceUri, JsonCompilerService jsonCompilerService)
        : base(resourceUri, jsonCompilerService)
    {
    }
}