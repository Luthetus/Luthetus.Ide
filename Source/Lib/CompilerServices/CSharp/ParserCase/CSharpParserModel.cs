using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.CompilerServices.CSharp.ParserCase.Internals;
using Luthetus.CompilerServices.CSharp.BinderCase;

namespace Luthetus.CompilerServices.CSharp.ParserCase;

public class CSharpParserModel : IParserModel
{
    public CSharpParserModel(
        CSharpBinder binder,
        CSharpBinderSession binderSession,
        TokenWalker tokenWalker,
        Stack<ISyntax> syntaxStack,
        DiagnosticBag diagnosticBag,
        CodeBlockBuilder globalCodeBlockBuilder,
        CodeBlockBuilder currentCodeBlockBuilder)
    {
        Binder = binder;
        BinderSession = binderSession;
        TokenWalker = tokenWalker;
        SyntaxStack = syntaxStack;
        DiagnosticBag = diagnosticBag;
        GlobalCodeBlockBuilder = globalCodeBlockBuilder;
        CurrentCodeBlockBuilder = currentCodeBlockBuilder;
        
        ExpressionList = new();
        ExpressionList.Add((SyntaxKind.StatementDelimiterToken, null));
        
        ForceParseExpressionInitialPrimaryExpression = EmptyExpressionNode.Empty;
    }

    public CSharpBinder Binder { get; }
    public CSharpBinderSession BinderSession { get; }
    public TokenWalker TokenWalker { get; }
    public Stack<ISyntax> SyntaxStack { get; set; }
    public StatementBuilder StatementBuilder { get; set; } = new();
    public List<(SyntaxKind DelimiterSyntaxKind, IExpressionNode ExpressionNode)> ExpressionList { get; set; }
    public IExpressionNode? NoLongerRelevantExpressionNode { get; set; }
    public List<SyntaxKind> TryParseExpressionSyntaxKindList { get; } = new();
    public IExpressionNode ForceParseExpressionInitialPrimaryExpression { get; set; }
    public DiagnosticBag DiagnosticBag { get; }
    public CodeBlockBuilder GlobalCodeBlockBuilder { get; set; }
    public CodeBlockBuilder CurrentCodeBlockBuilder { get; set; }
    
    IBinder IParserModel.Binder => Binder;
    IBinderSession IParserModel.BinderSession => BinderSession;
}
