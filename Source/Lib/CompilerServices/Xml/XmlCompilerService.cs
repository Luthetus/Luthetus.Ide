using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.CompilerServices.Lang.Xml.Html.SyntaxActors;

namespace Luthetus.CompilerServices.Lang.Xml;

public sealed class XmlCompilerService : CompilerService
{
    public XmlCompilerService(ITextEditorService textEditorService)
        : base(textEditorService)
    {
        _compilerServiceOptions = new()
        {
            RegisterResourceFunc = resourceUri => new CompilerServiceResource(resourceUri, this),
            GetLexerFunc = (resource, sourceText) => new TextEditorXmlLexer(resource.ResourceUri, sourceText),
            GetParserFunc = (resource, lexer) => new Parser(lexer),
            GetBinderFunc = (resource, parser) => Binder
        };
    }
}