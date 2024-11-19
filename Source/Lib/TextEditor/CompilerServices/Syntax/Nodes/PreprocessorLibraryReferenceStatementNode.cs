using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class PreprocessorLibraryReferenceStatementNode : IStatementNode
{
    public PreprocessorLibraryReferenceStatementNode(
        ISyntaxToken includeDirectiveSyntaxToken,
        ISyntaxToken libraryReferenceSyntaxToken)
    {
        IncludeDirectiveSyntaxToken = includeDirectiveSyntaxToken;
        LibraryReferenceSyntaxToken = libraryReferenceSyntaxToken;
    }

	private ISyntax[] _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

    public ISyntaxToken IncludeDirectiveSyntaxToken { get; }
    public ISyntaxToken LibraryReferenceSyntaxToken { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.PreprocessorLibraryReferenceStatementNode;
    
    public int GetStartInclusiveIndex()
    {
    }
    
    public int GetEndExclusiveIndex()
    {
    }
    
    public ISyntax[] GetChildList()
    {
    	if (!_childListIsDirty)
    		return _childList;
    	
    	var childCount = 2; // IncludeDirectiveSyntaxToken, LibraryReferenceSyntaxToken,
            
        var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = IncludeDirectiveSyntaxToken;
		childList[i++] = LibraryReferenceSyntaxToken;
            
        _childList = childList;
        
    	_childListIsDirty = false;
    	return _childList;
    }
}