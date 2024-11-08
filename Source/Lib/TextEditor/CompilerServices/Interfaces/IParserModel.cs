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
    
    /// <summary>
    /// This property is likely to become obsolete.
    /// It is a hacky sort of 'catch all' case
    /// where one can push onto it whatever they want.
    /// 
    /// <see cref="StatementBuilderStack"/> is being
    /// added and is expected to fully replace this property.
    /// </summary>
    public Stack<ISyntax> SyntaxStack { get; set; }
    
    /// <summary>
    /// Unlike <see cref="SyntaxStack"/> there are more
    /// defined rules for this property.
    ///
    /// It will be cleared after every <see cref="StatementDelimiterToken"/>,
    /// <see cref="OpenBraceToken"/>, and <see cref="CloseBraceToken"/>
    /// that is handled by the main loop.
    ///
    /// The intent is to build up ambiguous syntax by pushing it onto this stack,
    /// then once it can be disambiguated, pop off all the syntax and construct an
    /// <see cref="ISyntaxNode"/>.
    ///
    /// A syntax for definition is being counted when referring to 'Statement' here.
    /// So, to parse a <see cref="TypeDefinitionNode"/> one would check this for the
    /// access modifier (public, private, etc...).
    /// </summary>
    public Stack<ISyntax> StatementBuilderStack { get; set; }
    
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
    /// The source code: "<int>" is sort of nonsensical.
    ///
    /// But, nothing stops the user from typing this as a statement,
    /// 	it just is that C# won't compile it.
    ///
    /// So, if a statement starts with 'OpenAngleBracketToken',
    /// what is the best parse we can provide for that?
    ///
    /// The idea is to tell the expression parsing code to return a certain SyntaxKind.
    ///
    /// So, its another 'forceExit' but instead of matching on 'CloseAngleBracketToken'
    /// you are saying: when the top most 'SyntaxKind' is completed,
    /// return it to me (stop parsing expressions).
    ///
    /// The wording of 'top most' is referring to the recursive, "nonsensical" syntax
    /// of <<int>>.
    ///
    /// Here the outermost GenericParametersListingNode contains
    /// a GenericParametersListingNode, and neither of them have an identifier
    /// that comes before the OpenAngleBracketToken.
    /// The "more sensible" syntax would be to type 'MyClass<int>;' rather than just '<int>;'
    /// They both will not compile, but one is on an extreme end of odd syntax
    /// and needs to be specifically targeted.
    ///
    /// Luthetus.CompilerServices.CSharp.ParserCase.Internals.ParseOthers.Force_ParseExpression(SyntaxKind syntaxKind, CSharpParserModel model) {...}
    /// </summary>
    public SyntaxKind? ForceParseExpressionSyntaxKind { get; set; }
    
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
