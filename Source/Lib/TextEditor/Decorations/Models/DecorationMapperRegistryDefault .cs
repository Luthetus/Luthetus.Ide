using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.RazorLib.Decorations.Models;

public class DecorationMapperRegistryDefault : IDecorationMapperRegistry
{
    private Dictionary<string, IDecorationMapper> _map { get; } = new();

    public IReadOnlyDictionary<string, IDecorationMapper> Map => _map;

    public DecorationMapperRegistryDefault()
    {
        DefaultDecorationMapper = new TextEditorDecorationMapperDefault();
        GenericDecorationMapper = new GenericDecorationMapper();

        _map.Add(ExtensionNoPeriodFacts.C_SHARP_CLASS, GenericDecorationMapper);
        _map.Add(ExtensionNoPeriodFacts.RAZOR_CODEBEHIND, GenericDecorationMapper);
        _map.Add(ExtensionNoPeriodFacts.JAVA_SCRIPT, GenericDecorationMapper);
        _map.Add(ExtensionNoPeriodFacts.TYPE_SCRIPT, GenericDecorationMapper);
        _map.Add(ExtensionNoPeriodFacts.F_SHARP, GenericDecorationMapper);
        _map.Add(ExtensionNoPeriodFacts.C, GenericDecorationMapper);
        _map.Add(ExtensionNoPeriodFacts.H, GenericDecorationMapper);
        _map.Add(ExtensionNoPeriodFacts.CPP, GenericDecorationMapper);
        _map.Add(ExtensionNoPeriodFacts.HPP, GenericDecorationMapper);
    }

    public GenericDecorationMapper GenericDecorationMapper { get; }
    public TextEditorDecorationMapperDefault DefaultDecorationMapper { get; }

    public IDecorationMapper GetDecorationMapper(string extensionNoPeriod)
    {
        if (_map.TryGetValue(extensionNoPeriod, out var decorationMapper))
            return decorationMapper;

        return DefaultDecorationMapper;
    }
}
