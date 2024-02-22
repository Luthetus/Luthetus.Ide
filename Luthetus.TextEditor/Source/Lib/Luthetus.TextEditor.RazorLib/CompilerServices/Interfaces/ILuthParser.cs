using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

public interface ILuthParser
{
    public ImmutableArray<TextEditorDiagnostic> DiagnosticsList { get; }
    public ILuthBinder Binder { get; }
    public ILuthBinderSession BinderSession { get; }
    public ILuthLexer Lexer { get; }

    public CompilationUnit Parse();
    /// <summary>
    /// This method is used when parsing many files as a single compilation.
    /// The first binder instance would be passed to the following parsers.
    /// The resourceUri is passed in so if a file is parsed for a second time,
    /// the previous symbols can be deleted so they do not duplicate.
    /// </summary>
    public CompilationUnit Parse(ILuthBinder previousBinder, ResourceUri resourceUri);
}
