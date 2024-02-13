using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;

namespace Luthetus.Ide.RazorLib.IntegratedTerminal;

public sealed class IntegratedTerminalCompilerService : LuthCompilerService
{
    public IntegratedTerminalCompilerService(
            ITextEditorService textEditorService,
            Func<ResourceUri, string, ILuthLexer> getLexerFunc,
            Func<ILuthLexer, ILuthParser> getParserFunc)
        : base(textEditorService, getLexerFunc, getParserFunc)
    {
    }
}