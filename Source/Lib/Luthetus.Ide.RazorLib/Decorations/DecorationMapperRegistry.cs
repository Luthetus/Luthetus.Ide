using Luthetus.CompilerServices.Lang.Css.Css.Decoration;
using Luthetus.CompilerServices.Lang.Json.Json.Decoration;
using Luthetus.CompilerServices.Lang.Xml.Html.Decoration;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.Decorations;

public class DecorationMapperRegistry : IDecorationMapperRegistry
{
    private Dictionary<string, IDecorationMapper> _map { get; } = new();

    public ImmutableDictionary<string, IDecorationMapper> Map => _map.ToImmutableDictionary();

    public DecorationMapperRegistry()
    {
        CssDecorationMapper = new TextEditorCssDecorationMapper();
        JsonDecorationMapper = new TextEditorJsonDecorationMapper();
        GenericDecorationMapper = new GenericDecorationMapper();
        HtmlDecorationMapper = new TextEditorHtmlDecorationMapper();
        DefaultDecorationMapper = new TextEditorDecorationMapperDefault();
    }

    public TextEditorCssDecorationMapper CssDecorationMapper { get; }
    public TextEditorJsonDecorationMapper JsonDecorationMapper { get; }
    public GenericDecorationMapper GenericDecorationMapper { get; }
    public TextEditorHtmlDecorationMapper HtmlDecorationMapper { get; }
    public TextEditorDecorationMapperDefault DefaultDecorationMapper { get; }

    public IDecorationMapper GetDecorationMapper(string extensionNoPeriod)
    {
        if (_map.TryGetValue(extensionNoPeriod, out var decorationMapper))
            return decorationMapper;

        return DefaultDecorationMapper;
    }
}
