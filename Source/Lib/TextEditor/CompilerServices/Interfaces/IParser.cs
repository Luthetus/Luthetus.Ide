using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

public interface IParser
{
    public TextEditorDiagnostic[] DiagnosticsList { get; }
    public IBinder Binder { get; }
    public IBinderSession BinderSession { get; }
    public ILexer Lexer { get; }

    public CompilationUnit Parse(IBinder binder, ResourceUri resourceUri);
}
