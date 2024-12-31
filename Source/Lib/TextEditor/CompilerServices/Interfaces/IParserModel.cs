using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

/// <summary>
/// See 'Luthetus.CompilerServices.CSharp.ParserCase.CSharpParserModel'
/// for an example of an "implementation".
///
/// Actually implementing this interface is not required,
/// this instead acts only as a guide on 'default' decisions
/// if one desires it.
///
/// In order to avoid implementing this interface you need to
/// implement ICompilerService.ParseAsync youself.
///
/// If you inherit 'CompilerService',
/// then you need to override 'CompilerService.ParseAsync'.
///
/// It is at this point that you now have full control
/// over how parsing is done.
/// </summary>
public interface IParserModel
{
    public IBinder Binder { get; }
    public IBinderSession BinderSession { get; }
    public TokenWalker TokenWalker { get; }
    
    /// <summary>
    /// This property is likely to become obsolete.
    /// It is a hacky sort of 'catch all' case
    /// where one can push onto it whatever they want.
    /// 
    /// <see cref="StatementBuilder"/> is being
    /// added and is expected to fully replace this property.
    /// </summary>
    public Stack<ISyntax> SyntaxStack { get; set; }
    
    /// <summary>
    /// Unlike <see cref="SyntaxStack"/> there are more defined rules for this property.
    ///
    /// It will be cleared after every <see cref="StatementDelimiterToken"/>,
    /// <see cref="OpenBraceToken"/>, and <see cref="CloseBraceToken"/>
    /// that is handled by the main loop.
    ///
    /// The intent is to build up ambiguous syntax by pushing it onto this stack,
    /// then once it can be disambiguated, pop off all the syntax and construct an
    /// <see cref="ISyntaxNode"/>.
    ///
    /// A syntax for a definition is being treated as a 'Statement' here.
    /// So, to parse a <see cref="TypeDefinitionNode"/> one would check this for the
    /// access modifier (public, private, etc...).
    /// </summary>
    public StatementBuilder StatementBuilder { get; set; }
    
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
    
    /// <summary>
    /// If a ParenthesizedExpressionNode is determined to more accurately be described
    /// by a CommaSeparatedExpressionNode, then any state related
    /// to the ParenthesizedExpressionNode instance needs to be cleared.
    /// </summary>
    public IExpressionNode? NoLongerRelevantExpressionNode { get; set; }
    
    /// <summary>
    /// TypeClauseNode code exists in the expression code.
	/// As a result, some statements need to read a TypeClauseNode by invoking 'ParseExpression(...)'.
	///
	/// In order to "short circut" or "force exit" from the expression code back to the statement code:
	/// if the root primary expression is not equal to the model.ForceParseExpressionSyntaxKind
	/// then stop 'ParseExpression(...)' from consuming any further tokens.
    ///
    /// Luthetus.CompilerServices.CSharp.ParserCase.Internals.ParseOthers.TryParseExpression(SyntaxKind syntaxKind, CSharpParserModel model, out IExpressionNode expressionNode) {...}
    /// </summary>
    public List<SyntaxKind> TryParseExpressionSyntaxKindList { get; }
    
    /// <summary>
    /// Something that is syntactically recursive but without creating a new scope needs
    /// to be parsed as an expression.
    ///
    /// TODO: It was decided to parse function arguments differently than how this example describes...
    ///       ...This example should be changed to one that is actually in use.
    /// For example: a function definition contains its arguments.
    ///
    /// But, the arguments could contain an entry which is a TypeClauseNode
    /// that has GenericArgumentEntryNode of TypeClauseNode that also goes on to have its own GenericArgumentEntryNode...
    ///
    /// So the expression code needs to be temporarily invoked from the statement code.
    ///
    /// The example that was described is parsing a FunctionArgumentsListingNode.
    ///
    /// The expression code only "naturally" can encounter a FunctionParametersListingNode
    /// which occurs from function invocation.
    ///
    /// So, this property exists in order to change the initial primary expression that is used.
    ///
    /// After every invocation of:
    ///     Luthetus.CompilerServices.CSharp.ParserCase.Internals.ParseOthers.TryParseExpression(SyntaxKind syntaxKind, CSharpParserModel model, out IExpressionNode expressionNode) {...}
    ///
    /// This property will be reset to 'EmptyExpressionNode.Empty'. (which is quite a hacky manner of going about things).
    /// TODO: Consider making 'ForceParseExpressionInitialPrimaryExpression' an argument to the 'TryParseExpression' method?
    ///
    /// Through this property, one can tell the expression code to parse a FunctionArgumentsListingNode
    /// by providing it as the initial expression. This must be done because it
    /// can never occur "naturally" by invoking the expression code and starting with 'EmptyExpressionNode.Empty'.
    /// </summary>
    public IExpressionNode ForceParseExpressionInitialPrimaryExpression { get; set; }
    
    public DiagnosticBag DiagnosticBag { get; }
    public CodeBlockBuilder GlobalCodeBlockBuilder { get; set; }
    public CodeBlockBuilder CurrentCodeBlockBuilder { get; set; }
}
