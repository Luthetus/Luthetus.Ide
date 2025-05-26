using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.CompilerServices.Css.Decoration;
using Luthetus.CompilerServices.Json.Decoration;
using Luthetus.CompilerServices.Xml.Html.Decoration;
using Luthetus.Ide.RazorLib.Terminals.Models;

namespace Luthetus.Extensions.Config.Decorations;

public class DecorationMapperRegistry : IDecorationMapperRegistry
{
    private Dictionary<string, IDecorationMapper> _map { get; } = new();

    public IReadOnlyDictionary<string, IDecorationMapper> Map => _map;

    public DecorationMapperRegistry()
    {
        CssDecorationMapper = new TextEditorCssDecorationMapper();
        JsonDecorationMapper = new TextEditorJsonDecorationMapper();
        GenericDecorationMapper = new GenericDecorationMapper();
        HtmlDecorationMapper = new TextEditorHtmlDecorationMapper();
        TerminalDecorationMapper = new TerminalDecorationMapper();
        DefaultDecorationMapper = new TextEditorDecorationMapperDefault();

        _map.Add(ExtensionNoPeriodFacts.HTML, HtmlDecorationMapper);
        _map.Add(ExtensionNoPeriodFacts.XML, HtmlDecorationMapper);
        _map.Add(ExtensionNoPeriodFacts.C_SHARP_PROJECT, HtmlDecorationMapper);
        _map.Add(ExtensionNoPeriodFacts.C_SHARP_CLASS, GenericDecorationMapper);
        _map.Add(ExtensionNoPeriodFacts.RAZOR_CODEBEHIND, GenericDecorationMapper);
        _map.Add(ExtensionNoPeriodFacts.RAZOR_MARKUP, HtmlDecorationMapper);
        _map.Add(ExtensionNoPeriodFacts.CSHTML_CLASS, HtmlDecorationMapper);
        _map.Add(ExtensionNoPeriodFacts.CSS, CssDecorationMapper);
        _map.Add(ExtensionNoPeriodFacts.JAVA_SCRIPT, GenericDecorationMapper);
        _map.Add(ExtensionNoPeriodFacts.JSON, JsonDecorationMapper);
        _map.Add(ExtensionNoPeriodFacts.TYPE_SCRIPT, GenericDecorationMapper);
        _map.Add(ExtensionNoPeriodFacts.F_SHARP, GenericDecorationMapper);
        _map.Add(ExtensionNoPeriodFacts.C, GenericDecorationMapper);
        _map.Add(ExtensionNoPeriodFacts.PYTHON, GenericDecorationMapper);
        _map.Add(ExtensionNoPeriodFacts.H, GenericDecorationMapper);
        _map.Add(ExtensionNoPeriodFacts.CPP, GenericDecorationMapper);
        _map.Add(ExtensionNoPeriodFacts.HPP, GenericDecorationMapper);
        _map.Add(ExtensionNoPeriodFacts.DOT_NET_SOLUTION, HtmlDecorationMapper);
        _map.Add(ExtensionNoPeriodFacts.DOT_NET_SOLUTION_X, HtmlDecorationMapper);
        _map.Add(ExtensionNoPeriodFacts.TERMINAL, TerminalDecorationMapper);
    }

    public TextEditorCssDecorationMapper CssDecorationMapper { get; }
    public TextEditorJsonDecorationMapper JsonDecorationMapper { get; }
    public GenericDecorationMapper GenericDecorationMapper { get; }
    public TextEditorHtmlDecorationMapper HtmlDecorationMapper { get; }
    public TerminalDecorationMapper TerminalDecorationMapper { get; }
    public TextEditorDecorationMapperDefault DefaultDecorationMapper { get; }

    public IDecorationMapper GetDecorationMapper(string extensionNoPeriod)
    {
        if (_map.TryGetValue(extensionNoPeriod, out var decorationMapper))
            return decorationMapper;

        return DefaultDecorationMapper;
    }
}
