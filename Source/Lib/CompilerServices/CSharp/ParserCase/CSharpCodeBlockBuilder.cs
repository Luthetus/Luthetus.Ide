using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;

namespace Luthetus.CompilerServices.CSharp.ParserCase;

public class CSharpCodeBlockBuilder
{
    public CSharpCodeBlockBuilder(CSharpCodeBlockBuilder? parent, ICodeBlockOwner? codeBlockOwner)
    {
        Parent = parent;
        CodeBlockOwner = codeBlockOwner;
        
        if (CodeBlockOwner.ScopeDirectionKind == ScopeDirectionKind.Both)
        	ParseChildScopeQueue = new();
    }

    public List<ISyntax> ChildList { get; } = new();
    public CSharpCodeBlockBuilder? Parent { get; }
    /// <summary>
    /// Given: "public class MyClass { ... }"<br/><br/>Then: The 'MyClass' body-code-block would
    /// have an owner of 'TypeDefinitionNode'.<br/><br/>
    /// Purpose: When parsing a class definition's constructor. I need to know if the code block I
    /// exist in is one which a class owns. Furthermore, I need to verify that the code-block-owner's
    /// Identifier is equal to the constructor's identifier.
    /// </summary>
    public ICodeBlockOwner? CodeBlockOwner { get; }
    
    // (2025-01-13)
	// ========================================================
	// - This was changed so it no longer initialized to 'new();'
	//   (not every scope uses deferred parsing so no need to allocate everytime).
    public Queue<CSharpDeferredChildScope>? ParseChildScopeQueue { get; set; }
    
	public bool PermitCodeBlockParsing { get; set; } = true;
	public int? DequeuedIndexForChildList { get; set; }
	
	public int? ScopeIndexKey { get; set; }

    public CodeBlockNode Build()
    {
        return new CodeBlockNode(ChildList.ToArray());
    }

    public CodeBlockNode Build(TextEditorDiagnostic[] diagnostics)
    {
        return new CodeBlockNode(ChildList.ToArray(), diagnostics);
    }
}