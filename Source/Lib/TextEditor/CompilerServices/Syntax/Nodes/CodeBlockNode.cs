using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

/// <summary>
/// The <see cref="CodeBlockNode"/> is used for storing a sequence of statements (or a single
/// expression-statement).<br/><br/>
/// Perhaps one might use <see cref="CodeBlockNode"/> for the body of a class definition, for example.
/// </summary>
public sealed class CodeBlockNode : ISyntaxNode
{
    public CodeBlockNode(ISyntax[] childList)
    {
        _childList = childList;
        DiagnosticsList = Array.Empty<TextEditorDiagnostic>();
    }

    public CodeBlockNode(
        ISyntax[] childList,
        TextEditorDiagnostic[] diagnostics)
    {
        _childList = childList.ToArray();
        DiagnosticsList = diagnostics;
    }

	private ISyntax[] _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

    public TextEditorDiagnostic[] DiagnosticsList { get; init; }
    
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.CodeBlockNode;
    
    public ISyntax[] GetChildList()
    {
    	if (!_childListIsDirty)
    		return _childList;
    	
    	_childListIsDirty = false;
    	return _childList;
    }
}