using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

/// <summary>
/// I just realized as I made this type,
/// is it "global" or "top level statements"?
///
/// Now that I think about it I think
/// this should be named top-level-statements.
///
/// But I'm in the middle of a lot of changes
/// and cannot mess with the name at the moment.
///
/// --------------------------------------------
///
/// When invoking 'GetChildList()'
/// this will return 'CodeBlockNode.GetChildList();'
/// if 'CodeBlockNode' is not null.
/// </summary>
public sealed class GlobalCodeBlockNode : ICodeBlockOwner
{
	private IReadOnlyList<ISyntax> _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

    // ICodeBlockOwner properties.
	public ScopeDirectionKind ScopeDirectionKind => ScopeDirectionKind.Both;
	public TextEditorTextSpan? OpenCodeBlockTextSpan { get; set; }
	public CodeBlockNode? CodeBlockNode { get; private set; }
	public TextEditorTextSpan? CloseCodeBlockTextSpan { get; set; }
	public int? ScopeIndexKey { get; set; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.GlobalCodeBlockNode;
    
    #region ICodeBlockOwner_Methods
    public TypeClauseNode? GetReturnTypeClauseNode()
    {
    	return null;
    }
    
	public ICodeBlockOwner SetOpenCodeBlockTextSpan(TextEditorTextSpan? openCodeBlockTextSpan, DiagnosticBag diagnosticBag, TokenWalker tokenWalker)
	{
		if (OpenCodeBlockTextSpan is not null)
			ICodeBlockOwner.ThrowMultipleScopeDelimiterException(diagnosticBag, tokenWalker);
	
		OpenCodeBlockTextSpan = openCodeBlockTextSpan;
    	
    	_childListIsDirty = true;
    	return this;
	}
	
	public ICodeBlockOwner SetCloseCodeBlockTextSpan(TextEditorTextSpan? closeCodeBlockTextSpan, DiagnosticBag diagnosticBag, TokenWalker tokenWalker)
	{
		if (CloseCodeBlockTextSpan is not null)
			ICodeBlockOwner.ThrowMultipleScopeDelimiterException(diagnosticBag, tokenWalker);
	
		CloseCodeBlockTextSpan = closeCodeBlockTextSpan;
    	
    	_childListIsDirty = true;
    	return this;
	}
	
	public ICodeBlockOwner SetCodeBlockNode(CodeBlockNode codeBlockNode, DiagnosticBag diagnosticBag, TokenWalker tokenWalker)
	{
		if (CodeBlockNode is not null)
			ICodeBlockOwner.ThrowAlreadyAssignedCodeBlockNodeException(diagnosticBag, tokenWalker);
	
		CodeBlockNode = codeBlockNode;
    	
    	_childListIsDirty = true;
    	return this;
	}
	#endregion
    
    public IReadOnlyList<ISyntax> GetChildList()
    {
    	if (CodeBlockNode is not null)
    		return CodeBlockNode.GetChildList();
    
    	if (!_childListIsDirty)
    		return _childList;
    	
    	var childCount = 0;
    	if (CodeBlockNode is not null)
    		childCount++;
            
        var childList = new ISyntax[childCount];
		var i = 0;

    	if (CodeBlockNode is not null)
    		childList[i++] = CodeBlockNode;
            
        _childList = childList;
        
    	_childListIsDirty = false;
    	return _childList;
    }
}
