using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Utility;

public class CodeBlockBuilder
{
    public CodeBlockBuilder(CodeBlockBuilder? parent, ISyntaxNode? codeBlockOwner)
    {
        Parent = parent;
        CodeBlockOwner = codeBlockOwner;
    }

    public List<ISyntax> ChildList { get; } = new();
    public CodeBlockBuilder? Parent { get; }
    /// <summary>
    /// Given: "public class MyClass { ... }"<br/><br/>Then: The 'MyClass' body-code-block would
    /// have an owner of 'TypeDefinitionNode'.<br/><br/>
    /// Purpose: When parsing a class definition's constructor. I need to know if the code block I
    /// exist in is one which a class owns. Furthermore, I need to verify that the code-block-owner's
    /// Identifier is equal to the constructor's identifier.
    /// </summary>
    public ISyntaxNode? CodeBlockOwner { get; }

    public CodeBlockNode Build()
    {
        return new CodeBlockNode(ChildList.ToImmutableArray());
    }

    public CodeBlockNode Build(ImmutableArray<TextEditorDiagnostic> diagnostics)
    {
        return new CodeBlockNode(ChildList.ToImmutableArray(), diagnostics);
    }
}