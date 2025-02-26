using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;

namespace Luthetus.TextEditor.RazorLib.CompilerServices;

public class ParserModel : IParserModel
{
    public ParserModel(
        IBinder binder,
        IBinderSession binderSession,
        TokenWalker tokenWalker,
        CodeBlockBuilder globalCodeBlockBuilder,
        CodeBlockBuilder currentCodeBlockBuilder)
    {
        Binder = binder;
        BinderSession = binderSession;
        TokenWalker = tokenWalker;
        GlobalCodeBlockBuilder = globalCodeBlockBuilder;
        CurrentCodeBlockBuilder = currentCodeBlockBuilder;
        
        ForceParseExpressionInitialPrimaryExpression = EmptyExpressionNode.Empty;
    }

    public IBinder Binder { get; }
    public IBinderSession BinderSession { get; }
    public TokenWalker TokenWalker { get; }
    public StatementBuilder StatementBuilder { get; set; } = new();
    public List<(SyntaxKind DelimiterSyntaxKind, IExpressionNode ExpressionNode)> ExpressionList { get; set; } = new();
    public IExpressionNode? NoLongerRelevantExpressionNode { get; set; }
    public List<SyntaxKind> TryParseExpressionSyntaxKindList { get; } = new();
    public IExpressionNode ForceParseExpressionInitialPrimaryExpression { get; set; }
    public CodeBlockBuilder GlobalCodeBlockBuilder { get; set; }
    public CodeBlockBuilder CurrentCodeBlockBuilder { get; set; }
}
