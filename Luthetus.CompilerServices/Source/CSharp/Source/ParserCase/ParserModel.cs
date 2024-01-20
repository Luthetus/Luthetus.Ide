using Luthetus.CompilerServices.Lang.CSharp.BinderCase;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices;

namespace Luthetus.CompilerServices.Lang.CSharp.ParserCase;

/// <param name="FinalizeNamespaceFileScopeCodeBlockNodeAction">
/// If a file scoped namespace is found, then set this field,
/// so that prior to finishing the parser constructs the namespace node.
/// </param>
/// <param name="FinalizeCodeBlockNodeActionStack">
/// When parsing the body of a function this is used in order to keep the function
/// definition node itself in the syntax tree immutable.<br/><br/>
/// That is to say, this action would create the function definition node and then append it.
/// </param>
internal class ParserModel
{
    public ParserModel(
        CSharpBinder binder,
        TokenWalker tokenWalker,
        Stack<ISyntax> syntaxStack,
        LuthetusDiagnosticBag diagnosticBag,
        CodeBlockBuilder globalCodeBlockBuilder,
        CodeBlockBuilder currentCodeBlockBuilder,
        Action<CodeBlockNode>? finalizeNamespaceFileScopeCodeBlockNodeAction,
        Stack<Action<CodeBlockNode>> finalizeCodeBlockNodeActionStack)
    {
        Binder = binder;
        TokenWalker = tokenWalker;
        SyntaxStack = syntaxStack;
        DiagnosticBag = diagnosticBag;
        GlobalCodeBlockBuilder = globalCodeBlockBuilder;
        CurrentCodeBlockBuilder = currentCodeBlockBuilder;
        FinalizeNamespaceFileScopeCodeBlockNodeAction = finalizeNamespaceFileScopeCodeBlockNodeAction;
        FinalizeCodeBlockNodeActionStack = finalizeCodeBlockNodeActionStack;
    }

    public CSharpBinder Binder { get; }
    public TokenWalker TokenWalker { get; }
    public Stack<ISyntax> SyntaxStack { get; set; }
    public LuthetusDiagnosticBag DiagnosticBag { get; }
    public CodeBlockBuilder GlobalCodeBlockBuilder { get; set; }
    public CodeBlockBuilder CurrentCodeBlockBuilder { get; set; }
    public Action<CodeBlockNode>? FinalizeNamespaceFileScopeCodeBlockNodeAction { get; set; }
    public Stack<Action<CodeBlockNode>> FinalizeCodeBlockNodeActionStack { get; set; }
}
