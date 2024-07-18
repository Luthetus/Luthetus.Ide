using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.CompilerServices.Xml;

public sealed class XmlResource : CompilerServiceResource
{
    public XmlResource(ResourceUri resourceUri, XmlCompilerService xmlCompilerService)
        : base(resourceUri, xmlCompilerService)
    {
    }
}