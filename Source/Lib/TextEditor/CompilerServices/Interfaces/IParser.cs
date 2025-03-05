using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

public interface IParser
{
    public IBinder Binder { get; }
    public ILexer Lexer { get; }

    public ICompilationUnit Parse(IBinder binder, ResourceUri resourceUri);
}
