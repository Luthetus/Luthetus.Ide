using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Utility;

public class CodeBlockBuilder
{
    public CodeBlockBuilder(CodeBlockBuilder? parent, ICodeBlockOwner? codeBlockOwner)
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
    public ICodeBlockOwner? CodeBlockOwner { get; }
    
    /// <summary>
    /// This property is used to permit "single statement code blocks".
	/// 
    /// Preferably, an extra property which exists solely for this purpose
    /// would not be the solution.
    ///
    /// But the worry is whether the <see cref="Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces.IParserModel"/>
    /// property 'SyntaxStack' is accurately enough managed at the moment.
    ///
    /// Because the code is going to parse a StatementDelimiterToken,
    /// and then check if there is a "pending" ICodeBlockOwner.
    ///
    /// The biggest worry regarding the 'SyntaxStack' is with expressions.
    /// Possibly the final answer is to Peek() the 'SyntaxStack' for an ICodeBlockOwner
    /// that has a null OpenBraceToken.
    ///
    /// But for now, I don't think the 'SyntaxStack' usage is well defined enough yet.
    ///
    /// foreach (var item in list)
    /// {
	///     Console.WriteLine(item);
	/// }
	///
	/// Versus
	/// 
	/// foreach (var item in list)
	///     Console.WriteLine(item);
    /// </summary>
    public ICodeBlockOwner? PendingChild { get; set; }
    
    public Queue<Action<int>> ParseChildScopeQueue { get; set; } = new();
	public int DequeueChildScopeCounter { get; set; }
	public int? DequeuedIndexForChildList { get; set; }

    public CodeBlockNode Build()
    {
        return new CodeBlockNode(ChildList.ToImmutableArray());
    }

    public CodeBlockNode Build(ImmutableArray<TextEditorDiagnostic> diagnostics)
    {
        return new CodeBlockNode(ChildList.ToImmutableArray(), diagnostics);
    }
}