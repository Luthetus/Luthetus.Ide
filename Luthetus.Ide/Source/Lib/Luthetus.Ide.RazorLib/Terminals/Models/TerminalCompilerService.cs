using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

public sealed class TerminalCompilerService : LuthCompilerService
{
    public TerminalCompilerService(ITextEditorService textEditorService)
        : base(textEditorService)
    {
    }
}