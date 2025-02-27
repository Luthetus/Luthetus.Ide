using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

/// <summary>
/// The <see cref="CompilationUnit"/> is used to represent
/// a singular C# resource file (that is to say a singular file on the user's file system).<br/><br/>
/// TODO: How should <see cref="CompilationUnit"/> work in regards to the C# 'partial' keyword, would many C# resource files need be stitched together into a single <see cref="CompilationUnit"/>?
/// </summary>
public sealed class CompilationUnit : ICompilationUnit, ISyntaxNode
{
    public CompilationUnit(
        ISyntaxNode? rootCodeBlockNode,
        ILexer? lexer,
        IParser? parser,
        IBinder? binder)
    {
        RootCodeBlockNode = rootCodeBlockNode ?? new CodeBlockNode(Array.Empty<ISyntax>());
        Lexer = lexer ?? new Lexer(ResourceUri.Empty, null, null);
        Parser = parser ?? new Parser(Lexer);
        Binder = binder ?? new Binder();
    }

	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

    public ISyntaxNode RootCodeBlockNode { get; }
    public ILexer Lexer { get; }
    public IParser Parser { get; }
    public IBinder Binder { get; }
    public List<TextEditorDiagnostic> __DiagnosticList { get; } = new();
    public IReadOnlyList<TextEditorDiagnostic> DiagnosticList => __DiagnosticList;

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.CompilationUnit;
    
    public IReadOnlyList<ISyntax> GetChildList()
    {
    	if (!_childListIsDirty)
    		return _childList;
    	
    	var childCount = 1; // RootCodeBlockNode,
            
        var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = RootCodeBlockNode;
            
        _childList = childList;
        
    	_childListIsDirty = false;
    	return _childList;
    }
}