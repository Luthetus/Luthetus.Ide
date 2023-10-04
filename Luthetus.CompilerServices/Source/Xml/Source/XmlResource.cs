using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.Xml;

public class XmlResource : ICompilerServiceResource
{
    public XmlResource(
        ResourceUri resourceUri,
        XmlCompilerService textEditorXmlCompilerService)
    {
        ResourceUri = resourceUri;
        TextEditorXmlCompilerService = textEditorXmlCompilerService;
    }

    public ResourceUri ResourceUri { get; }
    public XmlCompilerService TextEditorXmlCompilerService { get; }
    public ImmutableArray<TextEditorTextSpan>? SyntacticTextSpans { get; internal set; }
}