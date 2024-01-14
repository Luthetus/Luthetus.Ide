using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices;

/// <summary>
/// The <see cref="CompilationUnit"/> is used to represent
/// a singular C# resource file (that is to say a singular file on the user's file system).<br/><br/>
/// TODO: How should <see cref="CompilationUnit"/> work in regards to the C# 'partial' keyword, would many C# resource files need be stitched together into a single <see cref="CompilationUnit"/>?
/// </summary>
public sealed record CompilationUnit : ISyntaxNode
{
    public CompilationUnit(
        CodeBlockNode? rootCodeBlockNode,
        ILexer? lexer,
        IParser? parser,
        IBinder? binder)
    {
        RootCodeBlockNode = rootCodeBlockNode ?? new CodeBlockNode(ImmutableArray<ISyntax>.Empty);
        Lexer = lexer ?? new TextEditorDefaultLexer();
        Parser = parser ?? new TextEditorDefaultParser();
        Binder = binder ?? new TextEditorDefaultBinder();

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
    public ILexer Lexer { get; }
    public IParser Parser { get; }
    public IBinder Binder { get; }
    public ImmutableArray<TextEditorDiagnostic> DiagnosticsList { get; init; }

    public ImmutableArray<ISyntax> ChildList { get; init; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.CompilationUnit;
}