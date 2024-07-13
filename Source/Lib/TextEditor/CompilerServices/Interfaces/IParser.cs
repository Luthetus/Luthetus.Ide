using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

public interface IParser
{
    public ImmutableArray<TextEditorDiagnostic> DiagnosticsList { get; }
    public IBinder Binder { get; }
    public IBinderSession BinderSession { get; }
    public ILexer Lexer { get; }

    public CompilationUnit Parse();
    /// <summary>
    /// This method is used when parsing many files as a single compilation.
    /// The first binder instance would be passed to the following parsers.
    /// The resourceUri is passed in so if a file is parsed for a second time,
    /// the previous symbols can be deleted so they do not duplicate.
    /// </summary>
    public CompilationUnit Parse(IBinder previousBinder, ResourceUri resourceUri);
}
