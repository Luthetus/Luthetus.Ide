using Luthetus.TextEditor.RazorLib.CompilerServices;

namespace Luthetus.TextEditor.RazorLib.Decorations.Models;

public interface ITextEditorRegistryWrap
{
    public IDecorationMapperRegistry DecorationMapperRegistry { get; set; }
    public ICompilerServiceRegistry CompilerServiceRegistry { get; set; }
}
