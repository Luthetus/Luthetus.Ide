using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;

public class LuthParser : ILuthParser
{
    public LuthParser(ILuthLexer lexer)
    {
        Lexer = lexer;
        Binder = new LuthBinder();
        BinderSession = Binder.ConstructBinderSession(lexer.ResourceUri);
    }

    public ImmutableArray<TextEditorDiagnostic> DiagnosticsList { get; private set; } = ImmutableArray<TextEditorDiagnostic>.Empty;
    public ILuthBinder Binder { get; private set; }
    public ILuthBinderSession BinderSession { get; private set; }
    public ILuthLexer Lexer { get; }

    /// <summary>This method is used when parsing many files as a single compilation. The first binder instance would be passed to the following parsers. The resourceUri is passed in so if a file is parsed for a second time, the previous symbols can be deleted so they do not duplicate.</summary>
    public CompilationUnit Parse(
        ILuthBinder previousBinder,
        ResourceUri resourceUri)
    {
        Binder = previousBinder;
        BinderSession = Binder.ConstructBinderSession(resourceUri);
        Binder.ClearStateByResourceUri(resourceUri);
        return Parse();
    }

    public virtual CompilationUnit Parse()
    {
        var globalCodeBlockBuilder = new CodeBlockBuilder(null, null);
        var currentCodeBlockBuilder = globalCodeBlockBuilder;
        var diagnosticBag = new LuthDiagnosticBag();

        var model = new LuthParserModel(
            Binder,
            BinderSession,
            new TokenWalker(Lexer.SyntaxTokenList, diagnosticBag),
            new Stack<ISyntax>(),
            diagnosticBag,
            globalCodeBlockBuilder,
            currentCodeBlockBuilder,
            null,
            new Stack<Action<CodeBlockNode>>());

        DiagnosticsList = DiagnosticsList.AddRange(model.DiagnosticBag.ToImmutableArray());

        var topLevelStatementsCodeBlock = model.CurrentCodeBlockBuilder.Build(
            DiagnosticsList
                .Union(Binder.DiagnosticsList)
                .Union(Lexer.DiagnosticList)
                .ToImmutableArray());

        return new CompilationUnit(
            topLevelStatementsCodeBlock,
            Lexer,
            this,
            Binder);
    }
}
