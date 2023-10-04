using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;

namespace Luthetus.CompilerServices.Lang.CSharp.CompilerServiceCase;

/// <summary>
/// An abstract semantic represenation of the input. (example in the XML doc)
/// </summary>
/// <example>
/// Given the input: "(5 + 2) * 3"
/// The expected output is the following tree structure:
/// <br/><br/>
///     *
///    / \
///   +   3
///  / \
/// 5   2
/// <br/><br/>
/// Note how the parentheses are no longer present in the AST (AbstractSyntaxTree).
/// They were simplified out because one can just move the ordering of nodes and
/// operators within the tree itself.
/// </example>
public record AbstractSyntaxTree
{
    public AbstractSyntaxTree(ISyntaxNode rootSyntaxNode)
    {
        RootSyntaxNode = rootSyntaxNode;
    }

    /// <summary>
    /// 
    /// </summary>
    public ISyntaxNode RootSyntaxNode { get; }
}