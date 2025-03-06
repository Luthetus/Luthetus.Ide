using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.RazorLib.Decorations.Models;

public class DecorationMapperRegistryDefault : IDecorationMapperRegistry
{
    private Dictionary<string, IDecorationMapper> _map { get; } = new();

    public IReadOnlyDictionary<string, IDecorationMapper> Map => _map;

    public DecorationMapperRegistryDefault()
    {
        DefaultDecorationMapper = new TextEditorDecorationMapperDefault();
    }

    public TextEditorDecorationMapperDefault DefaultDecorationMapper { get; }

    public IDecorationMapper GetDecorationMapper(string extensionNoPeriod)
    {
        if (_map.TryGetValue(extensionNoPeriod, out var decorationMapper))
            return decorationMapper;

        return DefaultDecorationMapper;
    }
}
