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

/// <summary>
/// The computational state for the CSharpParser is contained within this type.
/// The output of the CSharpParser is the <see cref="CSharpCompilationUnit"/>.<see cref="CSharpCompilationUnit.RootCodeBlockNode"/>
/// </summary>
public struct CSharpParserModel
{
    public CSharpParserModel(
        TokenWalker tokenWalker,
        Stack<ISyntax> syntaxStack,
        DiagnosticBag diagnosticBag,
        CSharpCodeBlockBuilder globalCodeBlockBuilder,
        CSharpCodeBlockBuilder currentCodeBlockBuilder)
    {
    	#if DEBUG
    	++LuthetusDebugSomething.ParserModel_ConstructorInvocationCount;
    	#endif
    
        TokenWalker = tokenWalker;
        SyntaxStack = syntaxStack;
        DiagnosticBag = diagnosticBag;
        GlobalCodeBlockBuilder = globalCodeBlockBuilder;
        CurrentCodeBlockBuilder = currentCodeBlockBuilder;
        
        ExpressionList = new();
        ExpressionList.Add((SyntaxKind.StatementDelimiterToken, null));
        
        ForceParseExpressionInitialPrimaryExpression = EmptyExpressionNode.Empty;
    }

    public TokenWalker TokenWalker { get; }
    public Stack<ISyntax> SyntaxStack { get; set; }
    public CSharpStatementBuilder StatementBuilder { get; set; } = new();
    
    /// <summary>
    /// The C# IParserModel implementation will only "short circuit" if the 'SyntaxKind DelimiterSyntaxKind'
    /// is registered as a delimiter.
    ///
    /// This is done in order to speed up the while loop, as the list of short circuits doesn't have to be
    /// iterated unless the current token is a possible delimiter.
    ///
    /// Luthetus.CompilerServices.CSharp.ParserCase.Internals.ParseOthers.SyntaxIsEndDelimiter(SyntaxKind syntaxKind) {...}
    /// </summary>
    public List<(SyntaxKind DelimiterSyntaxKind, IExpressionNode ExpressionNode)> ExpressionList { get; set; }
    
    public IExpressionNode? NoLongerRelevantExpressionNode { get; set; }
    public List<SyntaxKind> TryParseExpressionSyntaxKindList { get; } = new();
    public IExpressionNode ForceParseExpressionInitialPrimaryExpression { get; set; }
    public bool ForceParseTypeClauseNode { get; set; }
    public DiagnosticBag DiagnosticBag { get; }
    public CSharpCodeBlockBuilder GlobalCodeBlockBuilder { get; set; }
    public CSharpCodeBlockBuilder CurrentCodeBlockBuilder { get; set; }
}
