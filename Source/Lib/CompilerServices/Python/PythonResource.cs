using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.CompilerServices.Python;

public class PythonResource : CompilerServiceResource
{
    public PythonResource(ResourceUri resourceUri, PythonCompilerService pythonCompilerService)
        : base(resourceUri, pythonCompilerService)
    {
    }
}