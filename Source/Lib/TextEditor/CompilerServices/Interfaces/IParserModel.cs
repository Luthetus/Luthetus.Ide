using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

public interface IParserModel
{
    public IBinder Binder { get; }
    public IBinderSession BinderSession { get; }
    public TokenWalker TokenWalker { get; }
    
    public Stack<ISyntax> SyntaxStack { get; set; }
    
    /// <summary>
    /// This list permits primitive recursion via a while loop for the expression parsing logic.
    ///
    /// Each entry in the list is a "short circuit" such that if a child expression encounters
    /// the entry's DelimiterSyntaxKind, it will set the primary expression to the entry's ExpressionNode
    /// (of which is an ancestor expression to the child).
    ///
    /// After that, the restored primary expression "combines" with the child expression that "short circuit"-ed.
    ///
    /// Then, the new primary expression expression node parses the 'DelimiterSyntaxKind' which triggered the "short circuit".
    ///
    /// The C# IParserModel implementation will only "short circuit" if the 'SyntaxKind DelimiterSyntaxKind'
    /// is registered as a delimiter.
    ///
    /// This is done in order to speed up the while loop, as the list of short circuits doesn't have to be
    /// iterated unless the current token is a possible delimiter.
    ///
    /// Luthetus.CompilerServices.CSharp.ParserCase.Internals.ParseOthers.SyntaxIsEndDelimiter(SyntaxKind syntaxKind) {...}
    /// </summary>
    public List<(SyntaxKind DelimiterSyntaxKind, IExpressionNode ExpressionNode)> ExpressionList { get; set; }
    
    public DiagnosticBag DiagnosticBag { get; }
    public CodeBlockBuilder GlobalCodeBlockBuilder { get; set; }
    public CodeBlockBuilder CurrentCodeBlockBuilder { get; set; }
    /// <summary>
    /// If a file scoped namespace is found, then set this field,
    /// so that prior to finishing the parser constructs the namespace node.
    /// </summary>
    public Action<CodeBlockNode>? FinalizeNamespaceFileScopeCodeBlockNodeAction { get; set; }
    /// <summary>
    /// When parsing the body of a function this is used in order to keep the function
    /// definition node itself in the syntax tree immutable.<br/><br/>
    /// That is to say, this action would create the function definition node and then append it.
    /// </summary>
    public Stack<Action<CodeBlockNode>> FinalizeCodeBlockNodeActionStack { get; set; }
}
