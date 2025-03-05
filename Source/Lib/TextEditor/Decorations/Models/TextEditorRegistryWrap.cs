using Luthetus.TextEditor.RazorLib.CompilerServices;

namespace Luthetus.TextEditor.RazorLib.Decorations.Models;

public class TextEditorRegistryWrap : ITextEditorRegistryWrap
{
    public IDecorationMapperRegistry DecorationMapperRegistry { get; set; } = new DecorationMapperRegistryDefault();
    public ICompilerServiceRegistry CompilerServiceRegistry { get; set; } = new CompilerServiceRegistryDefault();
}
