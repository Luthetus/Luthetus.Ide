using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

public sealed class IntegratedTerminalCompilerService : LuthCompilerService
{
    public IntegratedTerminalCompilerService(ITextEditorService textEditorService)
        : base(textEditorService)
    {
    }
}