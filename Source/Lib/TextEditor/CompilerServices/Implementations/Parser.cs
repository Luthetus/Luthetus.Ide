using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;

public class Parser : IParser
{
    public Parser(ILexer lexer)
    {
        Lexer = lexer;
        Binder = new Binder();
        BinderSession = Binder.StartBinderSession(lexer.ResourceUri);
    }

    public TextEditorDiagnostic[] DiagnosticsList { get; private set; } = Array.Empty<TextEditorDiagnostic>();
    public IBinder Binder { get; private set; }
    public IBinderSession BinderSession { get; private set; }
    public ILexer Lexer { get; }

    /// <summary>This method is used when parsing many files as a single compilation. The first binder instance would be passed to the following parsers. The resourceUri is passed in so if a file is parsed for a second time, the previous symbols can be deleted so they do not duplicate.</summary>
    public ICompilationUnit Parse(
        IBinder previousBinder,
        ResourceUri resourceUri)
    {
        Binder = previousBinder;
        Binder.ClearStateByResourceUri(resourceUri);
        BinderSession = Binder.StartBinderSession(resourceUri);
        return Parse();
    }

    public virtual ICompilationUnit Parse()
    {
        var globalCodeBlockBuilder = new CodeBlockBuilder(null, null);
        var currentCodeBlockBuilder = globalCodeBlockBuilder;
        var diagnosticBag = new DiagnosticBag();

        var model = new ParserModel(
            Binder,
            BinderSession,
            new TokenWalker(Lexer.SyntaxTokenList, diagnosticBag),
            new Stack<ISyntax>(),
            diagnosticBag,
            globalCodeBlockBuilder,
            currentCodeBlockBuilder);
		
        DiagnosticsList = model.DiagnosticBag.ToArray();

        var topLevelStatementsCodeBlock = model.CurrentCodeBlockBuilder.Build(
            DiagnosticsList
                .Union(Binder.DiagnosticsList)
                .Union(Lexer.DiagnosticList)
                .ToArray());

        return new CompilationUnit(
            topLevelStatementsCodeBlock,
            Lexer,
            this,
            Binder);
    }
}
