using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

/// <summary>
/// The <see cref="CompilationUnit"/> is used to represent
/// a singular C# resource file (that is to say a singular file on the user's file system).<br/><br/>
/// TODO: How should <see cref="CompilationUnit"/> work in regards to the C# 'partial' keyword, would many C# resource files need be stitched together into a single <see cref="CompilationUnit"/>?
/// </summary>
public sealed record CompilationUnit : ISyntaxNode
{
    public CompilationUnit(
        CodeBlockNode? rootCodeBlockNode,
        ILuthLexer? lexer,
        ILuthParser? parser,
        ILuthBinder? binder)
    {
        RootCodeBlockNode = rootCodeBlockNode ?? new CodeBlockNode(ImmutableArray<ISyntax>.Empty);
        Lexer = lexer ?? new LuthLexer(null, null, null);
        Parser = parser ?? new LuthParser(Lexer);
        Binder = binder ?? new LuthBinder();

        var diagnosticsListBuilder = new List<TextEditorDiagnostic>();

        diagnosticsListBuilder.AddRange(Lexer.DiagnosticList);
        diagnosticsListBuilder.AddRange(Parser.DiagnosticsList);
        diagnosticsListBuilder.AddRange(Binder.DiagnosticsList);

        DiagnosticsList = diagnosticsListBuilder.ToImmutableArray();

        ChildList = new ISyntax[]
        {
            RootCodeBlockNode
        }.ToImmutableArray();
    }

    public CodeBlockNode RootCodeBlockNode { get; }
    public ILuthLexer Lexer { get; }
    public ILuthParser Parser { get; }
    public ILuthBinder Binder { get; }
    public ImmutableArray<TextEditorDiagnostic> DiagnosticsList { get; init; }

    public ImmutableArray<ISyntax> ChildList { get; init; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.CompilationUnit;
}