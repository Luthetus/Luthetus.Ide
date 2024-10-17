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
        
        SetChildList();
    }

    public ISyntaxToken IncludeDirectiveSyntaxToken { get; }
    public ISyntaxToken LibraryReferenceSyntaxToken { get; }

    public ISyntax[] ChildList { get; private set; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.PreprocessorLibraryReferenceStatementNode;
    
    public void SetChildList()
    {
    	var childCount = 2; // IncludeDirectiveSyntaxToken, LibraryReferenceSyntaxToken,
            
        var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = IncludeDirectiveSyntaxToken;
		childList[i++] = LibraryReferenceSyntaxToken;
            
        ChildList = childList;
    }
}