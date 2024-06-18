using Luthetus.CompilerServices.Lang.CSharp.BinderCase;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;

namespace Luthetus.CompilerServices.Lang.CSharp.ParserCase;

public class ParserModel
{
    public ParserModel(
        CSharpBinder binder,
        CSharpBinderSession binderSession,
        TokenWalker tokenWalker,
        Stack<ISyntax> syntaxStack,
        LuthDiagnosticBag diagnosticBag,
        CodeBlockBuilder globalCodeBlockBuilder,
        CodeBlockBuilder currentCodeBlockBuilder,
        Action<CodeBlockNode>? finalizeNamespaceFileScopeCodeBlockNodeAction,
        Stack<Action<CodeBlockNode>> finalizeCodeBlockNodeActionStack)
    {
        Binder = binder;
        BinderSession = binderSession;
        TokenWalker = tokenWalker;
        SyntaxStack = syntaxStack;
        DiagnosticBag = diagnosticBag;
        GlobalCodeBlockBuilder = globalCodeBlockBuilder;
        CurrentCodeBlockBuilder = currentCodeBlockBuilder;
        FinalizeNamespaceFileScopeCodeBlockNodeAction = finalizeNamespaceFileScopeCodeBlockNodeAction;
        FinalizeCodeBlockNodeActionStack = finalizeCodeBlockNodeActionStack;
    }

    public CSharpBinder Binder { get; }
    public CSharpBinderSession BinderSession { get; }
    public TokenWalker TokenWalker { get; }
    public Stack<ISyntax> SyntaxStack { get; set; }
    public LuthDiagnosticBag DiagnosticBag { get; }
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
	public Queue<Action<int>> ParseChildScopeQueue { get; set; } = new();
}
