using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.TextEditor.RazorLib.Analysis;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.General;

/// <summary>
/// The <see cref="CompilationUnit"/> is used to represent
/// a singular C# resource file (that is to say a singular file on the user's file system).
/// <br/><br/>
/// TODO: How should <see cref="CompilationUnit"/> work in regards to the C# 'partial' keyword, would many C# resource files need be stitched together into a single <see cref="CompilationUnit"/>?
/// </summary>
public sealed record CompilationUnit : ISyntaxNode
{
    public CompilationUnit(
        CodeBlockNode topLevelStatementsCodeBlockNode,
        ILexer lexer,
        IParser parser,
        IBinder binder)
    {
        TopLevelStatementsCodeBlockNode = topLevelStatementsCodeBlockNode;
        Lexer = lexer;
        Parser = parser;
        Binder = binder;

        Diagnostics = Lexer.Diagnostics
            .Union(Parser.Diagnostics)
            .Union(Binder.Diagnostics)
            .ToImmutableArray();

        Children = new ISyntax[] 
        {
            TopLevelStatementsCodeBlockNode 
        }.ToImmutableArray();
    }

    public CodeBlockNode TopLevelStatementsCodeBlockNode { get; }
    public ILexer Lexer { get; }
    public IParser Parser { get; }
    public IBinder Binder { get; }
    public ImmutableArray<TextEditorDiagnostic> Diagnostics { get; init; }

    public ImmutableArray<ISyntax> Children { get; init; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.CompilationUnitNode;
}
