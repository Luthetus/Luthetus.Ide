using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices;

/// <summary>
/// The <see cref="CompilationUnit"/> is used to represent
/// a singular C# resource file (that is to say a singular file on the user's file system).<br/><br/>
/// TODO: How should <see cref="CompilationUnit"/> work in regards to the C# 'partial' keyword, would many C# resource files need be stitched together into a single <see cref="CompilationUnit"/>?
/// </summary>
public sealed record CompilationUnitTests : ISyntaxNode
{
    public CompilationUnit(
        CodeBlockNode? rootCodeBlockNode,
        ILexer? lexer,
        IParser? parser,
        IBinder? binder)
    {
        TopLevelStatementsCodeBlockNode = rootCodeBlockNode ?? new CodeBlockNode(ImmutableArray<ISyntax>.Empty);
        Lexer = lexer ?? new TextEditorDefaultLexer();
        Parser = parser ?? new TextEditorDefaultParser();
        Binder = binder ?? new TextEditorDefaultBinder();

        var diagnosticsBagBuilder = new List<TextEditorDiagnostic>();
        
        diagnosticsBagBuilder.AddRange(Lexer.DiagnosticsBag);
        diagnosticsBagBuilder.AddRange(Parser.DiagnosticsBag);
        diagnosticsBagBuilder.AddRange(Binder.DiagnosticsBag);

        DiagnosticsBag = diagnosticsBagBuilder.ToImmutableArray();

        ChildBag = new ISyntax[]
        {
            TopLevelStatementsCodeBlockNode
        }.ToImmutableArray();
    }

    public CodeBlockNode TopLevelStatementsCodeBlockNode { get; }
    public ILexer Lexer { get; }
    public IParser Parser { get; }
    public IBinder Binder { get; }
    public ImmutableArray<TextEditorDiagnostic> DiagnosticsBag { get; init; }

    public ImmutableArray<ISyntax> ChildBag { get; init; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.CompilationUnit;
}